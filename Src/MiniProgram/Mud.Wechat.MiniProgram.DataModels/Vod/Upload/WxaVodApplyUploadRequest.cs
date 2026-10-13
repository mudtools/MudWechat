// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「申请分片上传」请求体（<c>POST /wxa/sec/vod/applyupload</c>）：换取 <c>upload_id</c>，随后逐片走 <c>uploadpart</c>。
/// </summary>
/// <remarks>
/// <para><b>不送文件本体</b>：本请求只声明媒体与封面的<b>格式</b>；<b>填了 <c>cover_type</c> 即表示本次分片上传除视频外还要传封面图</b>，不填则官方默认截取视频首帧作封面。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_applyupload.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodApplyUploadRequest
{
    /// <summary>media_name（官方类型 <c>string</c>，<b>必填</b>）：文件名，需按照“剧目名 - 对应剧集数”格式命名文件</summary>
    [JsonPropertyName("media_name")]
    public string? MediaName { get; set; }

    /// <summary>media_type（官方类型 <c>string</c>，<b>必填</b>）：视频格式，支持：MP4，TS，MOV，MXF，MPG，FLV，WMV，AVI，M4V，F4V，MPEG，3GP，ASF，MKV</summary>
    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    /// <summary>cover_type（官方类型 <c>string</c>）：封面图图片格式，支持：JPG、JPEG、PNG、BMP、TIFF、AI、CDR、EPS、TIF</summary>
    [JsonPropertyName("cover_type")]
    public string? CoverType { get; set; }

    /// <summary>source_context（官方类型 <c>string</c>）：来源上下文，会在上传完成事件中透传给开发者。</summary>
    [JsonPropertyName("source_context")]
    public string? SourceContext { get; set; }
}
