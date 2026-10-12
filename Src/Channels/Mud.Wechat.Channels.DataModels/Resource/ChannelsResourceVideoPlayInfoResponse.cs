// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Resource;

/// <summary>获取上传后的视频临时 URL（<c>video_getplayinfo</c>）响应。</summary>
/// <remarks>官方契约：<b>GET</b> + URL 参数 <c>video_upload_key</c>；完成上传后轮询获取（转码期间返回 10020347）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoPlayInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置播放信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsResourceVideoPlayInfo? Data { get; set; }
}
