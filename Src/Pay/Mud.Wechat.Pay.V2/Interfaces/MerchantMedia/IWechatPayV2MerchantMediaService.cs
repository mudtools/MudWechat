// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>商户媒体（资质图片）</b>域 1 支端点 —— 本线唯一的 <c>multipart/form-data</c> 面。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 跨境侧，2026-10-18 逐页核验）：
/// 上传图片 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013641477"/>。</para>
/// <para><b>为什么手写</b>：本页「是否需要证书」为<b>「是」</b> ⇒ 与 <see cref="IWechatPayV2DepositSecureService"/>
/// 同一理由（证书按商户绑定，声明式管道拿不到），走 <c>WechatPayV2SecureInvoker</c> 的逐商户证书通道。
/// 更硬的一条理由是<b>载体</b>：请求是表单（文件分节 + 若干字段分节），不是本线统一的 XML 体，
/// 声明式内容序列化器无法表达文件分节。</para>
/// <para><b>域名口径（本接口的关键契约点）</b>：官方本页<b>同时印了两个域名</b> ——
/// 「接口说明」栏给 <c>apihk.mch.weixin.qq.com</c>，而同一页的 curl 示例给
/// <c>api.mch.weixin.qq.com</c>。这是官方源文的自相矛盾，SDK 取<b>接口说明栏</b>
/// （<see cref="WechatPayV2HttpClientNames.GlobalBaseAddress"/>）为准，
/// 与 <see cref="IWechatPayV2InstitutionSubService"/> 同域 —— 理由是本支产出的
/// <c>media_id</c> 正是该族进件字段的取值，两支属同一套跨境流程。</para>
/// <para><b>频率上限</b>：官方无数字，仅在错误码表给出 <c>FREQUENCY_LIMITED</c>「操作过快，请稍后重试」。</para>
/// <para><b>不出密钥</b>（PAY-B7）：异常消息不含密钥、<c>sign</c>、签名原文或证书材料。</para>
/// </remarks>
public interface IWechatPayV2MerchantMediaService
{
    /// <summary>
    /// 上传图片并换取 <c>media_id</c>（APIv2，需证书，<c>multipart/form-data</c>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013641477"/>。
    /// </summary>
    /// <param name="request">表单字段请求体（<c>mch_id</c> + <c>media_hash</c> + 选填 <c>sign_type</c>），
    /// 见 <see cref="V2UploadMediaRequest"/>。</param>
    /// <param name="media">图片内容流。<b>所有权仍归调用方</b>：本方法不关闭该流，
    /// 由 <c>using</c> 或调用方自行释放。</param>
    /// <param name="fileName">文件名（表单分节的 <c>filename</c>，官方要求带扩展名，取
    /// <c>jpeg</c>/<c>jpg</c>/<c>bmp</c>/<c>png</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>媒体标识与验签字段，见 <see cref="V2UploadMediaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/mch/uploadmedia</c>；请求 <c>multipart/form-data</c>，
    /// 文件分节名固定为 <c>media</c>；<b>应答仍是 XML</b> ⇒ 应答 <c>sign</c> 照本线口径做验签。</para>
    /// <para><b>文件不参与签名</b>：官方原文「不参与签名计算」⇒ <c>sign</c> 只覆盖
    /// <c>mch_id</c>/<c>media_hash</c>（及选填 <c>sign_type</c>）这几个字段分节。
    /// 因此 <c>media_hash</c> 写错（含大写十六进制）不会被签名拦下，而是被官方的图片校验拦成
    /// <c>PARAM_ERROR｜图片文件错误</c>。</para>
    /// <para><b>限制</b>：<c>jpeg</c>/<c>jpg</c>/<c>bmp</c>/<c>png</c>，图片<b>不超过 2M</b>；
    /// <c>media_hash</c> 为图片内容的 <b>MD5 小写</b>十六进制。</para>
    /// <para><b>产物用途</b>：<c>media_id</c> 供机构子商户进件的 <c>registration_certificate_copy</c>
    /// 与 <c>store_photos</c>（后者是 <c>media_id</c> 的 JSON 数组文本）引用。</para>
    /// </remarks>
    Task<V2UploadMediaResponse> UploadMediaAsync(
        V2UploadMediaRequest request,
        Stream media,
        string fileName,
        CancellationToken cancellationToken = default);
}
