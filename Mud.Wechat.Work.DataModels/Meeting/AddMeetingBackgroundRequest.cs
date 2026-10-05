// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 添加会议背景请求体（<c>/cgi-bin/meeting/layout/add_background</c>；对成功预定的会议添加会议背景，支持多个背景图片的添加）。
/// </summary>
/// <remarks>
/// <para>官方限制：一场会议最多添加 7 个背景，且仅支持不超过 10MB 大小的 PNG 格式图片，分辨率最小为 1920x1080；
/// 背景图片上传方式为异步上传，可以通过订阅「素材上传结果」获取上传结果通知。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class AddMeetingBackgroundRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置图片对象列表（官方必填，详见 <see cref="LayoutBackgroundImage"/>）。</summary>
    [JsonPropertyName("image_list")]
    public List<LayoutBackgroundImage>? ImageList { get; set; }

    /// <summary>获取或设置图片列表中会议需要使用的背景图片序号（从 1 开始计数；不填默认为 1）。</summary>
    [JsonPropertyName("default_image_order")]
    public int? DefaultImageOrder { get; set; }
}
