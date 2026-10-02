// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 模板卡片选择题样式（<c>template_card.checkbox</c>，仅投票选择型），被发送应用消息（<c>/cgi-bin/message/send</c>）与更新模版卡片消息（<c>/cgi-bin/message/update_template_card</c>）两个端点共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class TemplateCardCheckbox
{
    /// <summary>
    /// 获取或设置选择题 key 值，用户提交选项后随回调事件返回表示该题，最长 1024 字节。
    /// </summary>
    [JsonPropertyName("question_key")]
    public string? QuestionKey { get; set; }

    /// <summary>
    /// 获取或设置是否可以选择状态（仅更新模版卡片消息接口支持）。
    /// </summary>
    [JsonPropertyName("disable")]
    public bool? Disable { get; set; }

    /// <summary>
    /// 获取或设置选择题模式：单选 0，多选 1，不填默认 0。
    /// </summary>
    [JsonPropertyName("mode")]
    public int? Mode { get; set; }

    /// <summary>
    /// 获取或设置选项列表，最少 1 个、不超过 20 个。
    /// </summary>
    [JsonPropertyName("option_list")]
    public List<TemplateCardOption>? OptionList { get; set; }
}
