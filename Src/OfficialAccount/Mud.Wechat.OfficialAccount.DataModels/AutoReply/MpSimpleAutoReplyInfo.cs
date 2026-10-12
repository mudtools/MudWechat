// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.AutoReply;

/// <summary>
/// 简单自动回复信息（关注后 / 消息默认两处共用；官方两处字段表同形 <c>type</c>+<c>content</c> ⇒ 共用 DTO）。
/// </summary>
/// <remarks>官方枚举：text / img / voice / video / news（关注后与消息自动回复仅支持文本、图片、语音、视频）。</remarks>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpSimpleAutoReplyInfo
{
    /// <summary>获取或设置自动回复类型（官方 <c>type</c>：text/img/voice/video/news）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置回复内容（官方 <c>content</c>：文本类型为文本内容；图文/图片/语音/视频类型为 mediaID）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
