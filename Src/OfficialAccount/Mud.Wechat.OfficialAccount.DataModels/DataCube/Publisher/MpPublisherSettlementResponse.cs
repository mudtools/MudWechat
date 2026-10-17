// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 「获取服务号结算收入数据及结算主体信息」(<c>publisher_settlement</c>) 响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpPublisherSettlementResponse : MpResponse
{
    /// <summary>回包错误信封（官方 <c>base_resp</c>）。</summary>
    [JsonPropertyName("base_resp")]
    public MpPublisherBaseResp? BaseResp { get; set; }

    /// <summary>结算主体名称（官方 <c>body</c>）。</summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>累计收入(分)（官方 <c>revenue_all</c>）。</summary>
    [JsonPropertyName("revenue_all")]
    public long RevenueAll { get; set; }

    /// <summary>扣除金额(分)（官方 <c>penalty_all</c>）。</summary>
    [JsonPropertyName("penalty_all")]
    public long PenaltyAll { get; set; }

    /// <summary>已结算金额(分)（官方 <c>settled_revenue_all</c>）。</summary>
    [JsonPropertyName("settled_revenue_all")]
    public long SettledRevenueAll { get; set; }

    /// <summary>结算区间列表（官方 <c>settlement_list</c>）。</summary>
    [JsonPropertyName("settlement_list")]
    public List<MpPublisherSettlementItem>? SettlementList { get; set; }

    /// <summary>请求返回总条数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long TotalNum { get; set; }
}
