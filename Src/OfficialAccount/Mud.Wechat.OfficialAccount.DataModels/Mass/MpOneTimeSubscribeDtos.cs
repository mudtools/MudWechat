// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>
/// 发送一次性订阅消息（<c>message/template/subscribe</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>touser</c>/<c>template_id</c>/<c>scene</c>/<c>title</c>/<c>data</c>
/// 必填；<c>url</c>/<c>miniprogram</c> 可选（都不传则无跳转；都传优先跳小程序）。
/// </para>
/// <para>
/// <b>与订阅通知 bizsend 的 data 形态不同</b>：本端点 data 为固定单键 <c>content</c>
/// （<c>{"content": {"value": …, "color": …}}</c>，value 200 字内、color 字体颜色如 #FF0000）——
/// 与模板消息（key→{value}）和订阅通知（key→{value} 类型化前缀键袋）都不同，强类型建模。
/// </para>
/// <para>官方「注意事项」原文：①「url 和 miniprogram 都是非必填字段，若都不传则模板无跳转；
/// 若都传会优先跳转至小程序」；②「用户已关注公众号时消息下发到公众号会话，未关注时下发到服务通知」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpOneTimeSubscribeRequest
{
    /// <summary>获取或设置接收消息的用户 openid（官方 <c>touser</c>，必填）。</summary>
    [JsonPropertyName("touser")]
    public string ToUser { get; set; } = string.Empty;

    /// <summary>获取或设置订阅消息模板 ID（官方 <c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置点击消息跳转链接（官方 <c>url</c>，可选；官方示例标注需 ICP 备案）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置跳小程序配置（官方 <c>miniprogram</c>，可选；appid/pagepath 官方标必填——与模板消息同形共用）。</summary>
    [JsonPropertyName("miniprogram")]
    public Template.MpTemplateMiniProgram? MiniProgram { get; set; }

    /// <summary>获取或设置订阅场景值（官方 <c>scene</c>，必填；官方示例为字符串形态 "1000"）。</summary>
    [JsonPropertyName("scene")]
    public string Scene { get; set; } = string.Empty;

    /// <summary>获取或设置消息标题（官方 <c>title</c>，必填；<b>15 字以内</b>）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置消息内容（官方 <c>data</c>，必填；固定单键 content 形态——见类型 remarks）。</summary>
    [JsonPropertyName("data")]
    public MpOneTimeSubscribeData Data { get; set; } = new MpOneTimeSubscribeData();
}

/// <summary>一次性订阅消息内容容器（官方 <c>data</c>；固定单键 content）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpOneTimeSubscribeData
{
    /// <summary>获取或设置内容信息（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public MpOneTimeSubscribeContent? Content { get; set; }
}

/// <summary>一次性订阅消息内容（官方 <c>data.content</c>；value + color 两字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpOneTimeSubscribeContent
{
    /// <summary>获取或设置消息文本（官方 <c>value</c>，<b>200 字内</b>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>获取或设置字体颜色（官方 <c>color</c>，如 #FF0000；本端点官方字段表<b>有</b> color——与模板消息 send 页不同，照各自页面原文）。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
}
