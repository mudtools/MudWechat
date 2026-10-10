// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 企业打卡规则加班响应侧休息扣除配置（获取企业所有打卡规则响应 <c>ot_info.*.ot_*_restinfo</c>，旧 ot_info 结构）。
/// </summary>
/// <remarks>
/// <para>与创建/修改打卡规则请求侧休息扣除配置（<see cref="CheckinOtV2RestInfo"/>）不同构：本旧结构的指定休息时间为单对象 <c>fix_time_rule</c>，请求侧为 <c>fix_time_rule_list</c> 数组。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtRestInfo
{
    /// <summary>获取或设置休息扣除类型：0 - 不开启扣除；1 - 指定休息时间扣除；2 - 按加班时长扣除休息时间。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置指定休息时间配置（当 type 为 1 时有意义）。</summary>
    [JsonPropertyName("fix_time_rule")]
    public CheckinOtFixTimeRule? FixTimeRule { get; set; }

    /// <summary>获取或设置按加班时长扣除配置（当 type 为 2 时有意义）。</summary>
    [JsonPropertyName("cal_ottime_rule")]
    public CheckinOtCalRule? CalOttimeRule { get; set; }
}
