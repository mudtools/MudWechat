// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions;

namespace Mud.Wechat.OpenPlatform.Demo;

/// <summary>一次推送的处置留痕（只记非敏感字段，票据与授权码原文不入此记录）。</summary>
/// <param name="Outcome">接收器给出的处置结果分类。</param>
/// <param name="InfoType">解密后的 <c>InfoType</c>（验签解密失败时为 <c>null</c>）。</param>
/// <param name="AuthorizerAppId">授权方 appid（仅授权变更事件有值）。</param>
/// <param name="HasAuthorizationCode">事件是否携带授权码（只记「有没有」，不记原文）。</param>
/// <param name="ReceivedAt">本地接收时刻。</param>
internal sealed record PushReceipt(
    string Outcome,
    string? InfoType,
    string? AuthorizerAppId,
    bool HasAuthorizationCode,
    DateTimeOffset ReceivedAt);

/// <summary>一次「授权码换令牌」的留痕。</summary>
/// <param name="AuthorizerAppId">授权方 appid。</param>
/// <param name="HasApiScope">是否拿到接口调用令牌（<c>false</c> 属正常形态：授权未含 API 权限）。</param>
/// <param name="RefreshTokenStored">刷新令牌是否已落库（长期凭据，丢失即须重新授权）。</param>
/// <param name="AccessTokenExpiresAt">接口调用令牌失效时刻（UTC）。</param>
/// <param name="ExchangedAt">本地换取时刻。</param>
internal sealed record AuthorizationRecord(
    string? AuthorizerAppId,
    bool HasApiScope,
    bool RefreshTokenStored,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset ExchangedAt);

/// <summary>
/// 开放平台 Demo 的进程内台账：推送处置留痕 + 最近一次未消费的授权码 + 换取结果。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要有「未消费的授权码」这一份</b>：官方在用户完成授权后，<c>authorized</c> 推送里就带了
/// <c>authorization_code</c>（10 分钟有效、一次性）。要跑通「票据 → 平台令牌 → 预授权 → 用户授权 → 换令牌」
/// 整条链，宿主要么从 <c>redirect_uri</c> 的 URL 参数里人工复制，要么直接从推送里取 ——
/// 后者让 Demo 可以一条命令走完全程，且<b>不需要把授权码回显到任何响应或日志里</b>
/// （AGENTS §8：auth_code 属不得暴露的凭据）。
/// </para>
/// <para>
/// <b>取后即清，且已消费的码不再重新入台账</b>：授权码是一次性的，重复换取必然失败（官方回 <c>40029</c>）。
/// 台账把它当作「一次性票据」交付给消费方，避免同一 Demo 里两次点击都拿同一个已失效的码去撞官方。
/// 只清不记留不住这个语义 —— 微信侧「没收到 success 就重试」是同一条推送再进来一次，
/// 若那条 <c>authorized</c> 的码已被消费，重试会把它<b>重新装满</b>台账，下一次换取凭空得到一个 40029，
/// 而 Demo 表面上「什么都没做错」。故消费时登记进有界集合，<see cref="Record"/> 见到旧码直接跳过。
/// </para>
/// <para>
/// 容量固定（<see cref="RecentCapacity"/>）并整体替换 —— 演示进程可能长驻，无上限的台账会让内存随推送增长。
/// </para>
/// </remarks>
internal sealed class OpenPlatformLedger
{
    private const int RecentCapacity = 20;

    /// <summary>已消费授权码的登记容量（超出即淘汰最旧一条；官方授权码本身只有 10 分钟寿命）。</summary>
    private const int ConsumedCapacity = 32;

    private readonly object _gate = new();
    private readonly List<PushReceipt> _recent = new();
    private readonly List<AuthorizationRecord> _authorizations = new();

    /// <summary>已消费授权码（HashSet 判重 + Queue 定序淘汰，二者由 <see cref="_gate"/> 同锁保护）。</summary>
    private readonly HashSet<string> _consumedAuthCodes = new(StringComparer.Ordinal);
    private readonly Queue<string> _consumedOrder = new();
    private (string? AuthorizerAppId, string AuthCode, DateTimeOffset ReceivedAt)? _pendingAuthCode;

    /// <summary>记录一次推送处置。</summary>
    /// <param name="result">接收器返回值。</param>
    /// <param name="receivedAt">本地接收时刻。</param>
    public void Record(Mud.Wechat.OpenPlatform.ComponentPushResult result, DateTimeOffset receivedAt)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        var evt = result.AuthorizerEvent;
        var hasCode = !string.IsNullOrWhiteSpace(evt?.AuthorizationCode);

