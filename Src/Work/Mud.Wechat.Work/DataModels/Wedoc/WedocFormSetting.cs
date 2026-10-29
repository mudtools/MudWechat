// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表的设置（官方 <c>form_setting</c>；创建收集表请求、编辑收集表请求与获取收集表信息响应共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：定时重复与定时结束互斥，若都填优先定时重复；
/// 编辑收集表时若收集表当前为家校范围，<c>fill_out_auth</c> 无法修改。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormSetting
{
    /// <summary>
    /// 获取或设置填写权限（官方 <c>fill_out_auth</c>，默认 0）。
    /// 官方取值：<c>0</c> 所有人、<c>1</c> 企业内指定人/部门、<c>4</c> 家校所有范围。
    /// </summary>
    [JsonPropertyName("fill_out_auth")]
    public uint? FillOutAuth { get; set; }

    /// <summary>获取或设置指定的可填写的人/部门（官方 <c>fill_in_range</c>），定时重复开启时必填。</summary>
    [JsonPropertyName("fill_in_range")]
    public WedocFormRange? FillInRange { get; set; }

    /// <summary>获取或设置收集表管理员（官方 <c>setting_manager_range</c>）。</summary>
    [JsonPropertyName("setting_manager_range")]
    public WedocFormRange? SettingManagerRange { get; set; }

    /// <summary>获取或设置定时重复设置项（官方 <c>timed_repeat_info</c>）。</summary>
    [JsonPropertyName("timed_repeat_info")]
    public WedocFormTimedRepeatInfo? TimedRepeatInfo { get; set; }

    /// <summary>获取或设置是否允许每人提交多份（官方 <c>allow_multi_fill</c>），默认 <c>false</c>。</summary>
    [JsonPropertyName("allow_multi_fill")]
    public bool? AllowMultiFill { get; set; }

    /// <summary>获取或设置定时关闭时间戳（官方 <c>timed_finish</c>）；定时重复与定时结束互斥，若都填优先定时重复。</summary>
    [JsonPropertyName("timed_finish")]
    public uint? TimedFinish { get; set; }

    /// <summary>获取或设置是否支持匿名填写（官方 <c>can_anonymous</c>），默认 <c>false</c>。</summary>
    [JsonPropertyName("can_anonymous")]
    public bool? CanAnonymous { get; set; }

    /// <summary>获取或设置是否有回复时提醒（官方 <c>can_notify_submit</c>），默认 <c>false</c>。</summary>
    [JsonPropertyName("can_notify_submit")]
    public bool? CanNotifySubmit { get; set; }
}
