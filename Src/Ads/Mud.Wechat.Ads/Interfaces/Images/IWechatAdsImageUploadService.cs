// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Images;

namespace Mud.Wechat.Ads;

/// <summary>
/// 图片上传通道（<c>POST /v3.0/images/add</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不在 <see cref="IWechatAdsImageService"/> 里</b>：官方本支的
/// <c>Content-Type</c> 是 <c>multipart/form-data</c>（文件字段 <c>file</c>），
/// 声明式 <c>[HttpClientApi]</c> 面承载不了文件流（组件对复杂类型走「逐属性展平」、无文件部分语义）⇒
/// 走手写 <see cref="Mud.HttpUtils.Clients.IEnhancedHttpClient"/> 传输
/// （形态照支付线 <c>WechatPayFapiaoFileService</c>）：注入带令牌 Handler 的 <see cref="IAdsHttpClient"/>，
/// 由传输层照常成组注入 <c>access_token</c> / <c>timestamp</c> / <c>nonce</c>（守卫 ADS-B1 的注入语义不变）。
/// 本页未另列 <c>user_token</c>。
/// </para>
/// <para><b>上传方式二选一</b>（<c>upload_type</c> 可选值
/// <c>{UPLOAD_TYPE_FILE, UPLOAD_TYPE_BYTES}</c>，2026-10-11 枚举详情核验；常量见实现类
/// <c>AdsMaterialUploadService.UploadTypeFile</c> / <c>UploadTypeBytes</c>）：
/// 文件方式用 <paramref name="file"/>（文件流，<b>不</b>整体读入内存）、
/// 字节串方式用 <paramref name="bytes"/>（base64 文本）。
/// 官方尺寸上限未逐项核验（图片素材一般 ≤10MB），越界由应答 <c>code</c> 表达。</para>
/// </remarks>
public interface IWechatAdsImageUploadService
{
    /// <summary>上传图片。</summary>
    /// <param name="accountId">账户 id（表单字段 <c>account_id</c>）。</param>
    /// <param name="uploadType">上传方式（表单字段 <c>upload_type</c>，必填；
    /// 取值见 <see cref="UploadTypeFile"/> / <see cref="UploadTypeBytes"/>）。</param>
    /// <param name="signature">图片签名（表单字段 <c>signature</c>，必填，MD5）。</param>
    /// <param name="file">图片文件流（<c>UPLOAD_TYPE_FILE</c> 时必填；由调用方提供与释放语义，
    /// 本方法<b>不</b>关闭调用方传入的流）。</param>
    /// <param name="fileName">文件名（<c>file</c> 部分的 filename；<paramref name="file"/> 非空时必填）。</param>
    /// <param name="bytes">base64 文件内容（<c>UPLOAD_TYPE_BYTES</c> 时必填）。</param>
    /// <param name="imageUsage">图片用途（表单字段 <c>image_usage</c>，<c>enum</c>；可选值未逐项核验）。</param>
    /// <param name="description">描述（表单字段 <c>description</c>）。</param>
    /// <param name="organizationId">组织 id（表单字段 <c>organization_id</c>）。</param>
    /// <param name="resizeWidth">缩放宽度（表单字段 <c>resize_width</c>，<c>integer</c>）。</param>
    /// <param name="resizeHeight">缩放高度（表单字段 <c>resize_height</c>，<c>integer</c>）。</param>
    /// <param name="resizeFileSize">缩放后目标大小（表单字段 <c>resize_file_size</c>，<c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>上传结果（图片 id、宽高、大小等 9 字段）。</returns>
    Task<AdsImageUploadResponse> UploadAsync(
        long? accountId,
        string uploadType,
        string signature,
        Stream? file = null,
        string? fileName = null,
        string? bytes = null,
        string? imageUsage = null,
        string? description = null,
        long? organizationId = null,
        long? resizeWidth = null,
        long? resizeHeight = null,
        long? resizeFileSize = null,
        CancellationToken cancellationToken = default);
}
