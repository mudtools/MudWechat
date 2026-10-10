// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Finance;
using Mud.Wechat.Work.ExtendedSDK.Finance.InteropServices;

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// <see cref="IWechatWorkFinanceClient"/> 的默认实现：一个实例对应一个原生 SDK 句柄与一个存档机器人。
/// </summary>
/// <remarks>
/// <para>
/// <b>两层判错各走各的门</b>：原生返回码非 0 → <see cref="WechatFinanceNativeException"/>；
/// 原生返回 0 但信封 <c>errcode != 0</c> → <see cref="WechatWorkException"/>
/// （<c>ThrowIfFailed</c> 同一咽喉点）。两者不可合并 —— 前者意味着「根本没有可用内容」，
/// 后者意味着「内容有、业务不通过」（守卫 FIN-B1）。
/// </para>
/// <para>
/// <b>实例上不存在凭据字段</b>：构造完成后，<c>corpid</c> 与存档 <c>secret</c> 已交予原生 <c>Init</c>，
/// 托管侧<b>不</b>保留其副本（代理口令因每次调用都要传，是唯一驻留的敏感串，且只作入参、永不进诊断文本）。
/// RSA 私钥<b>每次解密现取现用</b>，不缓存、不落字段（守卫 FIN-B2 / FIN-B5）。
/// </para>
/// <para>
/// <b>串行闸</b>：同一实例上的原生调用互斥。理由是两层叠加 —— <c>seq</c> 游标协议本身要求「取回后推进」，
/// 并发拉取只会重复取回；且原生库的线程安全性<b>未经官方核验</b>，不敢假设。多机器人由工厂分实例，
/// 跨机器人仍可并行。
/// </para>
/// <para>
/// <b>无终结器</b>：本类型不持裸 <see cref="IntPtr"/>，原生内存全部由 <c>SafeHandle</c> 承载
/// （其自身有最终器）。忘记 Dispose 时句柄仍会被回收，只是推迟到最终器运行。
/// </para>
/// </remarks>
public sealed class WechatWorkFinanceClient : IWechatWorkFinanceClient, IDisposable
{
    /// <summary>Dispose 前等待在途调用排空的上限（秒）。超时仍继续，安全性由 SafeHandle 引用计数保证。</summary>
    private const int DisposeDrainWaitSeconds = 30;

    /// <summary>官方设备控制符转义序列（明文 JSON 的已知脏数据形态，见 <c>Parse</c>）。</summary>
    private static readonly string[] ControlCharEscapes =
    {
        "\\u0011", "\\u0012", "\\u0013", "\\u0014",
    };

    private readonly FinanceSdkHandle _sdk;
    private readonly string _robotKey;
    private readonly WechatFinanceRobotOptions _robot;
    private readonly ISecretProvider _secrets;
    private readonly string? _proxyAddress;
    private readonly string? _proxyPassword;
    private readonly int _timeoutSeconds;
    private readonly int _retryCount;
    private readonly int _retryDelayMs;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private int _disposed;

