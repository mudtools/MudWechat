// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 在指定位置插入表格操作（官方 InsertTable）。
/// <para>官方业务限制：行数 &lt;= 100，列数 &lt;= 60，单元格总数 &lt;= 1000。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentInsertTable
{
    /// <summary>获取或设置表格行数（官方 rows）。</summary>
    [JsonPropertyName("rows")]
    public long? Rows { get; set; }

    /// <summary>获取或设置表格列数（官方 cols）。</summary>
    [JsonPropertyName("cols")]
    public long? Cols { get; set; }

    /// <summary>获取或设置插入位置（官方 location，<see cref="WedocDocumentLocation"/>）。</summary>
    [JsonPropertyName("location")]
    public WedocDocumentLocation? Location { get; set; }
}
