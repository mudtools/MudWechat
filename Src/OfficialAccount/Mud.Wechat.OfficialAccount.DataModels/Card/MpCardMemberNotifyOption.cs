// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 会员卡变更通知选项（官方 <c>notify_optional</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberNotifyOption
{
    /// <summary>是否推送积分变更通知（官方 <c>is_notify_bonus</c>，可选）。</summary>
    [JsonPropertyName("is_notify_bonus")]
    public bool? IsNotifyBonus { get; set; }

    /// <summary>是否推送余额变更通知（官方 <c>is_notify_balance</c>，可选）。</summary>
    [JsonPropertyName("is_notify_balance")]
    public bool? IsNotifyBalance { get; set; }

    /// <summary>是否推送自定义字段 1 变更通知（官方 <c>is_notify_custom_field1</c>，可选）。</summary>
    [JsonPropertyName("is_notify_custom_field1")]
    public bool? IsNotifyCustomField1 { get; set; }

    /// <summary>是否推送自定义字段 2 变更通知（官方 <c>is_notify_custom_field2</c>，可选）。</summary>
    [JsonPropertyName("is_notify_custom_field2")]
    public bool? IsNotifyCustomField2 { get; set; }

    /// <summary>是否推送自定义字段 3 变更通知（官方 <c>is_notify_custom_field3</c>，可选）。</summary>
    [JsonPropertyName("is_notify_custom_field3")]
    public bool? IsNotifyCustomField3 { get; set; }
}
