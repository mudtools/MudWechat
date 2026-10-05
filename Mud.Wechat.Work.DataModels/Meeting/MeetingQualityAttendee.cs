// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 参会成员健康度对象（获取会议健康度响应 <c>attendees</c> 嵌套对象；按成员入会时间正序排列，成员使用不同端入会时平铺返回）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingQualityAttendee
{
    /// <summary>获取或设置成员 userid。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置当场会议的成员临时 ID（可用于会控操作，适用于所有成员）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>
    /// 获取或设置成员的终端设备类型：0 - PSTN；1 - PC；2 - Mac；3 - Android；4 - iOS；5 - Web；6 - iPad；7 - Android Pad；
    /// 8 - 小程序；9 - voip、sip 设备；10 - linux；20 - Rooms for Touch Windows；21 - Rooms for Touch MacOS；
    /// 22 - Rooms for Touch Android；30 - Controller for Touch Windows；32 - Controller for Touch Android；
    /// 33 - Controller for Touch iOS。
    /// </summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }

    /// <summary>获取或设置健康度：0 - 无数据；1 - 健康；2 - 告警。</summary>
    [JsonPropertyName("quality")]
    public int? Quality { get; set; }

    /// <summary>获取或设置音频质量：0 - 无数据；1 - 好；2 - 较好；3 - 中；4 - 差。</summary>
    [JsonPropertyName("audio_quality")]
    public int? AudioQuality { get; set; }

    /// <summary>获取或设置视频质量：0 - 无数据；1 - 好；2 - 较好；3 - 中；4 - 差。</summary>
    [JsonPropertyName("video_quality")]
    public int? VideoQuality { get; set; }

    /// <summary>获取或设置共享屏幕质量：0 - 无数据；1 - 好；2 - 较好；3 - 中；4 - 差。</summary>
    [JsonPropertyName("screen_share_quality")]
    public int? ScreenShareQuality { get; set; }

    /// <summary>获取或设置网络质量：0 - 无数据；1 - 好；2 - 较好；3 - 中；4 - 差。</summary>
    [JsonPropertyName("network_quality")]
    public int? NetworkQuality { get; set; }

    /// <summary>获取或设置告警的具体问题列表。</summary>
    [JsonPropertyName("problems")]
    public List<string>? Problems { get; set; }
}
