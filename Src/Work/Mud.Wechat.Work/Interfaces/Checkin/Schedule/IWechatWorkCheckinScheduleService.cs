// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Checkin;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「打卡」模块打卡排班域公共 SDK（获取打卡人员排班信息 + 为打卡人员排班）。
/// <para>
/// 官方对三类应用开放一致的 2 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalCheckinScheduleService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderCheckinScheduleService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyCheckinScheduleService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：自建应用须配置到
/// 「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkCheckinScheduleService
{
    /// <summary>
    /// 获取打卡人员排班信息
    /// <para>获取应用可见范围内、打卡规则为「按班次上下班」规则的指定员工指定时间段内的排班信息。</para>
    /// <para>官方限制：仅适用于打卡规则为「按班次上下班」规则的员工；用户列表不超过 100 个；endtime 与 starttime 跨度不超过一个月；接口调用频率限制为 60 次/分钟；专属错误码 301021 userid 错误 / 301070 系统错误 / 301075 输入参数错误。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinScheduleListRequest"/>：useridlist 用户列表 / starttime / endtime）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>排班表信息（schedule_list：userid / yearmonth 年月数字 / groupid / groupname / schedule.scheduleList 个人排班表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93380"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94208"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96500"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：schedule.scheduleList 为 camelCase 字段名（同页其它字段均 snake_case，照抄勿改）；yearmonth 为 uint32 数字而非字符串；时段 id 字段名为 id（区别于打卡规则中的 time_id）。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckinschedulist")]
    Task<GetCheckinScheduleListResponse> GetCheckinScheduleListAsync(
        [Body] GetCheckinScheduleListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为打卡人员排班
    /// <para>为打卡规则为「按班次上下班」规则的指定员工排班。</para>
    /// <para>官方限制：仅支持为打卡规则为「按班次上下班」的规则排班；day 取值 1-31，与 yearmonth 联合组成唯一日期；schedule_id 为 0 代表休息；接口调用频率限制为 60 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetCheckinScheduleListRequest"/>：groupid 规则 id / yearmonth 年月数字 / items 排班项列表（userid / day / schedule_id））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93385"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94209"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96501"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：官方参数表把顶层参数（groupid/yearmonth）与 items 元素字段（userid/day/schedule_id）混在一张表里且未标注嵌套关系，嵌套结构以官方请求示例为准。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/setcheckinschedulist")]
    Task<WechatWorkResponse> SetCheckinScheduleListAsync(
        [Body] SetCheckinScheduleListRequest request,
        CancellationToken cancellationToken = default);
}
