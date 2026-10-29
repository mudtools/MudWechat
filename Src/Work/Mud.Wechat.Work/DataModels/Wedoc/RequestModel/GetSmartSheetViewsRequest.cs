// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 查询视图请求体（<c>/cgi-bin/wedoc/smartsheet/get_views</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetSmartSheetViewsRequest
{
    /// <summary>获取或设置文档的 docid（官方必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置 Smartsheet 子表 ID（官方必填）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置需要查询的视图 ID 数组（官方 <c>view_ids</c>，非必填）。</summary>
    [JsonPropertyName("view_ids")]
    public List<string>? ViewIds { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>offset</c>，非必填），初始值为 0。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// 获取或设置分页大小，每页返回多少条数据（官方 <c>limit</c>，非必填）。
    /// 最大值为 1000；不填写或设置为 0 时，总数大于 1000 一次性返回 1000 个视图，总数小于 1000 时返回全部视图。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
