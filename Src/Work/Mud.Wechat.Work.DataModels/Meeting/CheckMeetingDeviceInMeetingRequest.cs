// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取成员设备是否入会请求体（<c>/cgi-bin/meeting/check_device_in_meeting</c>；查询企业内成员在当前时间前是否有设备进入指定的会议中）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class CheckMeetingDeviceInMeetingRequest
{
    /// <summary>获取或设置企业成员的 userid（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置终端设备类型列表（不带则查询所有设备上的会议信息，带则查询指定设备）。
    /// <para>取值：0 - PSTN；1 - PC；2 - Mac；3 - Android；4 - iOS；5 - Web；6 - iPad；7 - Android Pad；8 - 小程序；
    /// 9 - voip、sip 设备；10 - linux；20 - Rooms for Touch Windows；21 - Rooms for Touch MacOS；22 - Rooms for Touch Android；
    /// 30 - Controller for Touch Windows；32 - Controller for Touch Android；33 - Controller for Touch iOS；
    /// 81 - 鸿蒙手机；82 - 鸿蒙平板；83 - 鸿蒙 PC；84 - 鸿蒙汽车。</para>
    /// </summary>
    [JsonPropertyName("instance_id_list")]
    public List<int>? InstanceIdList { get; set; }

    /// <summary>获取或设置会议 ID 列表（官方必填；须为本企业创建的会议）。</summary>
    [JsonPropertyName("meetingid_list")]
    public List<string>? MeetingidList { get; set; }
}
