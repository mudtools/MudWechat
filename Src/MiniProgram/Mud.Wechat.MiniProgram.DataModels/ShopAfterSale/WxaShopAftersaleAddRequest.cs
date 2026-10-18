// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 新增售后单请求体（<c>POST /shop/aftersale/add</c>）。
/// </summary>
/// <remarks>
/// <para><b>商家侧 id 全量自拟</b>：<c>out_order_id</c> 与 <c>out_aftersale_id</c> 均为必填且由商家生成，</para>
/// <para>微信不回服务端单号；<c>out_aftersale_id</c> 是后续详情 / 更新 / 期限四支的唯一定位键，必须跨批次唯一。</para>
/// <para><b>状态由调用方回填</b>：<c>status</c> 官方标必填且取值 0~19（见 <see cref="WxaShopAftersaleStatuses"/>），</para>
/// <para>新建时通常传 <c>0</c>（未受理）；<c>finish_all_aftersale=1</c> 表示订单维度售后全部完成并触发订单状态流转。</para>
/// <para><b>金额单位分</b>；<c>create_time</c> 为 <c>yyyy-MM-dd HH:mm:ss</c> 字符串（非时间戳）。</para>
/// <para><b>敏感值</b>：<c>openid</c> 属用户标识，<c>refund_reason</c> 可能含个人信息 ⇒ 禁写入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopAftersaleAddRequest
{
    /// <summary>商家自定义订单 ID（必填）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>商家自定义售后 ID（必填，后续全族定位键）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>商家小程序售后单页面 path（必填；不存在则使用订单 path）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>退款金额，单位<b>分</b>（必填）。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>用户的 <c>openid</c>（必填，敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>售后类型（必填），取值见 <see cref="WxaShopAftersaleTypes"/>。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>发起申请时间，<c>yyyy-MM-dd HH:mm:ss</c>（必填）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>售后状态（必填），取值见 <see cref="WxaShopAftersaleStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>订单维度是否售后全部完成（必填）：0 存在可售后商品 / 1 已全部完成并流转订单状态。</summary>
    [JsonPropertyName("finish_all_aftersale")]
    public int? FinishAllAftersale { get; set; }

    /// <summary>退货相关商品列表（必填），见 <see cref="WxaShopAftersaleProductInfo"/>。</summary>
    [JsonPropertyName("product_infos")]
    public List<WxaShopAftersaleProductInfo>? ProductInfos { get; set; }

    /// <summary>退款原因（选填，敏感）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }
}
