// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 创建企业群发响应体（<c>/cgi-bin/externalcontact/add_msg_template</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class AddGroupMsgTemplateResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置无效或无法发送的 external_userid / chat_id 列表
    /// （成员需要确认后才会真正发送，此处仅表示创建任务时即无效的目标）。
    /// </summary>
    [JsonPropertyName("fail_list")]
    public List<string>? FailList { get; set; }

    /// <summary>
    /// 获取或设置群发消息 id（用于查询群发消息发送结果、提醒 / 停止群发）。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }
}
