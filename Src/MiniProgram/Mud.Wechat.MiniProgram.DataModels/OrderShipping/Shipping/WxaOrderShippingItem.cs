// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「物流单（上传侧）」，<c>shipping_list</c> 数组元素。
/// </summary>
/// <remarks>
/// <para><b>与查询侧元素不是同一类型</b>：本类型用 <c>item_desc</c> 且无 <c>upload_time</c>；
/// 查询侧元素（<see cref="WxaOrderShippingRecord"/>）用 <c>goods_desc</c> 并带 <c>upload_time</c>（时间戳）
/// ⇒ 官方两侧字段名与字段面均不同，守卫 OS4 锁定。</para>
/// <para><b>条件必填</b>：<c>tracking_no</c> 与 <c>express_company</c> 官方标「否」但原文均写
/// 「物流快递发货时必填」⇒ 必填性取决于 <c>logistics_type</c>，SDK 侧不做校验。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderShippingItem
{
    /// <summary>tracking_no（官方类型 <c>string</c>）：物流单号，物流快递发货时必填，字符字节限制: [1, 128]</summary>
    [JsonPropertyName("tracking_no")]
    public string? TrackingNo { get; set; }

    /// <summary>express_company（官方类型 <c>string</c>）：物流公司编码，快递公司ID，参见「获取运力 id 列表 get_delivery_list」，物流快递发货时必填，字符字节限制: [1, 128]</summary>
    [JsonPropertyName("express_company")]
    public string? ExpressCompany { get; set; }

    /// <summary>item_desc（官方类型 <c>string</c>，必填）：商品信息，例如：微信红包抱枕*1个，限120个字以内</summary>
    [JsonPropertyName("item_desc")]
    public string? ItemDesc { get; set; }

    /// <summary>contact（官方类型 <c>object</c>）：联系方式，当发货的物流公司为顺丰时，联系方式为必填，收件人或寄件人联系方式二选一</summary>
    [JsonPropertyName("contact")]
    public WxaOrderContact? Contact { get; set; }
}
