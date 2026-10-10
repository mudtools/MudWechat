// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.ExtendedSDK.Finance.InteropServices;

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// <see cref="IWechatWorkFinanceClientFactory"/> 的默认实现：按机器人键缓存原生 SDK 实例。
/// </summary>
/// <remarks>
/// <para>
/// <b>装配时序</b>：<c>SetProbePath</c> → <c>EnsureRegistered</c> → <c>NewSdk</c> → <c>Init</c>。
/// 前两步必须在<b>首次 P/Invoke 之前</b>完成（解析器登记晚于装载即失效，Linux 侧尤其致命 ——
/// 默认探测找不到 <c>libWeWorkFinanceSdk_C.so</c>）。二者皆幂等，故每次装配都调用，不依赖「谁先跑」。
/// </para>
/// <para>
/// <b>失败不留缓存条目</b>：密钥中心临时不可达、<c>Init</c> 网络抖动都属可恢复故障，把失败的
/// <see cref="Task"/> 永久留在字典里等于「一次抖动，该机器人到进程重启前全废」。移除时以
/// <b>引用相等</b>为条件（只删自己等过的那条），否则会删掉并发调用者刚装好的新实例。
/// </para>
/// <para>
/// <b>配置不热更（有意如此）</b>：<c>corpid</c>/<c>secret</c> 交给原生 <c>Init</c> 后托管侧不留副本，
/// 超时值也在建实例时快照 —— 让 <c>IOptionsMonitor</c> 的热更「看起来生效」而实际不生效，
/// 比明确不支持热更更糟。换 secret 的做法是换机器人键（新键 ⇒ 新实例 ⇒ 新 <c>Init</c>），
/// 或在宿主重建服务容器时完成。
/// </para>
/// <para>
/// <b>无终结器</b>：忘记释放工厂时，原生句柄仍会由 <c>SafeHandle</c> 的最终器回收，
/// 只是推迟；不做「工厂自带终结器再释放子实例」（终结器顺序无保证，反而可能出现子先释、父后触）。
/// </para>
/// </remarks>
public sealed class WechatFinanceClientFactory : IWechatWorkFinanceClientFactory
{
    private readonly IOptions<WechatFinanceOptions> _options;
    private readonly ISecretProvider _secrets;
    private readonly ConcurrentDictionary<string, Task<IWechatWorkFinanceClient>> _clients = new();
    private int _disposed;

