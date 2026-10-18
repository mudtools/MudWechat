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
/// 直播间订单发货单条目（<c>delivery_detail.delivery_list[]</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>delivery_id</c> 官方口径</b>：快递公司 ID，取值须「通过获取快递公司列表获取」（官方原文），
/// 与发货信息管理域 <c>/wxa/sec/order/*</c> 的运单结构同形但按域独立建模（守卫 SHOPSH6 不跨域复用）。</para>
/// <para><b>敏感面</b>：<c>waybill_id</c>（快递单号）禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerDeliveryItem
{
    /// <summary>快递公司 ID（<c>delivery_id</c>，string，取官方快递公司列表接口值，示例「SF」）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递单号（<c>waybill_id</c>，string）。禁落日志。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }
}
