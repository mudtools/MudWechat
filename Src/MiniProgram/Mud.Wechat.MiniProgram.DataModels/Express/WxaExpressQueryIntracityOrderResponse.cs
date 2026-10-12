// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送查询配送单应答（配送单全量明细）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryIntracityOrderResponse : WxaResponse
{
    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>配送单状态（<c>order_status</c>；1 待接单，2 待取货，3 配送中，4 已完成，5 已取消）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>用户 OpenId（<c>user_openid</c>）。</summary>
    [JsonPropertyName("user_openid")]
    public string? UserOpenId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>运单号（<c>delivery_no</c>）。</summary>
    [JsonPropertyName("delivery_no")]
    public string? DeliveryNo { get; set; }

    /// <summary>配送距离（<c>distance</c>；单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>实际费用（<c>actualfee</c>；单位：分）。</summary>
    [JsonPropertyName("actualfee")]
    public int ActualFee { get; set; }

    /// <summary>扣款金额（<c>deductfee</c>；单位：分）。</summary>
    [JsonPropertyName("deductfee")]
    public int DeductFee { get; set; }

    /// <summary>下单时间（<c>create_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>接单时间（<c>accept_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("accept_time")]
    public long? AcceptTime { get; set; }

    /// <summary>取货时间（<c>fetch_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("fetch_time")]
    public long? FetchTime { get; set; }

    /// <summary>送达时间（<c>finish_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("finish_time")]
    public long? FinishTime { get; set; }

    /// <summary>取消时间（<c>cancel_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("cancel_time")]
    public long? CancelTime { get; set; }

    /// <summary>预计送达时间（<c>expected_finish_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("expected_finish_time")]
    public long? ExpectedFinishTime { get; set; }

    /// <summary>取货码（<c>fetch_code</c>）。</summary>
    [JsonPropertyName("fetch_code")]
    public string? FetchCode { get; set; }

    /// <summary>收货码（<c>recv_code</c>）。</summary>
    [JsonPropertyName("recv_code")]
    public string? ReceiveCode { get; set; }

    /// <summary>序号（<c>order_seq</c>）。</summary>
    [JsonPropertyName("order_seq")]
    public string? OrderSequence { get; set; }

    /// <summary>商品列表（<c>item_list</c>），见 <see cref="WxaExpressIntracityItem"/>。</summary>
    [JsonPropertyName("item_list")]
    public List<WxaExpressIntracityItem>? ItemList { get; set; }

    /// <summary>货品信息（<c>cargo_info</c>），见 <see cref="WxaExpressIntracityCargo"/>。</summary>
    [JsonPropertyName("cargo_info")]
    public WxaExpressIntracityCargo? CargoInfo { get; set; }

    /// <summary>收货人信息（<c>receiver_info</c>），见 <see cref="WxaExpressIntracityReceiver"/>。</summary>
    [JsonPropertyName("receiver_info")]
    public WxaExpressIntracityReceiver? ReceiverInfo { get; set; }

    /// <summary>门店信息（<c>store_info</c>），见 <see cref="WxaExpressIntracityStoreInfo"/>。</summary>
    [JsonPropertyName("store_info")]
    public WxaExpressIntracityStoreInfo? StoreInfo { get; set; }

    /// <summary>骑手信息（<c>transporter_info</c>），见 <see cref="WxaExpressIntracityTransporter"/>。</summary>
    [JsonPropertyName("transporter_info")]
    public WxaExpressIntracityTransporter? TransporterInfo { get; set; }
}
