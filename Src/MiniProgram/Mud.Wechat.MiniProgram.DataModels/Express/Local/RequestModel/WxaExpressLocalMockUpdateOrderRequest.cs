// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 即时配送「模拟更新配送单状态」<b>共用</b>请求体
/// （<c>POST .../business/{test_update_order,realmock_update_order}</c>）。
/// </summary>
/// <remarks>
/// <b>两端点差异（官方原文）</b>：<c>test_update_order</c> 用于<b>沙盒环境</b>（订单不真实流转到运力方，
/// <c>shopid</c> 必须是 <c>test_shop_id</c>、<b>不传</b> <c>delivery_sign</c>）；
/// <c>realmock_update_order</c> 会把请求<b>转发到运力测试环境</b>（目前支持顺丰同城与达达，<b>必传</b> <c>delivery_sign</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalMockUpdateOrderRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；沙盒端点 <c>test_update_order</c> 必须为 <c>test_shop_id</c>）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送状态（<c>order_status</c>，必填；官方枚举见 <see cref="WxaExpressLocalOrderStatus"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>状态变更时间点（<c>action_time</c>，必填；Unix 秒级时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>附加信息（<c>action_msg</c>，可选）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>；<b>仅</b> <c>realmock_update_order</c> 必填，沙盒端点不传；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }
}
