// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 订单资金结算信息（官方 <c>SettlementInfo</c> 表）。
/// </summary>
/// <remarks>
/// <para><b>结算状态四值</b>（官方 <c>settle_status</c> 表）：1 待结算 / 2 已结算 / 3 已关闭 / 4 交易异常；
/// 异常时读 <see cref="SettleAbnormalReason"/>。</para>
/// <para><b>手续费与结算时间只在结算后有意义</b>：<c>handing_fee</c> 是结算最终收取的支付手续费（单位分）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopSettlementInfo
{
    /// <summary>settlement_list（官方类型 <c>SettlementRecord[]</c>）：分账记录列表。</summary>
    [JsonPropertyName("settlement_list")]
    public List<WxaShopSettlementRecord>? SettlementList { get; set; }

    /// <summary>handing_fee（官方类型 <c>number</c>）：结算最终收取的支付手续费，单位分。</summary>
    [JsonPropertyName("handing_fee")]
    public long? HandingFee { get; set; }

    /// <summary>settle_status（官方类型 <c>number</c>）：商家结算状态，1 待结算 / 2 已结算 / 3 已关闭 / 4 交易异常。</summary>
    [JsonPropertyName("settle_status")]
    public int? SettleStatus { get; set; }

    /// <summary>settle_time（官方类型 <c>string</c>）：商家结算时间。</summary>
    [JsonPropertyName("settle_time")]
    public string? SettleTime { get; set; }

    /// <summary>settle_abnormal_reason（官方类型 <c>string</c>）：结算异常原因。</summary>
    [JsonPropertyName("settle_abnormal_reason")]
    public string? SettleAbnormalReason { get; set; }
}
