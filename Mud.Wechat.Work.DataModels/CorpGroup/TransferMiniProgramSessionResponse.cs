// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup;

/// <summary>
/// 获取下级/下游企业小程序 session 响应体（<c>/cgi-bin/miniprogram/transfer_session</c>）。
/// </summary>
/// <remarks><see cref="SessionKey"/> 为会话密钥，属敏感凭据，调用方不得记录到日志。</remarks>
public class TransferMiniProgramSessionResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置下级/下游企业用户的 ID。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置属于下级/下游企业的会话密钥（敏感，不得记录到日志）。
    /// </summary>
    [JsonPropertyName("session_key")]
    public string? SessionKey { get; set; }
}
