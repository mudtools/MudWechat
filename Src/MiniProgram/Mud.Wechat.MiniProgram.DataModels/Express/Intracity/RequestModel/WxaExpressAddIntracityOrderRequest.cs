// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送下单请求体（<c>POST /cgi-bin/express/intracity/addorder</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddIntracityOrderRequest : WxaExpressIntracityOrderBaseRequest
{
    /// <summary>商家配送单号（<c>store_order_id</c>，必填；用于幂等，重复下单返回同一配送单）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>用户 OpenId（<c>user_openid</c>，必填）。</summary>
    [JsonPropertyName("user_openid")]
    public string? UserOpenId { get; set; }

    /// <summary>序号（<c>order_seq</c>，可选；搭配 <c>#冲单#</c> 用于催单消息）。</summary>
    [JsonPropertyName("order_seq")]
    public string? OrderSequence { get; set; }

    /// <summary>收货码类型（<c>verify_code_type</c>，可选；0 收货原则，1 收货码）。</summary>
    [JsonPropertyName("verify_code_type")]
    public int? VerifyCodeType { get; set; }

    /// <summary>订单详情页路径（<c>order_detail_path</c>，可选）。</summary>
    [JsonPropertyName("order_detail_path")]
    public string? OrderDetailPath { get; set; }

    /// <summary>回调 URL（<c>callback_url</c>，可选；配送状态变化时异步通知）。</summary>
    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; set; }

    /// <summary>商品列表（<c>item_list</c>，可选），见 <see cref="WxaExpressIntracityItem"/>。</summary>
    [JsonPropertyName("item_list")]
    public List<WxaExpressIntracityItem>? ItemList { get; set; }
}
