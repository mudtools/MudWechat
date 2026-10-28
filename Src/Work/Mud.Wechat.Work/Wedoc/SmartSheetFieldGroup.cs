// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 编组（官方 FieldGroup；添加编组与更新编组响应的 <c>field_group</c>、获取编组响应的 <c>field_groups</c> 元素共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetFieldGroup
{
    /// <summary>获取或设置编组 ID（官方 <c>field_group_id</c>）。</summary>
    [JsonPropertyName("field_group_id")]
    public string? FieldGroupId { get; set; }

    /// <summary>获取或设置编组名称（官方 <c>name</c>），不能和已有名称重复。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置编组内容（官方 <c>children</c>）。</summary>
    [JsonPropertyName("children")]
    public List<SmartSheetFieldGroupChild>? Children { get; set; }
}
