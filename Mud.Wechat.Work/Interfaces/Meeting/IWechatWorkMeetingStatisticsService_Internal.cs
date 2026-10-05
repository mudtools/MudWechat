// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块会议统计管理域企业自建应用 SDK（承载本域唯一端点「获取会议发起记录」）。
/// <para>
/// 官方仅向企业自建应用开放本域端点，端点全部声明于本接口；
/// 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），父接口见 <see cref="IWechatWorkMeetingStatisticsService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；会议发起者需要在应用可见范围内。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingStatisticsService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingStatisticsService : IWechatWorkMeetingStatisticsService
{
    /// <summary>
    /// 获取会议发起记录
    /// <para>获取企业内员工发起的会议记录及状态，包含员工主动发起快速会议以及作为首位参与者进入预约会议的场景。</para>
    /// <para>官方限制：查询时间跨度不能超过 30 天，查询区间左闭右开；记录按时间从大到小排序；
    /// 会过滤会议发起者不在应用可见范围中的记录，故返回记录数可能小于 limit；
    /// limit 默认 200、最大 1000。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingStartListRequest"/>：type / begin_time / end_time / limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分页游标（next_cursor）、是否还有数据待拉取（has_more）与发起成功或失败的记录列表（meeting_list：userid / start_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99651"/></para>
    /// <para>官方权限：会议发起者需要在应用可见范围内；自建应用需配置在「可调用接口的应用」列表中。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/statistics/get_start_list")]
    Task<GetMeetingStartListResponse> GetMeetingStartListAsync(
        [Body] GetMeetingStartListRequest request,
        CancellationToken cancellationToken = default);
}