    /// <summary>创建工厂。</summary>
    /// <param name="options">会话存档配置（构造期即校验，配置非法在解析工厂时失败而非首次取客户端时失败）。</param>
    /// <param name="secrets">组件密钥端口。</param>
    /// <exception cref="ArgumentNullException">任一参数为 <c>null</c> 时抛出。</exception>
    /// <exception cref="InvalidOperationException">配置校验失败时抛出（详见 <see cref="WechatFinanceOptions.Validate"/>）。</exception>
    public WechatFinanceClientFactory(IOptions<WechatFinanceOptions> options, ISecretProvider secrets)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));

        // 立刻读一次 Value：把「配置非法」钉在解析期，而不是拖到某个机器人首次被用到。
        // IOptions<T>.Value 本身会跑注册期挂上的 Validate 委托，此处再显式调一次，
        // 使「宿主手工 new 工厂（未经 AddWechatFinanceSdk）」也得到同一道校验。
        var snapshot = _options.Value;
        snapshot.Validate();
    }

    /// <inheritdoc />
    public async Task<IWechatWorkFinanceClient> GetClientAsync(
        string robotKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(robotKey))
        {
            throw new ArgumentException("机器人键不能为空。", nameof(robotKey));
        }

        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(WechatFinanceClientFactory), "会话存档客户端工厂已释放。");
        }

        // 单飞：先把「一个待完成的 Task」放进字典，只有写入成功的调用者负责真正装配。
        // 用 TCS 而非 Lazy<Task> 的理由：GetOrAdd 的值工厂可能被并发执行多次（Lazy 只保证自己的
        // 工厂单飞，而字典层的重复插入仍会留下多份 Lazy ⇒ 多份 NewSdk），TCS 形态让「谁赢」显式可见。
        var tcs = new TaskCompletionSource<IWechatWorkFinanceClient>(TaskCreationOptions.RunContinuationsAsynchronously);
        var ours = tcs.Task;
        var effective = _clients.GetOrAdd(robotKey, ours);

        if (!ReferenceEquals(effective, ours))
        {
            return await AwaitSharedAsync(effective, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            var client = await BuildAsync(robotKey).ConfigureAwait(false);
            tcs.SetResult(client);
            return client;
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);

            // 失败不留条目：装配失败的可恢复成因（密钥中心抖动 / Init 网络抖动）不该永久毒化该机器人。
            // 只删「本线程写入的那条」——并发下他人可能已换成成功的新条目。
            if (_clients.TryGetValue(robotKey, out var current) && ReferenceEquals(current, ours))
            {
                _clients.TryRemove(robotKey, out _);
            }

            throw;
        }
    }

    /// <summary>
    /// 等待他人发起的装配 Task，同时尊重<b>本调用者</b>的取消令牌。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 装配 Task 本身<b>不</b>携带任何调用者的令牌：若把首个调用者的令牌接进 <c>NewSdk</c>/<c>Init</c>
    /// 链路，他一人超时会让其余并发调用者看到「取消」而非真实结果（共享 Task 无法区分谁的取消）。
    /// </para>
    /// <para>
    /// 因此这里只让本调用者「放弃等待」，装配继续在后台进行：成功则下次 <c>GetClientAsync</c> 直接命中，
    /// 失败则由写入者移除条目、后续重试。宿主不应据此以为可以靠取消省掉一次原生 <c>Init</c>。
    /// </para>
    /// </remarks>
    private static async Task<IWechatWorkFinanceClient> AwaitSharedAsync(
        Task<IWechatWorkFinanceClient> shared, CancellationToken cancellationToken)
    {
        if (!cancellationToken.CanBeCanceled)
        {
            return await shared.ConfigureAwait(false);
        }

        var winner = await Task.WhenAny(shared, Task.Delay(Timeout.Infinite, cancellationToken))
            .ConfigureAwait(false);

        if (winner == shared)
        {
            return await shared.ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return await shared.ConfigureAwait(false);
    }

    /// <summary>释放工厂：销毁全部已装配实例的原生 SDK 句柄。</summary>
    /// <remarks>
    /// <b>经本工厂取得的客户端不要自行 Dispose</b>（交叉释放会让其他使用者突然拿到已销毁句柄）。
    /// 单个实例释放失败不阻断其余实例（一个机器人释放不掉不该拖住整个容器的停机）。
    /// </remarks>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        foreach (var pair in _clients.ToArray())
        {
            _clients.TryRemove(pair.Key, out _);

            // 只释放「装配成功」的实例；未完成的 Task 交给它的写入者（本线程不观测其结果，
            // 也不主动取消 —— 原生 Init 不可中断，取消只会造出一个「装配半途」的原生实例）。
            if (pair.Value.Status != TaskStatus.RanToCompletion)
            {
                continue;
            }

            try
            {
                (pair.Value.Result as IDisposable)?.Dispose();
            }
            catch
            {
                // 一个机器人释放失败不阻断其余机器人（停机路径上的宽容是刻意的）。
            }
        }
    }

    /// <summary>真实装配：取 secret → NewSdk → Init → 建实例。</summary>
    /// <remarks>
    /// <b>不带调用者令牌</b>（原因见 <c>AwaitSharedAsync</c>）：原生 <c>Init</c> 本就不可中断，
    /// 密钥取用一旦开始就跑完它，避免「一个调用者的超时改写所有人的结果」。
    /// </remarks>
    private async Task<IWechatWorkFinanceClient> BuildAsync(string robotKey)
    {
        var options = _options.Value;

        FinanceNativeLibrary.SetProbePath(options.NativeLibraryPath);
        FinanceNativeLibrary.EnsureRegistered();

        if (!options.Robots.TryGetValue(robotKey, out var robot) || robot == null)
        {
            throw new InvalidOperationException(
                $"会话存档配置中没有机器人键「{robotKey}」（已配置：{string.Join(", ", options.Robots.Keys)}）。" +
                $"请补 WechatFinance:Robots:{robotKey} 或改正调用侧传入的键。");
        }

        var secret = await ResolveSecretAsync(robot.SecretSecretName, nameof(robot.SecretSecretName), robotKey)
            .ConfigureAwait(false);

        // 代理口令可选：留空即「代理无凭据」，此时向原生传 null（传空串与传 null 在原生侧语义不同，
        // 空串会被当作「带一个空口令的认证请求」）。
        string? proxyPassword = null;
        if (!string.IsNullOrWhiteSpace(robot.ProxyPasswordSecretName))
        {
            proxyPassword = await ResolveSecretAsync(
                robot.ProxyPasswordSecretName, nameof(robot.ProxyPasswordSecretName), robotKey).ConfigureAwait(false);
        }

        var sdk = CreateSdkHandle(robotKey);
        try
        {
            var ret = FinanceNativeMethods.Init(sdk, robot.CorpId, secret);
            if (ret != WechatFinanceNativeCodes.Success)
            {
                // 消息不带 CorpId 与 secret（FIN-B5）：Init 失败的三大成因（corpid/secret 不符、
                // 未开通存档、网络不通）都能由「机器人键 + 返回码」定位。
                throw new WechatFinanceNativeException(
                    nameof(FinanceNativeMethods.Init), ret,
                    $"会话存档机器人「{robotKey}」原生 Init 返回 {ret}（非 0 即失败）。" +
                    "请核对 CorpId 与存档 secret、并确认该企业已开通会话内容存档。");
            }

            var timeoutSeconds = robot.ResolveTimeoutSeconds(options.DefaultTimeoutSeconds);

            return new WechatWorkFinanceClient(
                robotKey,
                sdk,
                robot,
                _secrets,
                timeoutSeconds,
                options.MediaShardRetryCount,
                options.MediaShardRetryDelayMs,
                proxyPassword);
        }
        catch
        {
            // Init 失败 / 建实例抛 ⇒ 句柄仍归工厂（客户端尚未接管所有权），必须就地销毁，
            // 否则每次装配失败都留下一份原生 SDK 实例直到最终器运行。
            sdk.Dispose();
            throw;
        }
    }

    /// <summary>建原生 SDK 句柄，把「库找不到」翻译成带探测候选的诊断文本。</summary>
    private static FinanceSdkHandle CreateSdkHandle(string robotKey)
    {
        FinanceSdkHandle handle;
        try
        {
            handle = FinanceSdkHandle.Create();
        }
        catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
        {
            // 装载类错误必须带上「按什么名字找过」——否则 Linux 宿主只会看到一句
            // "Unable to load DLL 'WeWorkFinanceSdk'"，而真实原因是官方文件名带 _C 段。
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」无法装载原生库（{ex.GetType().Name}）。" +
                $"当前探测目标：{FinanceNativeLibrary.DescribeProbeCandidates()}。" +
                "库文件不随 NuGet 包分发，需按平台部署 WeWorkFinanceSdk.dll / libWeWorkFinanceSdk_C.so，" +
                "必要时配置 WechatFinance:NativeLibraryPath 为绝对路径。", ex);
        }

        if (handle.IsInvalid)
        {
            handle.Dispose();
            throw new WechatFinanceNativeException(
                nameof(FinanceNativeMethods.NewSdk), 0,
                $"会话存档机器人「{robotKey}」原生 NewSdk 返回空句柄。");
        }

        return handle;
    }

    /// <summary>经 <c>ISecretProvider</c> 取密钥值，查无即 fail-fast（不回显疑似密钥内容）。</summary>
    private async Task<string> ResolveSecretAsync(string secretName, string propertyName, string robotKey)
    {
        if (string.IsNullOrWhiteSpace(secretName))
        {
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」缺少 {propertyName}（只登记 ISecretProvider 中的**密钥名**）。");
        }

        var value = await _secrets.GetSecretAsync(secretName).ConfigureAwait(false);

        if (string.IsNullOrEmpty(value))
        {
            // 只回显截断后的密钥名：误把密钥原文填进 *SecretName 时，全名回显等于把内容写进日志。
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」的 {propertyName} 密钥名「{Truncate(secretName)}」" +
                "在 ISecretProvider 中查无值。注意该字段存的是**密钥名**，不是密钥本身。");
        }

        return value;
    }

    private static string Truncate(string value)
        => value.Length <= 24 ? value : value.Substring(0, 24) + "...";
}
