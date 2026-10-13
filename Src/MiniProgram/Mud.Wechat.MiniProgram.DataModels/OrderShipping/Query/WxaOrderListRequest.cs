// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「查询订单列表」请求体（<c>POST /wxa/sec/order/get_order_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>五个参数官方全部选填</b>，翻页走 <c>last_index</c> 游标而非 offset：
/// 第一页不传 <c>last_index</c>，返回体 <c>has_more</c> 为 <c>true</c> 时把本次的
/// <c>last_index</c> 原样带上取下一页 ⇒ <b>游标不可自行构造或复用上一批</b>。</para>
/// <para><b><c>page_size</c> 默认 100</b>（官方原文「默认为100」）。</para>
/// <para><c>openid</c> 属用户级标识（禁止落日志、遥测或异常消息）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorderlist.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderListRequest
{
    /// <summary>pay_time_range（官方类型 <c>object</c>）：支付时间所属范围。</summary>
    [JsonPropertyName("pay_time_range")]
    public WxaOrderPayTimeRange? PayTimeRange { get; set; }

    /// <summary>order_state（官方类型 <c>number</c>）：订单状态枚举：(1) 待发货；(2) 已发货；(3) 确认收货；(4) 交易完成；(5) 已退款；(6) 资金待结算。</summary>
    [JsonPropertyName("order_state")]
    public long? OrderState { get; set; }

    /// <summary>openid（官方类型 <c>string</c>）：支付者openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>last_index（官方类型 <c>string</c>）：翻页时使用，获取第一页时不用传入，如果查询结果中 has_more 字段为 true，则传入该次查询结果中返回的 last_index 字段可获取下一页。</summary>
    [JsonPropertyName("last_index")]
    public string? LastIndex { get; set; }

    /// <summary>page_size（官方类型 <c>number</c>）：翻页时使用，返回列表的长度，默认为100。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }
}
