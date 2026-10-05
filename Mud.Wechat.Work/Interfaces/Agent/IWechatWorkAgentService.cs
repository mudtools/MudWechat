// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Agent;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「应用管理」模块基础管理域公共 SDK（获取指定的应用详情 + 获取 access_token 对应的应用列表）。
/// <para>
/// 官方对三类应用开放一致的 2 个端点，收敛声明于本接口；应用类型子接口承载官方开放面差异端点：
/// 企业自建应用见 <see cref="IWechatWorkInternalAgentService"/>（额外开放「设置应用」，
/// 官方仅企业可调用、第三方以及代开发自建应用不可调用），
/// 第三方应用见 <see cref="IWechatWorkThirdPartyAgentService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderAgentService"/>（均为零差异端点空标记）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// 官方权限口径：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAgentService
{
    /// <summary>
    /// 获取指定的应用详情
    /// <para>获取指定应用（agentid）的详情：应用名称、头像、详情描述、可见范围（人员/部门/标签）、
    /// 是否停用、可信域名、地理位置上报、进入应用事件上报、应用主页 url 等。</para>
    /// <para>官方限制：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。
    /// <c>customized_publish_status</c> 仅代开发自建应用返回。</para>
    /// </summary>
    /// <param name="agentid">应用 id（官方必填；可在应用管理设置页查看）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应用详情（agentid / name / square_logo_url / description / allow_userinfos / allow_partys / allow_tags / close / redirect_domain / report_location_flag / isreportenter / home_url / customized_publish_status）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90227"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90363"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96448"/></para>
    /// </remarks>
    [Get("/cgi-bin/agent/get")]
    Task<GetAgentResponse> GetAgentAsync(
        [Query("agentid")] int agentid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 access_token 对应的应用列表
    /// <para>获取当前凭证对应的应用列表（应用 id、名称、方形头像 url）。</para>
    /// <para>官方限制：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应用列表（agentlist：agentid / name / square_logo_url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90227"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90363"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96448"/></para>
    /// </remarks>
    [Get("/cgi-bin/agent/list")]
    Task<GetAgentListResponse> GetAgentListAsync(
        CancellationToken cancellationToken = default);
}
