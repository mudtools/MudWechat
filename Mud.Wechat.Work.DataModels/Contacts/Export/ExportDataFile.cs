// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Export;

/// <summary>
/// 导出数据文件信息（获取导出结果响应中 <c>data_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Export")]
public class ExportDataFile
{
    /// <summary>
    /// 获取或设置数据下载链接（有效期 2 个小时，支持指定 Range 头分段下载）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置密文数据大小（字节）。
    /// </summary>
    [JsonPropertyName("size")]
    public long? Size { get; set; }

    /// <summary>
    /// 获取或设置密文数据 MD5 校验值。
    /// </summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }
}
