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
/// 直播间订单价格信息（<c>order_detail.price_info</c>；五支金额字段官方一律<b>单位分</b>）。
/// </summary>
/// <remarks>
/// <para><b>金额语义</b>（官方逐行原文）：<c>order_price</c> 订单最终金额、<c>freight</c> 运费、
/// <c>discounted_price</c> 优惠金额、<c>additional_price</c> 附加金额（配 <c>additional_remarks</c> 备注，示例「税费」）。
/// 官方未给「最终金额 = 商品价 × 数量 + 运费 − 优惠 + 附加」的对账恒等式 ⇒ 宿主不得自行推导演算差额。</para>
/// <para><b>敏感面</b>：全部金额字段属经营/个人金融信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerPriceInfo
{
    /// <summary>订单最终金额（<c>order_price</c>，number，<b>单位分</b>）。禁落日志。</summary>
    [JsonPropertyName("order_price")]
    public long? OrderPrice { get; set; }

    /// <summary>运费（<c>freight</c>，number，<b>单位分</b>）。禁落日志。</summary>
    [JsonPropertyName("freight")]
    public long? Freight { get; set; }

    /// <summary>优惠金额（<c>discounted_price</c>，number，<b>单位分</b>）。禁落日志。</summary>
    [JsonPropertyName("discounted_price")]
    public long? DiscountedPrice { get; set; }

    /// <summary>附加金额（<c>additional_price</c>，number，<b>单位分</b>）。禁落日志。</summary>
    [JsonPropertyName("additional_price")]
    public long? AdditionalPrice { get; set; }

    /// <summary>附加金额备注（<c>additional_remarks</c>，string，示例「税费」）。</summary>
    [JsonPropertyName("additional_remarks")]
    public string? AdditionalRemarks { get; set; }
}
