// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DynamicMessage;

/// <summary>创建动态消息 <c>activity_id</c> 请求体（<c>POST /cgi-bin/message/wxopen/activityid/create</c>）。</summary>
/// <remarks>
/// 官方文档：<c>mp-message-management/updatable-message/api_createactivityid.html</c>。
/// 官方无<b>必填</b>参数（请求体可为空对象）；<see cref="UnionId"/> 为选填的开放平台用户标识。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicMessage")]
public class WxaCreateActivityIdRequest
{
    /// <summary>开放平台用户标识（<c>unionid</c>，选填）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }
}

/// <summary>创建动态消息 <c>activity_id</c> 应答（<c>activity_id</c> + 有效期）。</summary>
/// <remarks>
/// <para>官方文档：<c>mp-message-management/updatable-message/api_createactivityid.html</c>。</para>
/// <para><b>一次性（官方原文）</b>：<see cref="ActivityId"/> 只能被使用一次（分享后即失效）；逾期亦失效。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicMessage")]
public class WxaCreateActivityIdResponse : WxaResponse
{
    /// <summary>动态消息活动唯一标识（<c>activity_id</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>过期时间（<c>expiration_time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("expiration_time")]
    public long? ExpirationTime { get; set; }

    /// <summary>是否已过期（<c>is_expired</c>）。</summary>
    [JsonPropertyName("is_expired")]
    public bool? IsExpired { get; set; }
}

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

/// <summary>动态消息模板信息（<c>template_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicMessage")]
public class WxaDynamicMessageTemplateInfo
{
    /// <summary>参数列表（<c>parameter_list</c>，必填；键为模板占位名、值为展示文本），见 <see cref="WxaDynamicMessageParameter"/>。</summary>
    [JsonPropertyName("parameter_list")]
    public List<WxaDynamicMessageParameter>? ParameterList { get; set; }
}

/// <summary>动态消息模板参数（<c>parameter_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicMessage")]
public class WxaDynamicMessageParameter
{
    /// <summary>参数名（<c>name</c>，必填；与动态消息模板占位对应）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>参数值（<c>value</c>，必填；展示文本，长度上限以官方页面为准）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}