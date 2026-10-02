// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 群聊会话文本消息体（text，应用推送消息到群聊会话专属，支持 @ 群成员）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class AppChatTextBody
{
    /// <summary>
    /// 获取或设置消息内容（官方必填），最长不超过 2048 个字节（支持转义的 \n 换行；支持
    /// &lt;@userid&gt; 扩展语法 @ 群成员，企业微信 5.0.6 及以上版本支持）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 获取或设置提醒群中指定成员（@ 群成员）的 userid 列表；特殊值 @all 表示提醒所有人。
    /// </summary>
    [JsonPropertyName("mentioned_list")]
    public List<string>? MentionedList { get; set; }
}
