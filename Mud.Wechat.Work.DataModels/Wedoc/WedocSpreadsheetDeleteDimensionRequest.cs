// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 删除表格连续的行或列操作（官方 DeleteDimensionRequest）。
/// <para>官方业务限制：删除范围为左闭右开 [start_index, end_index)，若 end_index &lt;= start_index 则该请求报错；
/// 该操作会导致表格缩表。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSpreadsheetDeleteDimensionRequest
{
    /// <summary>获取或设置工作表唯一标识（官方 sheet_id）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置删除的维度（官方 dimension）：ROW - 行，COLUMN - 列。</summary>
    [JsonPropertyName("dimension")]
    public string? Dimension { get; set; }

    /// <summary>获取或设置删除行列的起始序号（官方 start_index，从 1 开始）。</summary>
    [JsonPropertyName("start_index")]
    public long? StartIndex { get; set; }

    /// <summary>获取或设置删除行列的终止序号（官方 end_index，从 1 开始）。</summary>
    [JsonPropertyName("end_index")]
    public long? EndIndex { get; set; }
}
