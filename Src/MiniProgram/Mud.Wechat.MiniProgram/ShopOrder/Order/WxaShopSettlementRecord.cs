// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 结算分账记录（官方 <c>SettlementRecord</c> 表，交易组件侧）。
/// </summary>
/// <remarks>
/// <para><b>本类型是「交易组件侧」结算记录，与合单支付侧的 <c>WxaShopCombineProfitSharingRecord</c> 同名不同源</b>：
/// 本表时间是 <c>string</c>、对方是秒级时间戳 <c>number</c>；本表用 <c>settle_no</c> / <c>settle_id</c>，
/// 对方用 <c>profit_sharing_no</c> / <c>profit_sharing_id</c> ⇒ 两套类型不得合并（守卫 SO5 锁定）。</para>
/// <para><b>分账方类型</b>（官方 <c>settlement_type</c> 表）：0 商家 / 1 服务商；
/// 分账结果（官方 <c>settlement_result</c> 表）：PENDING 待分账 / SUCCESS 分账成功 / CLOSED 已关闭。</para>
/// <para><b>敏感值</b>：<c>mchid</c> 与 <c>settle_id</c>（微信支付系统分账单号）不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopSettlementRecord
{
    /// <summary>type（官方类型 <c>number</c>）：分账方类型，0 商家 / 1 服务商。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>amount（官方类型 <c>number</c>）：金额，单位分。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>settle_no（官方类型 <c>string</c>）：视频号系统分账单号。</summary>
    [JsonPropertyName("settle_no")]
    public string? SettleNo { get; set; }

    /// <summary>settle_id（官方类型 <c>string</c>）：微信支付系统分账单号（敏感，不得进日志）。</summary>
    [JsonPropertyName("settle_id")]
    public string? SettleId { get; set; }

    /// <summary>create_time（官方类型 <c>string</c>）：分账创建时间。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>finish_time（官方类型 <c>string</c>）：分账完成时间。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

    /// <summary>result（官方类型 <c>string</c>）：分账结果，PENDING / SUCCESS / CLOSED。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>fail_reason（官方类型 <c>string</c>）：分账失败原因。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>is_finished（官方类型 <c>number</c>）：是否完成分账，0 未完成 / 1 已完成。</summary>
    [JsonPropertyName("is_finished")]
    public int? IsFinished { get; set; }
}
