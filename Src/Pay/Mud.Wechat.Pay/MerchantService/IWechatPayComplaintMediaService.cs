// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Download;

namespace Mud.Wechat.Pay.MerchantService;

/// <summary>
/// 微信支付消费者投诉<b>图片通道</b>（上传 / 拉取）。
/// </summary>
/// <remarks>
/// <para><b>为何独立于声明式端点</b>：官方《商户上传反馈图片》的请求体是 <c>multipart/form-data</c>
/// （表单字段 <c>file</c> + <c>meta</c>），而 <c>[HttpClientApi]</c> 只承载 JSON；
/// 《获取投诉单图片》的应答是二进制流而非 JSON ⇒ 两者都无法用声明式表达。</para>
/// <para><b>形态与既有先例一致</b>：参照 <see cref="Fapiao.IWechatPayFapiaoFileService"/>（上传）
/// 与 <see cref="Download.IWechatPayBillDownloadService"/>（下载）。</para>
/// <para><b>官方文档</b>（2026-10-14 逐页核验）：上传
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467250"/>、获取
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467251"/>。<b>支持商户：【普通商户】</b>。</para>
/// <para><b>图片规格限制</b>（官方原文）：<c>.jpg</c>/<c>.bmp</c>/<c>.png</c>，不超过 <b>2M</b>。</para>
/// </remarks>
public interface IWechatPayComplaintMediaService
{
    /// <summary>
    /// 「商户上传反馈图片」：POST <c>/v3/merchant-service/images/upload</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467250"/>。
    /// </summary>
    /// <param name="content">图片内容流（官方：仅支持 jpg/bmp/png，不超过 2M）。</param>
    /// <param name="fileName">文件名（随表单 <c>file</c> 部分提交）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>媒体 ID（<c>media_id</c>），见 <see cref="MerchantServiceUploadMediaResponse"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> 为 <c>null</c>。</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> 为空白。</exception>
    Task<MerchantServiceUploadMediaResponse> UploadComplaintImageAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取投诉单图片」：GET <c>/v3/merchant-service/images/{media_id}</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467251"/>。
    /// </summary>
    /// <param name="mediaId">媒体文件标识 ID（path <c>media_id</c>，必填 string(256)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>图片流结果（含 Content-Type 与文件名）。</returns>
    Task<WechatPayBillDownloadResult> GetComplaintImageAsync(
        string mediaId,
        CancellationToken cancellationToken = default);
}
