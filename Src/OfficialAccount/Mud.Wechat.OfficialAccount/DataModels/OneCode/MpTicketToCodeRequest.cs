// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

/// <summary>
/// CODE_TICKET 换 CODE（<c>POST /intp/marketcode/tickettocode</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>openid</c>（用户 openid）/ <c>code_ticket</c>（跳转时带上的临时票据，用于换取正式营销码）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：① 本页<b>无 Query 参数表</b>（其余 5 页均列 <c>access_token</c>）⇒
/// SDK 按同域契约仍以 <b>Query 注入 <c>access_token</c></b>（第三方代调用时为 <c>authorizer_access_token</c>）；
/// ② 接口概述称 <c>code_ticket</c> 为「用户<b>扫码</b>获得」，字段说明称「<b>跳转</b>时带上」，获取途径描述不一致。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpTicketToCodeRequest
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置临时票据（官方 <c>code_ticket</c>，必填；用户扫码/跳转携带，用于换取正式营销码）。</summary>
    [JsonPropertyName("code_ticket")]
    public string CodeTicket { get; set; } = string.Empty;
}
