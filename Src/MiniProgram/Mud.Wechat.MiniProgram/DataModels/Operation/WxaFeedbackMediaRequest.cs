// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>获取用户反馈图片请求体（<c>POST /cgi-bin/media/getfeedbackmedia</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_getfeedbackmedia.html</c>；成功响应为<b>图片二进制流</b>（走独立图片通道，见 <c>IWxaFeedbackMediaService</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaFeedbackMediaRequest
{
    /// <summary>反馈图片的媒体 ID（<c>media_id</c>，必填；取自 <see cref="WxaFeedbackItem.MediaIds"/>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}
