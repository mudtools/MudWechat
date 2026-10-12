// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 创建退货 ID 请求体（<c>POST /cgi-bin/express/delivery/no_worry_return/add</c>，无忧退货运费险）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_addreturnid"/>。</para>
/// <para>
/// <c>shop_order_id</c> 与退货 ID 一一对应；“一个订单需要多次退货”可在商家内部用 1 个退货订单号映射多个 <c>shop_order_id</c>。
/// 创建退货 ID 时平台自动下发模板消息给用户提醒退货。相关字段用 UTF-8 编码。
/// <c>wx_pay_id</c> 为<b>已投保的微信支付单号</b>（必填）：无该字段返回 9300569，未投保即 9300570。
/// 创建成功后返回 <c>return_id</c>，平台据其下发模板消息。
/// </para>
/// <para>官方错误码：40097（order_id 为空或退货地址为空或 wx_pay_id 为空）、9300522（shop_order_id 已存在）、9300569 / 9300570（微信支付单号问题）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddReturnIdRequest
{
    /// <summary>商家内部系统使用的退货编号（<c>shop_order_id</c>，必填）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>商家退货地址（<c>biz_addr</c>，必填），见 <see cref="WxaExpressReturnAddress"/>。</summary>
    [JsonPropertyName("biz_addr")]
    public WxaExpressReturnAddress? ShopAddress { get; set; }

    /// <summary>用户购物时的收货地址（<c>user_addr</c>，可选），见 <see cref="WxaExpressReturnAddress"/>。</summary>
    [JsonPropertyName("user_addr")]
    public WxaExpressReturnAddress? UserAddress { get; set; }

    /// <summary>退货用户的 OpenId（<c>openid</c>，必填；用于下发模板消息）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>退货订单在商家小程序的路径（<c>order_path</c>，必填）。</summary>
    [JsonPropertyName("order_path")]
    public string? OrderPath { get; set; }

    /// <summary>退货商品列表（<c>goods_list</c>，必填），见 <see cref="WxaExpressReturnGood"/>。</summary>
    [JsonPropertyName("goods_list")]
    public List<WxaExpressReturnGood>? GoodsList { get; set; }

    /// <summary>退货订单的价格（<c>order_price</c>，必填）。</summary>
    [JsonPropertyName("order_price")]
    public int? OrderPrice { get; set; }

    /// <summary>已投保的微信支付单号（<c>wx_pay_id</c>，必填）。</summary>
    [JsonPropertyName("wx_pay_id")]
    public string? WxPayId { get; set; }
}
