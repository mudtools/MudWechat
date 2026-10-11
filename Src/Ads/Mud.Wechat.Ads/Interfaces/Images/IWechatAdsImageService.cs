// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Images;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「图片素材」域 SDK —— 声明式三端点
/// （<c>images/get|update|delete</c>；<c>images/add</c> 为 <c>multipart/form-data</c> 文件上传，
/// 走手写通道 <see cref="IWechatAdsImageUploadService"/>，理由见该接口 remarks）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-11，层级经 DOM <c>level-*</c> 树核验）：
/// 获取图片 <see href="https://developers.e.qq.com/v3.0/docs/api/images/get"/>、
/// 修改图片描述 <see href="https://developers.e.qq.com/v3.0/docs/api/images/update"/>、
/// 删除图片 <see href="https://developers.e.qq.com/v3.0/docs/api/images/delete"/>。
/// 权限：<c>get</c> 为 <c>ads_management,account_management</c>，<c>update</c> / <c>delete</c> 为 <c>ads_management</c>。</para>
/// <para><b>无 <c>[Token]</c>、凭据在传输层成组注入</b>（守卫 ADS-B1）。本域页面均未另列 <c>user_token</c>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Images", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsImageService
{
    /// <summary>
    /// 获取图片。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/images/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>）。</param>
    /// <param name="organizationId">组织 id（Query <c>organization_id</c>，官方 <c>integer</c>）。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，JSON 数组字符串，用 <see cref="AdsQueryJson.Filtering"/> 构造）。</param>
    /// <param name="page">搜索页码（Query <c>page</c>，官方 <c>integer</c>）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>）。</param>
    /// <param name="labelId">标签 id（Query <c>label_id</c>，官方 <c>integer</c>）。</param>
    /// <param name="businessScenario">业务场景（Query <c>business_scenario</c>，官方 <c>integer</c>）。</param>
    /// <param name="needAigcFlag">是否返回 AIGC 标记（Query <c>need_aigc_flag</c>，官方 <c>boolean</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>图片列表（21 字段）+ 分页元信息。</returns>
    /// <remarks><b>官方契约</b>：<b>GET</b> <c>/v3.0/images/get</c>。</remarks>
    [Get("/v3.0/images/get")]
    Task<AdsImageGetResponse> GetAsync(
        [Query("account_id")] long? accountId = null,
        [Query("organization_id")] long? organizationId = null,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        [Query("label_id")] long? labelId = null,
        [Query("business_scenario")] long? businessScenario = null,
        [Query("need_aigc_flag")] bool? needAigcFlag = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改图片描述。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/images/update"/>。
    /// </summary>
    /// <param name="request">请求体（官方 4 键；必填 <c>image_id</c> / <c>description</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改结果（<c>data</c> 只回 <c>image_id</c>）。</returns>
    /// <remarks><b>官方契约</b>：<b>POST</b> <c>/v3.0/images/update</c>，<c>Content-Type: application/json</c>。</remarks>
    [Post("/v3.0/images/update")]
    Task<AdsImageUpdateResponse> UpdateAsync(
        AdsImageUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除图片。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/images/delete"/>。
    /// </summary>
    /// <param name="request">请求体（官方 3 键；必填 <c>image_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>删除结果（<c>data</c> 只回 <c>image_id</c>）。</returns>
    /// <remarks><b>官方契约</b>：<b>POST</b> <c>/v3.0/images/delete</c>，<c>Content-Type: application/json</c>。</remarks>
    [Post("/v3.0/images/delete")]
    Task<AdsImageDeleteResponse> DeleteAsync(
        AdsImageDeleteRequest request,
        CancellationToken cancellationToken = default);
}
