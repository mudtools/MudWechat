// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 等候室成员记录对象（获取等候室成员记录响应 <c>user_list</c> 嵌套对象；包括等候室内所有进出过的成员，会前/会中/会后均可获取）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingWaitingRoomUserRecord
{
    /// <summary>获取或设置企业成员 userid。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置会议中为每个参会成员授予的临时 ID。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>
    /// 获取或设置成员的终端设备类型：0 - PSTN；1 - PC；2 - Mac；3 - Android；4 - iOS；5 - Web；6 - iPad；7 - Android Pad；
    /// 8 - 小程序；9 - voip、sip 设备；10 - linux；20 - Rooms for Touch Windows；21 - Rooms for Touch MacOS；
    /// 22 - Rooms for Touch Android；30 - Controller for Touch Windows；32 - Controller for Touch Android；
    /// 33 - Controller for Touch iOS；81 - 鸿蒙手机；82 - 鸿蒙平板；83 - 鸿蒙 PC；84 - 鸿蒙汽车。
    /// </summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }

    /// <summary>获取或设置加入等候室时间（单位毫秒）。</summary>
    [JsonPropertyName("join_time")]
    public long? JoinTime { get; set; }

    /// <summary>获取或设置离开等候室时间（单位毫秒）。</summary>
    [JsonPropertyName("quit_time")]
    public long? QuitTime { get; set; }
}
