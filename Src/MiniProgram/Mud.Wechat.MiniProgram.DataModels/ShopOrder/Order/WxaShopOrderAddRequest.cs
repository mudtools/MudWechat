// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 「生成订单」请求体（<c>POST /shop/order/add</c>）。
/// </summary>
/// <remarks>
/// <para><b>本端点可重入、且是覆盖式更新</b>：官方原文「该接口可重入，如果 order_id 或 out_order_id 已存在，
/// 会直接更新整个订单数据」⇒ 传已有 <c>out_order_id</c> 不是「创建失败」而是<b>整单覆盖</b>，
/// 重试管道必须携带完全一致的请求体，否则会静默改写价格与地址。</para>
/// <para><b>官方本页参数表无 <c>fund_type</c></b>：而 <c>/shop/order/getpaymentparams</c> 页要求
/// 「使用本接口的订单需要在生成订单时将 fund_type 设为 1」⇒ 官方两页自相矛盾。本仓按
/// 「参数表未出现即不建模」裁决<b>不提供</b>该支（裁决与追踪号见守卫 SO6 / remarks），
/// 二级商户单场景须由宿主与官方确认后处置，不得由 SDK 臆造字段。</para>
/// <para><b>成功后订单状态恒为 10</b>（待付款），官方另注「目前只有订单支付成功后进入的才会将订单同步至订单中心」，
/// 合单支付拆单场景官方要求「以母单同步」。</para>
/// <para><b>敏感值</b>：<c>openid</c>、<c>out_user_id</c>（商家侧用户标识）与 <see cref="AddressInfo"/>（收件人姓名 / 手机号 / 住址）
/// 均属个人信息 ⇒ 不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/add_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderAddRequest
{
    /// <summary>create_time（官方类型 <c>string</c>，必填）：创建时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>out_order_id（官方类型 <c>string</c>，必填）：商家自定义订单 ID；合单支付场景下是主外部订单 ID。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>openid（官方类型 <c>string</c>，必填）：用户 openid（敏感标识，不得进日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>path（官方类型 <c>string</c>，必填）：商家小程序该订单的页面 path，用于微信侧订单中心跳转；官方建议 path 内某参数值与 out_order_id 匹配。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>scene（官方类型 <c>number</c>，必填）：下单时小程序场景值（可由 getLaunchOptionsSync / onLaunch 取得）。</summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }

    /// <summary>out_user_id（官方示例键，本页参数表未列）：商家侧用户标识（敏感，不得进日志）。官方示例出现而参数表缺席 ⇒ 建模保留、注释留档。</summary>
    [JsonPropertyName("out_user_id")]
    public string? OutUserId { get; set; }

    /// <summary>order_detail（官方类型 <c>object</c>，必填）：订单详细数据（写侧形状，见 <see cref="WxaShopOrderAddDetail"/>）。</summary>
    [JsonPropertyName("order_detail")]
    public WxaShopOrderAddDetail? OrderDetail { get; set; }

    /// <summary>delivery_detail（官方类型 <c>object</c>）：配送信息，写侧只有 delivery_type 一支（见 <see cref="WxaShopOrderAddDeliveryDetail"/>）。</summary>
    [JsonPropertyName("delivery_detail")]
    public WxaShopOrderAddDeliveryDetail? DeliveryDetail { get; set; }

    /// <summary>address_info（官方类型 <c>object</c>，选填）：地址信息；官方标注 delivery_type=2 无需设置、=4 填自提门店地址（敏感，不得进日志）。</summary>
    [JsonPropertyName("address_info")]
    public WxaShopAddressInfo? AddressInfo { get; set; }
}
