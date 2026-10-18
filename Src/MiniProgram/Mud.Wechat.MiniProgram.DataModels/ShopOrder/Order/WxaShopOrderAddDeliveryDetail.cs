// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 「生成订单」的配送信息（官方写侧 <c>delivery_detail</c>，参数表<b>只有 delivery_type 一支</b>）。
/// </summary>
/// <remarks>
/// <para><b>写侧与读侧同键名不同形</b>：读侧 <see cref="WxaShopOrderDeliveryDetail"/> 有 finish_all_delivery 与
/// delivery_list 两支，写侧参数表一概没有 ⇒ 独立建类型，不得复用读侧类型后「只填一支」（守卫 SO5 锁定）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/add_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderAddDeliveryDetail
{
    /// <summary>delivery_type（官方类型 <c>number</c>，必填）：1 正常快递 / 2 无需快递 / 3 线下配送 / 4 用户自提，默认 1，见 <see cref="WxaShopDeliveryTypes"/>。</summary>
    [JsonPropertyName("delivery_type")]
    public int? DeliveryType { get; set; }
}
