// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 查询子表请求体（<c>/cgi-bin/wedoc/smartsheet/get_sheet</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetSmartSheetSheetRequest
{
    /// <summary>获取或设置文档的 docid（官方必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置指定子表 ID，仅查询该子表（官方 <c>sheet_id</c>，非必填）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>
    /// 获取或设置是否获取所有类型子表（官方 <c>need_all_type_sheet</c>，非必填）。
    /// 为 <c>true</c> 时可获取包含仪表盘（<c>dashboard</c>）和说明页（<c>external</c>）在内的所有类型的子表。
    /// </summary>
    [JsonPropertyName("need_all_type_sheet")]
    public bool? NeedAllTypeSheet { get; set; }
}
