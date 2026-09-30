// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调事件处理器：按事件类型分发到对应仓储（对齐 Mud.Feishu.Webhook 的处理语义）。
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><c>suite_ticket</c> → 写 <see cref="IWechatSuiteTicketStore"/>（驱动 get_suite_token）；</item>
/// <item><c>change_auth</c> → 级联失效该企业的授权企业令牌（缓存 + Store 双清，
/// 下次调用按最新 permanent_code 重新拉取）；</item>
/// <item><c>cancel_auth</c> / <c>del_auth</c> → 清理 <see cref="IWechatCorpAuthStore"/>
/// 中该企业的永久授权码并级联失效其企业令牌。</item>
/// </list>
/// </remarks>
public sealed class WechatCallbackHandler
{
    private readonly IWechatSuiteTicketStore _suiteTicketStore;
    private readonly IWechatCorpAuthStore _corpAuthStore;
    private readonly IWechatAppManager? _appManager;
    private readonly ILogger<WechatCallbackHandler> _logger;

    /// <summary>创建回调事件处理器。</summary>
    public WechatCallbackHandler(
        IWechatSuiteTicketStore suiteTicketStore,
        IWechatCorpAuthStore corpAuthStore,
        ILogger<WechatCallbackHandler> logger,
        IWechatAppManager? appManager = null)
    {
        _suiteTicketStore = suiteTicketStore ?? throw new ArgumentNullException(nameof(suiteTicketStore));
        _corpAuthStore = corpAuthStore ?? throw new ArgumentNullException(nameof(corpAuthStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appManager = appManager;
    }

    /// <summary>处理回调事件（按 InfoType 分发；未知事件仅记录 Debug 日志）。</summary>
    public async Task HandleAsync(WechatCallbackEvent evt, CancellationToken cancellationToken = default)
    {
        if (evt == null) throw new ArgumentNullException(nameof(evt));

        if (evt.IsSuiteTicket)
        {
            if (string.IsNullOrEmpty(evt.SuiteTicket))
            {
                _logger.LogWarning("收到 suite_ticket 事件但 SuiteTicket 为空，忽略。SuiteId: {SuiteId}", evt.SuiteId);
                return;
            }

            await _suiteTicketStore.SetAsync(evt.SuiteTicket, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("suite_ticket 已入库（SuiteId: {SuiteId}）。", evt.SuiteId);
            return;
        }

        if (evt.IsChangeAuth)
        {
            _logger.LogInformation("收到授权变更事件（AuthCorpId: {AuthCorpId}），级联失效其企业令牌。", evt.AuthCorpId);
            await InvalidateCorpTokenAsync(evt.AuthCorpId, cancellationToken).ConfigureAwait(false);
            return;
        }

        if (evt.IsCancelAuth)
        {
            _logger.LogInformation("收到取消授权事件（AuthCorpId: {AuthCorpId}），清理永久授权码并失效企业令牌。", evt.AuthCorpId);
            if (!string.IsNullOrEmpty(evt.AuthCorpId))
            {
                await _corpAuthStore.RemoveAsync(evt.AuthCorpId, cancellationToken).ConfigureAwait(false);
            }

            await InvalidateCorpTokenAsync(evt.AuthCorpId, cancellationToken).ConfigureAwait(false);
            return;
        }

        _logger.LogDebug("收到未处理的企业微信回调事件（InfoType: {InfoType}）。", evt.InfoType);
    }

    private async Task InvalidateCorpTokenAsync(string? authCorpId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(authCorpId) || _appManager == null)
        {
            return;
        }

        try
        {
            // 对每个已配置的第三方/服务商应用级联失效该企业的授权企业令牌
            //（企业级令牌以 authCorpId 为 scope；多应用互不串扰，逐应用清理）。
            foreach (var appKey in _appManager.ConfiguredAppKeys)
            {
                try
                {
                    await _appManager.InvalidateTokenAsync(
                        appKey, WechatTokenTypes.AccessToken, new[] { authCorpId }, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (InvalidOperationException)
                {
                    // 非第三方/服务商应用没有企业级令牌管理器，跳过。
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "级联失效企业令牌失败（AuthCorpId: {AuthCorpId}）。", authCorpId);
        }
    }
}
