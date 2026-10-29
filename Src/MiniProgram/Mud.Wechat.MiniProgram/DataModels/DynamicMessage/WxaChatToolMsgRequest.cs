// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DynamicMessage;

/// <summary>修改小程序聊天工具的动态卡片消息请求体（<c>POST /cgi-bin/message/wxopen/chattoolmsg/send</c>）。</summary>
/// <remarks>
/// 官方文档：<c>mp-message-management/updatable-message/api_setchattoolmsg.html</c>。
/// 与 <see cref="WxaUpdateDynamicMessageRequest"/> 同构（<c>parameter_list</c>），但<b>不含</b>游戏态 <c>target_state</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicMessage")]
public class WxaChatToolMsgRequest
{
    /// <summary>动态消息的活动 ID（<c>activity_id</c>，必填）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>动态卡片模板信息（<c>template_info</c>，必填），见 <see cref="WxaDynamicMessageTemplateInfo"/>。</summary>
    [JsonPropertyName("template_info")]
    public WxaDynamicMessageTemplateInfo? TemplateInfo { get; set; }
}
