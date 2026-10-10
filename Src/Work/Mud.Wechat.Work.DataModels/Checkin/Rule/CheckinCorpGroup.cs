// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 企业打卡规则信息（获取企业所有打卡规则响应 <c>group</c> 元素，含打卡范围、加班信息、汇报对象等管理字段）。
/// </summary>
/// <remarks>
/// <para>
/// 本响应模型与创建/修改打卡规则请求 <c>group</c>（<see cref="CheckinRuleGroup"/>）不同构：
/// 响应侧加班信息为旧 <c>ot_info</c> 结构（<see cref="CheckinOtInfo"/>），
/// 官方错误表明确「ot_info是旧字段，不建议使用」，创建/修改请求应使用 <c>ot_info_v2</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinCorpGroup
{
    /// <summary>获取或设置打卡规则类型：1 - 固定时间上下班；2 - 按班次上下班；3 - 自由上下班。</summary>
    [JsonPropertyName("grouptype")]
    public int? Grouptype { get; set; }

    /// <summary>获取或设置打卡规则 id。</summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>获取或设置打卡规则名称。</summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>获取或设置是否开启审批打卡（若未返回则默认开启）。</summary>
    [JsonPropertyName("open_sp_checkin")]
    public bool? OpenSpCheckin { get; set; }

    /// <summary>获取或设置打卡时间（当规则类型为排班时没有意义）。</summary>
    [JsonPropertyName("checkindate")]
    public List<CheckinGroupCheckindate>? Checkindate { get; set; }

    /// <summary>获取或设置特殊日期 - 必须打卡日期信息列表（timestamp 表示具体时间）。</summary>
    [JsonPropertyName("spe_workdays")]
    public List<CheckinSpeDay>? SpeWorkdays { get; set; }

    /// <summary>获取或设置特殊日期 - 不用打卡日期信息列表（timestamp 表示具体时间）。</summary>
    [JsonPropertyName("spe_offdays")]
    public List<CheckinSpeDay>? SpeOffdays { get; set; }

    /// <summary>获取或设置是否同步法定节假日（true 为同步；当前排班不支持）。</summary>
    [JsonPropertyName("sync_holidays")]
    public bool? SyncHolidays { get; set; }

    /// <summary>获取或设置是否打卡必须拍照（true 为必须拍照）。</summary>
    [JsonPropertyName("need_photo")]
    public bool? NeedPhoto { get; set; }

    /// <summary>获取或设置打卡地点 - WiFi 打卡信息。</summary>
    [JsonPropertyName("wifimac_infos")]
    public List<CheckinWifiInfo>? WifimacInfos { get; set; }

    /// <summary>获取或设置是否备注时允许上传本地图片（true 为允许）。</summary>
    [JsonPropertyName("note_can_use_local_pic")]
    public bool? NoteCanUseLocalPic { get; set; }

    /// <summary>获取或设置是否非工作日允许打卡（true 为允许）。</summary>
    [JsonPropertyName("allow_checkin_offworkday")]
    public bool? AllowCheckinOffworkday { get; set; }

    /// <summary>获取或设置是否允许提交补卡申请（true 为允许）。</summary>
    [JsonPropertyName("allow_apply_offworkday")]
    public bool? AllowApplyOffworkday { get; set; }

    /// <summary>获取或设置打卡地点 - 位置打卡信息。</summary>
    [JsonPropertyName("loc_infos")]
    public List<CheckinLocInfo>? LocInfos { get; set; }

    /// <summary>获取或设置打卡人员信息。</summary>
    [JsonPropertyName("range")]
    public CheckinRange? Range { get; set; }

    /// <summary>获取或设置创建打卡规则时间（Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置打卡人员白名单（即不需要打卡人员，需要有设置白名单才能查看）。</summary>
    [JsonPropertyName("white_users")]
    public List<string>? WhiteUsers { get; set; }

    /// <summary>获取或设置打卡方式：0 - 手机；2 - 智慧考勤机；3 - 手机+智慧考勤机。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置汇报对象信息。</summary>
    [JsonPropertyName("reporterinfo")]
    public CheckinReporterInfo? Reporterinfo { get; set; }

    /// <summary>获取或设置加班信息（相关信息需要设置后才能显示；官方错误表明确 ot_info 是旧字段不建议使用，读取侧仍按本结构返回）。</summary>
    [JsonPropertyName("ot_info")]
    public CheckinOtInfo? OtInfo { get; set; }

    /// <summary>获取或设置每月最多补卡次数（默认 -1 表示不限制）。</summary>
    [JsonPropertyName("allow_apply_bk_cnt")]
    public int? AllowApplyBkCnt { get; set; }

    /// <summary>获取或设置范围外打卡处理方式：0 - 视为范围外异常（默认值）；1 - 视为正常外勤；2 - 不允许范围外打卡。</summary>
    [JsonPropertyName("option_out_range")]
    public int? OptionOutRange { get; set; }

    /// <summary>获取或设置规则创建人 userid。</summary>
    [JsonPropertyName("create_userid")]
    public string? CreateUserid { get; set; }

    /// <summary>获取或设置人脸识别打卡开关（true 为启用）。</summary>
    [JsonPropertyName("use_face_detect")]
    public bool? UseFaceDetect { get; set; }

    /// <summary>获取或设置允许补卡时限（默认 -1 表示不限制；单位天）。</summary>
    [JsonPropertyName("allow_apply_bk_day_limit")]
    public int? AllowApplyBkDayLimit { get; set; }

    /// <summary>获取或设置规则最近编辑人 userid。</summary>
    [JsonPropertyName("update_userid")]
    public string? UpdateUserid { get; set; }

    /// <summary>获取或设置排班信息（只有规则为按班次上下班打卡时才有该配置）。</summary>
    [JsonPropertyName("schedulelist")]
    public List<CheckinScheduleInfo>? Schedulelist { get; set; }

    /// <summary>获取或设置上班打卡后 xx 秒可打下班卡（应用范围：固定上下班加班、自由排班加班、自由签到上下班、自由签到加班）。</summary>
    [JsonPropertyName("offwork_interval_time")]
    public int? OffworkIntervalTime { get; set; }

    /// <summary>
    /// 获取或设置补卡指定异常类型（按比特位设置，大端模式，某位 bit 置位为 1 表示关闭某类型；
    /// 从低到高四个比特位分别表示缺卡类型、迟到类型、早退类型、其他异常类型；默认值 0 表示所有异常类型均允许补卡；官方类型 uint64）。
    /// </summary>
    [JsonPropertyName("buka_restriction")]
    public long? BukaRestriction { get; set; }

    /// <summary>获取或设置自由上下班规则的跨天时间（距离 0 点的秒数）。</summary>
    [JsonPropertyName("span_day_time")]
    public int? SpanDayTime { get; set; }

    /// <summary>获取或设置自由上下班规则的工作时长（秒；-1 表示不限制）。</summary>
    [JsonPropertyName("standard_work_duration")]
    public int? StandardWorkDuration { get; set; }

    /// <summary>获取或设置打卡交替方式：0 - 多组交替，1 - 单组交替，2 - 仅记录打卡时间和位置。对固定上下班和排班上下班非工作日生效，自由签到都生效。</summary>
    [JsonPropertyName("checkin_method_type")]
    public int? CheckinMethodType { get; set; }
}
