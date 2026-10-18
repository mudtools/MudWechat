// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 物流单对应的商品条目（官方 <c>DeliveryProduct</c>，发货侧 <c>product_info_list</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b>两支 id 官方类型列印为 string，但发货页示例给的是数字</b>（<c>"out_product_id": 1</c>）⇒
/// 按类型列取 <see cref="string"/>；若宿主按示例发数字，源生成序列化器会因类型不符报错，须自行转字符串。</para>
/// <para><b>只填商家侧 id</b>：本表无微信侧 <c>product_id</c> / <c>sku_id</c> 两支 ⇒ 发货归因走 <c>out_*</c> 单轨。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/send.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopDeliveryProductInfo
{
    /// <summary>out_product_id（官方类型列 <c>string</c>，必填）：订单里的商家自定义商品 ID。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>out_sku_id（官方类型列 <c>string</c>，必填）：订单里的商家自定义 sku ID。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>product_cnt（官方类型 <c>number</c>，示例给 1）：本物流单实际发货件数。</summary>
    [JsonPropertyName("product_cnt")]
    public int? ProductCnt { get; set; }
}
