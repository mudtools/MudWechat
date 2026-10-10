// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 模板卡片一级标题对象（<c>template_card.main_title</c>），被发送应用消息（<c>/cgi-bin/message/send</c>）与更新模版卡片消息（<c>/cgi-bin/message/update_template_card</c>）两个端点共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class TemplateCardTitle
{
    /// <summary>
    /// 获取或设置一级标题，建议不超过 36 个字（投票选择型不超过 16 个字；支持 id 转译；文本通知型非必填但不可与 sub_title_text 都不填，其余卡片型必填）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置标题辅助信息/二级普通文本，建议不超过 44 个字（投票/多项选择型不超过 160 个字；支持 id 转译）。
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}
