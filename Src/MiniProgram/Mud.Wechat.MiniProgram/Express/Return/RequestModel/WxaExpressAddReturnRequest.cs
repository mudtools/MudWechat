// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 创建退货 ID 请求体（<c>POST /cgi-bin/express/delivery/return/add</c>，退货组件）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_addreturn"/>。</para>
/// <para>
/// 与无忧退货运费险不同：<b>本族无 <c>wx_pay_id</c></b>（不要求保单）；<c>shop_order_id</c> 与退货 ID 一一对应，
/// “一个订单需要多次退货”时可在商家内部 1 个退货订单号映射多个 <c>shop_order_id</c>。
/// 创建退货 ID 时平台自动下发模板消息给用户提醒退货。
/// </para>
/// <para>官方错误码：40097（order_id 或退货地址为空）、9300522（shop_order_id 已存在）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddReturnRequest
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
    public int OrderPrice { get; set; }
}
