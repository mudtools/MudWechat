// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 编辑表格内容的单个操作结果（官方 UpdateResponse；responses 元素按操作类型仅承载其中一组响应）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSpreadsheetUpdateResponse
{
    /// <summary>获取或设置新增工作表响应（官方 add_sheet_response，<see cref="WedocSpreadsheetAddSheetResponse"/>）。</summary>
    [JsonPropertyName("add_sheet_response")]
    public WedocSpreadsheetAddSheetResponse? AddSheetResponse { get; set; }

    /// <summary>获取或设置删除工作表响应（官方 delete_sheet_response，<see cref="WedocSpreadsheetDeleteSheetResponse"/>）。</summary>
    [JsonPropertyName("delete_sheet_response")]
    public WedocSpreadsheetDeleteSheetResponse? DeleteSheetResponse { get; set; }

    /// <summary>获取或设置更新范围内单元格内容响应（官方 update_range_response，<see cref="WedocSpreadsheetUpdateRangeResponse"/>）。</summary>
    [JsonPropertyName("update_range_response")]
    public WedocSpreadsheetUpdateRangeResponse? UpdateRangeResponse { get; set; }

    /// <summary>获取或设置删除行列响应（官方 delete_dimension_response，<see cref="WedocSpreadsheetDeleteDimensionResponse"/>）。</summary>
    [JsonPropertyName("delete_dimension_response")]
    public WedocSpreadsheetDeleteDimensionResponse? DeleteDimensionResponse { get; set; }
}
