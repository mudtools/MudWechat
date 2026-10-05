// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 网络研讨会媒体参数配置响应对象（获取网络研讨会详情响应 <c>media_setting</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：获取网络研讨会详情文档页参数表将入会静音字段列为 <c>enable_enter_mute</c>（与请求形态同名），
/// 但官方响应示例为 <c>mute_enable_join</c>，本模型以示例为准（与承载请求形态的 <see cref="WebinarMediaSetting"/> 分型）。
/// <see cref="AllowExternalUser"/> 的官方口径在文档页间相互矛盾，本模型仅承载字段、口径以实际联调为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class WebinarMediaSettingInfo
{
    /// <summary>获取或设置嘉宾入会时静音（默认值为 false；官方响应示例字段名，参数表作 enable_enter_mute）。</summary>
    [JsonPropertyName("mute_enable_join")]
    public bool? MuteEnableJoin { get; set; }

    /// <summary>获取或设置允许嘉宾取消静音（默认值为 false）：true - 允许；false - 不允许。</summary>
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
    /// 获取或设置自动会议录制类型：none - 禁用；local - 本地录制；cloud - 云录制。
    /// <para>官方限制：该参数依赖企业账户设置，当企业强制锁定后必须与企业配置保持一致；仅客户端 2.7.0 及以上版本可生效。</para>
    /// </summary>
    [JsonPropertyName("auto_record_type")]
    public string? AutoRecordType { get; set; }

    /// <summary>获取或设置当有参会成员入会时立即开启云录制（默认值为 false；仅 <see cref="AutoRecordType"/> 为 cloud 时生效）。</summary>
    [JsonPropertyName("attendee_join_auto_record")]
    public bool? AttendeeJoinAutoRecord { get; set; }

    /// <summary>获取或设置允许主持人暂停或者停止云录制（默认值为 true；仅 <see cref="AutoRecordType"/> 为 cloud 时生效）。</summary>
    [JsonPropertyName("enable_host_pause_auto_record")]
    public bool? EnableHostPauseAutoRecord { get; set; }
}
