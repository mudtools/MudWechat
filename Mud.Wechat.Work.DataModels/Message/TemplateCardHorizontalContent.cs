// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 模板卡片二级标题+文本列表项（<c>template_card.horizontal_content_list</c> 项），被发送应用消息（<c>/cgi-bin/message/send</c>）与更新模版卡片消息（<c>/cgi-bin/message/update_template_card</c>）两个端点共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class TemplateCardHorizontalContent
{
    /// <summary>
    /// 获取或设置链接类型：0 或不填=不是链接，1=跳转 url，2=下载附件，3=点击跳转成员详情（type 3 需企业微信 3.1.18 及以上）。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置二级标题，建议不超过 5 个字。
    /// </summary>
    [JsonPropertyName("keyname")]
    public string? KeyName { get; set; }

    /// <summary>
    /// 获取或设置二级文本，建议不超过 30 个字（支持 id 转译；type 是 2 时代表文件名称，要包含文件类型）。
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// 获取或设置链接跳转的 url，type 是 1 时必填。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置附件的 media_id，type 是 2 时必填。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置成员详情的 userid，type 是 3 时必填。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }
}
