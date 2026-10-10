// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「登录与用户」域 SDK —— <b>带令牌</b>通道（4 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：小程序服务端 API（<c>https://developers.weixin.qq.com/miniprogram/dev/server/API/</c>），
/// 2026-10-09 逐页核验：
/// 校验 SessionKey <c>user-login/api_checksessionkey.html</c>、
/// 重置 SessionKey <c>user-login/api_resetusersessionkey.html</c>、
/// 获取手机号 <c>user-info/phone-number/api_getphonenumber.html</c>、
/// 支付后 UnionID <c>user-info/basic-info/api_getpaidunionid.html</c>。
/// </para>
/// <para>
/// <b>本域为何拆两个接口（对齐公众号线 <c>IMpOpenApiService</c> / <c>IMpOpenApiTokenFreeService</c> 先例）</b>：
/// 登录 <c>jscode2session</c> 以 <c>appid</c> + <c>secret</c> 换会话、<b>不消费应用级 access_token</b>，
/// 而 <c>[Token]</c> 是<b>接口级</b>特性（作用于该接口全部方法）⇒ 把它与带令牌端点数混写会让登录端点
/// 被错误注入应用级令牌。故免令牌端点独立成
/// <c>IWxaCode2SessionService</c>，两者同挂 <c>Auth</c> 注册组。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>
/// ——小程序与公众号<b>同平台、同令牌域</b>，不新增令牌类型常量（新增会让
/// <c>MpTokenManagerRegistry</c> 单槽 <c>Resolve</c> 返回 <c>null</c> ⇒ errcode 令牌自愈<b>静默失效</b>）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Auth", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaAuthService
{
    /// <summary>
    /// 校验 <c>session_key</c> 是否有效。官方文档：<c>user-login/api_checksessionkey.html</c>。
    /// </summary>
    /// <param name="signature">用户登录态签名（官方 <c>signature</c>，必填；对 <c>session_key</c> 做 HMAC-SHA256 所得）。</param>
    /// <param name="sigMethod">签名算法（官方 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示有效）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>，无请求体；Query <c>access_token</c> / <c>signature</c> / <c>sig_method</c>。</para>
    /// <para>
    /// <b>签名算法（官方原文）</b>：<c>signature = hmac_sha256(session_key, rawData)</c>，
    /// 其中 <c>rawData</c> 为小程序端 <c>wx.checkSession</c> 场景下参与签名的原始字符串；
    /// <b>签名归小程序端与宿主</b>，SDK 只透传（不在服务端重算 —— 服务端拿不到 <c>session_key</c> 之外的端上输入）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>87009</c>（signature 校验失败）。</para>
    /// </remarks>
    [Get("/wxa/checksession")]
    Task<WxaResponse> CheckSessionAsync(
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重置 <c>session_key</c>。官方文档：<c>user-login/api_resetusersessionkey.html</c>。
    /// </summary>
    /// <param name="signature">用户登录态签名（官方 <c>signature</c>，必填）。</param>
    /// <param name="sigMethod">签名算法（官方 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示重置成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>，无请求体；Query <c>access_token</c> / <c>signature</c> / <c>sig_method</c>。</para>
    /// <para>
    /// <b>重置后旧 <c>session_key</c> 立即失效</b>（官方语义）：小程序端须重新 <c>wx.login</c> 换新会话，
    /// 故本接口<b>不得</b>用于例行调用（会造成端上登录态抖动）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>87009</c>（signature 校验失败）。</para>
    /// </remarks>
    [Get("/wxa/resetusersessionkey")]
    Task<WxaResponse> ResetUserSessionKeyAsync(
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取手机号。官方文档：<c>user-info/phone-number/api_getphonenumber.html</c>。
    /// </summary>
    /// <param name="request">手机号请求体（<c>code</c>），见 <see cref="WxaGetPhoneNumberRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>手机号信息（含 <c>watermark</c>），见 <see cref="WxaGetPhoneNumberResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"code":…}</c>；Query 携带 <c>access_token</c>；
    /// 必带 <c>Content-Type: application/json</c>。
    /// </para>
    /// <para>
    /// <b>付费与次数限制（官方原文要点，勿弱化）</b>：本能力<b>按次计费</b>（须在小程序后台开通并充值），
    /// 且<b>有调用次数上限</b>；<c>code</c> 由小程序端 <c>&lt;button open-type="getPhoneNumber"&gt;</c>
    /// 回调取得、<b>一次性且不可复用</b>（重复使用报 <c>40029</c> 类无效 code）。
    /// </para>
    /// <para>
    /// <b>水印校验（官方要求）</b>：响应 <c>phone_info.watermark</c> 携带 <c>timestamp</c> + <c>appid</c>，
    /// 宿主须校验 <c>appid</c> 与自身一致以防数据来源伪造。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>40029</c>（code 无效）。</para>
    /// </remarks>
    [Post("/wxa/business/getuserphonenumber")]
    Task<WxaGetPhoneNumberResponse> GetPhoneNumberAsync(
        [Body] WxaGetPhoneNumberRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 支付后获取 UnionID。官方文档：<c>user-info/basic-info/api_getpaidunionid.html</c>。
    /// </summary>
    /// <param name="openId">用户标识（官方 <c>openid</c>，必填）。</param>
    /// <param name="transactionId">微信支付订单号（官方 <c>transaction_id</c>，与 <paramref name="outTradeNo"/> <b>二选一</b>）。</param>
    /// <param name="outTradeNo">商户订单号（官方 <c>out_trade_no</c>，与 <paramref name="transactionId"/> <b>二选一</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>UnionID（<c>unionid</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，无请求体；Query <c>access_token</c> / <c>openid</c> ＋
    /// （<c>transaction_id</c> 或 <c>out_trade_no</c> 二选一，<b>两者都传时以 <c>transaction_id</c> 为准</b>）。
    /// </para>
    /// <para>
    /// <b>前置约束（官方原文）</b>：须<b>在用户支付完成后</b>调用（未支付或订单不属于该用户时报错）；
    /// 且小程序须已绑定微信开放平台账号（否则 <c>unionid</c> 无从产生）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>9300501</c>（订单不存在或不匹配）。</para>
    /// </remarks>
    [Get("/wxa/getpaidunionid")]
    Task<WxaGetPaidUnionIdResponse> GetPaidUnionIdAsync(
        [Query("openid")] string openId,
        [Query("transaction_id")] string? transactionId = null,
        [Query("out_trade_no")] string? outTradeNo = null,
        CancellationToken cancellationToken = default);
}

/// <summary>登录态签名算法取值（官方 <c>sig_method</c>）。</summary>
public static class WxaSignatureMethods
{
    /// <summary>HMAC-SHA256（官方当前唯一取值）。</summary>
    public const string HmacSha256 = "hmac_sha256";
}