    /// <summary>
    /// 构造客户端。<b>内部构造入口</b>：原生 SDK 句柄类型 <see cref="FinanceSdkHandle"/> 是实现细节
    /// （<c>internal</c>），宿主拿不到也造不出合法句柄 ⇒ 常规路径一律经
    /// <see cref="IWechatWorkFinanceClientFactory"/>（负责取 secret、<c>NewSdk</c> + <c>Init</c> 与实例缓存）。
    /// </summary>
    /// <param name="robotKey">机器人键（诊断文本使用）。</param>
    /// <param name="sdk">已 <c>Init</c> 成功的 SDK 句柄（本实例接管其释放）。</param>
    /// <param name="robot">该机器人的配置（<c>ProxyAddress</c> / <c>PrivateKeySecretNames</c>）。</param>
    /// <param name="secrets">组件密钥端口（取 RSA 私钥与代理口令）。</param>
    /// <param name="timeoutSeconds">原生调用超时（秒）。</param>
    /// <param name="retryCount">媒体分片同游标重试次数。</param>
    /// <param name="retryDelayMs">媒体分片重试退避（毫秒）。</param>
    /// <param name="proxyPassword">已解析的代理口令（<c>null</c> = 无凭据）；只作原生入参。</param>
    /// <exception cref="ArgumentNullException">必填参数为 <c>null</c> 时抛出。</exception>
    internal WechatWorkFinanceClient(
        string robotKey,
        FinanceSdkHandle sdk,
        WechatFinanceRobotOptions robot,
        ISecretProvider secrets,
        int timeoutSeconds,
        int retryCount,
        int retryDelayMs,
        string? proxyPassword)
    {
        if (string.IsNullOrWhiteSpace(robotKey))
        {
            throw new ArgumentException("机器人键不能为空。", nameof(robotKey));
        }

        _robotKey = robotKey;
        _sdk = sdk ?? throw new ArgumentNullException(nameof(sdk));
        _robot = robot ?? throw new ArgumentNullException(nameof(robot));
        _secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));

        if (timeoutSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeoutSeconds), "超时秒数必须为正。");
        }

        if (retryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(retryCount), "重试次数不得为负（0 = 关闭重试）。");
        }

        if (retryDelayMs < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(retryDelayMs), "重试退避毫秒数不得为负。");
        }

        _timeoutSeconds = timeoutSeconds;
        _retryCount = retryCount;
        _retryDelayMs = retryDelayMs;
        _proxyPassword = proxyPassword;
        _proxyAddress = string.IsNullOrWhiteSpace(robot.ProxyAddress) ? null : robot.ProxyAddress;

        if (sdk.IsInvalid)
        {
            throw new WechatFinanceNativeException(
                nameof(FinanceNativeMethods.NewSdk), 0, "原生 SDK 句柄无效（NewSdk 返回空指针）。");
        }
    }

    /// <inheritdoc />
    public string RobotKey => _robotKey;

    /// <inheritdoc />
    public async Task<FinanceChatDataEnvelope> GetChatDataAsync(
        ulong seq, int limit, CancellationToken cancellationToken = default)
    {
        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limit), "limit 必须为正整数（单次条数的官方上限未逐页核验，故不做本地硬拦）。");
        }

        var json = await RunOnSliceAsync(
            nameof(FinanceNativeMethods.GetChatData),
            cancellationToken,
            slice => FinanceNativeMethods.GetChatData(
                _sdk, seq, limit, _proxyAddress, _proxyPassword, _timeoutSeconds, slice))
            .ConfigureAwait(false);

        var envelope = ParseEnvelope(json);

        // 信封层判错走企微统一出口：errcode != 0 抛 WechatWorkException（与原生返回码分属两层）。
        WechatWorkException.ThrowIfFailed(envelope);

        return envelope;
    }

    /// <inheritdoc />
    public async Task<FinanceChatMessage> DecryptChatRecordAsync(
        FinanceChatDataRow row, CancellationToken cancellationToken = default)
    {
        if (row == null) throw new ArgumentNullException(nameof(row));

        if (string.IsNullOrWhiteSpace(row.EncryptRandomKey) || string.IsNullOrWhiteSpace(row.EncryptChatMsg))
        {
            throw new InvalidOperationException(
                $"机器人「{_robotKey}」的记录 seq={row.Seq} 缺少 encrypt_random_key 或 encrypt_chat_msg，无法解密。");
        }

        cancellationToken.ThrowIfCancellationRequested();

        // 私钥现取现用：不进字段、不缓存（FIN-B2）。异常消息只带机器人与版本号（FIN-B5）。
        var privateKeyPem = await ResolvePrivateKeyAsync(row.PublicKeyVersion, cancellationToken)
            .ConfigureAwait(false);

        // DecryptRandomKey 与原生调用都是阻塞式 CPU/IO 混合段，串在同一个 await 链里即可，
        // 不做 Task.Run 伪异步。
        var encryptKey = FinanceCipher.DecryptRandomKey(
            privateKeyPem, row.EncryptRandomKey!, _robotKey, row.PublicKeyVersion);

        var json = await RunOnSliceAsync(
            nameof(FinanceNativeMethods.DecryptData),
            cancellationToken,
            slice => FinanceNativeMethods.DecryptData(encryptKey, row.EncryptChatMsg!, slice))
            .ConfigureAwait(false);

        return ParseMessage(json);
    }

    /// <inheritdoc />
    public Task<FinanceMediaChunk> GetMediaChunkAsync(
        string? indexBuffer, string sdkFileId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sdkFileId))
        {
            throw new ArgumentException("sdkfileid 不能为空。", nameof(sdkFileId));
        }

        return FetchChunkAsync(indexBuffer, sdkFileId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<byte[]> GetMediaDataAsync(string sdkFileId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sdkFileId))
        {
            throw new ArgumentException("sdkfileid 不能为空。", nameof(sdkFileId));
        }

        return FinanceMediaAssembler.AggregateAsync(
            cursor => FetchChunkAsync(cursor, sdkFileId, cancellationToken),
            _retryCount,
            _retryDelayMs,
            cancellationToken);
    }

    /// <summary>
    /// 释放实例：先尽力排空在途调用，再销毁 SDK 句柄。
    /// </summary>
    /// <remarks>
    /// <b>经工厂取得的实例不要调用本方法</b>（工厂在自身释放时统一销毁；对共享实例调 Dispose 会让其他
    /// 使用者突然拿到已销毁句柄）。构造入口是 <c>internal</c> ⇒ 正常装配下不存在「宿主自行 new」的形态，
    /// 本方法即为工厂的销毁通道；<c>IWechatWorkFinanceClient</c> 接口<b>不</b>继承 <c>IDisposable</c>
    /// 正是为了让调用侧拿不到它。
    /// </remarks>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        var entered = _gate.Wait(TimeSpan.FromSeconds(DisposeDrainWaitSeconds));
        try
        {
            // SafeHandle 的引用计数保证：即使仍有调用在飞，原生 DestroySdk 也要等它结束才真正执行。
            _sdk.Dispose();
        }
        finally
        {
            if (entered)
            {
                _gate.Release();
            }

            _gate.Dispose();
        }
    }

    /// <summary>取一个媒体分片（串行闸内）。</summary>
    private async Task<FinanceMediaChunk> FetchChunkAsync(
        string? indexBuffer, string sdkFileId, CancellationToken cancellationToken)
    {
        var entryName = nameof(FinanceNativeMethods.GetMediaData);

        return await GateAsync(entryName, cancellationToken, () =>
        {
            using var media = FinanceMediaDataHandle.Create();

            var ret = FinanceNativeMethods.GetMediaData(
                _sdk, indexBuffer, sdkFileId, _proxyAddress, _proxyPassword, _timeoutSeconds, media);

            EnsureNativeSuccess(entryName, ret);

            var content = FinanceNativeReader.ReadMediaBytes(media, entryName);
            var nextIndexBuffer = FinanceNativeReader.ReadIndexBuffer(media);
            var isFinished = FinanceNativeMethods.IsMediaDataFinish(media) != 0;

            return new FinanceMediaChunk(content, nextIndexBuffer, isFinished);
        }).ConfigureAwait(false);
    }

    /// <summary>在 <c>Slice_t</c> 上执行一次原生写出调用并取回文本（成对创建/释放由 <c>using</c> 保证）。</summary>
    private async Task<string> RunOnSliceAsync(
        string entryName, CancellationToken cancellationToken, Func<FinanceSliceHandle, int> invoke)
    {
        return await GateAsync(entryName, cancellationToken, () =>
        {
            using var slice = FinanceSliceHandle.Create();

            var ret = invoke(slice);
            EnsureNativeSuccess(entryName, ret);

            return FinanceNativeReader.ReadSliceText(slice, entryName);
        }).ConfigureAwait(false);
    }

    /// <summary>串行闸：入闸前后各查一次释放标记，避免「Dispose 与在途调用」交叉。</summary>
    private async Task<T> GateAsync<T>(string entryName, CancellationToken cancellationToken, Func<T> invoke)
    {
        EnsureNotDisposed();

        var entered = false;
        try
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            entered = true;

            // 排队期间可能已被释放。
            EnsureNotDisposed();

            return invoke();
        }
        catch (OperationCanceledException)
        {
            // 取消不是原生故障：原样上抛，不包装成 WechatFinanceNativeException（否则调用方的
            // 「取消即跳过本轮」逻辑会被吞掉）。
            throw;
        }
        catch (Exception ex) when (entryName != null && !(ex is WechatFinanceNativeException)
                                   && !(ex is ObjectDisposedException)
                                   && !(ex is DllNotFoundException)
                                   && !(ex is EntryPointNotFoundException))
        {
            throw new InvalidOperationException($"原生入口 {entryName} 调用失败（{ex.GetType().Name}）。", ex);
        }
        finally
        {
            if (entered)
            {
                _gate.Release();
            }
        }
    }

    /// <summary>按版本号取 RSA 私钥文本（PEM）。</summary>
    private async Task<string> ResolvePrivateKeyAsync(int publicKeyVersion, CancellationToken cancellationToken)
    {
        if (!_robot.TryGetPrivateKeySecretName(publicKeyVersion, out var secretName) || string.IsNullOrWhiteSpace(secretName))
        {
            throw new InvalidOperationException(
                $"机器人「{_robotKey}」未配置 publickey_ver={publicKeyVersion} 的私钥密钥名。" +
                "官方支持密钥轮换，历史记录的版本号不会随新密钥改变 ⇒ 必须保留旧版本的映射。");
        }

        var pem = await _secrets.GetSecretAsync(secretName!).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(pem))
        {
            // 只回显**截断后的密钥名**：填错成密钥原文时，全名回显等于把内容洒进日志（同支付侧做法）。
            throw new InvalidOperationException(
                $"机器人「{_robotKey}」的 privatekey 密钥名「{Truncate(secretName!)}」在 ISecretProvider 中查无值" +
                $"（publickey_ver={publicKeyVersion}）。注意该配置存的是**密钥名**，不是私钥本身。");
        }

        cancellationToken.ThrowIfCancellationRequested();
        return pem;
    }

    /// <summary>原生返回码判错（0 之外的每一支都带入口名与返回码，不带任何报文内容）。</summary>
    private static void EnsureNativeSuccess(string entryName, int returnCode)
    {
        if (returnCode == WechatFinanceNativeCodes.Success)
        {
            return;
        }

        throw new WechatFinanceNativeException(
            entryName, returnCode,
            $"原生入口 {entryName} 返回 {returnCode}（非 0 即失败）。" +
            "取数阶段不重试：会话记录由上层轮询推进，媒体分片的重试判据见 WechatFinanceNativeCodes。");
    }

    /// <summary>释放后禁止再进入原生调用。</summary>
    private void EnsureNotDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(WechatWorkFinanceClient),
                $"机器人「{_robotKey}」的会话存档客户端已释放。");
        }
    }

    /// <summary>
    /// 解析 <c>GetChatData</c> 写回的明文 JSON 信封。
    /// </summary>
    private static FinanceChatDataEnvelope ParseEnvelope(string json)
        => Parse(json, DeserializeEnvelope, nameof(FinanceNativeMethods.GetChatData));

    /// <summary>
    /// 解析 <c>DecryptData</c> 写回的明文 JSON 消息体。
    /// </summary>
    private static FinanceChatMessage ParseMessage(string json)
        => Parse(json, DeserializeMessage, nameof(FinanceNativeMethods.DecryptData));

    /// <summary>
    /// 解析内核：先按原样解析，仅在 <see cref="JsonException"/> 路径上剥设备控制符重试一次。
    /// </summary>
    /// <remarks>
    /// 「先原样、失败再清洗」而非「总是清洗」：总是清洗会把真正的坏数据洗成合法形状，
    /// 从而把协议问题伪装成业务数据（清洗后仍失败时，诊断文本会明确指出走到了清洗分支）。
    /// </remarks>
    private static T Parse<T>(string json, Func<string, T?> deserialize, string entryName)
        where T : class
    {
        try
        {
            return deserialize(json) ?? throw EmptyPayload(entryName);
        }
        catch (JsonException ex)
        {
            var cleaned = StripDeviceControlEscapes(json);
            var stripped = !ReferenceEquals(cleaned, json);

            if (stripped)
            {
                try
                {
                    return deserialize(cleaned) ?? throw EmptyPayload(entryName);
                }
                catch (JsonException)
                {
                    // 清洗后仍失败 ⇒ 落到下方的统一异常（保留首轮 JsonException 为 inner）。
                }
            }

            // 明文即敏感：异常消息只报入口名与「是否已清洗」，绝不回显报文（守卫 FIN-B5）。
            throw new InvalidOperationException(
                $"{entryName} 写回的明文 JSON 解析失败（{(stripped ? "已剥设备控制符转义后仍失败" : "未检出设备控制符转义")}）。" +
                "出于「解密后明文不得进异常消息」的约束，此处不含报文内容，请结合本轮 seq 与 msgtype 定位。",
                ex);
        }
    }

    private static InvalidOperationException EmptyPayload(string entryName)
        => new($"{entryName} 原生返回 0 但写回的明文为空 JSON（null），无法解析。");

    /// <summary>剥官方偶发的设备控制符转义序列（无命中时原样返回同一实例，供上层判断是否发生过清洗）。</summary>
    private static string StripDeviceControlEscapes(string json)
    {
        var result = json;
        foreach (var escape in ControlCharEscapes)
        {
            // ns2.0 无 string.Contains(string, StringComparison)，用 IndexOf 判定（守卫 FIN-B4 关心该分支可达性）。
            if (result.IndexOf(escape, StringComparison.Ordinal) >= 0)
            {
                result = result.Replace(escape, string.Empty);
            }
        }

        return result;
    }

