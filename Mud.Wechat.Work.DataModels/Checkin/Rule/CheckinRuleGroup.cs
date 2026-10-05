// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则请求模型（创建/修改打卡规则请求 <c>group</c> 字段，管理打卡规则 <c>/cgi-bin/checkin/add_checkin_option</c> 等）。
/// </summary>
/// <remarks>
/// <para>
/// 本请求模型与获取企业所有打卡规则（getcorpcheckinoption）返回的规则模型<b>不同构</b>：
/// 请求侧加班配置使用 <see cref="CheckinOtInfoV2"/>（workdayconf/restdayconf/holidayconf），
/// 旧字段 <c>ot_info</c> 官方错误表明确「ot_info是旧字段，不建议使用」（301094）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRuleGroup
{
    /// <summary>获取或设置打卡规则 id（更新打卡规则时必填；创建时无需传入，该字段会被忽略）。</summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>获取或设置规则类型（官方必填）：1 - 固定时间上下班；2 - 按班次上下班；3 - 自由上下班。</summary>
    [JsonPropertyName("grouptype")]
    public int? Grouptype { get; set; }

    /// <summary>获取或设置规则名（官方必填；不能为空，且最大字符个数不超过 40）。</summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>获取或设置打卡时间（「固定时间上下班」类型时必填；个数需要大于 0 且不超过 7；自定义排班不可设置本字段）。</summary>
    [JsonPropertyName("checkindate")]
    public List<CheckinRuleCheckindate>? Checkindate { get; set; }

    /// <summary>获取或设置特殊工作日（个数不超过 366；自定义排班/自由签到不可设置特殊工作日）。</summary>
    [JsonPropertyName("spe_workdays")]
    public List<CheckinRuleSpeWorkday>? SpeWorkdays { get; set; }

    /// <summary>获取或设置特殊非工作日（个数不超过 366）。</summary>
    [JsonPropertyName("spe_offdays")]
    public List<CheckinRuleSpeOffday>? SpeOffdays { get; set; }

    /// <summary>获取或设置是否同步法定节假日（默认为 true）。</summary>
    [JsonPropertyName("sync_holidays")]
    public bool? SyncHolidays { get; set; }

    /// <summary>获取或设置是否需要拍照（默认为 false）。</summary>
    [JsonPropertyName("need_photo")]
    public bool? NeedPhoto { get; set; }

    /// <summary>获取或设置 WiFi 打卡信息（wifimac_infos 与 loc_infos 不能同时为空；个数不可超过 500）。</summary>
    [JsonPropertyName("wifimac_infos")]
    public List<CheckinWifiInfo>? WifimacInfos { get; set; }

    /// <summary>获取或设置备注时是否允许上传本地图片（默认可以使用本地图片）。</summary>
    [JsonPropertyName("note_can_use_local_pic")]
    public bool? NoteCanUseLocalPic { get; set; }

    /// <summary>获取或设置固定上下班是否允许非工作日打卡（默认不允许）。</summary>
    [JsonPropertyName("allow_checkin_offworkday")]
    public bool? AllowCheckinOffworkday { get; set; }

    /// <summary>获取或设置位置打卡地点信息（wifimac_infos 与 loc_infos 不能同时为空；个数不可超过 500）。</summary>
    [JsonPropertyName("loc_infos")]
    public List<CheckinLocInfo>? LocInfos { get; set; }

    /// <summary>获取或设置打卡人员信息（官方要求至少有一种人员来源；party_id、userid、tagid 不可同时为空）。</summary>
    [JsonPropertyName("range")]
    public CheckinRange? Range { get; set; }

    /// <summary>获取或设置打卡人员白名单（即不需要打卡人员）。</summary>
    [JsonPropertyName("white_users")]
    public List<string>? WhiteUsers { get; set; }

    /// <summary>获取或设置打卡方式（默认为 0）：字段表口径 0 - 手机，2 - 考勤机，3 - 手机/考勤机；错误表口径 0 - 手机，1 - u盘考勤机，3 - 手机\云考勤机融合打卡（官方两处表述不一致，照抄原文）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置汇报人信息。</summary>
    [JsonPropertyName("reporterinfo")]
    public CheckinReporterInfo? Reporterinfo { get; set; }

    /// <summary>获取或设置是否允许补卡（默认不允许；设置补卡相关字段需要先开启本项）。</summary>
    [JsonPropertyName("allow_apply_offworkday")]
    public bool? AllowApplyOffworkday { get; set; }

    /// <summary>获取或设置每月最多补卡次数（默认 -1 表示不限制，不可小于 -1 或大于 99）。</summary>
    [JsonPropertyName("allow_apply_bk_cnt")]
    public int? AllowApplyBkCnt { get; set; }

    /// <summary>获取或设置允许补卡时限（默认 -1 表示不限制；1 表示当天，2 表示当天和昨天，最大不超过 180 天）。</summary>
    [JsonPropertyName("allow_apply_bk_day_limit")]
    public int? AllowApplyBkDayLimit { get; set; }

    /// <summary>获取或设置补卡截止日期：次月第 n 天不能补卡（-1 表示不开启截止日期，不可小于 -1 或等于 0 或大于 28）。</summary>
    [JsonPropertyName("buka_limit_next_month")]
    public int? BukaLimitNextMonth { get; set; }

    /// <summary>获取或设置范围外打卡处理方式（默认为 0）：0 - 不在公司范围内视为异常；1 - 不在公司范围内视为外勤；2 - 禁止不在公司范围内打卡（不可大于 2）。</summary>
    [JsonPropertyName("option_out_range")]
    public int? OptionOutRange { get; set; }

    /// <summary>获取或设置自定义排班规则所有排班（「按班次上下班」类型时必填；最多添加 50 个排班；必须存在 schedule_id=0 的休息排班；自由签到规则不可设置）。</summary>
    [JsonPropertyName("schedulelist")]
    public List<CheckinRuleSchedule>? Schedulelist { get; set; }

    /// <summary>获取或设置上班打卡后 xx 秒可打下班卡（自由签到场景须为 5 分钟的倍数且不可超过 1435 分钟。应用范围：固定上下班加班、自由排班加班、自由签到上下班、自由签到加班）。</summary>
    [JsonPropertyName("offwork_interval_time")]
    public int? OffworkIntervalTime { get; set; }

    /// <summary>获取或设置是否人脸检测（默认为 false；开启活体检测前须先开启人脸检测）。</summary>
    [JsonPropertyName("use_face_detect")]
    public bool? UseFaceDetect { get; set; }

    /// <summary>获取或设置活体检测开关（默认为 false；开启活体检测时需要先开启人脸检测 use_face_detect）。</summary>
    [JsonPropertyName("open_face_live_detect")]
    public bool? OpenFaceLiveDetect { get; set; }

    /// <summary>获取或设置加班配置（官方必填；workdayconf/restdayconf/holidayconf 三组配置的 allow_ot、type 均为必填；旧字段 ot_info 不建议使用，301094）。</summary>
    [JsonPropertyName("ot_info_v2")]
    public CheckinOtInfoV2? OtInfoV2 { get; set; }

    /// <summary>获取或设置外出打卡同步至上下班（默认为 false）。</summary>
    [JsonPropertyName("sync_out_checkin")]
    public bool? SyncOutCheckin { get; set; }

    /// <summary>获取或设置补卡提醒（buka_remind_day/buka_remind_month 需要先设置 open_remind 为 true；buka_remind_month 为 0 时 buka_remind_day 不可小于 15、不可超过 30，为 1 时不可小于 1、不可超过 26）。</summary>
    [JsonPropertyName("buka_remind")]
    public CheckinBukaRemind? BukaRemind { get; set; }

    /// <summary>
    /// 获取或设置补卡指定异常类型（按比特位设置，大端模式，某位 bit 置位为 1 表示关闭某类型；
    /// 从低到高四个比特位分别表示缺卡类型、迟到类型、早退类型、其他异常类型；默认值 0 表示所有异常类型均允许补卡；官方类型 uint64）。
    /// </summary>
    [JsonPropertyName("buka_restriction")]
    public long? BukaRestriction { get; set; }

    /// <summary>获取或设置自由上下班规则的跨天时间（距离 0 点的秒数；必须为整分钟的秒数，且小于 24 小时；固定/自定义上下班规则不可设置本字段）。</summary>
    [JsonPropertyName("span_day_time")]
    public int? SpanDayTime { get; set; }

    /// <summary>获取或设置自由上下班规则的工作时长（秒；-1 表示不限制工作时长，否则值应以半小时为步长从 1 小时开始递增，最多不超过 24 小时；固定/自定义上下班规则不可设置本字段）。</summary>
    [JsonPropertyName("standard_work_duration")]
    public int? StandardWorkDuration { get; set; }

    /// <summary>获取或设置是否开启审批打卡。</summary>
    [JsonPropertyName("open_sp_checkin")]
    public bool? OpenSpCheckin { get; set; }

    /// <summary>获取或设置打卡交替方式：0 - 多组交替，1 - 单组交替，2 - 仅记录打卡时间和位置。对固定上下班和排班上下班休息日生效，自由签到都生效；固定上下班和排班上下班仅支持 0/1；官方类型标注写作「unit32」（官方拼错，照抄）。</summary>
    [JsonPropertyName("checkin_method_type")]
    public int? CheckinMethodType { get; set; }
}
