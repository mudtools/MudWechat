// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「获取媒资播放链接」响应：字段面是媒资详情的<b>子集</b>（无 <c>create_time</c>/<c>expire_time</c>/<c>drama_id</c>/<c>file_size</c>/<c>original_url</c>/<c>audit_detail</c>）。
/// </summary>
/// <remarks>
/// <para><b>不共用 <see cref="WxaVodMediaInfo"/></b>：官方本页子表字段集与 <c>getmedia</c> 明显不同 ⇒ 分立类型，否则「永不返回」的字段会诱导宿主误用。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedialink.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetMediaLinkResponse : WxaResponse
{
    /// <summary>media_info（官方类型 <c>object</c>）：媒体播放信息</summary>
    [JsonPropertyName("media_info")]
    public WxaVodMediaLinkInfo? MediaInfo { get; set; }
}