        lock (_gate)
        {
            _recent.Insert(0, new PushReceipt(
                result.Outcome.ToString(),
                result.InfoType,
                evt?.AuthorizerAppId,
                hasCode,
                receivedAt));

            if (_recent.Count > RecentCapacity)
            {
                _recent.RemoveAt(_recent.Count - 1);
            }

            // 只有「授权成功 / 更新授权」带可用授权码；取消授权（unauthorized）官方不带。
            // 已消费过的旧码（微信「没收到 success 就重试」会再推一次）不得重新入台账 —— 见类注释。
            if (hasCode && !evt!.IsUnauthorized && !IsConsumed(evt.AuthorizationCode!))
            {
                _pendingAuthCode = (evt.AuthorizerAppId, evt.AuthorizationCode!, receivedAt);
            }
        }
    }

    /// <summary>取走最近一次未消费的授权码（取后即清，第二次取返回 <see langword="false"/>）。</summary>
    /// <param name="authorizerAppId">事件里的授权方 appid（官方可能缺省，故可空）。</param>
    /// <param name="authCode">授权码原文。</param>
    /// <param name="receivedAt">推送到达时刻（用于判断是否已超官方的 10 分钟窗口）。</param>
    /// <returns>是否存在未消费的授权码。</returns>
    public bool TryTakePendingAuthCode(out string? authorizerAppId, out string? authCode, out DateTimeOffset? receivedAt)
    {
        lock (_gate)
        {
            if (_pendingAuthCode == null)
            {
                authorizerAppId = null;
                authCode = null;
                receivedAt = null;
                return false;
            }

            var taken = _pendingAuthCode.Value;
            _pendingAuthCode = null;
            MarkConsumed(taken.AuthCode);
            authorizerAppId = taken.AuthorizerAppId;
            authCode = taken.AuthCode;
            receivedAt = taken.ReceivedAt;
            return true;
        }
    }

    /// <summary>记录一次换取结果（令牌原文不进台账）。</summary>
    /// <param name="tokens">换取到的令牌集合。</param>
    /// <param name="refreshTokenStored">刷新令牌是否已落库。</param>
    /// <param name="exchangedAt">本地换取时刻。</param>
    public void RecordAuthorization(
        Mud.Wechat.OpenPlatform.Abstractions.AuthorizerTokens tokens,
        bool refreshTokenStored,
        DateTimeOffset exchangedAt)
    {
        if (tokens == null)
        {
            throw new ArgumentNullException(nameof(tokens));
        }

        lock (_gate)
        {
            _authorizations.Insert(0, new AuthorizationRecord(
                tokens.AuthorizerAppId,
                tokens.HasApiScope,
                refreshTokenStored,
                tokens.AccessTokenExpiresAt,
                exchangedAt));

            if (_authorizations.Count > RecentCapacity)
            {
                _authorizations.RemoveAt(_authorizations.Count - 1);
            }
        }
    }

    /// <summary>清掉某个授权方的待用授权码（取消授权后旧码不该再被消费）。</summary>
    /// <param name="authorizerAppId">被取消授权的授权方 appid。</param>
    public void ClearPendingAuthCode(string? authorizerAppId)
    {
        if (string.IsNullOrWhiteSpace(authorizerAppId))
        {
            return;
        }

        lock (_gate)
        {
            if (string.Equals(_pendingAuthCode?.AuthorizerAppId, authorizerAppId, StringComparison.Ordinal))
            {
                _pendingAuthCode = null;
            }
        }
    }

    /// <summary>该授权码是否已被消费过（调用方须持有 <see cref="_gate"/>）。</summary>
    private bool IsConsumed(string authCode) => _consumedAuthCodes.Contains(authCode);

    /// <summary>登记一条已消费的授权码，超出容量淘汰最旧（调用方须持有 <see cref="_gate"/>）。</summary>
    private void MarkConsumed(string authCode)
    {
        if (!_consumedAuthCodes.Add(authCode))
        {
            return;
        }

        _consumedOrder.Enqueue(authCode);
        while (_consumedOrder.Count > ConsumedCapacity)
        {
            _consumedAuthCodes.Remove(_consumedOrder.Dequeue());
        }
    }

    /// <summary>最近的推送处置（新→旧）。</summary>
    public IReadOnlyList<PushReceipt> RecentPushes()
    {
        lock (_gate)
        {
            return _recent.ToArray();
        }
    }

    /// <summary>最近的换取结果（新→旧）。</summary>
    public IReadOnlyList<AuthorizationRecord> RecentAuthorizations()
    {
        lock (_gate)
        {
            return _authorizations.ToArray();
        }
    }

    /// <summary>是否存在未消费的授权码（不回显原文）。</summary>
    public bool HasPendingAuthCode
    {
        get
        {
            lock (_gate)
            {
                return _pendingAuthCode != null;
            }
        }
    }
}
