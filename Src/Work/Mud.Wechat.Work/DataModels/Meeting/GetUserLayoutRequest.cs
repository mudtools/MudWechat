// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取用户布局请求体（<c>/cgi-bin/meeting/advanced_layout/get_user_layout</c>；根据会议 ID 和用户 ID 返回用户的布局设置信息）。
/// </summary>
/// <remarks>
/// <para>布局优先级：用户个性布局 &gt; 会议自定义布局（高级布局、基础布局）&gt; 会议默认布局；
/// 高级布局目前仅支持 H.323/SIP 会议室终端。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetUserLayoutRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置被操作用户临时身份 ID（官方必填）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>
    /// 获取或设置被操作用户终端设备类型 ID（官方必填）：0 - PSTN；1 - PC；2 - Mac；3 - Android；4 - iOS；5 - Web；6 - iPad；
    /// 7 - Android Pad；8 - 小程序；9 - voip、sip 设备；10 - linux；20 - Rooms for Touch Windows；21 - Rooms for Touch MacOS；
    /// 22 - Rooms for Touch Android；30 - Controller for Touch Windows；32 - Controller for Touch Android；
    /// 33 - Controller for Touch iOS。
    /// </summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }
}
