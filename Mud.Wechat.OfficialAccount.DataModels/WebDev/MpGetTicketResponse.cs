// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.WebDev;

/// <summary>
/// 获取 SDK 临时票据响应（官方 <c>getTicket</c>，<c>GET /cgi-bin/ticket/getticket</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/webdev/js/api_getticket.html"/>
/// （服务端 API → 网页开发 → JS-SDK → 获取 sdk 临时票据）。
/// </para>
/// <para>
/// <b>官方契约</b>：业务 Query 为 <c>type</c>（必填，取值 <c>jsapi</c> / <c>wx_card</c>）；
/// 响应 <c>ticket</c>（临时票据）+ <c>expires_in</c>（有效期秒数）。接口说明原文：
/// 「Api_ticket 是用于调用 js-sdk 的临时票据，有效期为 <b>7200 秒</b>，通过 access_token 来获取」。
/// </para>
/// <para>
/// <b>调用频次的硬约束（官方「注意事项」原文）</b>：「由于获取 api_ticket 的 api 调用次数非常有限，
/// 频繁刷新 api_ticket 会导致 api 调用受限，影响自身业务，开发者需在自己的服务存储与更新 api_ticket」
/// ⇒ SDK 侧由票据管理器统一缓存（单一飞行 + 提前刷新），宿主<b>不得</b>绕过管理器逐次调用本接口。
/// </para>
/// <para>
/// <b>账号适用性与第三方调用</b>：官方适用范围为「公众号 ✔ / 服务号 ✔ / 移动应用 ✔ / 网站应用 ✔」
/// （<b>非</b>「仅认证」，也<b>不是</b>服务号专属）；官方明确「本接口<b>不支持第三方平台调用</b>」且不支持云调用。
/// </para>
/// <para>官方错误码：<c>-1</c>（system error）/ <c>40001</c>（invalid credential）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WebDev")]
public class MpGetTicketResponse : MpResponse
{
    /// <summary>临时票据。</summary>
    [JsonPropertyName("ticket")]
    public string? Ticket { get; set; }

    /// <summary>凭证有效期（秒；官方为 7200）。</summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
