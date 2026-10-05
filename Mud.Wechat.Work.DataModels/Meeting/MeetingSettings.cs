// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议配置对象（创建/修改预约会议请求与获取会议详情响应 <c>settings</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 字段为预约会议基础管理（创建/修改/获取详情）与预约会议高级管理（创建/修改/获取详情）两组文档页的并集超集：
/// 仅获取会议详情响应返回的字段（<c>enable_doc_upload_permission</c> / <c>current_hosts</c> / <c>co_hosts</c>）
/// 与仅创建预约会议文档页声明的字段（<c>enable_interpreter</c>）均以可空属性承载，序列化时未赋值字段不输出。
/// </para>
/// <para>
/// 获取会议详情文档页参数表将 <c>password</c> 标注为 bool，但官方请求/响应示例均按字符串传输（如 "1234"），
/// 故本模型以字符串承载以兼容两种形态。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingSettings
{
    /// <summary>获取或设置入会密码（仅支持 4-6 位纯数字）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>获取或设置是否开启等候室（默认不开启）。</summary>
    [JsonPropertyName("enable_waiting_room")]
    public bool? EnableWaitingRoom { get; set; }

    /// <summary>获取或设置是否允许成员在主持人进会前加入（默认允许）。</summary>
    [JsonPropertyName("allow_enter_before_host")]
    public bool? AllowEnterBeforeHost { get; set; }

    /// <summary>
    /// 获取或设置会议开始时来电提醒方式：1 - 不提醒；2 - 仅提醒主持人；3 - 提醒所有成员；4 - 指定部分成员响铃。
    /// <para>创建预约会议文档页默认值为 2（仅提醒主持人）；获取会议详情文档页默认值为 3（提醒所有成员）。</para>
    /// </summary>
    [JsonPropertyName("remind_scope")]
    public int? RemindScope { get; set; }

    /// <summary>获取或设置成员入会时是否静音：1 - 开启；0 - 关闭；2 - 超过 6 人自动开启（默认）。</summary>
    [JsonPropertyName("enable_enter_mute")]
    public int? EnableEnterMute { get; set; }

    /// <summary>获取或设置是否开启屏幕水印（默认不开启）。</summary>
    [JsonPropertyName("enable_screen_watermark")]
    public bool? EnableScreenWatermark { get; set; }

    /// <summary>获取或设置主持人列表（最多 10 个，详见 <see cref="MeetingHosts"/>）。</summary>
    [JsonPropertyName("hosts")]
    public MeetingHosts? Hosts { get; set; }

    /// <summary>获取或设置指定响铃成员列表（详见 <see cref="MeetingRingUsers"/>）。</summary>
    [JsonPropertyName("ring_users")]
    public MeetingRingUsers? RingUsers { get; set; }

    /// <summary>
    /// 获取或设置是否允许参会者取消静音（默认允许）。
    /// <para>官方限制：<see cref="MuteAll"/> 单独使用不生效，当本字段为 false 时需指定 <see cref="MuteAll"/> = true。</para>
    /// </summary>
    [JsonPropertyName("allow_unmute_self")]
    public bool? AllowUnmuteSelf { get; set; }

    /// <summary>获取或设置是否全体静音（该字段单独使用不生效，当 <see cref="AllowUnmuteSelf"/> 为 false 时需指定为 true）。</summary>
    [JsonPropertyName("mute_all")]
    public bool? MuteAll { get; set; }

    /// <summary>获取或设置是否允许企业外部成员入会：true - 所有成员可入会；false - 仅企业内部成员可入会（默认所有成员可入会）。</summary>
    [JsonPropertyName("allow_external_user")]
    public bool? AllowExternalUser { get; set; }

    /// <summary>获取或设置水印样式（屏幕共享水印开启时生效）：0 - 单排；1 - 多排（默认单排）。</summary>
    [JsonPropertyName("watermark_type")]
    public int? WatermarkType { get; set; }

    /// <summary>
    /// 获取或设置自动会议录制类型：none - 禁用（不开启自动会议录制）；local - 本地录制（主持人入会后自动开启本地录制）；cloud - 云录制（主持人入会后自动开启云录制）。
    /// <para>官方限制：该参数依赖企业账户设置，当企业强制锁定后，该参数必须与企业配置保持一致。</para>
    /// </summary>
    [JsonPropertyName("auto_record_type")]
    public string? AutoRecordType { get; set; }

    /// <summary>
    /// 获取或设置是否有参会成员入会时立即开启云录制：false - 关闭（主持人入会自动开启云录制）；true - 开启（有参会成员入会自动开启云录制，默认关闭）。
    /// <para>官方限制：仅 <see cref="AutoRecordType"/> 为 cloud 时才能配置；依赖企业账户设置，当企业强制锁定后，该参数必须与企业配置保持一致。</para>
    /// </summary>
    [JsonPropertyName("attendee_join_auto_record")]
    public bool? AttendeeJoinAutoRecord { get; set; }

    /// <summary>
    /// 获取或设置是否允许主持人暂停或停止云录制（默认允许）。
    /// <para>官方限制：仅 <see cref="AutoRecordType"/> 为 cloud 时才能配置；依赖企业账户设置，当企业强制锁定后，该参数必须与企业配置保持一致。</para>
    /// </summary>
    [JsonPropertyName("enable_host_pause_auto_record")]
    public bool? EnableHostPauseAutoRecord { get; set; }

    /// <summary>获取或设置同声传译开关（默认不开启；仅创建预约会议文档页声明该参数）。</summary>
    [JsonPropertyName("enable_interpreter")]
    public bool? EnableInterpreter { get; set; }

    /// <summary>获取或设置是否激活会议报名（配合「修改/获取会议报名配置」接口使用）。</summary>
    [JsonPropertyName("enable_enroll")]
    public bool? EnableEnroll { get; set; }

    /// <summary>获取或设置是否开启主持人密钥（默认不开启）。</summary>
    [JsonPropertyName("enable_host_key")]
    public bool? EnableHostKey { get; set; }

    /// <summary>获取或设置主持人密钥（仅支持 6 位数字；开启主持人密钥后未填写此项时将自动分配一个 6 位数字密钥）。</summary>
    [JsonPropertyName("host_key")]
    public string? HostKey { get; set; }

    /// <summary>获取或设置是否允许成员上传文档（仅获取会议详情响应返回）。</summary>
    [JsonPropertyName("enable_doc_upload_permission")]
    public bool? EnableDocUploadPermission { get; set; }

    /// <summary>获取或设置会议当前主持人列表（仅获取会议详情响应返回；结构同 <see cref="MeetingHosts"/>）。</summary>
    [JsonPropertyName("current_hosts")]
    public MeetingHosts? CurrentHosts { get; set; }

    /// <summary>获取或设置联席主持人列表（仅获取会议详情响应返回；结构同 <see cref="MeetingHosts"/>）。</summary>
    [JsonPropertyName("co_hosts")]
    public MeetingHosts? CoHosts { get; set; }
}
