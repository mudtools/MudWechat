// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 已参会成员对象（获取已参会成员列表响应 <c>attendees</c> 嵌套对象；支持查询预约会议及网络研讨会）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingAttendedAttendee
{
    /// <summary>获取或设置企业内参会成员 userid（外部成员为空）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置会议中为每个参会成员授予的临时 ID。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>获取或设置参会者加入会议时间戳（单位秒）。</summary>
    [JsonPropertyName("join_time")]
    public string? JoinTime { get; set; }

    /// <summary>获取或设置参会者离开会议时间戳（单位秒）。</summary>
    [JsonPropertyName("quit_time")]
    public string? QuitTime { get; set; }

    /// <summary>
    /// 获取或设置成员的终端设备类型：0 - PSTN；1 - PC；2 - Mac；3 - Android；4 - iOS；5 - Web；6 - iPad；7 - Android Pad；
    /// 8 - 小程序；9 - voip、sip 设备（即 MRA 设备）；10 - linux；20 - Rooms for Touch Windows；21 - Rooms for Touch MacOS；
    /// 22 - Rooms for Touch Android；30 - Controller for Touch Windows；32 - Controller for Touch Android；
    /// 33 - Controller for Touch iOS/iPadOS；81 - 鸿蒙手机；82 - 鸿蒙平板；83 - 鸿蒙 PC；84 - 鸿蒙汽车。
    /// </summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }

    /// <summary>
    /// 获取或设置成员角色：0 - 普通成员角色；1 - 创建者角色；2 - 主持人；3 - 创建者+主持人；
    /// 4 - 游客（通过微信小程序入会的用户）；5 - 游客+主持人；6 - 联席主持人；7 - 创建者+联席主持人。
    /// </summary>
    [JsonPropertyName("role")]
    public int? Role { get; set; }

    /// <summary>
    /// 获取或设置网络研讨会成员角色：0 - 普通参会角色；1 - 内部嘉宾；2 - 外部嘉宾；3 - 邀请链接入会嘉宾；4 - 观众。
    /// </summary>
    [JsonPropertyName("webinar_role")]
    public int? WebinarRole { get; set; }

    /// <summary>
    /// 获取或设置入会方式：0 - PSTN 普通用户（标准的手机或固话类型）；1 - 普通 VOIP 用户；2 - 附属投屏 VOIP；
    /// 3 - linux sdk for VOIP；4 - 附属语音 PSTN；5 - 附属视频 PSTN；6 - linux sdk for PSTN。
    /// </summary>
    [JsonPropertyName("join_type")]
    public int? JoinType { get; set; }

    /// <summary>获取或设置网络类型：有线、WIFI、2G、3G、4G、未知（当成员在会中时才能返回）。</summary>
    [JsonPropertyName("net")]
    public string? Net { get; set; }

    /// <summary>获取或设置麦克风状态：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("audio_state")]
    public bool? AudioState { get; set; }

    /// <summary>获取或设置摄像头状态：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("video_state")]
    public bool? VideoState { get; set; }

    /// <summary>获取或设置屏幕共享状态：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("screen_shared_state")]
    public bool? ScreenSharedState { get; set; }

    /// <summary>获取或设置用户专属字段（如果参会成员是通过专属链接进会，给出成员专属字段）。</summary>
    [JsonPropertyName("customer_data")]
    public string? CustomerData { get; set; }
}
