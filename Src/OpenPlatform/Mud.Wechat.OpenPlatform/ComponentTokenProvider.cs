// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions.Transport;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 第三方平台令牌提供者（<see cref="IComponentTokenProvider"/>）：按需刷新 + 缓存 + <b>并发单飞</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>四条关键语义（每条都有对应用例）</b>：
/// </para>
/// <list type="number">
/// <item>
/// <b>提前刷新而非到期刷新</b>：判定用 <see cref="ComponentAccessTokenPolicy.ShouldRefresh"/>
/// （官方建议 1 小时 50 分即刷新），避免临界期用在途的旧令牌。
/// </item>
/// <item>
/// <b>并发单飞</b>：多个调用同时发现「该刷新」时，只有<b>第一个</b>打官方接口，
/// 其余在获得锁后<b>双检</b>缓存并复用结果 —— 否则高并发下会把官方接口打成洪水，
/// 而官方对令牌获取本身有频次限制。
/// </item>
/// <item>
/// <b>刷新失败但旧令牌仍可用 ⇒ 继续用</b>：进入提前窗口只是「该换了」，不等于「已失效」；
/// 此时官方偶发失败不应把失败传染给业务请求。
/// </item>
/// <item>
/// <b>旧令牌已失效 ⇒ fail-closed 抛出</b>：绝不返回一把过期令牌冒充成功
/// （否则调用方会收到一个语焉不详的 40001，掩盖「凭证链断了」这一真相）。
/// </item>
/// </list>
/// <para>
/// <b>取消不被吞</b>：失败回退的捕获面<b>只含</b> <see cref="WechatOpenPlatformException"/> 与
/// <see cref="HttpRequestException"/> —— <see cref="OperationCanceledException"/> <b>不在</b>捕获面，
/// 调用方的取消必须原样上抛（与本仓其它线同款纪律）。
/// </para>
/// <para>
/// <b>单例语义</b>：本类持有令牌缓存，<b>必须</b>注册为单例 —— 多实例各自缓存会让
/// 「刷新频率 × 实例数」放大，且实例间令牌不一致。
/// </para>
/// </remarks>
public sealed class ComponentTokenProvider : IComponentTokenProvider
{
    private readonly IWechatOpenPlatformHttpClient _httpClient;
    private readonly IComponentVerifyTicketStore _ticketStore;
    private readonly IOpenPlatformClock _clock;
    private readonly OpenPlatformAppConfig _config;
    private readonly ILogger<ComponentTokenProvider>? _logger;

    /// <summary>刷新闸（并发单飞；<c>1</c> 个许可即「同时只允许一次刷新」）。</summary>
    private readonly SemaphoreSlim _refreshGate = new SemaphoreSlim(1, 1);

    /// <summary>当前令牌状态（整体替换式写入；读取走 <see cref="Volatile"/> 以保证可见性）。</summary>
    private ComponentAccessTokenState? _state;

