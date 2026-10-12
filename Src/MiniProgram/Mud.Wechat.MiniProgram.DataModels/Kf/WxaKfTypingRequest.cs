// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Kf;

/// <summary>客服输入状态请求体（<c>POST /cgi-bin/message/custom/business/typing</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-message/api_typing"/>
/// （2026-10-12 核验）。
/// </para>
/// <para>
/// <b>路由按账号类型二分（官方原文）</b>：公众号 / 服务号用 <c>/cgi-bin/message/custom/typing</c>，
/// <b>小程序 / 小游戏</b>用 <c>/cgi-bin/message/custom/business/typing</c> —— 本线只承载后者。
/// </para>
/// <para>
/// <b>时限约束（官方错误码反推）</b>：<c>45080</c> 要求下发前 30 秒内与该用户有过消息交互；
/// <c>45081</c> 表示已在输入状态、不可重复下发；<c>45072</c> 为 <c>command</c> 取值非法。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaKfTypingRequest
{
    /// <summary>接收用户 OpenId（<c>touser</c>，必填）。</summary>
    [JsonPropertyName("touser")]
    public string? ToUser { get; set; }

    /// <summary>输入状态指令（<c>command</c>，必填），取值见 <see cref="WxaKfTypingCommands"/>。</summary>
    [JsonPropertyName("command")]
    public string? Command { get; set; }
}
