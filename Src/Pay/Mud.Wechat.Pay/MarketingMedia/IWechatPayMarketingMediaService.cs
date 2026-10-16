// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.MarketingMedia;

/// <summary>
/// 营销域图片上传服务（手工通道，<c>multipart/form-data</c>，<b>2 支</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不能声明式</b>：两支官方 <c>Content-Type</c> 均为 <c>multipart/form-data</c>，载荷是「文件二进制 +
/// <c>meta</c> JSON 对象」两个表单段，需自行计算 SHA-256 ⇒ 形态同 <c>IWechatPayMerchantMediaService</c>、
/// <c>IWechatPayComplaintMediaService</c> 与 <c>IWechatPayEcommerceCancelMediaService</c>。
/// <b>不计入生成式接口端点计数</b>（守卫 PAY-B2 与 <c>Baseline.Pay.Endpoints</c> 的既有口径一致）。
/// </para>
/// <para>
/// <b>两支官方页</b>：代金券/商家券侧图片上传（<c>POST /v3/marketing/favor/media/image-upload</c>）六页归并
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012557233"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012557243"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012557248"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012759802"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760240"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760270"/>；
/// 商品券侧图片上传（<c>POST /v3/marketing/partner/product-coupon/media/upload-image</c>）两页
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015781275"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016435731"/>。
/// </para>
/// <para>
/// <b>⚠️ 两支的应答字段名不同</b>：favor 侧回 <c>media_url</c>，product-coupon 侧回 <c>image_url</c>
/// ⇒ 不得把两个方法收敛成一个「返回 URL」的共用私有方法后按同一字段名解析。
/// </para>
/// <para>
/// <b>⚠️ <c>meta.filename</c> 长度上限不同</b>：favor 侧 string(128)，product-coupon 侧 string(150)；
/// 两支都要求文件名以 JPG / BMP / PNG 为后缀。<b>后缀校验留给宿主</b>，SDK 不做（官方无错误码字典说明该约束的失败码）。
/// </para>
/// </remarks>
public interface IWechatPayMarketingMediaService
{
    /// <summary>
    /// 上传营销图片，取 <c>media_url</c>（<c>POST /v3/marketing/favor/media/image-upload</c>）。
    /// </summary>
    /// <param name="content">图片内容流。</param>
    /// <param name="fileName">文件名（进入 <c>meta.filename</c>，官方 string(128)，须以 JPG/BMP/PNG 为后缀）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信返回的媒体文件标识 URL（<c>media_url</c>，官方注明<b>永久有效</b>）。</returns>
    Task<string> UploadFavorImageAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传商品券图片，取 <c>image_url</c>（<c>POST /v3/marketing/partner/product-coupon/media/upload-image</c>）。
    /// </summary>
    /// <param name="content">图片内容流。</param>
    /// <param name="fileName">文件名（进入 <c>meta.filename</c>，官方 string(150)，须以 JPG/BMP/PNG 为后缀）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信返回的媒体文件标识 URL（<c>image_url</c>，官方注明<b>永久有效</b>）。</returns>
    Task<string> UploadProductCouponImageAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);
}
