// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>
/// 设置用户备注名请求体（<c>updateRemark</c>，<c>POST /cgi-bin/user/info/updateremark</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>openid</c>（必填，用户标识）与 <c>remark</c>（必填，新的备注名，
/// <b>长度必须小于 30 字节</b>）。
/// </para>
/// <para>
/// <b>「30 字节」而非「30 字符」</b>：官方原文为「长度必须小于 30 字节」⇒ 中文备注按 UTF-8
/// 每字 3 字节计算时仅约 9 字，<b>调用方须自行按字节数校验</b>（超限错误码 <c>40092 invalid remark name</c>）。
/// SDK 仅以 <c>MaxRemarkBytes</c> 常量表达该限制，不做自动截断（截断会静默篡改用户数据）。
/// </para>
/// <para>响应仅 <c>errcode</c>/<c>errmsg</c>（由 <see cref="MpResponse"/> 承载，故不设专用响应 DTO）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpUpdateRemarkRequest
{
    /// <summary>
    /// 备注名长度上限（<b>字节</b>；官方原文「长度必须小于 30 字节」⇒ 合法值须 <b>严格小于</b> 本值）。
    /// </summary>
    /// <remarks>仅供调用方做前置校验；SDK 不做自动截断。</remarks>
    public const int RemarkByteLimit = 30;

    /// <summary>用户标识。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>新的备注名（<b>UTF-8 字节数须小于 30</b>）。</summary>
    [JsonPropertyName("remark")]
    public string Remark { get; set; } = string.Empty;
}
