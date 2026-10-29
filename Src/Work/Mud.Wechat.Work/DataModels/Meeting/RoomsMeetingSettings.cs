// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 会议室会议配置对象（获取 Rooms 会议室配置项响应 <c>meeting_settings</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>注意与预约会议的 <see cref="MeetingSettings"/> 为两个不同结构（本类型仅承载 Rooms 会议室配置项）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsMeetingSettings
{
    /// <summary>获取或设置水印：0 - 关闭；1 - 单排水印；2 - 多排水印。</summary>
    [JsonPropertyName("water_mark")]
    public int? WaterMark { get; set; }

    /// <summary>获取或设置自动接听：1 - 开启；2 - 关闭。</summary>
    [JsonPropertyName("auto_response")]
    public int? AutoResponse { get; set; }

    /// <summary>获取或设置字幕：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("caption")]
    public bool? Caption { get; set; }

    /// <summary>获取或设置专属 ID：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("room_pmi")]
    public bool? RoomPmi { get; set; }

    /// <summary>获取或设置 Rooms 屏幕是否展示消息通知：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("room_notification")]
    public bool? RoomNotification { get; set; }
}
