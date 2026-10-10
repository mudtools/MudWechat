// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 排序字段项（官方 SortSpec 的 <c>sort_infos</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方「更新视图」文档的参数表列名原文显示为 <c>sort_infoes.desc</c>（多一个 e），判断为官方文档排版错误；官方请求/响应示例均为 <c>desc</c>，故本模型按 <c>desc</c> 承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetSortInfo
{
    /// <summary>获取或设置参与排序的字段 ID（官方 <c>field_id</c>，必填）。</summary>
    [JsonPropertyName("field_id")]
    public string? FieldId { get; set; }

    /// <summary>获取或设置是否降序（官方 <c>desc</c>，非必填）。</summary>
    [JsonPropertyName("desc")]
    public bool? Desc { get; set; }
}
