// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「营销子域」（8 端点：买赠活动 activity/add、activity/del、activity/stop +
// 限时抢购 limiteddiscounttask/add、update、delete、stop、list/get）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。
//
// 官方文档矛盾照录：activity/del 的 activity_id 为 number、activity/stop 的 activity_id 为 string
// （原文即如此），SDK 分别建模为 long / string，不做统一。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>买赠活动详情（官方 <c>detail</c> 对象，<c>product/activity/add</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftActivityDetail
{
    /// <summary>
    /// 获取或设置买赠活动生效场景（官方 <c>show_scene</c>，必填：0 全场景展示，1 仅直播间展示；
    /// 当前 Open API 仅支持传 0（全场景），传 1 可能无法生效）。
    /// </summary>
    [JsonPropertyName("show_scene")]
    public int ShowScene { get; set; }

    /// <summary>
    /// 获取或设置活动类型（官方 <c>activity_type</c>：0 赠品独享（默认），1 赠品分享，2 满赠门槛；不传时按 0 处理）。
    /// </summary>
    [JsonPropertyName("activity_type")]
    public int? ActivityType { get; set; }

    /// <summary>获取或设置满赠门槛类型（官方 <c>threshold_type</c>，<c>activity_type=2</c> 时必填：1 满金额，2 满件数）。</summary>
    [JsonPropertyName("threshold_type")]
    public int? ThresholdType { get; set; }

    /// <summary>
    /// 获取或设置满赠门槛金额（官方 <c>threshold_money</c>，单位：分；<c>threshold_type=1</c> 时必填；
    /// 须为 100 的整数倍（即整元），大于 0 且不超过 10000000（10 万元））。
    /// </summary>
    [JsonPropertyName("threshold_money")]
    public long? ThresholdMoney { get; set; }

    /// <summary>获取或设置满赠门槛件数（官方 <c>threshold_product_num</c>，<c>threshold_type=2</c> 时必填；须大于 1 且不超过 1000）。</summary>
    [JsonPropertyName("threshold_product_num")]
    public int? ThresholdProductNum { get; set; }

    /// <summary>
    /// 获取或设置限领规则（官方 <c>receive_limit</c>；赠品分享活动（<c>activity_type=1</c>）时建议传入；
    /// 赠品独享/满赠门槛活动可不传或传空对象）。
    /// </summary>
    [JsonPropertyName("receive_limit")]
    public ChannelsGiftActivityReceiveLimit? ReceiveLimit { get; set; }

    /// <summary>获取或设置买赠活动主商品列表（官方 <c>main_products</c>，必填：至少 1 个、最多 1000 个）。</summary>
    [JsonPropertyName("main_products")]
    public List<ChannelsGiftActivityMainProduct> MainProducts { get; set; } = new();

    /// <summary>
    /// 获取或设置赠品详情（官方 <c>gift_set</c>，必填：至少包含 1 个赠品；
    /// 赠品分享活动（<c>activity_type=1</c>）时只能配置 1 种赠品）。
    /// </summary>
    [JsonPropertyName("gift_set")]
    public ChannelsGiftActivityGiftSet GiftSet { get; set; } = new();
}
