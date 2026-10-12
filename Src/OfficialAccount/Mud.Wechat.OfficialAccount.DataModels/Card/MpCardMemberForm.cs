// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 会员卡激活表单（官方 <c>required_form</c> / <c>optional_form</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberForm
{
    /// <summary>获取或设置是否允许用户修改（官方 <c>can_modify</c>，可选）。</summary>
    [JsonPropertyName("can_modify")]
    public bool? CanModify { get; set; }

    /// <summary>获取或设置富文本字段列表（官方 <c>rich_field_list</c>，可选）。</summary>
    [JsonPropertyName("rich_field_list")]
    public List<MpCardMemberRichField>? RichFieldList { get; set; }

    /// <summary>获取或设置通用字段 ID 列表（官方 <c>common_field_id_list</c>，可选）。</summary>
    [JsonPropertyName("common_field_id_list")]
    public List<string>? CommonFieldIdList { get; set; }

    /// <summary>获取或设置自定义字段 ID 列表（官方 <c>custom_field_list</c>，可选）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<string>? CustomFieldList { get; set; }
}
