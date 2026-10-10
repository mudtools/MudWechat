// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 模板卡片左图右文样式（<c>template_card.image_text_area</c>，仅图文展示型），被发送应用消息（<c>/cgi-bin/message/send</c>）与更新模版卡片消息（<c>/cgi-bin/message/update_template_card</c>）两个端点共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class TemplateCardImageTextArea
{
    /// <summary>
    /// 获取或设置区域点击事件：0 或不填=没有点击事件，1=跳转 url，2=跳转小程序。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置点击跳转的 url，type 是 1 时必填。
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

    /// <summary>
    /// 获取或设置左图右文样式的标题。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置左图右文样式的描述。
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>
    /// 获取或设置左图右文样式的图片 url。
    /// </summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}
