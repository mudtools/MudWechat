// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「客户联系」模块统计管理域公共 SDK
/// （联系客户统计 + 客户群数据统计）。
/// <para>
/// 官方对三类应用开放完全一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactStatisticsService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactStatisticsService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactStatisticsService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：联系客户统计端点要求自建应用使用配置到「可调用应用」列表中的 secret 获取的 access_token，
/// 第三方 / 代开发应用须具有「获取成员联系客户的数据统计」权限；群聊统计端点第三方 / 代开发应用
/// 须具有「企业客户权限-&gt;客户群-&gt;获取客户群的数据统计」权限。
/// 传入的成员 / 部门 / 群主须在应用可见范围内。
/// </para>
/// <para>
/// 官方约束：统计均为天维度闭区间查询，最大跨度 30 天，数据最多保留 180 天，非零点时间戳向下取整到当日 0 点。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactStatisticsService
{
    /// <summary>
    /// 获取「联系客户统计」数据
    /// <para>获取成员联系客户的数据：发起申请数、新增客户数、聊天数、发送消息数、
    /// 已回复聊天占比、平均首次回复时长、删除 / 拉黑成员的客户数等。</para>
    /// <para>userid 与 partyid 不可同时为空；数据以天为维度、闭区间，最大跨度 30 天，
    /// 最多可取最近 180 天；传入多个 userid 时返回总体数据。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetUserBehaviorDataRequest"/>：userid / partyid / start_time / end_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员行为数据列表（behavior_data，每项为一天的数据）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92132"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92275"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96359"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_user_behavior_data")]
    Task<GetUserBehaviorDataResponse> GetUserBehaviorDataAsync(
        [Body] GetUserBehaviorDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取「群聊数据统计」（按群主聚合）
    /// <para>按群主维度获取客户群数据统计（新增群数量、群总数、群人数、消息总数等），支持排序与分页。</para>
    /// <para>查询区间为闭区间，最大跨度 30 天，数据最多保留 180 天；
    /// 不指定群主过滤时取应用可见范围内全部群主（可见范围超 1000 人报错 81017）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupChatStatisticRequest"/>：day_begin_time / owner_filter / order_by 等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>命中总数（total）、下一页偏移（next_offset）与按群主聚合的记录列表（items）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92133"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93476"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96358"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/statistic")]
    Task<GetGroupChatStatisticResponse> GetGroupChatStatisticAsync(
        [Body] GetGroupChatStatisticRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取「群聊数据统计」（按自然日聚合）
    /// <para>按自然日维度获取客户群数据统计（字段与按群主聚合完全相同）。</para>
    /// <para>查询区间为闭区间，最大跨度 30 天，数据最多保留 180 天；
    /// 不指定群主过滤时取应用可见范围内全部群主（可见范围超 1000 人报错 81017）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupChatStatisticGroupByDayRequest"/>：day_begin_time / day_end_time / owner_filter）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按自然日聚合的记录列表（items：stat_time + data）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92133"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93476"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96358"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/statistic_group_by_day")]
    Task<GetGroupChatStatisticGroupByDayResponse> GetGroupChatStatisticGroupByDayAsync(
        [Body] GetGroupChatStatisticGroupByDayRequest request,
        CancellationToken cancellationToken = default);
}
