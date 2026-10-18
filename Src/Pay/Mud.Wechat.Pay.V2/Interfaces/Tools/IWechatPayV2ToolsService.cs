// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>工具</b>域 2 支端点（付款码查 openid / 转换短链接）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 商户侧，2026-10-18 逐页核验）：
/// 付款码查询 openid <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011940944"/>、
/// 转换短链接 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937195"/>。</para>
/// <para><b>两支均不需证书</b>：短链页「是否需要证书」为「否」；
/// 查 openid 页<b>通篇没有证书一栏</b>（官方页面本身未声明，SDK 按不需要证书处理并在此留痕）。</para>
/// <para><b>两支官方页面都不写「请求方式 / 数据格式」</b>：查 openid 页通篇没有该栏，
/// 短链页同样只给接口地址，POST + XML 只能从错误码表
/// （<c>REQUIRE_POST_METHOD</c>、<c>XML_FORMAT_ERROR</c>、<c>POST_DATA_EMPTY</c>）反推 ⇒
/// 本接口按 POST + XML 声明，理由记录在此而不是伪装成官方原文。</para>
/// <para><b>域名</b>：<c>api.mch.weixin.qq.com</c>；两页均未出现 <c>api2.mch</c>。</para>
/// <para><b>应答无 <c>err_code_des</c></b>：两支的失败原因都只在 <c>err_code</c> 一个字段里给。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "ToolsV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2ToolsService
{
    /// <summary>
    /// 付款码查询 openid（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011940944"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2AuthCodeToOpenIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户标识，见 <see cref="V2AuthCodeToOpenIdResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<c>/tools/authcodetoopenid</c>；请求面只有 5 个字段
    /// （<c>appid</c>/<c>mch_id</c>/<c>auth_code</c>/<c>nonce_str</c>/<c>sign</c>），
    /// <b>没有</b> <c>sign_type</c>，也<b>没有</b>服务商 <c>sub_*</c> 形态。</para>
    /// <para><b>付款码占用（官方原文）</b>：「调用查询后，该付款码只能由此商户号发起扣款，直至付款码更新」⇒
    /// 查过之后再用别的商户号扣同一张码会失败，这是本接口最主要的误用点。</para>
    /// <para><b>安全要求（官方原文）</b>：本接口仅用于取 openid；若随后要据此扣减用户资金/积分，
    /// 商户<b>须做二次身份校验</b>（短信验密、会员密码等）以防资损 ⇒ SDK 不会替调用方跳过这一步。</para>
    /// <para><b>失败码</b>：<c>AUTHCODEEXPIRE</c> 付款码过期 / <c>AUTH_CODE_ERROR</c> 付款码错误 /
    /// <c>AUTH_CODE_INVALID</c> 付款码检验错误 / <c>SYSTEMERROR</c>。</para>
    /// <para><b>不出身份标识</b>：应答的 <c>openid</c> 属用户身份标识，不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/tools/authcodetoopenid")]
    Task<V2AuthCodeToOpenIdResponse> AuthCodeToOpenIdAsync(
        V2AuthCodeToOpenIdRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 转换短链接（APIv2，Native 支付模式一的长链转短链）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937195"/>。
    /// </summary>
    /// <param name="request">转换请求体，见 <see cref="V2ShortUrlRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>短链接（<c>short_url</c>），见 <see cref="V2ShortUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<c>/tools/shorturl</c>；<c>sign_type</c> 选填，支持 MD5 与 HMAC-SHA256、默认 MD5。</para>
    /// <para><b>路径与字段名不同形</b>：路径是 <c>shorturl</c>（无下划线），应答字段是 <c>short_url</c>（有下划线）——
    /// 官方整页 <c>shorturl</c> 只出现在路径、<c>short_url</c> 只出现在字段名，DTO 按字段名建模。</para>
    /// <para><b>签名与编码的顺序（官方硬性）</b>：「需要转换的 URL，签名用原串，传输需 URLencode」⇒
    /// <see cref="V2ShortUrlRequest.LongUrl"/> 传<b>未编码原串</b>，编码由传输层负责；
    /// 提前编码会让签名与官方校验不一致（<c>SIGNERROR</c>）。</para>
    /// <para><b>门控形状</b>：官方把 <c>short_url</c> 与 <c>result_code</c> 放在同一张
    /// 由 <c>return_code</c> 把关的表里，<b>没有</b>第三级门控块。</para>
    /// <para><b>用途</b>：仅服务 Native 支付<b>模式一</b>（固定二维码），模式二无须转换。</para>
    /// </remarks>
    [Post("/tools/shorturl")]
    Task<V2ShortUrlResponse> ShortUrlAsync(
        V2ShortUrlRequest request, CancellationToken cancellationToken = default);
}
