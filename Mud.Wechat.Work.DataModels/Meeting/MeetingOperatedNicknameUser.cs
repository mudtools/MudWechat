// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会控被改昵称成员对象（修改成员在会中显示的昵称请求 <c>operated_users</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：修改成员在会中显示的昵称文档页参数表将本字段误写为 <c>opereated_users</c>（多一个 e），
/// 官方请求示例为 <c>operated_users</c>，本模型以示例为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingOperatedNicknameUser
{
    /// <summary>获取或设置被操作者在当场会议的成员临时 ID（官方必填；可用于会控操作，适用于所有成员）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>
    /// 获取或设置成员的终端设备类型（官方必填；请与被操作者的设备类型保持一致，否则不生效）。
    /// <para>取值：0 - PSTN；1 - PC；2 - Mac；3 - Android；4 - iOS；5 - Web；6 - iPad；7 - Android Pad；
    /// 8 - 小程序；9 - voip、sip 设备；10 - linux；20 - Rooms for Touch Windows；21 - Rooms for Touch MacOS；
    /// 22 - Rooms for Touch Android；30 - Controller for Touch Windows；32 - Controller for Touch Android；
    /// 33 - Controller for Touch iOS；81 - 鸿蒙手机；82 - 鸿蒙平板；83 - 鸿蒙 PC；84 - 鸿蒙汽车。</para>
    /// </summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }

    /// <summary>获取或设置成员昵称字符串（限制 20 个字符）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}
