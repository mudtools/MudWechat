// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

/// <summary>
/// 获取「群聊数据统计」（按自然日聚合）响应体（<c>/cgi-bin/externalcontact/groupchat/statistic_group_by_day</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GetGroupChatStatisticGroupByDayResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置按自然日聚合的统计记录列表。
    /// </summary>
    [JsonPropertyName("items")]
    public List<GroupChatStatisticDayItem>? Items { get; set; }
}

/// <summary>
/// 按自然日聚合的统计记录（按自然日聚合响应中 <c>items[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GroupChatStatisticDayItem
{
    /// <summary>
    /// 获取或设置数据日期（为当日 0 点的时间戳）。
    /// </summary>
    [JsonPropertyName("stat_time")]
    public long? StatTime { get; set; }

    /// <summary>
    /// 获取或设置该日的客户群统计数据明细。
    /// </summary>
    [JsonPropertyName("data")]
    public GroupChatStatisticData? Data { get; set; }
}
