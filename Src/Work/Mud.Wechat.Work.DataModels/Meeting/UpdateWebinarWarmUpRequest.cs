// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 管理网络研讨会暖场配置请求体（<c>/cgi-bin/meeting/webinar/update_warm_up</c>；可订阅「网络研讨会暖场上传结果」事件用于获得上传结果）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class UpdateWebinarWarmUpRequest
{
    /// <summary>获取或设置网络研讨会 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置暖场图片地址。
    /// <para>暖场图片与暖场视频只能选择一个，同时传入时以图片为准；推荐 1280*720 尺寸，支持 png/jpg 格式，大小不超过 5M；会议开始前 15 分钟进入的观众可观看暖场，仅支持 3.3.0 及以上版本客户端观看。</para>
    /// </summary>
    [JsonPropertyName("warm_up_picture")]
    public string? WarmUpPicture { get; set; }

    /// <summary>获取或设置暖场视频地址（推荐 1280*720 尺寸，支持 mp4 格式，大小不超过 1G）。</summary>
    [JsonPropertyName("warm_up_video")]
    public string? WarmUpVideo { get; set; }

    /// <summary>获取或设置允许参会者在暖场中邀请成员（默认允许）：true - 允许；false - 不允许。</summary>
    [JsonPropertyName("allow_attendees_invite_others")]
    public bool? AllowAttendeesInviteOthers { get; set; }
}
