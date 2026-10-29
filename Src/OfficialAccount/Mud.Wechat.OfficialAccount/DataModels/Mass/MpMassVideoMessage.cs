// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>视频消息分支（官方 <c>mpvideo</c>；mass/send 页比 sendall 页多 title/description 子字段 ⇒ 超集建模）。</summary>
/// <remarks>
/// <b>官方文档矛盾（照录）</b>：mass/send 页 msgtype 说明写「视频为 video」但请求体对象名为
/// <c>mpvideo</c>（sendall 页写「视频为 mpvideo」）——以请求体对象名为准。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassVideoMessage
{
    /// <summary>获取或设置用于群发的视频素材 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置视频标题（官方 <c>title</c>；仅 mass/send 页字段表出现）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置视频描述（官方 <c>description</c>；仅 mass/send 页字段表出现）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
