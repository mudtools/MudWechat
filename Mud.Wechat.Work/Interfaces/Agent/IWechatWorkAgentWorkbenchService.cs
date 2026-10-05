// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Agent;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「应用管理」模块「设置工作台自定义展示」域公共 SDK
/// （设置/获取应用在工作台展示的模版 + 设置/批量设置/获取应用在用户工作台展示的数据）。
/// <para>
/// 官方对三类应用开放一致的 5 个端点，收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalAgentWorkbenchService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyAgentWorkbenchService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderAgentWorkbenchService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// 官方权限口径：仅可设置当前凭证对应的应用；仅第三方应用、自建应用支持自定义展示；
/// 须先在管理后台启用自定义展示（管理后台—应用管理—应用—启用自定义展示）；
/// 设置用户数据时 userid 必须在应用可见范围内。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAgentWorkbenchService
{
    /// <summary>
    /// 设置应用在工作台展示的模版
    /// <para>指定应用在工作台自定义展示的模版类型（关键数据型 / 图片型 / 列表型 / 网页型），
    /// 可同时设置企业级默认数据；type 设为 "normal" 则取消自定义模式、切回普通展示模式。</para>
    /// <para>官方限制：须先在管理后台启用自定义展示；一个应用仅支持配置一种模版样式；
    /// <paramref name="request"/> 的 <c>replace_user_data</c> 设为 true 时覆盖所有用户当前数据（默认 false）；
    /// 跳转地址类型必须与应用主页匹配——主页为网页则仅配置 jump_url，为小程序则仅配置 pagepath，
    /// 类型不匹配时不同客户端平台/版本表现可能不一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetAgentWorkbenchTemplateRequest"/>：agentid / type / keydata / image / list / webview / replace_user_data）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92535"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94620"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96454"/></para>
    /// </remarks>
    [Post("/cgi-bin/agent/set_workbench_template")]
    Task<WechatWorkResponse> SetAgentWorkbenchTemplateAsync(
        [Body] SetAgentWorkbenchTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取应用在工作台展示的模版
    /// <para>获取应用当前设置的工作台自定义展示模版类型与企业级默认数据。</para>
    /// <para>官方限制：须先在管理后台启用自定义展示。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetAgentWorkbenchTemplateRequest"/>：agentid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>模版类型（type）与对应类型的模版数据（keydata / image / list / webview）、是否覆盖用户工作台数据（replace_user_data）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92535"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94620"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96454"/></para>
    /// </remarks>
    [Post("/cgi-bin/agent/get_workbench_template")]
    Task<GetAgentWorkbenchTemplateResponse> GetAgentWorkbenchTemplateAsync(
        [Body] GetAgentWorkbenchTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置应用在用户工作台展示的数据
    /// <para>为指定用户设置工作台自定义展示数据（模版类型 + 对应类型的模版数据）。</para>
    /// <para>官方限制：须先设置应用在工作台展示的模版；userid 必须在应用可见范围内；
    /// 频率限制：每个用户每个应用 10 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetAgentWorkbenchDataRequest"/>：agentid / userid / type / keydata / image / list / webview——四个模版数据字段官方平铺于请求体顶层）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92535"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94620"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96454"/></para>
    /// <para>频率限制：每个用户每个应用接口限制 10 次/分钟。</para>
    /// </remarks>
    [Post("/cgi-bin/agent/set_workbench_data")]
    Task<WechatWorkResponse> SetAgentWorkbenchDataAsync(
        [Body] SetAgentWorkbenchDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量设置应用在用户工作台展示的数据
    /// <para>为一批用户批量设置工作台自定义展示数据（与单用户设置同构，数据以 data 对象包裹）。</para>
    /// <para>官方限制：userid_list 最多 1000 个；userid 必须在应用可见范围内；
    /// 频率限制：每个应用 100000 人次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchSetAgentWorkbenchDataRequest"/>：agentid / userid_list / data——模版数据官方以 data 对象包裹，与单用户设置的平铺形态不同）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92535"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94620"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96454"/></para>
    /// <para>频率限制：每个应用接口限制 100000 人次/分钟。</para>
    /// </remarks>
    [Post("/cgi-bin/agent/batch_set_workbench_data")]
    Task<WechatWorkResponse> BatchSetAgentWorkbenchDataAsync(
        [Body] BatchSetAgentWorkbenchDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取应用在用户工作台展示的数据
    /// <para>获取指定用户当前的工作台自定义展示数据（模版类型 + 对应类型的模版数据）。</para>
    /// <para>官方限制：若设置了应用模版且配置 replace_user_data 为 true，应用数据会覆盖个人数据，
    /// 此时返回应用设置的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetAgentWorkbenchDataRequest"/>：agentid / userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户工作台展示数据（data：type + keydata / image / list / webview）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92535"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94620"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96454"/></para>
    /// </remarks>
    [Post("/cgi-bin/agent/get_workbench_data")]
    Task<GetAgentWorkbenchDataResponse> GetAgentWorkbenchDataAsync(
        [Body] GetAgentWorkbenchDataRequest request,
        CancellationToken cancellationToken = default);
}
