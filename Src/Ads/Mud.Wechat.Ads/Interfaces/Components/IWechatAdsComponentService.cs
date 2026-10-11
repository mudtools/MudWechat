// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Components;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「创意组件」域 SDK（<c>components</c> + <c>component_detail</c>，4 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-11，层级经 DOM <c>level-*</c> 树核验）：
/// 获取创意组件 <see href="https://developers.e.qq.com/v3.0/docs/api/components/get"/>、
/// 创建创意组件 <see href="https://developers.e.qq.com/v3.0/docs/api/components/add"/>、
/// 删除创意组件 <see href="https://developers.e.qq.com/v3.0/docs/api/components/delete"/>、
/// 获取组件详细信息 <see href="https://developers.e.qq.com/v3.0/docs/api/component_detail/get"/>。</para>
/// <para><b>权限</b>：四端点「所属权限」均为 <c>ads_management</c>。</para>
/// <para><b>无 <c>[Token]</c>、凭据在传输层成组注入</b>（守卫 ADS-B1）。</para>
/// <para><b>组件值两容器形态</b>：<c>components</c> 的 <c>component_value</c> 是 40 个组件键的<b>单数</b>
/// struct（与 <c>dynamic_creatives</c> 的数组形态同名不同构，见 <see cref="AdsComponentValue"/> remarks）；
/// 组件条目元素（<c>{component_id, value, is_deleted}</c>）与创意域同构 ⇒ 复用
/// <see cref="Mud.Wechat.Ads.DataModels.DynamicCreatives.AdsCreativeComponentItem"/>。</para>
/// <para><b>受限接口的 <c>user_token</c></b>：仅 <c>add</c> 一页另列（<c>delete</c> / 两支 get 页均无）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Components", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsComponentService
{
    /// <summary>
    /// 获取创意组件。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/components/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>）。</param>
    /// <param name="organizationId">组织 id（Query <c>organization_id</c>，官方 <c>integer</c>）。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，JSON 数组字符串，用 <see cref="AdsQueryJson.Filtering"/> 构造）。</param>
    /// <param name="page">搜索页码（Query <c>page</c>，官方 <c>integer</c>）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>）。</param>
    /// <param name="isDeleted">是否已删除（Query <c>is_deleted</c>，官方 <c>boolean</c>）。</param>
    /// <param name="fields">指定返回的字段列表（Query <c>fields</c>，JSON 数组字符串，用 <see cref="AdsQueryJson.Fields"/> 构造）。</param>
    /// <param name="componentIdFilteringMode">组件 id 过滤模式（Query <c>component_id_filtering_mode</c>，官方 <c>enum</c>；可选值未逐项核验）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>组件列表（<c>component_value</c> 40 组件单数键）+ 分页元信息。</returns>
    /// <remarks><b>官方契约</b>：<b>GET</b> <c>/v3.0/components/get</c>。</remarks>
    [Get("/v3.0/components/get")]
    Task<AdsComponentGetResponse> GetAsync(
        [Query("account_id")] long? accountId = null,
        [Query("organization_id")] long? organizationId = null,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        [Query("is_deleted")] bool? isDeleted = null,
        [Query("fields")] string? fields = null,
        [Query("component_id_filtering_mode")] string? componentIdFilteringMode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建创意组件。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/components/add"/>。
    /// </summary>
    /// <param name="request">请求体（官方 5 键；必填 <c>component_value</c> / <c>component_sub_type</c>）。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；<b>属凭据</b>，不得入日志）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建结果（<c>data</c> 只回 <c>component_id</c>）。</returns>
    /// <remarks><b>官方契约</b>：<b>POST</b> <c>/v3.0/components/add</c>，<c>Content-Type: application/json</c>。</remarks>
    [Post("/v3.0/components/add")]
    Task<AdsComponentAddResponse> AddAsync(
        AdsComponentAddRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除创意组件。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/components/delete"/>。
    /// </summary>
    /// <param name="request">请求体（官方 4 键；必填 <c>component_id</c>；
    /// <c>delete_strategy</c> 可选值 <c>{DELETE_STRATEGY_FORCE, DELETE_STRATEGY_RESTRICTED}</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>删除结果（<c>data</c> 只回 <c>component_id</c>）。</returns>
    /// <remarks><b>官方契约</b>：<b>POST</b> <c>/v3.0/components/delete</c>，<c>Content-Type: application/json</c>。</remarks>
    [Post("/v3.0/components/delete")]
    Task<AdsComponentDeleteResponse> DeleteAsync(
        AdsComponentDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取组件详细信息。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/component_detail/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>）。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，JSON 数组字符串，用 <see cref="AdsQueryJson.Filtering"/> 构造）。</param>
    /// <param name="page">搜索页码（Query <c>page</c>，官方 <c>integer</c>）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>）。</param>
    /// <param name="organizationId">组织 id（Query <c>organization_id</c>，官方 <c>integer</c>）。</param>
    /// <param name="adContext">广告上下文（Query <c>ad_context</c>，官方 <c>struct</c>，3 层深；
    /// 必填 <c>marketing_goal</c> / <c>marketing_carrier_type</c> / <c>marketing_target_type</c> /
    /// <c>site_set</c> / <c>creative_template_id</c>）。值为 <b>JSON 对象字符串</b>——
    /// 声明式客户端会把复杂类型逐属性展平（非官方线格式）⇒ 以 <see cref="string"/> 承载、调用方自构。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>组件详情列表（<c>component_detail</c> 4 键）+ 分页元信息。</returns>
    /// <remarks><b>官方契约</b>：<b>GET</b> <c>/v3.0/component_detail/get</c>。</remarks>
    [Get("/v3.0/component_detail/get")]
    Task<AdsComponentDetailGetResponse> GetDetailAsync(
        [Query("account_id")] long? accountId = null,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        [Query("organization_id")] long? organizationId = null,
        [Query("ad_context")] string? adContext = null,
        CancellationToken cancellationToken = default);
}
