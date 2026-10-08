// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「审批」模块假期管理域公共 SDK
/// （获取企业假期管理配置 + 获取成员假期余额 + 修改成员假期余额，三端点收敛面）。
/// <para>
/// 官方对三类应用开放一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalVacationService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderVacationService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyVacationService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：自建应用须配置到
/// 「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkVacationService
{
    /// <summary>
    /// 获取企业假期管理配置
    /// <para>获取可见范围内员工的「假期管理」配置，包括：各个假期的id、名称、请假单位、时长计算方式、发放规则等。</para>
    /// <para>官方限制：接口频率限制 600 次/分钟。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>假期列表（lists：id / name / time_attr / duration_type / quota_attr / perday_duration / is_newovertime / enter_comp_time_limit / expire_rule）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93375"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94211"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96512"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。</para>
    /// </remarks>
    [Get("/cgi-bin/oa/vacation/getcorpconf")]
    Task<GetVacationCorpConfResponse> GetCorpVacationConfAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员假期余额
    /// <para>获取应用可见范围内指定成员的假期余额数据。</para>
    /// <para>官方限制：接口频率限制 600 次/分钟；余额的时长单位都为秒——假期时间刻度为「按天」时需除以 86400 得到真实假期余额天数，
    /// 为「按小时」时需除以 3600 得到真实假期余额小时数。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetVacationUserQuotaRequest"/>：userid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>假期余额列表（lists：id / assignduration / usedduration / leftduration / vacationname / real_assignduration）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93376"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94212"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96513"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/vacation/getuservacationquota")]
    Task<GetVacationUserQuotaResponse> GetUserVacationQuotaAsync(
        [Body] GetVacationUserQuotaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改成员假期余额
    /// <para>修改可见范围内指定成员的「假期余额」。</para>
    /// <para>官方限制：接口频率限制 600 次/分钟；leftduration 单位为秒，不能大于 1000 天或 24000 小时，
    /// 当假期时间刻度为按小时请假时必须为 360 的整倍数（即 0.1 小时整倍数），按天请假时必须为 8640 的整倍数（即 0.1 天整倍数）；
    /// time_attr 主要用于校验，必须等于企业假期管理配置中设置的假期时间刻度类型；remarks 不超过 200 字符。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetVacationUserQuotaRequest"/>：userid / vacation_id / leftduration / time_attr / remarks）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93377"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94213"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96514"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/vacation/setoneuserquota")]
    Task<WechatWorkResponse> SetUserVacationQuotaAsync(
        [Body] SetVacationUserQuotaRequest request,
        CancellationToken cancellationToken = default);
}
