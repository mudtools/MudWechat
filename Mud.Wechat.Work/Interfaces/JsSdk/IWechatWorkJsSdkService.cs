// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.JsSdk;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「JS-SDK」模块公共 SDK（获取企业 jsapi_ticket + 获取应用 jsapi_ticket，
/// 为 <c>wx.config</c>（企业应用鉴权 getConfigSignature）与 <c>wx.agentConfig</c>
/// （应用鉴权 getAgentConfigSignature）两个鉴权入口提供服务端票据换取能力）。
/// <para>
/// 官方对自建应用、第三方应用与服务商代开发开放完全一致的 2 个端点，全部收敛声明于本接口；
/// 应用类型子接口均为零差异端点空标记：自建应用见 <see cref="IWechatWorkInternalJsSdkService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyJsSdkService"/>，服务商代开发见 <see cref="IWechatWorkProviderJsSdkService"/>。
/// </para>
/// <para>
/// JS-SDK 签名算法本身为页面/服务器端约定（对拼接串做 SHA-1），不属于 HTTP 端点面，
/// 官方三份文档（自建 90506 / 第三方 90539 / 代开发 96909）正文逐字一致，
/// 签名规则随端点写入各方法 <c>&lt;remarks&gt;</c>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——
/// 第三方/代开发的企业级令牌须先经 <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId)</c>
/// 建立「应用 + 企业」作用域后再调用。
/// </para>
/// <para>
/// 官方约束：获取 jsapi_ticket 的接口有非常严格的调用频率限制
///（一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次），
/// 开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkJsSdkService
{
    /// <summary>
    /// 获取企业 jsapi_ticket
    /// <para>企业的 jsapi_ticket 是企业页面调用企业微信 JS 接口的临时票据，用于企业应用鉴权
    ///（getConfigSignature，配合 <c>wx.config</c> 注入完成 JS-SDK 基础接口的权限校验）。</para>
    /// <para>
    /// 签名算法：参与签名的参数为 jsapi_ticket、noncestr（随机字符串）、timestamp（当前时间戳，单位为秒）
    /// 与 url（当前页面的 URL，不包含「#」及后面部分），按
    /// <c>jsapi_ticket=JSAPI_TICKET&amp;noncestr=NONCESTR&amp;timestamp=TIMESTAMP&amp;url=URL</c>
    /// 顺序拼接后对拼接串做 SHA-1，结果即为 JS 接口签名——只需按上述规则进行拼接，
    /// 不要改变参数顺序，不要进行 URL encode；出于安全考虑，官方要求开发者必须在服务器端实现签名的逻辑。
    /// </para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生成签名所需的 jsapi_ticket（ticket，最长 512 字节）与凭证的有效时间（expires_in，秒）。</returns>
    /// <remarks>
    /// <para>
    /// 频率限制：获取 jsapi_ticket 的接口有非常严格的调用频率限制
    ///（一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次），
    /// 开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存；
    /// 正常情况下 jsapi_ticket 的有效期为 7200 秒（2 小时），具体过期时间需要参考接口返回的 expires_in 属性。
    /// </para>
    /// <para><b>企业自建应用·JS-SDK 签名算法</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90506"/></para>
    /// <para><b>第三方应用·JS-SDK 签名算法</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90539"/></para>
    /// <para><b>服务商代开发·JS-SDK 签名算法</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96909"/></para>
    /// </remarks>
    [Get("/cgi-bin/get_jsapi_ticket")]
    Task<GetCorpJsapiTicketResponse> GetCorpJsapiTicketAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取应用 jsapi_ticket
    /// <para>应用的 jsapi_ticket 是应用调用企业微信 JS 接口的临时票据，用于应用鉴权
    ///（getAgentConfigSignature，配合 <c>wx.agentConfig</c> 注入完成更多 JS-SDK 接口的权限校验）。</para>
    /// <para>
    /// 官方请求地址以固定 Query 参数携带 <c>type=agent_config</c>（参数表不再单列），
    /// 本 SDK 以方法级固定 Query 参数发射该值；签名算法与企业 jsapi_ticket 完全一致
    ///（见 <see cref="GetCorpJsapiTicketAsync"/>，同样不要改变参数顺序、不要进行 URL encode，
    /// 并须在服务器端实现签名逻辑）。
    /// </para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生成签名所需的 jsapi_ticket（ticket，最长 512 字节）与凭证的有效时间（expires_in，秒）。</returns>
    /// <remarks>
    /// <para>
    /// 频率限制：官方在「获取企业 jsapi_ticket」节对获取 jsapi_ticket 的接口统一标注
    /// 非常严格的调用频率限制（一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次），
    /// 开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存；
    /// 正常情况下 jsapi_ticket 的有效期为 7200 秒（2 小时），具体过期时间需要参考接口返回的 expires_in 属性。
    /// </para>
    /// <para><b>企业自建应用·JS-SDK 签名算法</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90506"/></para>
    /// <para><b>第三方应用·JS-SDK 签名算法</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90539"/></para>
    /// <para><b>服务商代开发·JS-SDK 签名算法</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96909"/></para>
    /// </remarks>
    [Get("/cgi-bin/ticket/get")]
    [Query("type", "agent_config")]
    Task<GetAgentJsapiTicketResponse> GetAgentJsapiTicketAsync(CancellationToken cancellationToken = default);
}
