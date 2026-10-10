// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

/// <summary>
/// 获取「群聊数据统计」（按自然日聚合）请求体（<c>/cgi-bin/externalcontact/groupchat/statistic_group_by_day</c>）。
/// <para>查询区间为闭区间，最大跨度 30 天，数据最多保留 180 天；
/// 时间戳非 0 点时自动向下取整到当日 0 点。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GetGroupChatStatisticGroupByDayRequest
{
    /// <summary>
    /// 获取或设置起始日期的时间戳（官方必填；填当天 0 时 0 分 0 秒，非 0 点自动处理为当天 0 点；
    /// 取值范围为昨天至前 180 天）。
    /// </summary>
    [JsonPropertyName("day_begin_time")]
    public long? DayBeginTime { get; set; }

    /// <summary>
    /// 获取或设置结束日期的时间戳（不填默认同起始日期；规则同起始日期）。
    /// </summary>
    [JsonPropertyName("day_end_time")]
    public long? DayEndTime { get; set; }

    /// <summary>
    /// 获取或设置群主过滤条件（官方必填；不填表示取应用可见范围内全部群主，
    /// 可见范围人数超过 1000 人会报错 81017）。
    /// </summary>
    [JsonPropertyName("owner_filter")]
    public GroupChatStatisticOwnerFilter? OwnerFilter { get; set; }
}
