// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Checkin;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块打卡报表域第三方应用 SDK。
/// <para>
/// 承载「获取打卡日报数据」「获取打卡月报数据」的第三方文档口径旧字段结构（官方文档 94206/94207：
/// baseinfo（无下划线）/ acctivity_name / overwork / ov_time 等，与自建/代开发文档页的
/// base_info/summary_info/ot_info/overwork_info 现代结构不同构）。
/// </para>
/// <para>企业自建应用见 <see cref="IWechatWorkInternalCheckinReportService"/>；服务商代开发见 <see cref="IWechatWorkProviderCheckinReportService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：第三方应用须具有「打卡」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Checkin",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCheckinReportService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyCheckinReportService : IWechatWorkCheckinReportService
{
    /// <summary>
    /// 获取打卡日报数据
    /// <para>获取指定员工指定时间段内的打卡日报统计数据（第三方文档口径旧字段结构）。</para>
    /// <para>官方限制：获取记录时间跨度不超过 30 天；用户列表不超过 100 个，若超过请分批获取；系统会自动校验时间跨度，超过拒绝查询。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinDayDataRequest"/>：starttime / endtime / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日报数据列表（datas：baseinfo 基础信息 / rule_info 规则信息 / holiday_infos 假期信息 / scheduleinfo 班次信息）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94206"/></para>
    /// <para>官方权限：第三方应用须具有「打卡」权限；自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：顶层字段为 baseinfo（区别于自建文档页的 base_info）；record_type 已废弃改用 checkin_recordtype；dep_name 为 JSON 序列化字符串；rule_info.checkin_time 参数表描述为时间戳实为对象数组；scheduleinfo.timesec 参数表描述为标量实为对象；acctivity_name 官方拼写照抄。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckin_daydata")]
    Task<GetThirdPartyCheckinDayDataResponse> GetCheckinDayDataAsync(
        [Body] GetCheckinDayDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取打卡月报数据
    /// <para>获取指定员工的月报统计数据（第三方文档口径旧字段结构）。</para>
    /// <para>官方限制：获取记录时间跨度不超过 30 天；用户列表不超过 100 个，若超过请分批获取；系统会自动校验时间跨度，超过拒绝查询。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinMonthDataRequest"/>：starttime / endtime / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>统计数据列表（datas：baseinfo 统计基本信息，含 exception_infos / overwork / late_comes_cnt / absenteeisms_cnt / holiday_infos / ov_time 等）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94207"/></para>
    /// <para>官方权限：第三方应用须具有「打卡」权限；自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：overwork 与 ov_time 参数表描述为单对象、返回示例均为数组（以示例为准）；excepion_days_cnt/acctivity_name 为官方拼写照抄勿修正；exception_infos.type 为数字枚举（区别于打卡记录接口的中文描述串）。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckin_monthdata")]
    Task<GetThirdPartyCheckinMonthDataResponse> GetCheckinMonthDataAsync(
        [Body] GetCheckinMonthDataRequest request,
        CancellationToken cancellationToken = default);
}
