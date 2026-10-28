// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 会员信息（官方 <c>user_info</c>）—— 两张 name/value 列表。
/// </summary>
/// <remarks>
/// <b>为何建模在本域</b>：本对象是 <c>/card/code/get</c> 的应答组成部分，即便「会员卡接口族」本轮不建模，
/// 该形态仍必须承载，否则核销/查询面丢字段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberUserInfo
{
    /// <summary>获取或设置通用信息类目（官方 <c>common_field_list</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<MpCardUserField>? CommonFieldList { get; set; }

    /// <summary>获取或设置自定义信息类目（官方 <c>custom_field_list</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MpCardUserField>? CustomFieldList { get; set; }
}
