// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Agent;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「应用管理」模块「自建应用迁移成代开发应用」域 SDK（官方仅服务商代开发章节提供，1 端点）。
/// <para>
/// 双令牌契约（官方原文）：<c>access_token</c> 为 <b>URL 参数</b>——待迁移或尚未验证归属的
/// <b>自建应用</b>的接口调用凭证，经本接口的 Query 注入自动携带（调用时应用上下文须为该自建应用，
/// AppType = Internal，凭据归属域 <see cref="WechatTokenManagerKeys.InternalAccessToken"/>）；
/// <c>suite_access_token</c> 为<b>包体参数</b>——代开发应用模板接口调用凭证（获取方式与第三方应用凭证相同），
/// 官方不经令牌作用域机制表达，由调用方在 <see cref="ClaimAgentCustomizedAppRequest"/> 请求体中显式传入。
/// </para>
/// <para>官方业务背景：企业微信禁止服务商以自建应用的方式为企业开发产品，
/// 仅历史遗留的自建应用可迁移；服务商也可在服务商管理端提交自建应用列表进行迁移，
/// 但管理端迁移后若需重置应用 secret 或提交上线审核，仍须先调用本接口完成应用归属校验。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费待迁移自建应用自身的 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// Query 注入 <c>access_token</c>）；调用方须先持有该自建应用的 corpsecret 并以其注册应用上下文。
/// </para>
/// <para>
/// <c>suite_access_token</c> 为套件级敏感凭证：不得记录日志、遥测或异常消息。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Agent",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAgentMigrationService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalAgentMigrationService : IWechatWorkAgentMigrationService
{
    /// <summary>
    /// 自建应用迁移成代开发应用
    /// <para>服务商将企业侧的历史遗留自建应用迁移为代开发应用（完成应用归属校验，应用归属转为服务商代开发模式）。</para>
    /// <para>官方限制：仅历史遗留的自建应用可迁移（企业微信禁止服务商以自建应用的方式为企业开发产品）；
    /// 管理端迁移后，若需重置应用 secret 或提交上线审核，须先调用本接口完成归属校验；
    /// suite_access_token 为代开发应用模板接口调用凭证，获取方式与第三方应用凭证相同（依赖 suite_ticket 推送机制）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ClaimAgentCustomizedAppRequest"/>：suite_access_token——代开发应用模板接口调用凭证，官方包体参数）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99617"/></para>
    /// <para>官方权限：仅服务商可调用；access_token 为待迁移自建应用的接口调用凭证（URL 参数），
    /// suite_access_token 为代开发应用模板的调用凭证（包体参数）。</para>
    /// </remarks>
    [Post("/cgi-bin/agent/claim_customized_app")]
    Task<WechatWorkResponse> ClaimCustomizedAppAsync(
        [Body] ClaimAgentCustomizedAppRequest request,
        CancellationToken cancellationToken = default);
}
