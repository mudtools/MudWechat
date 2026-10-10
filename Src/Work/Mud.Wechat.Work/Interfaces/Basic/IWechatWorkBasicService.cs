// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Basic;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「基础接口」域公共 SDK（获取企业微信接口IP段 + 获取企业微信回调IP段，
/// 即官方「服务端API → 开发指南 → 基础接口」分组下的两个端点）。
/// <para>
/// 二者均为企业侧防火墙放行配置的基础设施端点：前者返回开发者调用企业微信端的接入 API 域名
/// （<c>qyapi.weixin.qq.com</c>）解析地址，后者返回企业微信回调企业指定 URL 时的来源 IP 段。
/// </para>
/// <para>
/// 官方对<b>企业自建应用与服务商代开发开放完全一致的 2 个端点</b>（同路由同契约），
/// 全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalBasicService"/>，服务商代开发见 <see cref="IWechatWorkProviderBasicService"/>。
/// </para>
/// <para>
/// 官方第三方应用开发文档树<b>无「基础接口」分组</b>，故本域不设第三方子接口
/// （形态对齐政民沟通·配置网格结构族：父接口 + 自建/代开发两个空标记子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 本域消费的令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——服务商代开发为授权企业级令牌，
/// 调用前须经 <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId)</c> 建立「应用 + 企业」作用域后再调用。
/// </para>
/// <para>
/// 官方权限说明：两个端点均为「无限定」，不额外要求应用配置到任何白名单。
/// </para>
/// <para>
/// 官方业务约束：IP 段有变更可能，当 IP 段变更时，新旧 IP 段会同时保留一段时间；
/// 官方建议企业<b>每天定时拉取 IP 段</b>、更新防火墙设置，避免因 IP 段变更导致网络不通
/// （两端点均无独立的数值型频率限制，走官方全局访问频率限制）。
/// </para>
/// <para>
/// 官方说明：自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkBasicService
{
    /// <summary>
    /// 获取企业微信接口IP段
    /// <para>API 域名 IP 即 <c>qyapi.weixin.qq.com</c> 的解析地址，由开发者调用企业微信端的接入 IP；
    /// 如果企业需要做防火墙配置，可以通过本端点获取到所有相关的 IP 段。</para>
    /// <para>官方权限说明：无限定。请求无业务参数、无请求体，仅经 <c>access_token</c> 鉴权。</para>
    /// <para>
    /// 官方业务约束：IP 段有变更可能，当 IP 段变更时，新旧 IP 段会同时保留一段时间；
    /// 官方建议企业每天定时拉取 IP 段、更新防火墙设置，避免因 IP 段变更导致网络不通。
    /// </para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业微信服务器 IP 段（ip_list 字符串数组；失败时官方返回空数组 + errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92520"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97073"/></para>
    /// <para>官方错误码：42001 <c>access_token</c> 过期（官方失败示例返回 <c>ip_list</c> 空数组 + 42001）。</para>
    /// </remarks>
    [Get("/cgi-bin/get_api_domain_ip")]
    Task<GetApiDomainIpResponse> GetApiDomainIpAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业微信回调IP段
    /// <para>企业微信在回调企业指定的 URL 时，是通过特定的 IP 发送出去的；
    /// 如果企业需要做防火墙配置，可以通过本端点获取到所有相关的 IP 段。</para>
    /// <para>官方权限说明：无限定。请求无业务参数、无请求体，仅经 <c>access_token</c> 鉴权。</para>
    /// <para>
    /// 官方业务约束：IP 段有变更可能，当 IP 段变更时，新旧 IP 段会同时保留一段时间；
    /// 官方建议企业每天定时拉取 IP 段、更新防火墙设置，避免因 IP 段变更导致网络不通。
    /// </para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业微信回调 IP 段（ip_list 字符串数组；失败时官方返回空数组 + errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92521"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98988"/></para>
    /// <para>
    /// 官方文档页正文另附一份「目前回调 IP 列表」静态清单，并明确标注仅供参考、最新 IP 列表以本接口返回结果为准；
    /// 该静态清单不建模于本 SDK（会随官方调整而漂移），仅以本端点返回值为唯一事实来源。
    /// </para>
    /// <para>官方错误码：42001 <c>access_token</c> 过期（官方失败示例返回 <c>ip_list</c> 空数组 + 42001）。</para>
    /// </remarks>
    [Get("/cgi-bin/getcallbackip")]
    Task<GetCallbackIpResponse> GetCallbackIpAsync(CancellationToken cancellationToken = default);
}
