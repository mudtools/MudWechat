// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「拉取上传」请求体（<c>POST /wxa/sec/vod/pullupload</c>）：由微信侧拉取开发者提供的视频 URL 完成上传。
/// </summary>
/// <remarks>
/// <para><b>异步语义（官方原文）</b>：本接口为异步接口，上传完成会推送上传完成事件到开发者服务器，开发者也可调用「查询任务」端点（<c>IWxaVodUploadService.GetTaskAsync</c>）轮询上传结果。事件接收属公众号 XML 通道（本线无回调工程，MP-X 形态约束）。</para>
/// <para><b>不填 <c>cover_url</c> 时官方默认截取视频首帧作为封面图</b>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_pullupload.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodPullUploadRequest
{
    /// <summary>media_name（官方类型 <c>string</c>，<b>必填</b>）：文件名，需按照“剧目名 - 对应剧集数”格式命名文件</summary>
    [JsonPropertyName("media_name")]
    public string? MediaName { get; set; }

    /// <summary>media_url（官方类型 <c>string</c>，<b>必填</b>）：视频 URL</summary>
    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; set; }

    /// <summary>cover_url（官方类型 <c>string</c>）：封面图URL</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>source_context（官方类型 <c>string</c>）：来源上下文，会在上传完成事件中透传给开发者。</summary>
    [JsonPropertyName("source_context")]
    public string? SourceContext { get; set; }
}
