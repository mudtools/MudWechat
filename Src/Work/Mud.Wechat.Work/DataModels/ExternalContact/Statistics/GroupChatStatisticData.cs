// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

/// <summary>
/// 客户群统计数据明细（<c>items[].data</c>；按群主聚合与按自然日聚合两种统计接口共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GroupChatStatisticData
{
    /// <summary>
    /// 获取或设置新增客户群数量。
    /// </summary>
    [JsonPropertyName("new_chat_cnt")]
    public long? NewChatCnt { get; set; }

    /// <summary>
    /// 获取或设置截至当天的客户群总数量。
    /// </summary>
    [JsonPropertyName("chat_total")]
    public long? ChatTotal { get; set; }

    /// <summary>
    /// 获取或设置截至当天有发过消息的客户群数量。
    /// </summary>
    [JsonPropertyName("chat_has_msg")]
    public long? ChatHasMsg { get; set; }

    /// <summary>
    /// 获取或设置客户群新增群人数。
    /// </summary>
    [JsonPropertyName("new_member_cnt")]
    public long? NewMemberCnt { get; set; }

    /// <summary>
    /// 获取或设置截至当天的客户群总人数。
    /// </summary>
    [JsonPropertyName("member_total")]
    public long? MemberTotal { get; set; }

    /// <summary>
    /// 获取或设置截至当天有发过消息的群成员数。
    /// </summary>
    [JsonPropertyName("member_has_msg")]
    public long? MemberHasMsg { get; set; }

    /// <summary>
    /// 获取或设置截至当天的客户群消息总数。
    /// </summary>
    [JsonPropertyName("msg_total")]
    public long? MsgTotal { get; set; }

    /// <summary>
    /// 获取或设置截至当天新增的迁移群数（仅教培行业返回）。
    /// </summary>
    [JsonPropertyName("migrate_trainee_chat_cnt")]
    public long? MigrateTraineeChatCnt { get; set; }
}
