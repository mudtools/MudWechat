// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 模板卡片跳转指引样式列表项（<c>template_card.jump_list</c> 项），被发送应用消息（<c>/cgi-bin/message/send</c>）与更新模版卡片消息（<c>/cgi-bin/message/update_template_card</c>）两个端点共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class TemplateCardJump
{
    /// <summary>
    /// 获取或设置跳转链接类型：0 或不填=不是链接，1=跳转 url，2=跳转小程序，
    /// 3=触发消息智能回复（官方智能机器人 101032；<b>仅文本通知型/图文展示型卡片的跳转指引区支持</b>，且此时 <see cref="Question"/> 必填）。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置智能问答问题（官方智能机器人 101032；<see cref="Type"/> 为 3 时必填，最长不超过 200 个字节）。
    /// </summary>
    /// <remarks>该字段为智能机器人应答上下文专用；应用消息侧（<c>/cgi-bin/message/send</c>）不使用 Type=3 与 question。</remarks>
    [JsonPropertyName("question")]
    public string? Question { get; set; }

    /// <summary>
    /// 获取或设置跳转链接样式的文案内容，建议不超过 18 个字。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置跳转链接的 url，type 是 1 时必填。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置跳转小程序的 appid（须与当前应用关联），type 是 2 时必填。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置跳转小程序的 pagepath，type 是 2 时选填。
    /// </summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }
}
