// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 结算收入数据应答（<c>agency_get_settlement</c> 与 <c>get_agency_settled_revenue</c> 两端点应答形状逐字相同 ⇒ 共用本 DTO）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/settlement/GetAgencySettlement.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsSettlementResponse : OpenPlatformAmsResponse
{
    /// <summary>累计收入（官方字段名 <c>revenue_all</c>，number）。</summary>
    [JsonPropertyName("revenue_all")]
    public long RevenueAll { get; set; }

    /// <summary>扣除金额（官方字段名 <c>penalty_all</c>，number）。</summary>
    [JsonPropertyName("penalty_all")]
    public long PenaltyAll { get; set; }

    /// <summary>已结算金额（官方字段名 <c>settled_revenue_all</c>，number）。</summary>
    [JsonPropertyName("settled_revenue_all")]
    public long SettledRevenueAll { get; set; }

    /// <summary>结算明细列表（官方字段名 <c>settlement_list</c>）。</summary>
    [JsonPropertyName("settlement_list")]
    public List<OpenPlatformAmsSettlement>? SettlementList { get; set; }

    /// <summary>请求返回总条数（官方字段名 <c>total_num</c>，number）。</summary>
    [JsonPropertyName("total_num")]
    public long TotalNum { get; set; }
}
