// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「登录与用户」域 SDK —— 带令牌通道（7 端点）+ 免令牌登录端点（1 端点，方法级 opt-out）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：小程序服务端 API（<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/"/>），
/// 2026-10-09 逐页核验：
/// 校验 SessionKey <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-login/api_checksessionkey"/>、
/// 重置 SessionKey <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-login/api_resetusersessionkey"/>、
/// 获取手机号 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-info/phone-number/api_getphonenumber"/>、
/// 支付后 UnionID <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-info/basic-info/api_getpaidunionid"/>。
/// </para>
/// <para>
/// <b>免令牌登录端点（A-7 合并，2026-10-14 裁决）</b>：<c>jscode2session</c> 以 <c>appid</c> + <c>secret</c>
/// 换会话、<b>不消费应用级 access_token</b>，以<b>方法级</b> <c>[Token(Disabled = true)]</c> 显式免令牌
/// （上游 3.0.7 / F-1 起支持方法级覆盖接口级）——不再需要拆独立接口；原拆接口
/// <c>IWxaCode2SessionService</c> 已 <c>[Obsolete]</c>，一版后移除。
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
    /// 登录凭证校验（<c>code2Session</c>）：用小程序端 <c>wx.login</c> 的 <c>code</c> 换取
    /// <c>openid</c> + <c>session_key</c>。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-login/api_code2session"/>。
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
    /// <para>
    /// <b>A-7 合并（2026-10-14 裁决）</b>：本端点以 <c>appid</c> + <c>secret</c> 换会话、
    /// <b>不消费应用级 <c>access_token</c></b>——若随接口级 <c>[Token]</c> 注入应用级令牌属语义错误。
    /// 上游 3.0.7（F-1）起 <c>[Token(Disabled = true)]</c> 支持<b>方法级覆盖接口级</b>，
    /// 故免拆独立接口：方法级显式免令牌（不取令牌、不注入、不参与令牌恢复），
    /// 与同接口 7 个常规端点共存。原拆接口 <c>IWxaCode2SessionService</c> 已 <c>[Obsolete]</c>（一版后移除）。
    /// </para>
    /// </remarks>
    [Token(Disabled = true)]
    [Get("/sns/jscode2session")]
    Task<WxaCode2SessionResponse> Code2SessionAsync(
        [Query("appid")] string appId,
        [Query("secret")] string secret,
        [Query("js_code")] string jsCode,
        [Query("grant_type")] string grantType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 校验 <c>session_key</c> 是否有效。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-login/api_checksessionkey"/>。
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
    /// 重置 <c>session_key</c>。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-login/api_resetusersessionkey"/>。
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
    /// 获取手机号。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-info/phone-number/api_getphonenumber"/>。
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
    /// 支付后获取 UnionID。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-info/basic-info/api_getpaidunionid"/>。
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

    /// <summary>
    /// 获取插件用户 <c>openpid</c>。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-info/basic-info/api_getpluginopenpid"/>。
    /// </summary>
    /// <param name="code">插件用户标志凭证（官方 <c>code</c>，必填；经 <c>wx.pluginLogin</c> 取得）。</param>
    /// <param name="pluginAppId">插件 AppID（官方 <c>plugin_appid</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>插件用户唯一标识（<c>openpid</c>），见 <see cref="WxaGetPluginOpenPidResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>，无请求体；Query <c>access_token</c> / <c>code</c> / <c>plugin_appid</c>。</para>
    /// <para>
    /// <b>插件维度标识（官方原文）</b>：<c>openpid</c> 是<b>插件维度</b>的用户唯一标识
    /// （同一用户在<b>不同插件</b>下 openpid 不同），不可跨插件当 openid 使用。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>40029</c>（code 无效或已使用）。</para>
    /// </remarks>
    [Get("/wxa/getpluginopenpid")]
    Task<WxaGetPluginOpenPidResponse> GetPluginOpenPidAsync(
        [Query("code")] string code,
        [Query("plugin_appid")] string pluginAppId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查加密信息是否由微信生成。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-info/basic-info/api_checkencrypteddata"/>。
    /// </summary>
    /// <param name="request">加密消息哈希与用户标识，见 <see cref="WxaCheckEncryptedMsgRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>校验结论（<c>vaild</c>，官方原文拼写），见 <see cref="WxaCheckEncryptedMsgResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/checkencryptedmsg</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>能力边界（官方原文）</b>：当前只支持<b>手机号加密数据</b>，且只能检测<b>最近 3 天</b>生成的加密数据；
    /// <c>encrypted_msg_hash</c> = <c>to_hexstr(sha1(encrypted_msg))</c>。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>47001</c>（数据格式错误）。</para>
    /// </remarks>
    [Post("/wxa/business/checkencryptedmsg")]
    Task<WxaCheckEncryptedMsgResponse> CheckEncryptedMsgAsync(
        [Body] WxaCheckEncryptedMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户 <c>encryptKey</c>（用于端侧对用户数据做对称加密 / 解密）。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/user-info/internet/api_getuserencryptkey"/>。
    /// </summary>
    /// <param name="request">用户标识与会话签名，见 <see cref="WxaGetUserEncryptKeyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>密钥信息列表（<c>key_info_list</c>），见 <see cref="WxaGetUserEncryptKeyResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/getuserencryptkey</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>签名算法（官方原文）</b>：<c>signature = hmac_sha256(session_key, openid)</c>，
    /// <c>sig_method</c> 固定 <see cref="WxaSignatureMethods.HmacSha256"/>；<c>session_key</c> 不得入日志（MP-X7）。
    /// </para>
    /// </remarks>
    [Post("/wxa/business/getuserencryptkey")]
    Task<WxaGetUserEncryptKeyResponse> GetUserEncryptKeyAsync(
        [Body] WxaGetUserEncryptKeyRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>登录态签名算法取值（官方 <c>sig_method</c>）。</summary>
public static class WxaSignatureMethods
{
    /// <summary>HMAC-SHA256（官方当前唯一取值）。</summary>
    public const string HmacSha256 = "hmac_sha256";
}
