// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微信客服」模块统计管理域公共 SDK（「客户数据统计」企业汇总数据 + 接待人员明细数据）。
/// <para>
/// 官方对三类应用开放完全一致的 2 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalKfStatisticsService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyKfStatisticsService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderKfStatisticsService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「微信客服-可调用接口的应用」中；
/// 第三方 / 代开发应用须具有「微信客服-&gt;服务工具-&gt;获取客服数据统计」权限。
/// </para>
/// <para>
/// 官方约束：查询区间为闭区间且最大跨度 31 天，最多获取最近 180 天数据；
/// 传入非 0 点时间戳会被官方向下取整到当天 0 点；当天数据次日才可获取（建议次日早上六点后调用）；
/// 开启 API 或授权第三方应用管理会话的，没有 2022 年 3 月 11 日以前的统计数据；
/// 无数据的指标官方直接缺省不返回，解析需空值容错。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkKfStatisticsService
{
    /// <summary>
    /// 获取「客户数据统计」企业汇总数据
    /// <para>按天获取指定客服账号的企业维度汇总统计数据（咨询会话数、咨询客户数、智能回复指标等）；
    /// 查询区间闭区间最大跨度 31 天，最多查最近 180 天。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetKfCorpStatisticRequest"/>：open_kfid / start_time / end_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐日统计列表（statistic_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95489"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95492"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96432"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/get_corp_statistic")]
    Task<GetKfCorpStatisticResponse> GetCorpStatisticAsync(
        [Body] GetKfCorpStatisticRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取「客户数据统计」接待人员明细数据
    /// <para>按天获取接待人员维度的统计数据（人工回复率、首次响应时长、满意度评价等）；
    /// 不指定 servicer_userid 时返回客服账号维度的汇总数据（并非全体接待人员明细）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetKfServicerStatisticRequest"/>：open_kfid / servicer_userid / start_time / end_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐日统计列表（statistic_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95490"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95493"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96433"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/get_servicer_statistic")]
    Task<GetKfServicerStatisticResponse> GetServicerStatisticAsync(
        [Body] GetKfServicerStatisticRequest request,
        CancellationToken cancellationToken = default);
}