#if NET8_0_OR_GREATER
    private static FinanceChatDataEnvelope? DeserializeEnvelope(string json)
        => JsonSerializer.Deserialize(json, FinanceJsonContext.Default.FinanceChatDataEnvelope);

    private static FinanceChatMessage? DeserializeMessage(string json)
        => JsonSerializer.Deserialize(json, FinanceJsonContext.Default.FinanceChatMessage);
#else
    // 低 TFM 分支：生成的 *JsonContext 本体是 #if NET8_0_OR_GREATER，ns2.0/net6 无上下文类型可用，
    // 只能用显式选项（与 RedisWechatCorpAuthStore 的 RD4 同款处置，AOT strict 门禁只断言 net8.0/net10.0）。
    // 口径逐字对齐 FinanceJsonContext 的 [JsonSourceGenerationOptions]（camelCase + 忽略 null + 大小写不敏感），
    // 保证跨 TFM 行为一致。本域每个属性都带显式 [JsonPropertyName] ⇒ 命名策略实为空操作，
    // 但仍照抄 —— 「两个 TFM 分支的序列化口径不同」这类差异只有对照源文件时才看得出。
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static FinanceChatDataEnvelope? DeserializeEnvelope(string json)
    {
#pragma warning disable IL2026 // 反射反序列化在裁剪下无法静态分析成员
#pragma warning disable IL3050 // 反射反序列化在 AOT 下需动态代码生成
        return JsonSerializer.Deserialize<FinanceChatDataEnvelope>(json, SerializerOptions);
#pragma warning restore IL3050
#pragma warning restore IL2026
    }

    private static FinanceChatMessage? DeserializeMessage(string json)
    {
#pragma warning disable IL2026 // 反射反序列化在裁剪下无法静态分析成员
#pragma warning disable IL3050 // 反射反序列化在 AOT 下需动态代码生成
        return JsonSerializer.Deserialize<FinanceChatMessage>(json, SerializerOptions);
#pragma warning restore IL3050
#pragma warning restore IL2026
    }
#endif

    private static string Truncate(string value)
        => value.Length <= 24 ? value : value.Substring(0, 24) + "...";
}
