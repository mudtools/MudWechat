// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 修改订单价格请求体（<c>POST /product/order/change_order_price</c>）。
/// </summary>
/// <remarks>
/// <para><b>硬限制</b>：仅未付款订单可改价（<c>101100</c>）、改价次数上限 50 次（<c>108022</c>）、</para>
/// <para>任一商品或运费改后不得超出原价（<c>108021</c>）、改后订单价格不得为 0（<c>108023</c>）。</para>
/// <para><b><c>express_fee</c> 只在 <c>change_express=true</c> 时生效</b>，不填按 0 处理（官方原文）⇒ 等于免运费。</para>
/// <para><b>非幂等</b>：每次调用都计入改价次数，不得自动重试。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderChangePriceRequest
{
    /// <summary>订单 ID（必填）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>是否修改运费（必填）。</summary>
    [JsonPropertyName("change_express")]
    public bool? ChangeExpress { get; set; }

    /// <summary>修改后的运费，单位<b>分</b>（<c>change_express=true</c> 时生效，不填默认 0）。</summary>
    [JsonPropertyName("express_fee")]
    public long? ExpressFee { get; set; }

    /// <summary>改价明细列表（必填）。</summary>
    [JsonPropertyName("change_order_infos")]
    public List<WxaProductOrderChangePriceInfo>? ChangeOrderInfos { get; set; }
}
