// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Security;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「高级功能账号管理」域企业自建应用 SDK：官方仅向自建应用开放本域端点
/// （分配 / 取消高级功能账号及其异步任务结果查询、获取已分配账号列表），全部声明于本接口。
/// <para>第三方应用与服务商代开发官方无对应文档，不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 需配置到「安全与管理 - 可调用接口的应用」中；仅可操作应用可见范围内的企业成员，
/// userid_list 单次最多 100 个成员；分配与取消均为<b>异步任务</b>，需以返回的 jobid 查询执行结果。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Security",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSecurityVipService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalSecurityVipService : IWechatWorkSecurityVipService
{
    /// <summary>
    /// 分配高级功能账号
    /// <para>为应用可见范围内的企业成员批量分配高级功能（异步任务）；userid_list 单次最多 100 个。</para>
    /// </summary>
    /// <param name="request">分配请求体（<see cref="SubmitBatchAddVipJobRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务 id 与非法 userid 列表（jobid / invalid_userid_list）。</returns>
    /// <remarks>
    /// <para>分配结果经 <see cref="GetBatchAddVipJobResultAsync"/> 以 jobid 查询。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99503"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/vip/submit_batch_add_job")]
    Task<SubmitBatchAddVipJobResponse> SubmitBatchAddVipJobAsync(
        [Body] SubmitBatchAddVipJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询分配高级功能账号结果
    /// <para>根据 jobid 查询批量分配高级功能账号任务的执行结果。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetBatchAddVipJobRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务执行结果（job_result：succ_userid_list / fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99503"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/vip/batch_add_job_result")]
    Task<GetBatchAddVipJobResultResponse> GetBatchAddVipJobResultAsync(
        [Body] GetBatchAddVipJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消高级功能账号
    /// <para>撤销分配给应用可见范围内企业成员的高级功能（异步任务）；userid_list 单次最多 100 个。</para>
    /// </summary>
    /// <param name="request">取消请求体（<see cref="SubmitBatchDelVipJobRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务 id 与非法 userid 列表（jobid / invalid_userid_list）。</returns>
    /// <remarks>
    /// <para>取消结果经 <see cref="GetBatchDelVipJobResultAsync"/> 以 jobid 查询。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99505"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/vip/submit_batch_del_job")]
    Task<SubmitBatchDelVipJobResponse> SubmitBatchDelVipJobAsync(
        [Body] SubmitBatchDelVipJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询取消高级功能账号结果
    /// <para>根据 jobid 查询批量取消高级功能账号任务的执行结果。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetBatchDelVipJobRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务执行结果（job_result：succ_userid_list / fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99505"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/vip/batch_del_job_result")]
    Task<GetBatchDelVipJobResultResponse> GetBatchDelVipJobResultAsync(
        [Body] GetBatchDelVipJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高级功能账号列表
    /// <para>查询企业已分配高级功能且在应用可见范围内的账号列表（分页）。</para>
    /// </summary>
    /// <param name="request">分页请求体（<see cref="ListVipAccountsRequest"/>；limit 默认 100 最大 200）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已分配账号分页结果（has_more / next_cursor / userid_list）。</returns>
    /// <remarks>
    /// <para>官方提示不保证每次返回条数恰为 limit，须以 has_more 判断是否继续请求。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99506"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/vip/list")]
    Task<ListVipAccountsResponse> ListVipAccountsAsync(
        [Body] ListVipAccountsRequest request,
        CancellationToken cancellationToken = default);
}
