// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

/// <summary>
/// 获取「联系客户统计」数据响应体（<c>/cgi-bin/externalcontact/get_user_behavior_data</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GetUserBehaviorDataResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置成员联系客户的行为数据列表（每项为一天的数据）。
    /// </summary>
    [JsonPropertyName("behavior_data")]
    public List<UserBehaviorData>? BehaviorData { get; set; }
}

/// <summary>
/// 成员联系客户的单日行为数据（<c>behavior_data[]</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class UserBehaviorData
{
    /// <summary>
    /// 获取或设置数据日期（为当日 0 点的时间戳）。
    /// </summary>
    [JsonPropertyName("stat_time")]
    public long? StatTime { get; set; }

    /// <summary>
    /// 获取或设置发起申请数（成员通过搜索手机号、扫一扫、微信好友 / 群聊添加、
    /// 添加共享分配客户等渠道主动发起的好友申请数量）。
    /// </summary>
    [JsonPropertyName("new_apply_cnt")]
    public long? NewApplyCnt { get; set; }

    /// <summary>
    /// 获取或设置新增客户数（成员新添加的客户数量）。
    /// </summary>
    [JsonPropertyName("new_contact_cnt")]
    public long? NewContactCnt { get; set; }

    /// <summary>
    /// 获取或设置聊天总数（有主动发过消息的单聊总数）。
    /// </summary>
    [JsonPropertyName("chat_cnt")]
    public long? ChatCnt { get; set; }

    /// <summary>
    /// 获取或设置发送消息数（单聊中发送的消息总数）。
    /// </summary>
    [JsonPropertyName("message_cnt")]
    public long? MessageCnt { get; set; }

    /// <summary>
    /// 获取或设置已回复聊天占比（浮点数；客户主动发起聊天后当日有回复的聊天数占比，
    /// 不含群聊，仅在确有聊天时返回）。
    /// </summary>
    [JsonPropertyName("reply_percentage")]
    public double? ReplyPercentage { get; set; }

    /// <summary>
    /// 获取或设置平均首次回复时长（分钟；所有聊天首次回复总时长 / 已回复聊天总数，
    /// 不含群聊，仅在确有聊天时返回）。
    /// </summary>
    [JsonPropertyName("avg_reply_time")]
    public double? AvgReplyTime { get; set; }

    /// <summary>
    /// 获取或设置删除 / 拉黑成员的客户数。
    /// </summary>
    [JsonPropertyName("negative_feedback_cnt")]
    public long? NegativeFeedbackCnt { get; set; }
}
