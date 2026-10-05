// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Agent;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「应用管理」模块基础管理域企业自建应用 SDK。
/// <para>
/// 官方对自建应用开放与三类应用公共面一致的 2 端点（继承自
/// <see cref="IWechatWorkAgentService"/>），并额外开放 1 个差异端点：
/// 「设置应用」（官方仅企业可调用——第三方以及代开发自建应用不可调用，
/// 见 <see cref="IWechatWorkThirdPartyAgentService"/> 与 <see cref="IWechatWorkProviderAgentService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：仅企业可调用，可设置当前凭证对应的应用；第三方以及代开发自建应用不可调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Agent",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAgentService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalAgentService : IWechatWorkAgentService
{
    /// <summary>
    /// 设置应用
    /// <para>设置当前凭证对应应用的自定义字段：地理位置上报、应用头像、应用名称、应用详情、可信域名、
    /// 进入应用事件上报、应用主页 url。</para>
    /// <para>官方限制：仅企业可调用（第三方以及代开发自建应用不可调用）；可设置当前凭证对应的应用；
    /// 应用名称不超过 32 个 utf8 字符；应用详情 4 至 120 个 utf8 字符；可信域名需通过域名所有权校验，
    /// 否则 jssdk 受限（错误码 85005）；应用主页 url 必须以 http 或 https 开头。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetAgentRequest"/>：agentid / report_location_flag / logo_mediaid / name / description / redirect_domain / isreportenter / home_url）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90228"/></para>
    /// <para>官方权限：仅企业可调用，可设置当前凭证对应的应用；第三方以及代开发自建应用不可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/agent/set")]
    Task<WechatWorkResponse> SetAgentAsync(
        [Body] SetAgentRequest request,
        CancellationToken cancellationToken = default);
}
