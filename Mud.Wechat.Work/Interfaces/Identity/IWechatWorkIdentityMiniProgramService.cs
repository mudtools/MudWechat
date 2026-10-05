// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Identity;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「身份验证」模块小程序登录域公共 SDK
/// （code2Session 登录凭证校验：小程序 <c>wx.qy.login</c> 取得的 js_code 经本端点换取用户身份与会话密钥）。
/// <para>
/// 官方对自建应用与服务商代开发开放完全一致的 1 个端点（<c>/cgi-bin/miniprogram/jscode2session</c>），
/// 收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalIdentityMiniProgramService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderIdentityMiniProgramService"/>。
/// 第三方应用走独立路由 <c>/cgi-bin/service/miniprogram/jscode2session</c>
///（suite_access_token 鉴权，响应多 open_userid 字段），见 <see cref="IWechatWorkIdentityMiniProgramSuiteService"/> 接口族，
/// 不在本接口收敛面内。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 自建应用消费应用自身 access_token；服务商代开发消费授权企业级 access_token（scope = authCorpId）——
/// 令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经
/// <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId)</c> 建立「应用 + 企业」作用域后再调用。
/// </para>
/// <para>
/// 官方约束（权限说明）：access_token 必须是由该小程序关联的企业微信应用 secret 所获得；
/// 获取 access_token 时请使用企业的 corpid 参数，请勿使用小程序的 appid。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkIdentityMiniProgramService
{
    /// <summary>
    /// code2Session（小程序登录凭证校验，官方即 GET）
    /// <para>凭小程序 <c>wx.qy.login</c> 取得的 js_code 换取用户身份：返回用户所属企业的
    /// corpid、用户在企业内的 userid 与会话密钥 session_key。企业微信的 jscode2session 请求 url
    /// 与微信的不同，且返回的是 userid 而微信返回的是 openid。</para>
    /// <para>官方固定 Query：grant_type 此处固定为 authorization_code（官方参数表单列，
    /// 本 SDK 以方法级固定 Query 参数发射，调用方无需传入）。</para>
    /// </summary>
    /// <param name="jsCode">登录时获取的 code（官方必填，即 <c>wx.qy.login</c> 返回的 js_code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户所属企业（corpid）、企业内用户身份（userid）与会话密钥（session_key）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用·小程序登录校验（code2Session）</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91507"/></para>
    /// <para><b>服务商代开发·小程序登录校验（code2Session）</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96959"/></para>
    /// <para>
    /// 安全约束：session_key 是对用户数据进行加密签名的密钥，为了应用自身的数据安全，
    /// 开发者服务器不应该把会话密钥下发到小程序，也不应该对外提供这个密钥。
    /// </para>
    /// </remarks>
    [Get("/cgi-bin/miniprogram/jscode2session")]
    [Query("grant_type", "authorization_code")]
    Task<Code2SessionResponse> Code2SessionAsync(
        [Query("js_code")] string jsCode,
        CancellationToken cancellationToken = default);
}
