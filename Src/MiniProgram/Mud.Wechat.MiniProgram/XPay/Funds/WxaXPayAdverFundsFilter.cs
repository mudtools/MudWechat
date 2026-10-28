// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 广告金发放记录筛选条件（官方请求侧子表 <c>filter</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_adver_funds.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayAdverFundsFilter
{
    /// <summary>settle_begin（官方类型 <c>number</c>）：查询结算周期开始时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_begin")]
    public long? SettleBegin { get; set; }

    /// <summary>settle_end（官方类型 <c>number</c>）：查询结算周期结束时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_end")]
    public long? SettleEnd { get; set; }

    /// <summary>fund_type（官方类型 <c>number</c>）：(可选)广告金发放原因， 0:通用赠送，1:广告激励，2:定向激励</summary>
    [JsonPropertyName("fund_type")]
    public long? FundType { get; set; }
}
