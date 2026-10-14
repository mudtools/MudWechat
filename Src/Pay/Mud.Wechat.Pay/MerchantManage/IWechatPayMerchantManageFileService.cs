// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Download;

namespace Mud.Wechat.Pay.MerchantManage;

/// <summary>
/// 商户管理记录文件上传服务（手工通道，multipart/form-data）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940425">4014940425</see>
/// （更新于 2025.07.18，支持商户【普通服务商】）。
/// </para>
/// <para>
/// <b>为何不能声明式</b>：本端点为 <c>multipart/form-data</c>，非 JSON 载荷；SDK 需自行构建表单、
/// 计算 SHA-256 摘要、序列化 <c>meta</c> JSON 对象。形态同 Fapiao 文件上传与 BillDownload 二进制流通道。
/// </para>
/// </remarks>
public interface IWechatPayMerchantManageFileService
{
    /// <summary>
    /// 上传管理记录文件（<c>POST /v3/mch-manage/submission-files/sub-mchid/{sub_mchid}/upload</c>）。
    /// </summary>
    /// <param name="subMchId">子商户号（path 参数）。</param>
    /// <param name="content">文件内容流。</param>
    /// <param name="fileName">文件名（用于 <c>meta.file_name</c>）。</param>
    /// <param name="itemId">资料项 ID（用于 <c>meta.item_id</c>）。</param>
    /// <param name="manageRecordId">商户管理记录 ID（选填，用于 <c>meta.manage_record_id</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>文件上传结果（含 <c>file_id</c>）。</returns>
    /// <remarks>
    /// <para><b>表单字段名</b>：<c>submission_file</c>（文件）+ <c>meta</c>（JSON 对象）。</para>
    /// <para><b>SHA-256 计算</b>：SDK 内部使用 BCL 内置 <c>System.Security.Cryptography.SHA256</c> 计算。</para>
    /// <para><b>文件大小限制</b>：官方未明确上限，建议 ≤10MB。</para>
    /// </remarks>
    Task<string> UploadSubmissionFileAsync(
        string subMchId,
        Stream content,
        string fileName,
        string itemId,
        long? manageRecordId = null,
        CancellationToken cancellationToken = default);
}
