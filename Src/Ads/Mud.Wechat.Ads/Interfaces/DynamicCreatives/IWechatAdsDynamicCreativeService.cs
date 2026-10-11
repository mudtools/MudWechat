// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.DynamicCreatives;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「组件化创意」域 SDK（<c>dynamic_creatives</c>，4 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-11，层级经 DOM <c>level-*</c> 树核验）：
/// 获取组件化创意 <see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/get"/>、
/// 创建组件化创意 <see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/add"/>、
/// 更新组件化创意 <see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/update"/>、
/// 删除组件化创意 <see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/delete"/>。
/// 路由按官方「请求地址」原文写为 <c>/v3.0/{resource}/{action}</c>。</para>
/// <para><b>权限</b>：四端点「所属权限」均为 <c>ads_management</c>。</para>
/// <para><b>无 <c>[Token]</c>、凭据在传输层成组注入</b>（守卫 ADS-B1，理由见
/// <c>IWechatAdsAdvertiserService</c> 的 remarks）。</para>
/// <para><b>组件 <c>value</c> 为逐组件 union</b>：请求侧由调用方以 JSON 字面量构造
/// <see cref="AdsCreativeComponentItem.Value"/>（构造方式见该属性 remarks）；应答侧以开放字典承载、零丢字段。</para>
/// <para><b>受限接口的 <c>user_token</c></b>：<c>add</c> / <c>update</c> / <c>delete</c> 三页在「全局参数」
/// 之外另列 <c>user_token</c>（实名认证令牌，<b>属凭据</b> ⇒ 不得写进日志 / 遥测 / 异常消息；
/// 模块注册期已登记为强制掩码键，守卫 ADS-B5）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "DynamicCreatives", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsDynamicCreativeService
{
    /// <summary>
    /// 获取组件化创意。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方标 <c>integer</c>、<b>必填</b>）。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，官方 <c>struct[]</c>）。
    /// 值为 <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.Filtering"/> 构造；
    /// <c>field</c> / <c>operator</c> 可选值未逐项核验。</param>
    /// <param name="page">搜索页码（Query <c>page</c>，官方 <c>integer</c>；普通翻页模式使用）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>）。</param>
    /// <param name="fields">指定返回的字段列表（Query <c>fields</c>，官方 <c>string[]</c>）。
    /// 值为 <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.Fields"/> 构造。</param>
    /// <param name="isDeleted">是否已删除（Query <c>is_deleted</c>，官方 <c>boolean</c>）。</param>
    /// <param name="paginationMode">分页方式（Query <c>pagination_mode</c>，官方 <c>enum</c>；可选值未逐项核验）。</param>
    /// <param name="cursor">游标值（Query <c>cursor</c>，官方 <c>string</c>；游标模式使用）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创意列表 + 两种分页元信息，见 <see cref="AdsDynamicCreativeGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/dynamic_creatives/get</c>（参数进 Query）。</para>
    /// <para><b>双分页形态</b>：应答同时定义 <c>page_info</c>（普通翻页）与 <c>cursor_page_info</c>
    /// （游标翻页，<c>next_cursor</c> / <c>previous_cursor</c> 为字符串——与 <c>advertiser/get</c> 的
    /// 整数游标不同构，逐域分建），取哪组由请求的 <c>pagination_mode</c> 决定。</para>
    /// <para><b>本域端点计数</b>：官方清单 <c>dynamic_creatives/*</c> 恰有四端点，本接口全覆盖（守卫 ADS-B2）。</para>
    /// </remarks>
    [Get("/v3.0/dynamic_creatives/get")]
    Task<AdsDynamicCreativeGetResponse> GetAsync(
        [Query("account_id")] long accountId,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        [Query("fields")] string? fields = null,
        [Query("is_deleted")] bool? isDeleted = null,
        [Query("pagination_mode")] string? paginationMode = null,
        [Query("cursor")] string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建组件化创意。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/add"/>。
    /// </summary>
    /// <param name="request">请求体（官方顶层 16 键，必填 <c>account_id</c> / <c>adgroup_id</c> /
    /// <c>dynamic_creative_name</c> / <c>creative_components</c>）。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；<b>属凭据</b>，不得入日志）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建结果（<c>data</c> 只回 <c>dynamic_creative_id</c>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/dynamic_creatives/add</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>组件必填组合由创意形式决定</b>：<c>creative_components</c> 的 44 个组件键官方不逐个标必填，
    /// 实际必填组合随 <c>creative_template_id</c>（创意形式）变化（官方未在页面给出逐形式矩阵）⇒
    /// SDK 不做本地校验，缺失由应答 <c>code</c> 表达。</para>
    /// </remarks>
    [Post("/v3.0/dynamic_creatives/add")]
    Task<AdsDynamicCreativeAddResponse> AddAsync(
        AdsDynamicCreativeAddRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新组件化创意。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/update"/>。
    /// </summary>
    /// <param name="request">请求体（官方顶层 10 键；与 <c>add</c> 不同构——创建期事实字段不可更新，
    /// 多 <c>is_retry_batch_update</c>；<c>creative_components</c> 为增量更新语义）。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；<b>属凭据</b>，不得入日志）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>更新结果（<c>data</c> 只回 <c>dynamic_creative_id</c>）。</returns>
    /// <remarks><b>官方契约</b>：<b>POST</b> <c>/v3.0/dynamic_creatives/update</c>，<c>Content-Type: application/json</c>。</remarks>
    [Post("/v3.0/dynamic_creatives/update")]
    Task<AdsDynamicCreativeUpdateResponse> UpdateAsync(
        AdsDynamicCreativeUpdateRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除组件化创意。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/delete"/>。
    /// </summary>
    /// <param name="request">请求体（官方 2 键：<c>account_id</c> / <c>dynamic_creative_id</c>，均必填）。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；<b>属凭据</b>，不得入日志）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>删除结果（<c>data</c> 只回 <c>dynamic_creative_id</c>）。</returns>
    /// <remarks><b>官方契约</b>：<b>POST</b> <c>/v3.0/dynamic_creatives/delete</c>，<c>Content-Type: application/json</c>。</remarks>
    [Post("/v3.0/dynamic_creatives/delete")]
    Task<AdsDynamicCreativeDeleteResponse> DeleteAsync(
        AdsDynamicCreativeDeleteRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);
}
