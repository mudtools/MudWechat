// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 模板卡片下拉式选择器（<c>template_card.select_list</c> 项，仅多项选择型），被发送应用消息（<c>/cgi-bin/message/send</c>）与更新模版卡片消息（<c>/cgi-bin/message/update_template_card</c>）两个端点共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class TemplateCardSelector
{
    /// <summary>
    /// 获取或设置选择器题目的 key，用户提交选项后随回调事件返回表示该题，最长 1024 字节，不可重复。
    /// </summary>
    [JsonPropertyName("question_key")]
    public string? QuestionKey { get; set; }

    /// <summary>
    /// 获取或设置下拉式选择器上面的 title。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置选项列表，最少 1 个、不超过 10 个。
    /// </summary>
    [JsonPropertyName("option_list")]
    public List<TemplateCardOption>? OptionList { get; set; }

    /// <summary>
    /// 获取或设置默认选定的 id，不填或错填默认第一个。
    /// </summary>
    [JsonPropertyName("selected_id")]
    public string? SelectedId { get; set; }

    /// <summary>
    /// 获取或设置是否可以选择状态（仅更新模版卡片消息接口支持）。
    /// </summary>
    [JsonPropertyName("disable")]
    public bool? Disable { get; set; }
}
