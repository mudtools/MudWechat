// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块高级功能账号管理域企业自建应用 SDK（承载本域全部 5 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingPremiumAccountService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中（代开发应用、第三方应用暂不支持）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingPremiumAccountService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingPremiumAccountService : IWechatWorkMeetingPremiumAccountService
{
    /// <summary>
    /// 分配高级功能账号
    /// <para>分配应用可见范围企业成员的高级功能。</para>
    /// <para>官方限制：userid_list 单次操作最大限制 100 个；自建应用需配置到「协作 - 会议 - 可调用接口的应用」中。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AssignPremiumAccountsRequest"/>：userid_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量分配高级功能的任务 ID（jobid，可用于查询分配结果）与非法的 userid 列表（invalid_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99508"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中；代开发应用、第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/vip/submit_batch_add_job")]
    Task<AssignPremiumAccountsResponse> AssignPremiumAccountsAsync(
        [Body] AssignPremiumAccountsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询分配高级功能账号结果
    /// <para>查询批量分配高级功能任务的执行结果。</para>
    /// <para>官方限制：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetAssignPremiumAccountsResultRequest"/>：jobid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>执行任务结果详情（job_result：succ_userid_list / fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99508"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中；代开发应用、第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/vip/batch_add_job_result")]
    Task<GetAssignPremiumAccountsResultResponse> GetAssignPremiumAccountsResultAsync(
        [Body] GetAssignPremiumAccountsResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消高级功能账号
    /// <para>撤销分配应用可见范围企业成员的高级功能。</para>
    /// <para>官方限制：userid_list 单次操作最多限制 100 个；自建应用需配置到「协作 - 会议 - 可调用接口的应用」中。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="RevokePremiumAccountsRequest"/>：userid_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量取消高级功能的任务 ID（jobid，可用于查询取消结果）与非法的 userid 列表（invalid_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99509"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中；代开发应用、第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/vip/submit_batch_del_job")]
    Task<RevokePremiumAccountsResponse> RevokePremiumAccountsAsync(
        [Body] RevokePremiumAccountsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询取消高级功能账号结果
    /// <para>查询批量取消高级功能任务的执行结果。</para>
    /// <para>官方限制：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetRevokePremiumAccountsResultRequest"/>：jobid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>执行任务结果详情（job_result：succ_userid_list / fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99509"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中；代开发应用、第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/vip/batch_del_job_result")]
    Task<GetRevokePremiumAccountsResultResponse> GetRevokePremiumAccountsResultAsync(
        [Body] GetRevokePremiumAccountsResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高级功能账号列表
    /// <para>查询企业已分配高级功能且在应用可见范围的账号列表。</para>
    /// <para>官方限制：limit 默认 100、最大 200，不保证每次返回的数据刚好为指定 limit，必须用返回的 <c>has_more</c> 判断是否继续请求；
    /// 自建应用需配置到「协作 - 会议 - 可调用接口的应用」中。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListPremiumAccountsRequest"/>：cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多数据未获取（has_more）、下一次请求的 cursor 值（next_cursor）与符合条件的企业成员 userid 列表（userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99510"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中；代开发应用、第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/vip/list")]
    Task<ListPremiumAccountsResponse> ListPremiumAccountsAsync(
        [Body] ListPremiumAccountsRequest request,
        CancellationToken cancellationToken = default);
}
