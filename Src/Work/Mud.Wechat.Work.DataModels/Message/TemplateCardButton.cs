// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 模板卡片按钮项（<c>template_card.button_list</c> 项，仅按钮交互型），被发送应用消息（<c>/cgi-bin/message/send</c>）与更新模版卡片消息（<c>/cgi-bin/message/update_template_card</c>）两个端点共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class TemplateCardButton
{
    /// <summary>
    /// 获取或设置按钮点击事件类型：0 或不填=回调点击事件，1=跳转 url。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置按钮文案，建议不超过 10 个字。
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 获取或设置按钮样式，可填 1~4，不填或错填默认 1。
    /// </summary>
    [JsonPropertyName("style")]
    public int? Style { get; set; }

    /// <summary>
    /// 获取或设置按钮 key 值，点击后作为 EventKey 返回，最长 1024 字节，不可重复（type 是 0 时必填）。
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// 获取或设置跳转事件的 url，type 是 1 时必填。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
