// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取 MRA 状态信息响应体（<c>/cgi-bin/meeting/mra/query_status</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class QueryMraStatusResponse : WechatWorkResponse
{
    /// <summary>获取或设置被查询成员当场会议的成员临时 ID。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>获取或设置被查询成员的终端设备类型（本端点为 9：voip、sip 设备，即 MRA 设备）。</summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }

    /// <summary>获取或设置成员角色：0 - 普通成员角色；1 - 创建者角色；2 - 主持人；3 - 创建者+主持人；4 - 小程序；5 - 小程序+主持人；6 - 联席主持人；7 - 创建者+联席主持人。</summary>
    [JsonPropertyName("user_role")]
    public int? UserRole { get; set; }

    /// <summary>获取或设置网络研讨会成员角色：0 - 普通参会角色；1 - 内部嘉宾；2 - 外部嘉宾；3 - 邀请链接入会嘉宾；4 - 观众。</summary>
    [JsonPropertyName("webinar_member_role")]
    public int? WebinarMemberRole { get; set; }

    /// <summary>获取或设置成员的 IP 地址（当成员在会中时才能返回）。</summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>获取或设置成员当前显示名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置麦克风状态：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("audio_state")]
    public bool? AudioState { get; set; }

    /// <summary>获取或设置摄像头状态：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("video_state")]
    public bool? VideoState { get; set; }

    /// <summary>获取或设置屏幕共享状态：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("screen_shared_state")]
    public bool? ScreenSharedState { get; set; }

    /// <summary>获取或设置当前成员的默认分屏设置：1 - 等分模式；2 - 全屏模式；3 - 1+N。</summary>
    [JsonPropertyName("default_layout")]
    public int? DefaultLayout { get; set; }

    /// <summary>获取或设置举手状态：true - 举手中；false - 手放下。</summary>
    [JsonPropertyName("raise_hands_state")]
    public bool? RaiseHandsState { get; set; }
}
