// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘文件信息列表（获取文件列表与移动文件响应的 file_list 对象，官方形态为对象内含 item 数组）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveFileList
{
    /// <summary>获取或设置文件信息列表。</summary>
    [JsonPropertyName("item")]
    public List<WedriveFileInfo>? Item { get; set; }
}
