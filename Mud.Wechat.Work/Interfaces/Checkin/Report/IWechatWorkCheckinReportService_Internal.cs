// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Checkin;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块打卡报表域企业自建应用 SDK。
/// <para>
/// 承载「获取打卡日报数据」「获取打卡月报数据」（现代字段结构 base_info/summary_info/ot_info/overwork_info，
/// 服务商代开发文档页同构，见 <see cref="IWechatWorkProviderCheckinReportService"/>）。
/// </para>
/// <para>服务商代开发见 <see cref="IWechatWorkProviderCheckinReportService"/>；第三方应用见 <see cref="IWechatWorkThirdPartyCheckinReportService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用须配置到「打卡 - 可调用接口的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Checkin",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCheckinReportService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalCheckinReportService : IWechatWorkCheckinReportService
{
    /// <summary>
    /// 获取打卡日报数据
    /// <para>获取应用可见范围内指定员工指定日期内的打卡日报统计数据。</para>
    /// <para>官方限制：获取记录时间跨度不超过 30 天；用户列表不超过 100 个，若超过请分批获取；starttime/endtime 为 0 点 Unix 时间戳；接口调用频率限制为 100 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinDayDataRequest"/>：starttime / endtime / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日报数据列表（datas：base_info 基础信息 / summary_info 汇总信息 / holiday_infos 假勤信息 / exception_infos 校准状态 / ot_info 加班信息 / sp_items 假勤统计）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93374"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96498"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：departs_name 是分号分隔的多部门字符串（非数组）；earliest_time/lastest_time（注意 lastest 拼写）是距离 0 点的秒数而非时间戳；日报加班信息 ot_info 与月报 overwork_info 字段命名不对称，两结构不可共用。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckin_daydata")]
    Task<GetCheckinDayDataResponse> GetCheckinDayDataAsync(
        [Body] GetCheckinDayDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取打卡月报数据
    /// <para>获取应用可见范围内指定员工指定日期内的打卡月报统计数据。</para>
    /// <para>官方限制：用户列表不超过 100 个，若超过请分批获取；starttime/endtime 为 0 点 Unix 时间戳；接口调用频率限制为 60 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinMonthDataRequest"/>：starttime / endtime / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>月报数据列表（datas：base_info 基础信息 / summary_info 汇总信息 / exception_infos 异常统计 / sp_items 假勤统计 / overwork_info 加班情况）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93387"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96499"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：overwork_info 三个「时长」字段为单数形式（workday_over_sec 等）、六个「记为调休/加班费」字段为复数形式（workdays_over_as_* 等），照抄勿改；summary_info.regular_days 仅参数表列出、示例未包含。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckin_monthdata")]
    Task<GetCheckinMonthDataResponse> GetCheckinMonthDataAsync(
        [Body] GetCheckinMonthDataRequest request,
        CancellationToken cancellationToken = default);
}
