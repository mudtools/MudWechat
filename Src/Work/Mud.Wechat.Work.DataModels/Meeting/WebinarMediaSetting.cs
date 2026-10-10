// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 网络研讨会媒体参数配置对象（创建/修改网络研讨会请求 <c>media_setting</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：修改网络研讨会文档页参数表将 <c>media_setting</c> 标注为 object[]，官方请求示例为单个对象
/// （与创建文档页参数表一致），本模型以单个对象承载。
/// </para>
/// <para>
/// <c>allow_external_user</c> 的官方口径在文档页间相互矛盾（创建页：true - 所有人可入会；修改/详情页：true - 仅企业内部成员可入会），
/// 本模型仅承载字段、口径以实际联调为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class WebinarMediaSetting
{
    /// <summary>获取或设置入会时静音（默认值为 true）：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_enter_mute")]
    public bool? EnableEnterMute { get; set; }

    /// <summary>获取或设置允许参会者取消静音（默认值为 true）：true - 允许；false - 不允许。</summary>
    [JsonPropertyName("allow_unmute_self")]
    public bool? AllowUnmuteSelf { get; set; }

    /// <summary>获取或设置是否允许嘉宾在主持人进会前加入会议（默认值为 true）：true - 允许；false - 不允许。</summary>
    [JsonPropertyName("allow_enter_before_host")]
    public bool? AllowEnterBeforeHost { get; set; }

    /// <summary>获取或设置是否开启屏幕共享水印（默认值为 false）：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_screen_watermark")]
    public bool? EnableScreenWatermark { get; set; }

    /// <summary>获取或设置水印样式（屏幕共享水印开启时生效，默认单排）：0 - 单排；1 - 多排。</summary>
    [JsonPropertyName("watermark_type")]
    public int? WatermarkType { get; set; }

    /// <summary>获取或设置外部成员入会限制（官方两文档页口径相反，详见类型 remarks）。</summary>
    [JsonPropertyName("allow_external_user")]
    public bool? AllowExternalUser { get; set; }

    /// <summary>
    /// 获取或设置自动会议录制类型：none - 禁用（不开启自动会议录制）；local - 本地录制（主持人入会后自动开启本地录制）；
    /// cloud - 云录制（主持人入会后自动开启云录制）。
    /// <para>官方限制：该参数依赖企业账户设置，当企业强制锁定后，该参数必须与企业配置保持一致；仅客户端 2.7.0 及以上版本可生效。</para>
    /// </summary>
    [JsonPropertyName("auto_record_type")]
    public string? AutoRecordType { get; set; }

    /// <summary>
    /// 获取或设置当有参会成员入会时立即开启云录制（默认值为 false）。
    /// <para>官方限制：仅 <see cref="AutoRecordType"/> 设置为 cloud 时才生效；依赖企业账户设置，当企业强制锁定后必须与企业配置保持一致；仅客户端 2.7.0 及以上版本生效。</para>
    /// </summary>
    [JsonPropertyName("attendee_join_auto_record")]
    public bool? AttendeeJoinAutoRecord { get; set; }

    /// <summary>
    /// 获取或设置允许主持人暂停或者停止云录制（默认值为 true）。
    /// <para>官方限制：仅 <see cref="AutoRecordType"/> 设置为 cloud 时才生效；依赖企业账户设置，当企业强制锁定后必须与企业配置保持一致；仅客户端 2.7.0 及以上版本生效。</para>
    /// </summary>
    [JsonPropertyName("enable_host_pause_auto_record")]
    public bool? EnableHostPauseAutoRecord { get; set; }
}
