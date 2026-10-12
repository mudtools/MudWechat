// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.SubscribeMessage;

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
