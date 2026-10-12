// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 付款礼品卡会员卡规则（官方 <c>member_rule</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftMemberRule
{
    /// <summary>获取或设置会员卡 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置最低消费金额（官方 <c>least_cost</c>，单位分，必填）。</summary>
    [JsonPropertyName("least_cost")]
    public int LeastCost { get; set; }

    /// <summary>获取或设置最高优惠金额（官方 <c>max_cost</c>，单位分，必填）。</summary>
    [JsonPropertyName("max_cost")]
    public int MaxCost { get; set; }

    /// <summary>获取或设置跳转链接（官方 <c>jump_url</c>，可选）。</summary>
    [JsonPropertyName("jump_url")]
    public string? JumpUrl { get; set; }

    /// <summary>获取或设置互联网品牌 ID（官方 <c>app_brand_id</c>，可选）。</summary>
    [JsonPropertyName("app_brand_id")]
    public string? AppBrandId { get; set; }

    /// <summary>获取或设置互联网品牌 Pass ID（官方 <c>app_brand_pass</c>，可选）。</summary>
    [JsonPropertyName("app_brand_pass")]
    public string? AppBrandPass { get; set; }
}
