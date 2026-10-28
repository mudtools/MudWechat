// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// V1 售后单详情（<c>aftersale/get</c> 应答 <c>aftersale_infos[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b>官方表与示例不一致（照录）</b>：回包参数表<b>未列</b> <c>create_time</c>，但回包示例给出了</para>
/// <para><c>create_time</c> ⇒ 本类按<b>可空并集</b>建模并保留该字段；其余字段以回包参数表为准。</para>
/// <para><b>无服务端售后单号</b>：回包表只给 <c>out_aftersale_id</c>，微信侧 <c>aftersale_id</c> 不出现在本族。</para>
/// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopAftersaleInfo
{
    /// <summary>商家自定义订单 ID。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>商家自定义售后 ID（唯一可回传的售后标识）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>售后单页面 path。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>用户的 <c>openid</c>（敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>售后类型，见 <see cref="WxaShopAftersaleTypes"/>。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>售后状态，见 <see cref="WxaShopAftersaleStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>退货相关商品列表。</summary>
    [JsonPropertyName("product_infos")]
    public List<WxaShopAftersaleProductInfo>? ProductInfos { get; set; }

    /// <summary>退款金额，单位<b>分</b>。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>退款原因（敏感）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>发起时间（官方回包示例有、回包表未列，照录为可空）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}
