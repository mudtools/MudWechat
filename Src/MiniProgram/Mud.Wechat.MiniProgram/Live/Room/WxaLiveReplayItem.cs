// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 回放视频条目（<c>live_replay[]</c>，<c>action = "get_replay"</c> 时的应答数组元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html"/>。
/// </para>
/// <para>
/// <b>两个时间字段官方类型为 string</b>（<c>create_time</c> / <c>expire_time</c>）：与同页 <c>room_info</c>
/// 内 <c>start_time</c> 的 number 口径不同 ⇒ 照录为 <see cref="string"/>，不得「顺手」按 number 建模。
/// </para>
/// <para>
/// <b><see cref="MediaUrl"/> 有时效</b>：官方以 <c>expire_time</c> 标注回放链接过期时间，
/// 宿主不得缓存 <c>media_url</c> 长期使用，须在使用前重新拉取。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveReplayItem
{
    /// <summary>回放视频创建时间（<c>create_time</c>，官方类型 string）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>回放视频 URL 过期时间（<c>expire_time</c>，官方类型 string）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

    /// <summary>回放视频链接（<c>media_url</c>，受 <see cref="ExpireTime"/> 约束）。</summary>
    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; set; }
}
