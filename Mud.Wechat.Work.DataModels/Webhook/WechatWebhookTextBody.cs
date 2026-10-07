// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人文本消息体（<c>msgtype=text</c>）。
/// <para>content 最长 2048 个字节（超出报错）；换行须使用转义的 \n。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookTextBody
{
    /// <summary>
    /// 获取或设置消息内容（官方必填），最长不超过 2048 个字节，超出报错；换行须使用转义的 \n。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 获取或设置提醒群成员的 userid 列表（支持特殊取值 <c>@all</c> 提醒所有人）；
    /// <b>仅在群聊中生效</b>，与 <see cref="MentionedMobileList"/> 合计最多 20 人。
    /// </summary>
    [JsonPropertyName("mentioned_list")]
    public List<string>? MentionedList { get; set; }

    /// <summary>
    /// 获取或设置提醒群成员的手机号列表（支持特殊取值 <c>@all</c> 提醒所有人）；
    /// <b>仅在群聊中生效</b>，与 <see cref="MentionedList"/> 合计最多 20 人。
    /// </summary>
    [JsonPropertyName("mentioned_mobile_list")]
    public List<string>? MentionedMobileList { get; set; }
}
