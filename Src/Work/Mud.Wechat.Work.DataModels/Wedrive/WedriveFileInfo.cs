// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘文件信息（获取文件列表 file_list.item、获取文件信息 file_info、重命名文件 file 与
/// 移动文件 file_list.item 共用的文件对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约差异：url 仅微文档类型返回访问链接；sha/md5 可用于校验与上传文件是否一致或避免重复上传，
/// 经文件分块上传的文件 md5 无效；file_type 各文档页枚举略有出入（获取文件列表页列 1~5，
/// 其余页列 1~6 含 6:文档(幻灯片)），本模型按 1~6 全量承载。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveFileInfo
{
    /// <summary>获取或设置文件 fileid。</summary>
    [JsonPropertyName("fileid")]
    public string? Fileid { get; set; }

    /// <summary>获取或设置文件名字。</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>获取或设置文件所在的空间 spaceid。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>获取或设置文件所在的目录 fileid（在根目录时为空间 spaceid）。</summary>
    [JsonPropertyName("fatherid")]
    public string? Fatherid { get; set; }

    /// <summary>获取或设置文件大小。</summary>
    [JsonPropertyName("file_size")]
    public ulong? FileSize { get; set; }

    /// <summary>获取或设置文件创建时间。</summary>
    [JsonPropertyName("ctime")]
    public ulong? Ctime { get; set; }

    /// <summary>获取或设置文件最后修改时间。</summary>
    [JsonPropertyName("mtime")]
    public ulong? Mtime { get; set; }

    /// <summary>
    /// 获取或设置文件类型：1 - 文件夹；2 - 文件；3 - 文档(文档)；4 - 文档(表格)；5 - 文档(收集表)；6 - 文档(幻灯片)。
    /// </summary>
    [JsonPropertyName("file_type")]
    public ulong? FileType { get; set; }

    /// <summary>获取或设置文件状态：1 - 正常；2 - 删除。</summary>
    [JsonPropertyName("file_status")]
    public ulong? FileStatus { get; set; }

    /// <summary>获取或设置文件 sha（可用于校验与上传文件是否一致或避免重复上传）。</summary>
    [JsonPropertyName("sha")]
    public string? Sha { get; set; }

    /// <summary>获取或设置文件 md5（经文件分块上传的文件该字段无效）。</summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }

    /// <summary>获取或设置访问链接（仅微文档类型返回）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
