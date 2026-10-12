// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 礼品卡订单项（官方 <c>order</c>；order/get 与 order/batchget 共用形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrder
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置微信支付订单号（官方 <c>trans_id</c>）。</summary>
    [JsonPropertyName("trans_id")]
    public string? TransactionId { get; set; }

    /// <summary>获取或设置购买者 OpenId（官方 <c>open_id</c>）。</summary>
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置收礼方 OpenId（官方 <c>accepter_openid</c>，可选）。</summary>
    [JsonPropertyName("accepter_openid")]
    public string? AccepterOpenId { get; set; }

    /// <summary>获取或设置订单总金额（官方 <c>total_price</c>，单位分）。</summary>
    [JsonPropertyName("total_price")]
    public int TotalPrice { get; set; }

    /// <summary>获取或设置是否聊天室订单（官方 <c>IsChatRoom</c>；<b>官方 JSON 键首字母大写为原始笔误形态，照抄</b>）。</summary>
    [JsonPropertyName("IsChatRoom")]
    public bool? IsChatRoom { get; set; }

    /// <summary>获取或设置订单内卡券列表（官方 <c>card_list</c>）。</summary>
    [JsonPropertyName("card_list")]
    public MpCardGiftOrderCard[]? CardList { get; set; }

    /// <summary>获取或设置支付完成时间（官方 <c>pay_finish_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("pay_finish_time")]
    public long? PayFinishTime { get; set; }

    /// <summary>获取或设置下单时间（官方 <c>create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置生成订单时的自定义参数（官方 <c>outer_str</c>，可选）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }
}
