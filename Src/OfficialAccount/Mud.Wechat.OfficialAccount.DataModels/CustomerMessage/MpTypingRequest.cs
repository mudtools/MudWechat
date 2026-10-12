// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

/// <summary>
/// 客服输入状态请求体（<c>typing</c>，<c>POST /cgi-bin/message/custom/typing</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>touser</c>（必填）与 <c>command</c>（必填，<c>Typing</c> / <c>CancelTyping</c>）。
/// </para>
/// <para>
/// <b>官方约束（硬性）</b>：下发输入状态需<b>之前 30 秒内与该用户有过消息交互</b>（否则 <c>45080</c>）；
/// 已处于输入状态时<b>不可重复下发</b>（<c>45081</c>）。
/// </para>
/// <para>
/// <b>路径差异（勿混用）</b>：公众号 / 服务号为 <c>/cgi-bin/message/custom/typing</c>；
/// 小程序 / 小游戏为 <c>/cgi-bin/message/custom/business/typing</c>（本 SDK 仅覆盖公众号形态）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpTypingRequest
{
    /// <summary>用户的 OpenID。</summary>
    [JsonPropertyName("touser")]
    public string ToUser { get; set; } = string.Empty;

    /// <summary>命令（<c>Typing</c> 下发「正在输入」/ <c>CancelTyping</c> 取消）。</summary>
    [JsonPropertyName("command")]
    public string Command { get; set; } = string.Empty;

    /// <summary>子商户 ID（<b>普通账号无需填写</b>）。</summary>
    [JsonPropertyName("businessid")]
    public string? BusinessId { get; set; }
}
