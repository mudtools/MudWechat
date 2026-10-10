// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 获取客户朋友圈的互动数据响应体（<c>/cgi-bin/externalcontact/get_moment_comments</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class GetMomentCommentsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置评论列表（元素为客户或企业成员，二者不会同时出现）。
    /// </summary>
    [JsonPropertyName("comment_list")]
    public List<MomentCommentItem>? CommentList { get; set; }

    /// <summary>
    /// 获取或设置点赞列表（元素为客户或企业成员，二者不会同时出现）。
    /// </summary>
    [JsonPropertyName("like_list")]
    public List<MomentCommentItem>? LikeList { get; set; }
}

/// <summary>
/// 朋友圈互动数据项（评论 / 点赞列表的元素）。
/// <para><see cref="ExternalUserid"/> 与 <see cref="Userid"/> 不会同时出现：
/// 客户的互动返回 external_userid，企业成员的互动返回 userid。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentCommentItem
{
    /// <summary>
    /// 获取或设置互动客户的 external_userid（仅客户互动时返回）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置互动企业成员的 userid（仅企业成员互动时返回）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置互动的时间戳（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}
