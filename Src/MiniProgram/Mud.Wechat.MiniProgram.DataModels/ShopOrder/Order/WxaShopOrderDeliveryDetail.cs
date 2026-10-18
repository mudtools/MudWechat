// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 订单配送信息（官方 <c>DeliveryDetail</c> 表，读侧）。
/// </summary>
/// <remarks>
/// <para><b>与写侧的 <c>delivery_detail</c> 不同形</b>：官方 <c>/shop/order/add</c> 的请求表里
/// <c>delivery_detail</c> 只有 <c>delivery_type</c> 一支 ⇒ 写侧用独立类型 <see cref="WxaShopOrderAddDeliveryDetail"/>，
/// 本类型只在读侧（详情与列表）出现，两者不得合并。</para>
/// <para><b>视频号场景限制</b>：官方原文「视频号场景目前只支持 1，正常快递」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderDeliveryDetail
{
    /// <summary>delivery_type（官方类型 <c>enum</c>）：1 正常快递 / 2 无需快递 / 3 线下配送 / 4 用户自提，取值见 <see cref="WxaShopDeliveryTypes"/>。</summary>
    [JsonPropertyName("delivery_type")]
    public int? DeliveryType { get; set; }

    /// <summary>finish_all_delivery（官方类型 <c>number</c>）：是否发货完成，0 未发完 / 1 已发完。</summary>
    [JsonPropertyName("finish_all_delivery")]
    public int? FinishAllDelivery { get; set; }

    /// <summary>delivery_list（官方类型 <c>DeliveryInfo[]</c>）：物流信息列表。</summary>
    [JsonPropertyName("delivery_list")]
    public List<WxaShopDeliveryInfo>? DeliveryList { get; set; }
}
