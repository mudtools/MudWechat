// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 搜索订单请求体（<c>POST /product/order/search</c>）。
/// </summary>
/// <remarks>
/// <para><b>默认只搜已付款</b>：官方原文「<c>status</c> 不填时默认搜索已付款订单」⇒ 要查待付款须显式传 <c>10</c>。</para>
/// <para><b><c>source</c> 默认值两页不一致</b>：本页示例注释默认 0（全部），参数表写默认 0；</para>
/// <para>列表页写默认 1（小商店）⇒ 两页共用值域但默认值不同，照录不自作统一。</para>
/// <para><b>翻页</b>：<c>page_size</c> 不超过 1,000（列表页是 10,000）。</para>
/// <para><b>敏感值</b>：<c>user_name</c> / <c>tel_number</c> 属个人信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderSearchRequest
{
    /// <summary>支付时间起始，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("start_pay_time")]
    public string? StartPayTime { get; set; }

    /// <summary>支付时间结束，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("end_pay_time")]
    public string? EndPayTime { get; set; }

    /// <summary>商品标题关键词。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>商品编码。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>收件人（敏感）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>收件人电话（敏感）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>售后筛选：不填全部 / 0 无售后中 / 1 售后单数 ≥ 1。</summary>
    [JsonPropertyName("on_aftersale_order_exist")]
    public long? OnAftersaleOrderExist { get; set; }

    /// <summary>订单状态，见 <see cref="WxaProductOrderStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>订单来源：0 全部 / 1 小商店自营 / 2 带货。</summary>
    [JsonPropertyName("source")]
    public long? Source { get; set; }

    /// <summary>只搜一个订单时传。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>第几页，最小 1（必填）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>每页数量，不超过 1,000（必填）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }
}
