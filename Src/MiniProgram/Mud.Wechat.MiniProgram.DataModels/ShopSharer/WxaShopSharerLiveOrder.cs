// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 分享员直播间订单条目（<c>orders[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b><c>order_id</c> 官方类型为 number(uint64)</b> ⇒ 建 <c>long?</c>，示例值 123455 为裸数字（不带引号），
/// 与本页 <c>out_order_id</c>（string）形态不同，不得合并成同一类型。</para>
/// <para><b><c>status</c> 官方本页未给取值表</b>（只写「订单状态」）⇒ 本仓不补枚举、保持 <c>int?</c>，
/// 宿主取值须对照交易组件订单状态面，不得凭第三方 SDK 记忆硬编码。</para>
/// <para><b>敏感面</b>：订单标识与金额字段禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerLiveOrder
{
    /// <summary>交易组件平台订单 ID（<c>order_id</c>，number(uint64)）。禁落日志。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>商家自定义订单 ID（<c>out_order_id</c>，string）。禁落日志。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>订单状态（<c>status</c>，number；官方本页未给取值表）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>商家小程序该订单的页面 path（<c>path</c>，string，用于微信侧订单中心跳转）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>订单明细（<c>order_detail</c>，object），见 <see cref="WxaShopSharerOrderDetail"/>。</summary>
    [JsonPropertyName("order_detail")]
    public WxaShopSharerOrderDetail? OrderDetail { get; set; }
}
