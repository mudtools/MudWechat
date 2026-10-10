// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 更新范围内单元格内容操作（官方 UpdateRangeRequest）。
/// <para>官方业务限制：范围行数 &lt;= 1000、列数 &lt;= 200、范围内总单元格数量 &lt;= 10000。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSpreadsheetUpdateRangeRequest
{
    /// <summary>获取或设置工作表唯一标识（官方 sheet_id）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置写入指定区域的数据（官方 grid_data，<see cref="WedocGridData"/>）。</summary>
    [JsonPropertyName("grid_data")]
    public WedocGridData? GridData { get; set; }
}
