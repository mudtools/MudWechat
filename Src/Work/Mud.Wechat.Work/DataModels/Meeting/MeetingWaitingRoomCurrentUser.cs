// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 实时等候室成员对象（获取实时等候室成员列表响应 <c>user_list</c> 嵌套对象；需开启等候室且会议为「会议进行中」状态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingWaitingRoomCurrentUser
{
    /// <summary>获取或设置等候室企业内成员 userid（企业外成员为空字符串）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置成员的终端设备类型：1 - PC；2 - Mac；3 - Android；4 - iOS；5 - Web；6 - iPad；7 - Android Pad；
    /// 8 - 小程序；9 - voip、sip 设备；10 - linux；20 - Rooms for Touch Windows；21 - Rooms for Touch Mac；
    /// 22 - Rooms for Touch Android；30 - Controller for Touch Windows；32 - Controller for Touch Android；
    /// 33 - Controller for Touch Iphone；81 - 鸿蒙手机；82 - 鸿蒙平板；83 - 鸿蒙 PC；84 - 鸿蒙汽车。
    /// </summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }

    /// <summary>获取或设置用户专属信息（通过专属参会链接入会时携带创建时指定的 <c>customer_data</c>）。</summary>
    [JsonPropertyName("customer_data")]
    public string? CustomerData { get; set; }

    /// <summary>获取或设置会议中为每个参会成员授予的临时 ID。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }
}
