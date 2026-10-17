// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 结算收入数据项（官方 <c>settlement_list</c> 数组内层，<c>agency_get_settlement</c> / <c>get_agency_settled_revenue</c> 共用）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/settlement/GetAgencySettlement.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsSettlement
{
    /// <summary>数据更新时间（官方字段名 <c>date</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>日期区间（官方字段名 <c>zone</c>）。</summary>
    [JsonPropertyName("zone")]
    public string? Zone { get; set; }

    /// <summary>收入月份（官方字段名 <c>month</c>）。</summary>
    [JsonPropertyName("month")]
    public string? Month { get; set; }

    /// <summary>1 = 上半月，2 = 下半月（官方字段名 <c>order</c>，number）。</summary>
    [JsonPropertyName("order")]
    public long Order { get; set; }

    /// <summary>结算状态：1 结算中；2/3 已结算；4 付款中；5 已付款（官方字段名 <c>sett_status</c>，number）。</summary>
    [JsonPropertyName("sett_status")]
    public long SettStatus { get; set; }

    /// <summary>区间内结算收入（官方字段名 <c>settled_revenue</c>，number）。</summary>
    [JsonPropertyName("settled_revenue")]
    public long SettledRevenue { get; set; }

    /// <summary>结算单编号（官方字段名 <c>sett_no</c>）。</summary>
    [JsonPropertyName("sett_no")]
    public string? SettNo { get; set; }

    /// <summary>申请补发结算单次数（官方字段名 <c>mail_send_cnt</c>，官方为 string）。</summary>
    [JsonPropertyName("mail_send_cnt")]
    public string? MailSendCnt { get; set; }

    /// <summary>各广告位结算金额（官方字段名 <c>slot_revenue</c>）。</summary>
    [JsonPropertyName("slot_revenue")]
    public List<OpenPlatformAmsSlotRevenue>? SlotRevenue { get; set; }
}
