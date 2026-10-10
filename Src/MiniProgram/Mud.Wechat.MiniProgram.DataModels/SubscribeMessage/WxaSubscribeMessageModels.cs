// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.SubscribeMessage;

/// <summary>发送订阅消息请求体（<c>POST /cgi-bin/message/subscribe/send</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>mp-message-management/subscribe-message/api_sendmessage.html</c>。</para>
/// <para>
/// <b>一次性 vs 长期（官方原文）</b>：一次性订阅消息须用户逐次授权（授权一次推一条）；长期订阅消息
/// 一次性授权可长期推送（受条数与灰度约束）。<c>data</c> 中的键必须与模板关键词一致，否则报 <c>47003</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscribeMessage")]
public class WxaSubscribeMessageSendRequest
{
    /// <summary>接收者（用户）的 <c>openid</c>（<c>touser</c>，必填）。</summary>
    [JsonPropertyName("touser")]
    public string? Touser { get; set; }

    /// <summary>所需下发的订阅模板 ID（<c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>点击模板卡片后的跳转页面（<c>page</c>，选填；仅限本小程序内页面，可带参数如 <c>index?foo=bar</c>）。</summary>
    [JsonPropertyName("page")]
    public string? Page { get; set; }

    /// <summary>跳转小程序类型（<c>miniprogram_state</c>，选填）：<c>developer</c> 开发版 / <c>trial</c> 体验版 / <c>formal</c> 正式版（默认）。</summary>
    [JsonPropertyName("miniprogram_state")]
    public string? MiniprogramState { get; set; }

    /// <summary>进入小程序查看「订阅消息」的语言（<c>lang</c>，选填）：<c>zh_CN</c> / <c>zh_TW</c> / <c>en-US</c>，默认 <c>zh_CN</c>。</summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }

    /// <summary>模板内容（<c>data</c>，必填；键为模板关键词 ID，键与值均须与模板匹配），见 <see cref="WxaSubscribeMessageDataValue"/>。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, WxaSubscribeMessageDataValue>? Data { get; set; }
}

/// <summary>订阅消息模板内容取值（<c>data.{key}</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "SubscribeMessage")]
public class WxaSubscribeMessageDataValue
{
    /// <summary>关键词值（<c>value</c>，必填；须与模板规定的类型一致）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>激活与更新服务卡片请求体（<c>POST /wxa/set_user_notify</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>mp-message-management/subscribe-message/api_setusernotify.html</c>。</para>
/// <para><c>template_id</c> 为服务卡片模板 ID；<c>data</c> 为卡片内容键值集（键须在模板中登记）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscribeMessage")]
public class WxaSetUserNotifyRequest
{
    /// <summary>用户 <c>openid</c>（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>服务卡片模板 ID（<c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>点击卡片后的跳转页面（<c>page</c>，选填；仅限本小程序内页面）。</summary>
    [JsonPropertyName("page")]
    public string? Page { get; set; }

    /// <summary>卡片内容（<c>data</c>，必填），见 <see cref="WxaServiceCardDataValue"/>。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, WxaServiceCardDataValue>? Data { get; set; }
}

/// <summary>更新服务卡片扩展信息请求体（<c>POST /wxa/set_user_notifyext</c>）。</summary>
/// <remarks>
/// 官方文档：<c>mp-message-management/subscribe-message/api_setusernotifyext.html</c>。
/// 只<b>追加/更新扩展信息</b>（如按钮、表单等交互组件数据），不改写卡片主体内容；卡片须先经
/// <c>set_user_notify</c> 激活。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscribeMessage")]
public class WxaSetUserNotifyExtRequest
{
    /// <summary>用户 <c>openid</c>（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>服务卡片模板 ID（<c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>扩展信息内容（<c>data</c>，必填），见 <see cref="WxaServiceCardDataValue"/>。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, WxaServiceCardDataValue>? Data { get; set; }
}

/// <summary>服务卡片内容取值（<c>{key: {value}}</c>）。</summary>
/// <remarks>value 可为字符串或数值（以模板定义为准）；SDK 不做本地类型转换。</remarks>
[HttpJsonSerializable(SerializerClassName = "SubscribeMessage")]
public class WxaServiceCardDataValue
{
    /// <summary>内容值（<c>value</c>，必填）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>查询服务卡片状态请求体（<c>POST /wxa/get_user_notify</c>）。</summary>
/// <remarks>官方文档：<c>mp-message-management/subscribe-message/api_getusernotify.html</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "SubscribeMessage")]
public class WxaGetUserNotifyRequest
{
    /// <summary>用户 <c>openid</c>（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>服务卡片模板 ID（<c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }
}

/// <summary>查询服务卡片状态应答（<c>notify_status</c>）。</summary>
/// <remarks><c>notify_status</c> = <c>1</c> 已激活 / <c>0</c> 未激活（数值以官方页面为准）。</remarks>
[HttpJsonSerializable(SerializerClassName = "SubscribeMessage")]
public class WxaGetUserNotifyResponse : WxaResponse
{
    /// <summary>服务卡片激活状态（<c>notify_status</c>）：<c>1</c> 已激活 / <c>0</c> 未激活。</summary>
    [JsonPropertyName("notify_status")]
    public long? NotifyStatus { get; set; }
}