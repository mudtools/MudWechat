// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 广告金发放记录条目（官方 <c>Res.adver_funds_list</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>fund_type</c> 三支取值</b>（0 通用赠送 / 1 广告激励 / 2 定向激励）决定后续可用性：「下载广告金对应的商户订单信息」官方原文<b>仅支持通用赠送（0）</b>。</para>
/// <para><b><c>remain_amount</c> 与 <c>expire_time</c> 须同时读</b>：广告金有过期时刻，只看余额会在过期日突然失血（与短剧流量包同口径提醒）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_adver_funds.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayAdverFund
{
    /// <summary>settle_begin（官方类型 <c>number</c>）：结算周期开始时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_begin")]
    public long? SettleBegin { get; set; }

    /// <summary>settle_end（官方类型 <c>number</c>）：结算周期结束时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_end")]
    public long? SettleEnd { get; set; }

    /// <summary>total_amount（官方类型 <c>number</c>）：发放广告金金额，单位分</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>remain_amount（官方类型 <c>number</c>）：剩余可用广告金金额，单位分</summary>
    [JsonPropertyName("remain_amount")]
    public long? RemainAmount { get; set; }

    /// <summary>expire_time（官方类型 <c>number</c>）：广告金过期时间，unix秒级时间戳</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>fund_type（官方类型 <c>number</c>）：广告金发放原因， 0:通用赠送，1:广告激励，2:定向激励</summary>
    [JsonPropertyName("fund_type")]
    public long? FundType { get; set; }

    /// <summary>fund_id（官方类型 <c>string</c>）：广告金发放ID</summary>
    [JsonPropertyName("fund_id")]
    public string? FundId { get; set; }
}
