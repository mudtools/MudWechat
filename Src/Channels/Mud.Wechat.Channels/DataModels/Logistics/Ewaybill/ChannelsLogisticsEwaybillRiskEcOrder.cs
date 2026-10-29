// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>疑似存在风险的订单目录（官方 <c>order_risk_info[].risk_ec_order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillRiskEcOrder
{
    /// <summary>获取或设置订单 id（官方 <c>ec_order_id</c>）。</summary>
    [JsonPropertyName("ec_order_id")]
    public long? EcOrderId { get; set; }

    /// <summary>获取或设置订单商品信息（官方 <c>goods_list</c>）。</summary>
    [JsonPropertyName("goods_list")]
    public List<ChannelsLogisticsEwaybillGoods>? GoodsList { get; set; }

    /// <summary>获取或设置电子面单跨店铺取号的订单密文（官方 <c>ewaybill_order_code</c>）。</summary>
    [JsonPropertyName("ewaybill_order_code")]
    public string? EwaybillOrderCode { get; set; }

    /// <summary>获取或设置跨店铺取号的订单所属店铺 appid（官方 <c>ewaybill_order_appid</c>）。</summary>
    [JsonPropertyName("ewaybill_order_appid")]
    public string? EwaybillOrderAppid { get; set; }
}
