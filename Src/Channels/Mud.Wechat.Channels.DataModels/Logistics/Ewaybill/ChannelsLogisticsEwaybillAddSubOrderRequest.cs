// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>电子面单子件追加（<c>ewaybill/biz/order/addsuborder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillAddSubOrderRequest
{
    /// <summary>获取或设置电子面单订单号（官方 <c>ewaybill_order_id</c>，必填）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string EwaybillOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置运单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置追加面单数量（官方 <c>add_package_quantity</c>，必填，1 ≤ value ≤ 300）。</summary>
    [JsonPropertyName("add_package_quantity")]
    public int AddPackageQuantity { get; set; }

    /// <summary>获取或设置面单模板 id（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置店铺 id（官方 <c>shop_id</c>，必填，从查询开通账号信息接口获取）。</summary>
    [JsonPropertyName("shop_id")]
    public string ShopId { get; set; } = string.Empty;

    /// <summary>获取或设置电子面单账号 id（官方 <c>ewaybill_acct_id</c>，必填，从查询开通账号信息接口获取）。</summary>
    [JsonPropertyName("ewaybill_acct_id")]
    public string EwaybillAcctId { get; set; } = string.Empty;

    /// <summary>获取或设置包裹的体积和重量信息（官方 <c>subpackage_list</c>，顺丰支持）。</summary>
    [JsonPropertyName("subpackage_list")]
    public List<ChannelsLogisticsEwaybillSubpackage>? SubpackageList { get; set; }

    /// <summary>获取或设置子单额外信息（官方 <c>sub_order_ext_info</c>，如商家与快递定义的交易分单类型）。</summary>
    [JsonPropertyName("sub_order_ext_info")]
    public string? SubOrderExtInfo { get; set; }
}
