// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评价详情主体（<c>info.content</c>，官方「查询评价详情」参数表）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与列表侧 <see cref="WxaCommentListItem"/> 的字段集差异照录（禁「顺手统一」）</b>：
/// ① 官方本页把 <see cref="Amount"/> / <see cref="Score"/> 标为 <c>string</c>（列表页标 <c>number</c>）；
/// ② 本页<b>无</b> <c>extInfo</c>（列表页有）；③ 其余字段与列表页同形并共用嵌套类型。
/// 两处类型差异均为官方原文，本仓「类型以参数表为准」，故列表与详情各持一份主体类型。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentDetail
{
    /// <summary>评价 id（<c>commentId</c>）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }

    /// <summary>金额（<c>amount</c>，<b>单位是分</b>；官方详情参数表标 string）。</summary>
    [JsonPropertyName("amount")]
    public string? Amount { get; set; }

    /// <summary>订单 id（<c>orderId</c>）。</summary>
    [JsonPropertyName("orderId")]
    public string? OrderId { get; set; }

    /// <summary>创建时间（<c>createTime</c>，<b>单位是秒</b>）。</summary>
    [JsonPropertyName("createTime")]
    public string? CreateTime { get; set; }

    /// <summary>支付时间（<c>payTime</c>，<b>单位是秒</b>）。</summary>
    [JsonPropertyName("payTime")]
    public string? PayTime { get; set; }

    /// <summary>商家订单信息（<c>orderInfo</c>），见 <see cref="WxaCommentOrderInfo"/>。</summary>
    [JsonPropertyName("orderInfo")]
    public WxaCommentOrderInfo? OrderInfo { get; set; }

    /// <summary>评价商品信息（<c>productInfo</c>），见 <see cref="WxaCommentProductInfo"/>。</summary>
    [JsonPropertyName("productInfo")]
    public WxaCommentProductInfo? ProductInfo { get; set; }

    /// <summary>微信支付交易单号（<c>wxPayId</c>，一般以 420 开头，即微信支付 APIv3 的 <c>transaction_id</c>）。</summary>
    [JsonPropertyName("wxPayId")]
    public string? WxPayId { get; set; }

    /// <summary>评价用户信息（<c>userInfo</c>），见 <see cref="WxaCommentUserInfo"/>。</summary>
    [JsonPropertyName("userInfo")]
    public WxaCommentUserInfo? UserInfo { get; set; }

    /// <summary>商家小程序信息（<c>bizInfo</c>），见 <see cref="WxaCommentBizInfo"/>。</summary>
    [JsonPropertyName("bizInfo")]
    public WxaCommentBizInfo? BizInfo { get; set; }

    /// <summary>评价分数（<c>score</c>，官方口径「每 100 分对应 1 星」；官方详情参数表标 string）。</summary>
    [JsonPropertyName("score")]
    public string? Score { get; set; }

    /// <summary>评价内容（<c>content</c>），见 <see cref="WxaCommentContent"/>。</summary>
    [JsonPropertyName("content")]
    public WxaCommentContent? Content { get; set; }
}
