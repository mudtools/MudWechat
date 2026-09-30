// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.Services.Authorization.Models;

namespace Mud.Wechat.Work.Services.Authorization;

/// <summary>
/// 企业微信授权编排服务：把官方授权链路的 6 个步骤（预授权码 → 授权链接 → 回调 → 换码 → 落库 → 刷授权信息）
/// 收敛为一次性调用。
/// </summary>
/// <remarks>
/// <para>
/// 全部方法支持显式 <c>appKey</c>（R7）：传入时内部包在
/// <c>IWechatAppContextSwitcher.UseApp(appKey)</c> 内执行，未传则使用默认应用
/// （<see cref="WechatAuthorizationOptions.DefaultAppKey"/> 优先，缺省为注册的默认应用）。
/// </para>
/// <para>
/// 令牌获取路径按应用类型分流（K1）：第三方应用 <c>get_corp_token</c>；服务商代开发 <c>gettoken</c>。
/// </para>
/// </remarks>
public interface IWechatWorkAuthorizationService
{
    // ── 授权发起 ────────────────────────────────────────────────

    /// <summary>第三方应用：取预授权码并拼装企业授权安装链接（可先 <c>set_session_info</c> 下发授权配置）。</summary>
    /// <param name="request">授权链接生成请求。</param>
    /// <param name="appKey">目标应用键；为 null 使用默认应用。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>授权安装链接（含预授权码与有效期）。</returns>
    Task<WechatAuthorizationUrl> CreateSuiteAuthorizationUrlAsync(
        WechatAuthorizationUrlRequest request, string? appKey = null, CancellationToken cancellationToken = default);

    /// <summary>代开发：取带参授权链接（官方二维码 URL，宿主自行渲染为二维码）。</summary>
    /// <param name="state">授权方标识（≤32 字节，仅 <c>a-zA-Z0-9</c>）。</param>
    /// <param name="templateIdList">代开发模板 id 列表（≤9 个）。</param>
    /// <param name="appKey">目标应用键；为 null 使用默认应用。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>带参授权二维码链接。</returns>
    Task<WechatCustomizedAuthUrl> CreateCustomizedAuthorizationUrlAsync(
        string state, IReadOnlyList<string> templateIdList, string? appKey = null, CancellationToken cancellationToken = default);

    // ── 授权落地 ────────────────────────────────────────────────

    /// <summary>临时授权码换取永久授权码并落库（幂等：同 authCode 并发/重复调用收敛为一次落库）。</summary>
    /// <param name="authCode">临时授权码。</param>
    /// <param name="appKey">目标应用键；为 null 使用默认应用。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>落库后的企业授权聚合。</returns>
    Task<WechatCorpAuthorization> ExchangeAuthCodeAsync(
        string authCode, string? appKey = null, CancellationToken cancellationToken = default);

    /// <summary>以 <c>get_auth_info</c>（v2）刷新授权企业信息并回写仓储。</summary>
    /// <param name="authCorpId">授权方（企业）CorpId。</param>
    /// <param name="appKey">目标应用键；为 null 使用默认应用。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>刷新后的企业授权聚合。</returns>
    Task<WechatCorpAuthorization> RefreshAuthorizationAsync(
        string authCorpId, string? appKey = null, CancellationToken cancellationToken = default);

    // ── 授权消费 / 维护 ─────────────────────────────────────────

    /// <summary>查询本地授权（仓储读，未命中返回 null）。</summary>
    /// <param name="authCorpId">授权方（企业）CorpId。</param>
    /// <param name="appKey">目标应用键；为 null 使用默认应用。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>企业授权聚合；未命中为 null。</returns>
    Task<WechatCorpAuthorization?> GetAuthorizationAsync(
        string authCorpId, string? appKey = null, CancellationToken cancellationToken = default);

    /// <summary>枚举本应用已授权企业。</summary>
    /// <param name="appKey">目标应用键；为 null 使用默认应用。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>企业授权聚合列表。</returns>
    Task<IReadOnlyList<WechatCorpAuthorization>> ListAuthorizationsAsync(
        string? appKey = null, CancellationToken cancellationToken = default);

    /// <summary>撤销/清理授权（清仓储 + 级联失效该企业令牌）。</summary>
    /// <param name="authCorpId">授权方（企业）CorpId。</param>
    /// <param name="appKey">目标应用键；为 null 使用默认应用。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task RevokeAuthorizationAsync(
        string authCorpId, string? appKey = null, CancellationToken cancellationToken = default);
}