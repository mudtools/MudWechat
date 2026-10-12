// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DynamicMessage;

/// <summary>修改动态消息请求体（<c>POST /cgi-bin/message/wxopen/updatablemsg/send</c>）。</summary>
/// <remarks>
/// 官方文档：<c>mp-message-management/updatable-message/api_setupdatablemsg.html</c>。
/// <see cref="TargetState"/>：<c>0</c> 未进入游戏 / <c>1</c> 已进入游戏。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicMessage")]
public class WxaUpdateDynamicMessageRequest
{
    /// <summary>动态消息的活动 ID（<c>activity_id</c>，必填；经 <see cref="WxaCreateActivityIdResponse.ActivityId"/> 取得）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>目标状态（<c>target_state</c>，必填）：<c>0</c> 未进入游戏 / <c>1</c> 已进入游戏。</summary>
    [JsonPropertyName("target_state")]
    public long? TargetState { get; set; }

    /// <summary>动态消息模板信息（<c>template_info</c>，必填），见 <see cref="WxaDynamicMessageTemplateInfo"/>。</summary>
    [JsonPropertyName("template_info")]
    public WxaDynamicMessageTemplateInfo? TemplateInfo { get; set; }
}
