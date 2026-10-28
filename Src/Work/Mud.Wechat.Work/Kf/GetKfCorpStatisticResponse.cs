// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 获取「客户数据统计」企业汇总数据响应体（<c>/cgi-bin/kf/get_corp_statistic</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfCorpStatisticResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置逐日统计数据列表。
    /// </summary>
    [JsonPropertyName("statistic_list")]
    public List<KfCorpStatisticItem>? StatisticList { get; set; }
}

/// <summary>
/// 企业汇总统计的单日数据项（<see cref="GetKfCorpStatisticResponse.StatisticList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfCorpStatisticItem
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
    public KfCorpStatisticData? Statistic { get; set; }
}

/// <summary>
/// 企业汇总统计的当日指标（无数据的指标官方直接缺省不返回，解析需空值容错）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfCorpStatisticData
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
    /// 获取或设置客户发送的消息总数。
    /// </summary>
    [JsonPropertyName("customer_msg_cnt")]
    public long? CustomerMsgCount { get; set; }

    /// <summary>
    /// 获取或设置升级服务客户数（同一客户添加多个专员 / 群只计一次；2022 年 3 月 10 日之后才有数据）。
    /// </summary>
    [JsonPropertyName("upgrade_service_customer_cnt")]
    public long? UpgradeServiceCustomerCount { get; set; }

    /// <summary>
    /// 获取或设置分配给智能助手的会话数。
    /// </summary>
    [JsonPropertyName("ai_session_reply_cnt")]
    public long? AiSessionReplyCount { get; set; }

    /// <summary>
    /// 获取或设置转人工率。
    /// </summary>
    [JsonPropertyName("ai_transfer_rate")]
    public double? AiTransferRate { get; set; }

    /// <summary>
    /// 获取或设置知识命中率（须开启智能回复原生功能并配置知识库才产生；不返回表示无法计算；
    /// API 托管会话分配时智能回复原生功能失效）。
    /// </summary>
    [JsonPropertyName("ai_knowledge_hit_rate")]
    public double? AiKnowledgeHitRate { get; set; }

    /// <summary>
    /// 获取或设置被拒收消息的客户数（点击了「不再接收消息」的客户）。
    /// </summary>
    [JsonPropertyName("msg_rejected_customer_cnt")]
    public long? MsgRejectedCustomerCount { get; set; }
}