    /// <summary>创建令牌提供者。</summary>
    /// <param name="httpClient">本线命名 HTTP 客户端。</param>
    /// <param name="ticketStore">凭证票存储。</param>
    /// <param name="clock">时间源。</param>
    /// <param name="config">平台配置（构造期即校验必填项）。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <exception cref="ArgumentNullException">任一必填依赖为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">配置缺少 <c>ComponentAppId</c> / <c>ComponentAppSecret</c>。</exception>
    public ComponentTokenProvider(
        IWechatOpenPlatformHttpClient httpClient,
        IComponentVerifyTicketStore ticketStore,
        IOpenPlatformClock clock,
        OpenPlatformAppConfig config,
        ILogger<ComponentTokenProvider>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ticketStore = ticketStore ?? throw new ArgumentNullException(nameof(ticketStore));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _config.EnsureValid();
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> GetComponentAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var cached = Volatile.Read(ref _state);
        if (IsFresh(cached))
        {
            return cached!.AccessToken!;
        }

        await _refreshGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 双检：等锁期间可能已被别的调用刷新（并发单飞的关键一步）。
            cached = Volatile.Read(ref _state);
            if (IsFresh(cached))
            {
                return cached!.AccessToken!;
            }

            try
            {
                var refreshed = await RefreshAsync(cancellationToken).ConfigureAwait(false);
                Volatile.Write(ref _state, refreshed);
                return refreshed.AccessToken!;
            }
            catch (Exception ex) when (ex is WechatOpenPlatformException or HttpRequestException)
            {
                // 刷新失败：旧令牌**仍可用**时继续用它（进入提前窗口 ≠ 已失效），
                // 否则必须上抛（fail-closed）。
                var fallback = Volatile.Read(ref _state);
                if (fallback != null && ComponentAccessTokenPolicy.IsUsable(fallback, _clock.UtcNow))
                {
                    // 只记事实，不含令牌 / 票据 / 密钥原文。
                    _logger?.LogWarning(
                        "第三方平台令牌刷新失败，本次沿用仍有效的旧令牌（原因类型：{ReasonType}）。", ex.GetType().Name);
                    return fallback.AccessToken!;
                }

                throw;
            }
        }
        finally
        {
            _refreshGate.Release();
        }
    }

    /// <summary>
    /// 使缓存的平台令牌立即失效（下次取用将强制重取）。
    /// </summary>
    /// <remarks>
    /// 供声明式 <c>[Token]</c> 客户端的令牌管理器在 errcode 恢复路径调用
    /// （<see cref="Mud.Wechat.OpenPlatform.Authentication.ComponentTokenManager.InvalidateTokenAsync"/>）。
    /// 仅清缓存、不打官方接口；票据缺失时下次刷新会按既有 fail-closed 语义上抛。
    /// </remarks>
    public void Invalidate()
    {
        Volatile.Write(ref _state, null);
    }

    /// <summary>「既可用、又无需刷新」——只有这种状态才走缓存快车道。</summary>
    private bool IsFresh(ComponentAccessTokenState? state)
    {
        var now = _clock.UtcNow;
        return ComponentAccessTokenPolicy.IsUsable(state, now)
               && !ComponentAccessTokenPolicy.ShouldRefresh(state, now);
    }

    /// <summary>打官方接口换取新令牌。</summary>
    private async Task<ComponentAccessTokenState> RefreshAsync(CancellationToken cancellationToken)
    {
        if (!_ticketStore.TryGet(out var snapshot) || snapshot == null)
        {
            throw new WechatOpenPlatformException(
                "尚未收到 component_verify_ticket —— 该票据由微信后台推送，须由宿主接收后写入 "
                + "IComponentVerifyTicketStore，否则无法获取平台令牌。");
        }

        var body = BuildRequestBody(snapshot.Ticket);

        using var request = new HttpRequestMessage(HttpMethod.Post, OpenPlatformContract.ComponentTokenPath)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };

        var now = _clock.UtcNow;

        using var response = await _httpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);
        var payload = response.Content != null
            ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
            : string.Empty;

        var parsed = Parse(payload);

        if (!response.IsSuccessStatusCode)
        {
            throw new WechatOpenPlatformException(
                $"获取第三方平台令牌失败：HTTP {(int)response.StatusCode}。", parsed.ErrorCode);
        }

        // 开放平台的错误体常用 HTTP 200 + errcode 表达业务失败 ⇒ 必须先看 errcode。
        if (!string.IsNullOrWhiteSpace(parsed.ErrorCode))
        {
            throw new WechatOpenPlatformException(
                $"获取第三方平台令牌被拒：errcode={parsed.ErrorCode}, errmsg={parsed.ErrorMessage}。",
                parsed.ErrorCode);
        }

        if (string.IsNullOrWhiteSpace(parsed.AccessToken))
        {
            throw new WechatOpenPlatformException("开放平台应答中不含 component_access_token。");
        }

        // expires_in 缺失或非正数时回落到官方契约有效期（2 小时）：宁可用得保守，也不要算出「已过期」。
        var lifetime = parsed.ExpiresIn > 0
            ? TimeSpan.FromSeconds(parsed.ExpiresIn)
            : TimeSpan.FromSeconds(OpenPlatformContract.ComponentTokenLifetimeSeconds);

        return new ComponentAccessTokenState(parsed.AccessToken, now.Add(lifetime));
    }

    /// <summary>组装官方请求体（三字段，<b>零反射</b>，AOT 安全）。</summary>
    private string BuildRequestBody(string ticket)
    {
        using var buffer = new MemoryStream();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();
            writer.WriteString(OpenPlatformContract.ComponentAppIdField, _config.ComponentAppId);
            writer.WriteString(OpenPlatformContract.ComponentAppSecretField, _config.ComponentAppSecret);
            writer.WriteString(OpenPlatformContract.ComponentVerifyTicketField, ticket);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    /// <summary>解析官方应答（容错：非 JSON / 字段缺失 / <c>expires_in</c> 为字符串）。</summary>
    private static (string? AccessToken, long ExpiresIn, string? ErrorCode, string? ErrorMessage) Parse(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return (null, 0, null, null);
        }

        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return (null, 0, null, null);
            }

            return (
                ReadString(root, OpenPlatformContract.ComponentAccessTokenField),
                ReadInt64(root, OpenPlatformContract.ExpiresInField),
                ReadString(root, "errcode"),
                ReadString(root, "errmsg"));
        }
        catch (JsonException)
        {
            // 非 JSON（如网关错误页）：交由调用方按「无 code、无 token」处理（不抛出，避免掩盖 HTTP 状态）。
            return (null, 0, null, null);
        }
    }

    private static string? ReadString(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var element))
        {
            return null;
        }

        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            // errcode 官方有时以数字给出（如 40001）⇒ 归一为字符串，便于调用方统一分支。
            JsonValueKind.Number => element.GetRawText(),
            _ => null,
        };
    }

    private static long ReadInt64(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var element))
        {
            return 0;
        }

        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var value))
        {
            return value;
        }

        return element.ValueKind == JsonValueKind.String
               && long.TryParse(element.GetString(), out var parsed)
            ? parsed
            : 0;
    }
}
