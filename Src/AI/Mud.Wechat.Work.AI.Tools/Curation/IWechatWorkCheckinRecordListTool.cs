// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>checkin.record.list</c>：获取成员指定时间段内的打卡记录。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么落在自建接口上</b>：打卡记录域父接口是零端点收敛面（三种应用类型的响应结构<b>不同构</b>，
/// 第三方为旧字段结构），故端点只存在于应用类型子接口；本工具消费自建口径
/// <c>IWechatWorkInternalCheckinRecordService</c>，代开发/第三方部署下该域工具软缺席。
/// </para>
/// <para>
/// <b>只给考勤事实、不给位置轨迹</b>：官方响应携带 <c>lat</c>/<c>lng</c>/<c>location_title</c>/<c>location_detail</c>/
/// <c>wifiname</c>/<c>wifimac</c>/<c>notes</c>/<c>mediaids</c>——它们是行踪与设备指纹，进入提示词与日志即不可撤回。
/// 执行器只投影「谁、哪一次打卡、什么类型、有没有异常、标准打卡时间、所属规则/班次」。
/// </para>
/// </remarks>
[AgentTool(
    name: "checkin.record.list",
    Description = "获取企业微信成员的打卡记录。参数 userid_list 为成员 userid 列表（1~100 个，超过请分批；可先用 contacts.user.list 取 userid）；start_time / end_time 为时间范围（Unix 秒，如 \"1700000000\"；也接受带时区的 ISO 8601，如 \"2026-10-10T09:00:00+08:00\"），跨度不得超过 30 天且 end_time 必须大于 start_time；data_type 为打卡类型（Commute 上下班打卡 / Field 外出打卡 / All 全部，默认 All）。返回每条记录的 userid、checkin_type（中文字符串，如「上班打卡」）、exception_type（中文字符串，多个以分号间隔，如「时间异常」）、checkin_time、sch_checkin_time（标准打卡时间）、规则名与规则/班次 ID。不含打卡地点、经纬度、WiFi、设备号与备注（行踪与设备指纹不外发）。",
    Source = "IWechatWorkInternalCheckinRecordService.GetCheckinDataAsync",
    RequiredScopes = ["checkin.record.read"])]
public interface IWechatWorkCheckinRecordListTool
{
    /// <summary>获取打卡记录。</summary>
    /// <param name="userid_list">成员 userid 列表（1~100 个）。</param>
    /// <param name="start_time">起始时间（Unix 秒，或带时区的 ISO 8601）。</param>
    /// <param name="end_time">结束时间（Unix 秒，或带时区的 ISO 8601；须大于 start_time 且跨度 ≤ 30 天）。</param>
    /// <param name="data_type">打卡类型（Commute 上下班 / Field 外出 / All 全部；可空 ⇒ All）。</param>
    void List(
        [ToolParameter("userid_list", "成员 userid 列表（1~100 个）。", Required = true)] string[] userid_list,
        [ToolParameter("start_time", "起始时间（Unix 秒，或带时区的 ISO 8601）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒，或带时区的 ISO 8601；须大于 start_time 且跨度 ≤ 30 天）。", Required = true)] string end_time,
        [ToolParameter("data_type", "打卡类型（Commute 上下班 / Field 外出 / All 全部；可空 ⇒ All）。", EnumType = typeof(WechatCheckinDataTypes))] int? data_type);
}
