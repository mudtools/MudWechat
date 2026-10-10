// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 发送应用消息响应体（<c>/cgi-bin/message/send</c>）。
/// <para>部分接收人无权限或不存在时发送仍会执行，但会返回无效部分，常见原因是接收人不在应用可见范围内；
/// 全部接收人无权限或不存在则调用失败，errcode 返回 81013。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageSendResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置不合法的 userid，不区分大小写，统一转为小写（格式 userid1|userid2）。
    /// </summary>
    [JsonPropertyName("invaliduser")]
    public string? InvalidUser { get; set; }

    /// <summary>
    /// 获取或设置不合法的 partyid（格式 partyid1|partyid2）。
    /// </summary>
    [JsonPropertyName("invalidparty")]
    public string? InvalidParty { get; set; }

    /// <summary>
    /// 获取或设置不合法的标签 id（格式 tagid1|tagid2）。
    /// </summary>
    [JsonPropertyName("invalidtag")]
    public string? InvalidTag { get; set; }

    /// <summary>
    /// 获取或设置没有基础接口许可（包含已过期）的 userid（在应用可见范围内但没有基础接口权限）。
    /// </summary>
    [JsonPropertyName("unlicenseduser")]
    public string? UnlicensedUser { get; set; }

    /// <summary>
    /// 获取或设置消息 id，用于撤回应用消息。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>
    /// 获取或设置更新卡片所需的 response_code，仅消息类型为按钮交互型、投票选择型、多项选择型，
    /// 以及填写了 action_menu 字段的文本通知型、图文展示型模板卡片消息返回；
    /// 应用可用其调用更新模版卡片消息接口，72 小时内有效且只能使用一次。
    /// </summary>
    [JsonPropertyName("response_code")]
    public string? ResponseCode { get; set; }
}
