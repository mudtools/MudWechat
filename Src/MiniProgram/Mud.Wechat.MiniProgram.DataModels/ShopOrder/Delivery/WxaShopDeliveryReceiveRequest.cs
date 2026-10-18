// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 「订单确认收货」请求体（<c>POST /shop/delivery/receive</c>）。
/// </summary>
/// <remarks>
/// <para><b>路由真名是 <c>receive</c>，文档页文件名与第三方清单拼作 <c>recieve</c></b>：官方页正文请求地址为
/// <c>https://api.weixin.qq.com/shop/delivery/receive</c> ⇒ 本仓按正文取 <c>receive</c>，按 <c>recieve</c> 落地即死路由（守卫 SO5 / SO6 锁定）。</para>
/// <para><b>本端点是状态跃迁而非查询</b>：官方原文「把订单状态从 30（待收货）流转到 100（完成）」⇒
/// 只对 30 的订单有意义，重复调用非幂等。</para>
/// <para><b>敏感值</b>：<c>openid</c> 不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/recieve.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopDeliveryReceiveRequest
{
    /// <summary>order_id（官方类型 <c>number</c>，选填）：交易组件订单 ID，与 <see cref="OutOrderId"/> 二选一。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>out_order_id（官方类型 <c>string</c>，选填）：商家自定义订单 ID，与 <see cref="OrderId"/> 二选一。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>openid（官方类型 <c>string</c>，必填）：用户 openid（敏感标识，不得进日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
