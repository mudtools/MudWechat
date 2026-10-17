// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.CustomAudiences;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「人群管理」域 SDK（声明式面：
/// <c>custom_audiences/add|get|update</c> + <c>custom_audience_files/get</c>，4 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-17）：创建客户人群
/// <see href="https://developers.e.qq.com/v3.0/docs/api/custom_audiences/add"/>、查询客户人群
/// <see href="https://developers.e.qq.com/v3.0/docs/api/custom_audiences/get"/>、修改客户人群
/// <see href="https://developers.e.qq.com/v3.0/docs/api/custom_audiences/update"/>、获取人群数据文件
/// <see href="https://developers.e.qq.com/v3.0/docs/api/custom_audience_files/get"/>。
/// 官方 v3.0 目录树另有 <c>custom_audiences/delete</c>（本 SDK 暂未消费，随需求增补）。</para>
/// <para><b>所属权限</b>（五页同为官方「所属权限」原文）：
/// <c>audience_management,ads_management,user_actions</c>（三者任一）。</para>
/// <para><b>无 <c>[Token]</c></b>：全局参数 <c>access_token</c> + <c>timestamp</c> + <c>nonce</c>
/// 由 <c>AdsAuthorizationHandler</c> 成组注入（守卫 ADS-B1），五页均<b>未</b>另列 <c>user_token</c>。</para>
/// <para><b><c>custom_audience_files/add</c> 不在本接口</b>：官方该支 <c>Content-Type</c> 为
/// <c>multipart/form-data</c>（文件字段 <c>file</c>），声明式面承载不了文件流 ⇒ 走手写通道
/// <see cref="IWechatAdsCustomAudienceFileUploadService"/>（形态同素材两域的上传通道）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "CustomAudiences", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsCustomAudienceService
{
    /// <summary>
    /// 创建客户人群。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/custom_audiences/add"/>。
    /// </summary>
    /// <param name="request">请求体（必填 <c>account_id</c> / <c>name</c> / <c>type</c>；
    /// <c>type</c> 决定 <c>audience_spec</c> 内生效的子 spec，互斥语义见 <see cref="AdsAudienceSpec"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建结果（<c>data.audience_id</c>），见 <see cref="AdsCustomAudienceAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/custom_audiences/add</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>同帐号不许重名</b>（<c>name</c> 1–32 字节）；<c>outer_audience_id</c> 具幂等语义：
    /// 该编码创建过人群时，重复创建返回之前的人群 id（官方原文）。</para>
    /// </remarks>
    [Post("/v3.0/custom_audiences/add")]
    Task<AdsCustomAudienceAddResponse> AddAsync(
        AdsCustomAudienceAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询客户人群。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/custom_audiences/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>）。</param>
    /// <param name="audienceId">人群 id（Query <c>audience_id</c>，官方 <c>integer</c>，选填；不填即查全量分页）。</param>
    /// <param name="page">当前页码（Query <c>page</c>，最小 1，默认 1）。</param>
    /// <param name="pageSize">分页大小（Query <c>page_size</c>，最小 1、最大 100，默认 10）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>人群列表（<c>data.list[]</c> + <c>page_info</c>），见 <see cref="AdsCustomAudienceGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/custom_audiences/get</c>。</para>
    /// <para><b>条目字段的条件出现</b>：<c>online_status</c> 仅在处理状态为成功时返回和生效；
    /// <c>audience_spec</c> 仅回显与 <c>type</c> 相关的子 spec。</para>
    /// </remarks>
    [Get("/v3.0/custom_audiences/get")]
    Task<AdsCustomAudienceGetResponse> GetAsync(
        [Query("account_id")] long accountId,
        [Query("audience_id")] long? audienceId = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改客户人群。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/custom_audiences/update"/>。
    /// </summary>
    /// <param name="request">请求体（必填 <c>account_id</c> / <c>audience_id</c>；
    /// 官方约束「人群名称、人群描述、是否深度合作至少填写一个」，SDK 不本地拦截）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>更新结果（官方 <c>data</c> 为空载荷），见 <see cref="AdsCustomAudienceUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/custom_audiences/update</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b><c>cooperated</c> 仅创建者可改</b>（官方原文）；本页 <c>description</c> 上限 100 字节，
    /// 与 add 页的 250 字节<b>不同构</b>，勿按 add 页口径构造超长描述。</para>
    /// </remarks>
    [Post("/v3.0/custom_audiences/update")]
    Task<AdsCustomAudienceUpdateResponse> UpdateAsync(
        AdsCustomAudienceUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取人群数据文件列表。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/custom_audience_files/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>）。</param>
    /// <param name="audienceId">人群 id（Query <c>audience_id</c>；官方原文：<b>只能是 CUSTOMER_FILE 类人群</b>）。</param>
    /// <param name="customAudienceFileId">数据文件 id（Query <c>custom_audience_file_id</c>；经上传通道
    /// <see cref="IWechatAdsCustomAudienceFileUploadService"/> 获得）。</param>
    /// <param name="page">当前页码（Query <c>page</c>，最小 1，默认 1）。</param>
    /// <param name="pageSize">分页大小（Query <c>page_size</c>，最小 1、最大 100，默认 10）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>数据文件列表（<c>data.list[]</c> + <c>page_info</c>），见 <see cref="AdsCustomAudienceFileGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/custom_audience_files/get</c>。</para>
    /// <para><b>双层判定</b>：文件处理结果看条目的 <c>process_code</c>（0 成功）与 <c>process_status</c>，
    /// 外层信封 <c>code == 0</c> 只表示查询成功。</para>
    /// </remarks>
    [Get("/v3.0/custom_audience_files/get")]
    Task<AdsCustomAudienceFileGetResponse> GetFilesAsync(
        [Query("account_id")] long accountId,
        [Query("audience_id")] long? audienceId = null,
        [Query("custom_audience_file_id")] long? customAudienceFileId = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        CancellationToken cancellationToken = default);
}
