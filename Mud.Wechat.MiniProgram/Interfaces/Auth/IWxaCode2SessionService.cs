// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「登录」域 SDK —— <b>免令牌</b>通道（1 端点：<c>jscode2session</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<c>user-login/api_code2session.html</c>（小程序服务端 API，2026-10-09 核验）。
/// </para>
/// <para>
/// <b>为何免令牌（I4 裁决同形）</b>：本端点以 <c>appid</c> + <c>secret</c> 入参换取用户级会话
/// （<c>openid</c> + <c>session_key</c>），<b>不消费应用级 <c>access_token</c></b>；
/// 若声明 <c>[Token]</c>，SDK 会把应用级令牌错误注入本请求（语义错误）。守卫 MP-X2 锁定本裁决。
/// </para>
/// <para>
/// <b>红线 MP-X7</b>：<c>session_key</c> 是<b>用户会话密钥</b>，与企微回调 <c>DecryptedXml</c> 同级敏感
/// ——<b>不得</b>入日志 / 遥测 / 异常消息；SDK 只在 DTO 上承载，不做任何回显。
/// </para>
/// <para>
/// <b>与公众号 <c>/cgi-bin/token</c> 的关系</b>：同域（<c>api.weixin.qq.com</c>）但<b>不同端点、不同语义</b>
/// （后者签发应用级令牌，前者换用户级会话）—— 二者并存不构成重复（MP-X6 参照集不会命中）。
/// </para>
/// </remarks>
// HTTPCLIENT018：生成器要求显式声明 TokenManagerKey/TokenType。本接口是**免令牌端点**
// —— 换会话走 appid + secret，刻意不声明 [Token]；声明之反而会把应用级令牌注入本请求（错误语义）。
#pragma warning disable HTTPCLIENT018
[HttpClientApi(RegistryGroupName = "Auth", TokenManage = nameof(IMpAppManager))]
public interface IWxaCode2SessionService
{
    /// <summary>
    /// 登录凭证校验（<c>code2Session</c>）：用小程序端 <c>wx.login</c> 的 <c>code</c> 换取
    /// <c>openid</c> + <c>session_key</c>。官方文档：<c>user-login/api_code2session.html</c>。
    /// </summary>
    /// <param name="appId">小程序 AppID（官方 <c>appid</c>，必填）。</param>
    /// <param name="secret">小程序 AppSecret（官方 <c>secret</c>，必填；官方契约 Query 传，脱敏词表已覆盖）。</param>
    /// <param name="jsCode">小程序端 <c>wx.login</c> 返回的登录凭证（官方 <c>js_code</c>，必填）。</param>
    /// <param name="grantType">授权类型（官方 <c>grant_type</c>，必填；固定 <see cref="WxaGrantTypes.AuthorizationCode"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会话信息（<c>openid</c> / <c>session_key</c> / <c>unionid</c>），见 <see cref="WxaCode2SessionResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，无请求体；Query <c>appid</c> / <c>secret</c> / <c>js_code</c> / <c>grant_type</c> 均必填。
    /// </para>
    /// <para>
    /// <b>时序与一次性（官方原文要点）</b>：<c>js_code</c> 仅能使用一次、有效期 5 分钟；
    /// <c>session_key</c> 是<b>会话密钥</b>（用于解密用户数据、校验用户登录态），
    /// <b>不得下发到前端</b>、不得入日志（MP-X7）。
    /// </para>
    /// <para>
    /// <b><c>unionid</c> 返回条件（官方原文）</b>：仅当小程序已绑定微信开放平台账号<b>且</b>用户已关注
    /// 该开放平台账号下的公众号时返回；否则缺省（非空串）。
    /// </para>
    /// <para>官方错误码：<c>40029</c>（invalid code，code 无效或已使用）/ <c>45011</c>（频率限制）/ <c>40226</c>（高风险用户，官方建议拦截）。</para>
    /// <para><b>MUD005 不适用</b>：本接口无 <c>access_token</c>；但 <c>secret</c> 走 Query 属官方强制（脱敏词表已覆盖）。</para>
    /// </remarks>
    [Get("/sns/jscode2session")]
    Task<WxaCode2SessionResponse> Code2SessionAsync(
        [Query("appid")] string appId,
        [Query("secret")] string secret,
        [Query("js_code")] string jsCode,
        [Query("grant_type")] string grantType,
        CancellationToken cancellationToken = default);
}
#pragma warning restore HTTPCLIENT018

/// <summary>登录凭证校验的 <c>grant_type</c> 取值（官方固定值）。</summary>
public static class WxaGrantTypes
{
    /// <summary>授权码模式（<c>code2Session</c> 唯一取值）。</summary>
    public const string AuthorizationCode = "authorization_code";
}
