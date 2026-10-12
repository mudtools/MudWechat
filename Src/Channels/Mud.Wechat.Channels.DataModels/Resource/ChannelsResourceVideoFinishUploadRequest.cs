// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Resource;

/// <summary>完成视频上传（<c>video_finishupload</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体；官方响应无业务字段（仅 errcode/errmsg，返回 <see cref="ChannelsResponse"/>）。
/// <c>finish_parts</c> 须按分块偏移从小到大排列且上传块编号连续。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoFinishUploadRequest
{
    /// <summary>获取或设置上传流程关联键（官方 <c>video_upload_key</c>，必填；通过申请上传视频接口获取）。</summary>
    [JsonPropertyName("video_upload_key")]
    public string VideoUploadKey { get; set; } = string.Empty;

    /// <summary>获取或设置文件分片列表（官方 <c>finish_parts</c>，必填；按分块偏移从小到大排列，且上传块编号连续）。</summary>
    [JsonPropertyName("finish_parts")]
    public List<ChannelsResourceVideoFinishPart> FinishParts { get; set; } = new();
}
