// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Resource;

/// <summary>申请上传视频（<c>video_initupload</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体；三字段均必填。
/// 场景限制：售后相关视频（scene_type=173）≤ 20MB；商品视频（scene_type=162）≤ 500MB 且 ≤ 180 秒。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoInitUploadRequest
{
    /// <summary>获取或设置上传场景类型（官方 <c>scene_type</c>，必填，见 <see cref="ChannelsResourceVideoSceneTypes"/>）。</summary>
    [JsonPropertyName("scene_type")]
    public int SceneType { get; set; }

    /// <summary>获取或设置文件类型（官方 <c>file_type</c>，必填，示例 mp4）。</summary>
    [JsonPropertyName("file_type")]
    public string FileType { get; set; } = string.Empty;

    /// <summary>获取或设置文件大小（官方 <c>file_size</c>，必填，单位：字节）。</summary>
    [JsonPropertyName("file_size")]
    public long FileSize { get; set; }
}
