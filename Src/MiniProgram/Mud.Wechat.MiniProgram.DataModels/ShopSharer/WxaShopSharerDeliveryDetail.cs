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
/// 直播间订单发货信息（<c>order_detail.delivery_detail</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方未给 <c>delivery_type</c> 取值表</b>（只写「发货类型」）⇒ 保持 <c>int?</c> 不补枚举；
/// <c>finish_all_delivery</c> 官方为 number（是否发货完成）而非 boolean，语义上是 0/1 数值，
/// 建 <c>int?</c> 照官方线格式，不得改成 <c>bool?</c>。</para>
/// <para><b>敏感面</b>：快递单号禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerDeliveryDetail
{
    /// <summary>发货类型（<c>delivery_type</c>，number；官方本页未给取值表）。</summary>
    [JsonPropertyName("delivery_type")]
    public int? DeliveryType { get; set; }

    /// <summary>是否发货完成（<c>finish_all_delivery</c>，number，官方类型即 number 非 boolean）。</summary>
    [JsonPropertyName("finish_all_delivery")]
    public int? FinishAllDelivery { get; set; }

    /// <summary>发货单列表（<c>delivery_list</c>，object array），见 <see cref="WxaShopSharerDeliveryItem"/>。</summary>
    [JsonPropertyName("delivery_list")]
    public List<WxaShopSharerDeliveryItem>? DeliveryList { get; set; }
}
