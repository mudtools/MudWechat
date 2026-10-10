// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Identity;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「身份验证」模块小程序登录域第三方应用 SDK（套件级）。
/// <para>
/// 官方对第三方应用开放的是<b>独立路由</b>端点（code2Session：
/// <c>/cgi-bin/service/miniprogram/jscode2session</c>），以 <c>suite_access_token</c>（套件级凭证）鉴权，
/// 与企业自建/代开发的 <c>access_token</c> 端点分属不同令牌路由键，故经零端点套件父接口
/// <see cref="IWechatWorkIdentityMiniProgramSuiteService"/> 独立成族、由本接口承载端点。
/// 第三方响应较自建/代开发多 <c>open_userid</c> 字段（见 <see cref="Code2Session3rdResponse"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.SuiteAccessToken"/>（Query 注入 <c>suite_access_token</c>，
/// 参数名已在组件 <c>SensitiveUrlRedactor</c> 词表内，无 G7 豁免负担）；
/// <c>suite_access_token</c> 为套件级凭证、无企业 scope，不经由令牌作用域机制表达。
/// 官方凭证口径：必须由该小程序关联的第三方应用的 secret 获取。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>suite_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Identity",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkIdentityMiniProgramSuiteService))]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkThirdPartyIdentityMiniProgramSuiteService : IWechatWorkIdentityMiniProgramSuiteService
{
    /// <summary>
    /// code2Session（小程序登录凭证校验，第三方独立路由，官方即 GET）
    /// <para>凭小程序 <c>wx.qy.login</c> 取得的 js_code 换取用户身份：返回用户所属企业的
    /// corpid、用户在企业内的 userid、会话密钥 session_key，以及第三方专属的
    /// open_userid（同一服务商下不同应用获取同一成员的 open_userid 相同）。
    /// 企业微信的 jscode2session 请求 url 与微信的不同，且返回的是 userid 而微信返回的是 openid。</para>
    /// <para>官方固定 Query：grant_type 此处固定为 authorization_code（本 SDK 以方法级固定
    /// Query 参数发射，调用方无需传入）。</para>
    /// </summary>
    /// <param name="jsCode">登录时获取的 code（官方必填，即 <c>wx.qy.login</c> 返回的 js_code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户所属企业（corpid）、企业内用户身份（userid）、会话密钥（session_key）
    /// 与服务商维度全局唯一身份（open_userid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用·小程序登录校验（code2Session）</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92423"/></para>
    /// <para>
    /// 安全约束：session_key 是对用户数据进行加密签名的密钥，为了应用自身的数据安全，
    /// 开发者服务器不应该把会话密钥下发到小程序，也不应该对外提供这个密钥。
    /// </para>
    /// <para>企业自建/服务商代开发走 <c>/cgi-bin/miniprogram/jscode2session</c>（access_token），
    /// 见 <see cref="IWechatWorkIdentityMiniProgramService"/>（官方文档 91507/96959）。</para>
    /// </remarks>
    [Get("/cgi-bin/service/miniprogram/jscode2session")]
    [Query("grant_type", "authorization_code")]
    Task<Code2Session3rdResponse> Code2SessionAsync(
        [Query("js_code")] string jsCode,
        CancellationToken cancellationToken = default);
}
