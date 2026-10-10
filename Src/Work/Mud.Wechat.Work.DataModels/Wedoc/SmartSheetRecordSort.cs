// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 查询记录的排序项（官方 Sort；查询记录请求 <c>sort</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetRecordSort
{
    /// <summary>获取或设置需要排序的字段标题（官方 <c>field_title</c>，必填）。</summary>
    [JsonPropertyName("field_title")]
    public string? FieldTitle { get; set; }

    /// <summary>获取或设置是否进行降序排序（官方 <c>desc</c>，非必填），默认值为 <c>false</c>。</summary>
    [JsonPropertyName("desc")]
    public bool? Desc { get; set; }
}
