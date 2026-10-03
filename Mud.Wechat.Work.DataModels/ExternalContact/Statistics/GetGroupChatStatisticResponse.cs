// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

/// <summary>
/// 获取「群聊数据统计」（按群主聚合）响应体（<c>/cgi-bin/externalcontact/groupchat/statistic</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GetGroupChatStatisticResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置命中过滤条件的记录总个数。
    /// </summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>
    /// 获取或设置下一个分页的 offset（与 total 相等时表示已经取完数据）。
    /// </summary>
    [JsonPropertyName("next_offset")]
    public long? NextOffset { get; set; }

    /// <summary>
    /// 获取或设置按群主聚合的统计记录列表。
    /// </summary>
    [JsonPropertyName("items")]
    public List<GroupChatStatisticItem>? Items { get; set; }
}

/// <summary>
/// 按群主聚合的统计记录（按群主聚合响应中 <c>items[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GroupChatStatisticItem
{
    /// <summary>
    /// 获取或设置群主的 userid。
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// 获取或设置该群主的客户群统计数据明细。
    /// </summary>
    [JsonPropertyName("data")]
    public GroupChatStatisticData? Data { get; set; }
}
