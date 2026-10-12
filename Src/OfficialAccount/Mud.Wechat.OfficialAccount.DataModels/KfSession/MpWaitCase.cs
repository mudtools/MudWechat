// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfSession;

/// <summary>
/// 未接入会话条目（官方 <c>getwaitcase</c> 响应 <c>waitcaselist</c> 元素）。
/// </summary>
/// <remarks>
/// 官方该页元素字段<b>仅</b> <c>latest_time</c>（用户的最后一条消息的时间）与 <c>openid</c>
/// ——页面<b>未出现</b> <c>kf_account</c>/<c>createtime</c> ⇒ SDK 不据其他页「补全」字段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpWaitCase
{
    /// <summary>用户的最后一条消息的时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("latest_time")]
    public long LatestTime { get; set; }

    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
