// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 物流单信息（官方 <c>DeliveryInfo</c>；发货请求侧与订单读侧 <c>delivery_detail.delivery_list</c> 共用同一类型）。
/// </summary>
/// <remarks>
/// <para><b>读侧示例只有两支键</b>：订单详情与两支列表的示例里 <c>delivery_list</c> 元素只有 <c>waybill_id</c> + <c>delivery_id</c>，
/// 而官方 DeliveryDetail 表把 <c>product_info_list</c> 也列在该元素内 ⇒ 读侧该支可能为 <c>null</c>，不得当作官方无此字段。</para>
/// <para><b>官方包装键名不对称</b>：物流单列表叫 <c>delivery_list</c>，其内商品信息却叫 <c>product_info_list</c>
/// （订单商品列表叫 <c>product_infos</c>）⇒ 三个键名各不同，不得互抄（守卫 SO5 锁定）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/send.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopDeliveryInfo
{
    /// <summary>delivery_id（官方类型 <c>string</c>，发货侧必填）：快递公司 ID，须取自 <c>/shop/delivery/get_company_list</c>，将影响物流信息查询。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>waybill_id（官方类型 <c>string</c>，发货侧必填）：快递单号。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>product_info_list（官方类型 <c>DeliveryProduct[]</c>，发货侧必填）：该物流单对应的商品信息。</summary>
    [JsonPropertyName("product_info_list")]
    public List<WxaShopDeliveryProductInfo>? ProductInfoList { get; set; }
}
