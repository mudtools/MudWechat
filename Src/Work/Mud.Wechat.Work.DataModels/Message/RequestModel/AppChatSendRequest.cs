// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 应用推送消息到群聊会话请求基类（<c>/cgi-bin/appchat/send</c>，官方文档 90248）。
/// <para>仅自建应用可调用，应用的可见范围必须为根部门，第三方应用不可调用；chatid 所代表的群必须是该应用所创建。</para>
/// <para>限频：每企业消息发送量不可超过 2 万人次/分（群 100 人，每发一次算 100 人次）；按企业规模分档，未认证或小型企业
/// ≤15 万人次/小时、中型 ≤35 万人次/小时、大型 ≤70 万人次/小时；每个成员在群中收到的同一应用消息不可超过
/// 200 条/分、1 万条/天，超过部分被丢弃且接口不报错。</para>
/// <para>本类仅承载信封公共字段，由各消息类型请求子类继承。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class AppChatSendRequest
{
    /// <summary>
    /// 获取或设置群聊 id（官方必填）。
    /// </summary>
    [JsonPropertyName("chatid")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置消息类型（官方必填），由各子类对应固定取值
    /// （text、image、voice、video、file、textcard、news、mpnews、markdown）。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }
}
