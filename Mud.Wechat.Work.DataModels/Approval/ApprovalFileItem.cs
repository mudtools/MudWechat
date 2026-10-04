// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 附件控件选项（files 元素，control 为 File）。
/// </summary>
/// <remarks>
/// <para>官方限制：目前一个审批申请单，全局仅支持上传 6 个附件，否则将失败。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalFileItem
{
    /// <summary>
    /// 获取或设置文件id（临时素材上传接口返回的media_id；提单后将作为单据内容转换为长期文件存储；微盘文件无法获取）。
    /// </summary>
    [JsonPropertyName("file_id")]
    public string? FileId { get; set; }

    /// <summary>
    /// 获取或设置文件名称，如果没有可以填空字符串。
    /// </summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>
    /// 获取或设置文件大小，如果没有可以填空字符串。
    /// </summary>
    /// <remarks>
    /// <para>官方文档对 file_size 的说明为「类型为number，如果没有可以填空字符串」，两处形态不一致；本模型以字符串承载以兼容两种形态。</para>
    /// </remarks>
    [JsonPropertyName("file_size")]
    public string? FileSize { get; set; }

    /// <summary>
    /// 获取或设置文件类型，如果没有可以填空字符串。
    /// </summary>
    [JsonPropertyName("file_type")]
    public string? FileType { get; set; }

    /// <summary>
    /// 获取或设置文件地址，如果没有可以填空字符串。
    /// </summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }
}
