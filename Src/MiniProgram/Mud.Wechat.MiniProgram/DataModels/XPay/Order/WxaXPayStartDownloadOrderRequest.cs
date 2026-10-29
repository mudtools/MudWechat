// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「下载支付订单」（创建账单下载任务）请求体（<c>POST /xpay/start_download_order</c>）。
/// </summary>
/// <remarks>
/// <para><b>异步两步式</b>：本端点只回 <c>task_id</c>，须再用「查询下载订单任务」取 <c>download_url</c>。</para>
/// <para><b>时间窗硬约束</b>：<c>begin_ds</c>/<c>end_ds</c> 为 <c>YYYYMMDD</c> 数字，官方原文要求间隔不超过 31 天。</para>
/// <para><b>条件必填是官方口径</b>：<c>is_provided</c> 在 <c>order_type</c> 为 2/3 时必须传入、<c>refund_status</c> 仅 <c>order_type = 4</c> 时有效 ⇒ 可空性承载「按类型条件必填」，SDK 不本地校验。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_download_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayStartDownloadOrderRequest
{
    /// <summary>begin_ds（官方类型 <c>number</c>，<b>必填</b>）：开始日期，格式 YYYYMMDD</summary>
    [JsonPropertyName("begin_ds")]
    public long? BeginDs { get; set; }

    /// <summary>end_ds（官方类型 <c>number</c>，<b>必填</b>）：结束日期，格式 YYYYMMDD，与 begin_ds 间隔不超过 31 天</summary>
    [JsonPropertyName("end_ds")]
    public long? EndDs { get; set; }

    /// <summary>order_type（官方类型 <c>number</c>，<b>必填</b>）：订单类型：1=代币交易订单 /2=道具直购交易订单 /3=会员订阅订单 /4=退款订单</summary>
    [JsonPropertyName("order_type")]
    public long? OrderType { get; set; }

    /// <summary>order_info（官方类型 <c>string</c>）：订单信息搜索关键字，支持按交易单号/商户单号/用户ID 模糊匹配</summary>
    [JsonPropertyName("order_info")]
    public string? OrderInfo { get; set; }

    /// <summary>is_provided（官方类型 <c>boolean</c>）：发货状态，order_type 为 2(道具) 或 3(会员订阅) 时必须传入；true=已发货 /false=未发货；不传默认 true</summary>
    [JsonPropertyName("is_provided")]
    public bool? IsProvided { get; set; }

    /// <summary>refund_status（官方类型 <c>number</c>）：退款状态筛选，仅 order_type=4(退款订单) 时有效；0=全部 /2=已退款 /4=退款中 /5=退款失败；不传默认 0（全部）</summary>
    [JsonPropertyName("refund_status")]
    public long? RefundStatus { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：环境标识：0=现网 /1=沙箱（用于基类签名校验）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>pay_channel（官方类型 <c>number</c>，<b>必填</b>）：支付渠道：1=普通虚拟支付 /2=苹果IAP</summary>
    [JsonPropertyName("pay_channel")]
    public long? PayChannel { get; set; }
}
