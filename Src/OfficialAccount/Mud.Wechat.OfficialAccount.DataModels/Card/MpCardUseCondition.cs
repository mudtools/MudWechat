// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>使用门槛（官方 <c>advanced_info.use_condition</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUseCondition
{
    /// <summary>获取或设置可用商品类目（官方 <c>accept_category</c>，选填）。</summary>
    [JsonPropertyName("accept_category")]
    public string? AcceptCategory { get; set; }

    /// <summary>获取或设置不可用商品类目（官方 <c>reject_category</c>，选填）。</summary>
    [JsonPropertyName("reject_category")]
    public string? RejectCategory { get; set; }

    /// <summary>获取或设置满减门槛（官方 <c>least_cost</c>，单位：分）。</summary>
    [JsonPropertyName("least_cost")]
    public int? LeastCost { get; set; }

    /// <summary>获取或设置可用类型门槛（官方 <c>object_use_for</c>，选填）。</summary>
    [JsonPropertyName("object_use_for")]
    public string? ObjectUseFor { get; set; }

    /// <summary>获取或设置是否可与其他优惠同享（官方 <c>can_use_with_other_discount</c>，选填）。</summary>
    [JsonPropertyName("can_use_with_other_discount")]
    public bool? CanUseWithOtherDiscount { get; set; }
}
