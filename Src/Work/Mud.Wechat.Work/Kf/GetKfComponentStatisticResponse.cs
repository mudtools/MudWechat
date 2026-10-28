// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 获取客服数据统计（组件版）响应体（<c>/cgi-bin/kf/get_statistic</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfComponentStatisticResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置逐日统计数据列表。
    /// </summary>
    [JsonPropertyName("statistic_list")]
    public List<KfComponentStatisticItem>? StatisticList { get; set; }
}

/// <summary>
/// 组件版统计的单日数据项（<see cref="GetKfComponentStatisticResponse.StatisticList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfComponentStatisticItem
{
    /// <summary>
    /// 获取或设置统计日期（当日 0 点时间戳）。
    /// </summary>
    [JsonPropertyName("stat_time")]
    public long? StatTime { get; set; }

    /// <summary>
    /// 获取或设置当日的统计数据（当天无数据或尚未计算完成时不返回）。
    /// </summary>
    [JsonPropertyName("statistic")]
    public KfComponentStatisticData? Statistic { get; set; }
}

/// <summary>
/// 组件版统计的当日指标（无数据的指标官方直接缺省不返回，解析需空值容错）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfComponentStatisticData
{
    /// <summary>
    /// 获取或设置咨询会话数（客户发过消息并分配给接待人员或智能助手的会话；转接不产生新会话）。
    /// </summary>
    [JsonPropertyName("session_cnt")]
    public long? SessionCount { get; set; }

    /// <summary>
    /// 获取或设置咨询客户数（同一客户多次咨询只计一次）。
    /// </summary>
    [JsonPropertyName("customer_cnt")]
    public long? CustomerCount { get; set; }

    /// <summary>
    /// 获取或设置咨询消息总数。
    /// </summary>
    [JsonPropertyName("customer_msg_cnt")]
    public long? CustomerMsgCount { get; set; }

    /// <summary>
    /// 获取或设置机器人会话数。
    /// </summary>
    [JsonPropertyName("ai_session_reply_cnt")]
    public long? AiSessionReplyCount { get; set; }

    /// <summary>
    /// 获取或设置接入人工会话数。
    /// </summary>
    [JsonPropertyName("servicer_session_cnt")]
    public long? ServicerSessionCount { get; set; }

    /// <summary>
    /// 获取或设置人工回复率（无客户发消息给接待人员则不返回）。
    /// </summary>
    [JsonPropertyName("reply_rate")]
    public double? ReplyRate { get; set; }

    /// <summary>
    /// 获取或设置平均首次响应时长（秒）。
    /// </summary>
    [JsonPropertyName("first_reply_average_sec")]
    public double? FirstReplyAverageSec { get; set; }

    /// <summary>
    /// 获取或设置满意度评价发送数（官方字段名即为 satisfaction_investgate_cnt；
    /// API 托管会话分配时满意度原生功能失效，此数为 0）。
    /// </summary>
    [JsonPropertyName("satisfaction_investgate_cnt")]
    public long? SatisfactionInvestgateCount { get; set; }

    /// <summary>
    /// 获取或设置满意度参评率。
    /// </summary>
    [JsonPropertyName("satisfaction_participation_rate")]
    public double? SatisfactionParticipationRate { get; set; }

    /// <summary>
    /// 获取或设置「满意」评价占比（无人参评则不返回）。
    /// </summary>
    [JsonPropertyName("satisfied_rate")]
    public double? SatisfiedRate { get; set; }

    /// <summary>
    /// 获取或设置「一般」评价占比（无人参评则不返回）。
    /// </summary>
    [JsonPropertyName("middling_rate")]
    public double? MiddlingRate { get; set; }

    /// <summary>
    /// 获取或设置「不满意」评价占比（无人参评则不返回）。
    /// </summary>
    [JsonPropertyName("dissatisfied_rate")]
    public double? DissatisfiedRate { get; set; }
}
