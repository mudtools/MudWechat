// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 智能表信息（官方 SmartSheet Sheet；查询子表响应的 <c>sheet_list</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetSheetInfo
{
    /// <summary>获取或设置子表 ID（官方 <c>sheet_id</c>）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置子表名称（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置子表是否可见（官方 <c>is_visible</c>）。</summary>
    [JsonPropertyName("is_visible")]
    public bool? IsVisible { get; set; }

    /// <summary>
    /// 获取或设置子表类型（官方 <c>type</c>）。
    /// 官方取值：<c>dashboard</c> 仪表盘；<c>external</c> 说明页；<c>smartsheet</c> 智能表。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
