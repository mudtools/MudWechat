// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Media;

/// <summary>
/// 商户媒体文件上传服务（手工通道，multipart/form-data）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面</b>：四页归并为同一端点（更新于 2025.07-2026.01），支持商户【普通服务商】：
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760432">4012760432</see>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760490">4012760490</see>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760509">4012760509</see>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016276333">4016276333</see>。
/// </para>
/// <para>
/// <b>为何不能声明式</b>：本端点为 <c>multipart/form-data</c>，非 JSON 载荷；SDK 需自行构建表单、
/// 计算 SHA-256 摘要、序列化 <c>meta</c> JSON 对象。形态同 Fapiao 文件上传与 ComplaintMedia 通道。
/// </para>
/// <para>
/// <b>表单字段名</b>：<c>file</c>（文件）+ <c>meta{filename, sha256}</c>（JSON 对象）。
/// </para>
/// </remarks>
public interface IWechatPayMerchantMediaService
{
    /// <summary>
    /// 上传图片/视频文件（<c>POST /v3/merchant/media/upload</c>）。
    /// </summary>
    /// <param name="content">文件内容流。</param>
    /// <param name="fileName">文件名（用于 <c>meta.filename</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>媒体 ID（<c>media_id</c>）。</returns>
    /// <remarks>
    /// <para><b>文件大小限制</b>：图片 ≤10M，视频 ≤1G（官方文档未明确数字，按既有经验值）。</para>
    /// <para><b>SHA-256 计算</b>：SDK 内部使用 BCL 内置 <c>System.Security.Cryptography.SHA256</c> 计算。</para>
    /// <para><b>四页归并</b>：四个文档页是同一契约在不同产品章节的重复成文，字段表逐字相同 ⇒ 单端点承载。</para>
    /// </remarks>
    Task<string> UploadMediaAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传视频文件专用通道（<c>POST /v3/merchant/media/video_upload</c>）。
    /// </summary>
    /// <param name="content">视频文件内容流。</param>
    /// <param name="fileName">文件名（用于 <c>meta.filename</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>媒体 ID（<c>media_id</c>）。</returns>
    /// <remarks>
    /// <para><b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012761084">4012761084</see></para>
    /// <para><b>与 upload 的差异</b>：路由不同（<c>video_upload</c>），但表单字段名相同（<c>file</c> + <c>meta</c>）。</para>
    /// </remarks>
    Task<string> UploadVideoAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);
}
