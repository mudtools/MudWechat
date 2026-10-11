// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.AsyncTasks;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「异步任务」域 SDK（<c>async_tasks</c>，2 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-11，层级经 DOM <c>level-*</c> 树核验）：
/// 创建异步任务 <see href="https://developers.e.qq.com/v3.0/docs/api/async_tasks/add"/>、
/// 获取异步任务 <see href="https://developers.e.qq.com/v3.0/docs/api/async_tasks/get"/>。
/// 权限：两页「所属权限」均为 <c>ads_management</c>。</para>
/// <para><b>无 <c>[Token]</c>、凭据在传输层成组注入</b>（守卫 ADS-B1）。两页均未另列 <c>user_token</c>。</para>
/// <para><b>双层失败语义</b>：<c>get</c> 应答的 <c>result.code</c> 才是<b>任务执行</b>结果
/// （外层信封 <c>code</c> 只表示「查询被受理」）—— 与批量族同款两层判定面；
/// 且 <c>result</c> 层无 <c>message_cn</c>（形态同 <c>async_reports/get</c>，见报表域守卫）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "AsyncTasks", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsAsyncTaskService
{
    /// <summary>
    /// 创建异步任务。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/async_tasks/add"/>。
    /// </summary>
    /// <param name="request">请求体（官方 4 键；必填 <c>account_id</c> / <c>task_name</c> / <c>task_type</c>；
    /// <c>task_spec</c> 三支互斥 spec 按 <c>task_type</c> 生效其一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建结果（<c>data</c> 只回 <c>task_id</c>）。</returns>
    /// <remarks><b>官方契约</b>：<b>POST</b> <c>/v3.0/async_tasks/add</c>，<c>Content-Type: application/json</c>。</remarks>
    [Post("/v3.0/async_tasks/add")]
    Task<AdsAsyncTaskAddResponse> AddAsync(
        AdsAsyncTaskAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取异步任务。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/async_tasks/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>）。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，JSON 数组字符串，用 <see cref="AdsQueryJson.Filtering"/> 构造）。</param>
    /// <param name="page">搜索页码（Query <c>page</c>，官方 <c>integer</c>）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>任务列表（<c>result</c> 为任务执行结果，双层判定）+ 分页元信息。</returns>
    /// <remarks><b>官方契约</b>：<b>GET</b> <c>/v3.0/async_tasks/get</c>。</remarks>
    [Get("/v3.0/async_tasks/get")]
    Task<AdsAsyncTaskGetResponse> GetAsync(
        [Query("account_id")] long accountId,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        CancellationToken cancellationToken = default);
}
