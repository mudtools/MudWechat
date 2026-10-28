// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 编辑表格内容的单个更新操作（官方 UpdateRequest）。
/// <para>每个操作对象只能填一个操作字段。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSpreadsheetUpdateOperation
{
    /// <summary>获取或设置新增工作表操作（官方 add_sheet_request，<see cref="WedocSpreadsheetAddSheetRequest"/>）。</summary>
    [JsonPropertyName("add_sheet_request")]
    public WedocSpreadsheetAddSheetRequest? AddSheetRequest { get; set; }

    /// <summary>获取或设置删除工作表操作（官方 delete_sheet_request，<see cref="WedocSpreadsheetDeleteSheetRequest"/>）。</summary>
    [JsonPropertyName("delete_sheet_request")]
    public WedocSpreadsheetDeleteSheetRequest? DeleteSheetRequest { get; set; }

    /// <summary>获取或设置更新范围内单元格内容操作（官方 update_range_request，<see cref="WedocSpreadsheetUpdateRangeRequest"/>）。</summary>
    [JsonPropertyName("update_range_request")]
    public WedocSpreadsheetUpdateRangeRequest? UpdateRangeRequest { get; set; }

    /// <summary>获取或设置删除表格连续的行或列操作（官方 delete_dimension_request，<see cref="WedocSpreadsheetDeleteDimensionRequest"/>）。</summary>
    [JsonPropertyName("delete_dimension_request")]
    public WedocSpreadsheetDeleteDimensionRequest? DeleteDimensionRequest { get; set; }
}
