// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 「结算收入数据」(<c>publisher_settlement</c>) 的 <c>settlement_list</c> 元素（单个结算区间）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpPublisherSettlementItem
{
    /// <summary>数据更新时间（官方 <c>date</c>，格式 yyyy-MM-dd）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>日期区间描述（官方 <c>zone</c>，如「2020年3月1日至15日」）。</summary>
    [JsonPropertyName("zone")]
    public string? Zone { get; set; }

    /// <summary>收入月份（官方 <c>month</c>，示例为字符串 yyyyMM）。</summary>
    [JsonPropertyName("month")]
    public string? Month { get; set; }

    /// <summary>上/下半月（官方 <c>order</c>：1=上半月，2=下半月）。</summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }

    /// <summary>结算状态（官方 <c>sett_status</c>：1=结算中；2/3=已结算；4=付款中；5=已付款）。</summary>
    [JsonPropertyName("sett_status")]
    public int SettStatus { get; set; }

    /// <summary>区间内结算收入(分)（官方 <c>settled_revenue</c>）。</summary>
    [JsonPropertyName("settled_revenue")]
    public long SettledRevenue { get; set; }

    /// <summary>结算单编号（官方 <c>sett_no</c>）。</summary>
    [JsonPropertyName("sett_no")]
    public string? SettNo { get; set; }

    /// <summary>申请补发结算单次数（官方 <c>mail_send_cnt</c>，示例回传为字符串）。</summary>
    [JsonPropertyName("mail_send_cnt")]
    public string? MailSendCount { get; set; }

    /// <summary>按广告位的结算金额拆分（官方 <c>slot_revenue</c>）。</summary>
    [JsonPropertyName("slot_revenue")]
    public List<MpPublisherSlotRevenue>? SlotRevenue { get; set; }
}
