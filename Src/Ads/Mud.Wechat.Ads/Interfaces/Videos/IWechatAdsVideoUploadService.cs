// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Videos;

namespace Mud.Wechat.Ads;

/// <summary>
/// 视频上传通道（<c>POST /v3.0/videos/add</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不在 <see cref="IWechatAdsVideoService"/> 里</b>：官方本支的
/// <c>Content-Type</c> 是 <c>multipart/form-data</c>（文件字段 <c>video_file</c>），
/// 声明式 <c>[HttpClientApi]</c> 面承载不了文件流 ⇒ 走手写传输（形态照
/// <see cref="IWechatAdsImageUploadService"/> 与支付线 <c>WechatPayFapiaoFileService</c>）。
/// 本页未另列 <c>user_token</c>。官方尺寸上限未逐项核验（视频素材一般 ≤10MB），越界由应答 <c>code</c> 表达。
/// </para>
/// </remarks>
public interface IWechatAdsVideoUploadService
{
    /// <summary>上传视频。</summary>
    /// <param name="accountId">账户 id（表单字段 <c>account_id</c>）。</param>
    /// <param name="signature">视频签名（表单字段 <c>signature</c>，必填，MD5）。</param>
    /// <param name="videoFile">视频文件流（表单字段 <c>video_file</c>，必填；调用方提供，本方法不关闭该流）。</param>
    /// <param name="fileName">文件名（<c>video_file</c> 部分的 filename，必填）。</param>
    /// <param name="description">描述（表单字段 <c>description</c>）。</param>
    /// <param name="adcreativeTemplateId">创意形式 id（表单字段 <c>adcreative_template_id</c>，<c>integer</c>）。</param>
    /// <param name="organizationId">组织 id（表单字段 <c>organization_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>上传结果（<c>video_id</c> + <c>cover_image_id</c>）。</returns>
    Task<AdsVideoUploadResponse> UploadAsync(
        long? accountId,
        string signature,
        Stream videoFile,
        string fileName,
        string? description = null,
        long? adcreativeTemplateId = null,
        long? organizationId = null,
        CancellationToken cancellationToken = default);
}
