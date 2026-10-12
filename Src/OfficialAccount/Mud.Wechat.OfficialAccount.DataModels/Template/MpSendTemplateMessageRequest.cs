// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>
/// 发送模板消息（<c>message/template/send</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07，页面 2025-11-26 更新版）：
/// <c>touser</c>/<c>template_id</c>/<c>data</c> 必填，<c>url</c>/<c>miniprogram</c>/<c>client_msg_id</c> 可选；
/// url 与 miniprogram 都不填则无跳转，都填优先跳小程序，客户端不支持跳小程序时跳 url。
/// </para>
/// <para>
/// <b>data 仅 value（逐页核验裁决）</b>：当前官方页 <c>data</c> 形态为
/// <c>{"key1": {"value": any}}</c>，<b>无 <c>color</c> 子字段</b>、无 #RRGGBB 约束、
/// 无单字段 200 字符限制表述（历史文档曾有，现页面已不可见——SDK 按核验事实建模 value-only）。
/// data 的 key 须以类型为前缀（如 <c>thing1.DATA</c>/<c>time2.DATA</c>），发送内容须与模板参数一致。
/// </para>
/// <para>
/// <b>client_msg_id 防重入</b>：官方原文「同一 openid + client_msg_id 只发一条，10 分钟有效」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpSendTemplateMessageRequest
{
    /// <summary>获取或设置接收者（用户）的 openid（官方 <c>touser</c>，必填）。</summary>
    [JsonPropertyName("touser")]
    public string ToUser { get; set; } = string.Empty;

    /// <summary>获取或设置所需下发的模板 id（官方 <c>template_id</c>，必填；官方描述沿用「订阅模板 id」措辞，照录）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置模板跳转链接（官方 <c>url</c>，可选；与 miniprogram 都不填则无跳转）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置跳小程序信息（官方 <c>miniprogram</c>，可选）。</summary>
    [JsonPropertyName("miniprogram")]
    public MpTemplateMiniProgram? MiniProgram { get; set; }

    /// <summary>
    /// 获取或设置模板内容（官方 <c>data</c>，必填；key 为模板参数名——类型前缀形态如 <c>thing1.DATA</c>，
    /// 值为 <c>{"value": …}</c> 形态，经 <see cref="MpTemplateDataValue"/> 承载）。
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, MpTemplateDataValue> Data { get; set; } = new Dictionary<string, MpTemplateDataValue>();

    /// <summary>获取或设置防重入 id（官方 <c>client_msg_id</c>，可选；同一 openid + client_msg_id 只发一条，10 分钟有效）。</summary>
    [JsonPropertyName("client_msg_id")]
    public string? ClientMsgId { get; set; }
}
