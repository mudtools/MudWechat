// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档内容位置范围（官方 Range；编辑文档内容各操作的 ranges 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentRange
{
    /// <summary>获取或设置起始位置（官方 start_index，从 0 开始）。</summary>
    [JsonPropertyName("start_index")]
    public long? StartIndex { get; set; }

    /// <summary>获取或设置范围长度（官方 length）。</summary>
    [JsonPropertyName("length")]
    public long? Length { get; set; }
}
