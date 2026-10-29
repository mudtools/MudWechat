// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfSession;

/// <summary>
/// 会话条目（官方 <c>getsessionlist</c> 响应 <c>sessionlist</c> 元素）。
/// </summary>
/// <remarks>官方字段：<c>createtime</c>（会话接入时间，timestamp）/ <c>openid</c>（用户 openid）。</remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpKfSession
{
    /// <summary>会话接入时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("createtime")]
    public long CreateTime { get; set; }

    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
