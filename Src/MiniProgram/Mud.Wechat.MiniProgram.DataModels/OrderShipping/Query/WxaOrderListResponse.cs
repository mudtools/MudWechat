// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「查询订单列表」响应（<c>POST /wxa/sec/order/get_order_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方返回体表未列 <c>errmsg</c></b>（只有 <c>errcode</c> + 业务三字段）⇒ 继承
/// <see cref="WxaResponse"/> 后该字段可能恒为 <c>null</c>，宿主判错须以 <c>errcode</c> 为准，
/// 不得依赖 <c>errmsg</c> 非空。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorderlist.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderListResponse : WxaResponse
{
    /// <summary>last_index（官方类型 <c>string</c>）：翻页时使用。</summary>
    [JsonPropertyName("last_index")]
    public string? LastIndex { get; set; }

    /// <summary>has_more（官方类型 <c>boolean</c>）：是否还有更多支付单。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>order_list（官方类型 <c>objarray</c>）：支付单信息列表。</summary>
    [JsonPropertyName("order_list")]
    public List<WxaOrderInfo>? OrderList { get; set; }
}
