// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.Qualifications;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「客户资质」域 SDK（<c>qualifications</c>，4 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-17 / 2026-10-18）：获取客户资质
/// <see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/get"/>、创建客户资质
/// <see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/add"/>、更新客户资质
/// <see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/update"/>、删除客户资质
/// <see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/delete"/>。</para>
/// <para><b>所属权限</b>（三页「所属权限」均为官方原文 <c>account_management</c>）。
/// 权限不足由应答 <c>code</c> 表达，SDK 不做本地权限预判。</para>
/// <para><b>无 <c>[Token]</c>、无 <c>user_token</c></b>（守卫 ADS-B1 / ADS-B5）：全局参数
/// <c>access_token</c> + <c>timestamp</c> + <c>nonce</c> 由 <c>AdsAuthorizationHandler</c> 成组注入；
/// 三页均<b>没有</b>「特定请求参数」表 ⇒ 一个 <c>user_token</c> 都不带。</para>
/// <para><b>三页共同口径</b>：<c>account_id</c> 官方原文「<b>不支持代理商 id</b>」（与
/// <c>funds</c> 域「包括代理商」口径相反）；<c>qualification_type</c> 三支可选值三页同一
/// <c>{ INDUSTRY_QUALIFICATION, AD_QUALIFICATION, ADDITIONAL_INDUSTRY_QUALIFICATION }</c>。</para>
/// <para><b>扩展字段不可本地枚举</b>：<c>expand_field_list[]</c> 的 <c>field_name</c> / <c>value</c>
/// 合法组合由官方「获取客户资质结构接口」（<c>qualification_structure/get</c>，本 SDK 未消费）动态返回 ⇒
/// SDK 不校验、不拦截，越界由应答 <c>code</c> 表达。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Qualifications", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsQualificationService
{
    /// <summary>
    /// 获取客户资质。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>；
    /// 官方原文「不支持代理商 id」）。</param>
    /// <param name="qualificationType">资质类型（Query <c>qualification_type</c>，官方 <c>enum</c>、<b>必填</b>）：
    /// 可选值 <c>{ INDUSTRY_QUALIFICATION, AD_QUALIFICATION, ADDITIONAL_INDUSTRY_QUALIFICATION }</c>。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，官方 <c>struct[]</c>，选填）。值为
    /// <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.Filtering"/> 构造；官方原文「若此字段不传，
    /// 或传空则视为无限制条件」。<b>本页官方限制</b>：数组长度为 1；<c>field</c> 可选值
    /// <c>{ qualification_id }</c>；<c>operator</c> 当 <c>field</c> 取 <c>qualification_id</c> 时可选值
    /// <c>{ EQUALS, IN }</c>；<c>values</c>：<c>IN</c> 时数组 1–100、<c>EQUALS</c> 时数组长度为 1。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>三类资质列表（<c>industry_qualifications</c> / <c>ad_qualifications</c> /
    /// <c>additional_industry_qualifications</c>，<b>无分页元信息</b>），见
    /// <see cref="AdsQualificationsGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/qualifications/get</c>。</para>
    /// <para><b>本域查询无翻页</b>：应答没有 <c>page_info</c>，与 <c>custom_audiences/get</c> 等分页域不同 ⇒
    /// 全量一次返回。</para>
    /// </remarks>
    [Get("/v3.0/qualifications/get")]
    Task<AdsQualificationsGetResponse> GetAsync(
        [Query("account_id")] long accountId,
        [Query("qualification_type")] string qualificationType,
        [Query("filtering")] string? filtering = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建客户资质。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/add"/>。
    /// </summary>
    /// <param name="request">请求体（必填 <c>account_id</c> / <c>qualification_type</c> /
    /// <c>qualification_spec</c>；三支互斥 spec 见 <see cref="AdsQualificationSpec"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建结果（<c>data.qualification_id</c>），见 <see cref="AdsQualificationAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/qualifications/add</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>类型与 spec 的「当且仅当」互斥</b>（原文逐支标注「当且仅当 qualification_type=X 时可填且必填」）
    /// ⇒ 填错/漏装配会被官方以应答 <c>code</c> 拒绝，SDK 不本地拦截。</para>
    /// </remarks>
    [Post("/v3.0/qualifications/add")]
    Task<AdsQualificationAddResponse> AddAsync(
        AdsQualificationAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新客户资质。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/update"/>。
    /// </summary>
    /// <param name="request">请求体（必填 <c>account_id</c> / <c>qualification_type</c> /
    /// <c>qualification_id</c> / <c>image_id_list</c>；<c>expand_field_list</c> 非 null 时全量更新）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>更新结果（<c>data.qualification_id</c>），见 <see cref="AdsQualificationUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/qualifications/update</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>更新面窄于创建面</b>：本页参数表没有行业 id / 业务范围 / 资质编码（即这些键创建后不可改），
    /// 只有 <c>image_id_list</c>（<b>必填即整体替换</b>）与 <c>expand_field_list</c>（「非 null 时全量更新」）。</para>
    /// </remarks>
    [Post("/v3.0/qualifications/update")]
    Task<AdsQualificationUpdateResponse> UpdateAsync(
        AdsQualificationUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除客户资质。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/delete"/>。
    /// </summary>
    /// <param name="request">请求体（必填 <c>account_id</c> / <c>qualification_type</c>；
    /// <c>qualification_id</c> 删单个、<c>qualification_id_list</c> 批量删，两者官方均标选填），
    /// 见 <see cref="AdsQualificationDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>被删除的资质 id / id 列表，见 <see cref="AdsQualificationDeleteResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/qualifications/delete</c>，<c>Content-Type: application/json</c>；
    /// 所属权限 <c>account_management</c>（与 get / add / update 同权限）。</para>
    /// <para><b>动词是 POST 而非 DELETE</b>：资源段 <c>qualifications</c> + 动作段 <c>delete</c> ⇒
    /// 不要按 REST 直觉改成 <c>DELETE /v3.0/qualifications</c>。</para>
    /// <para><b>两个删除入口</b>：<c>qualification_id</c>（单个）与 <c>qualification_id_list</c>（批量）
    /// 官方参数表均标选填、未印互斥约束 ⇒ SDK 不本地拦截，错误组合由应答 <c>code</c> 表达。</para>
    /// <para><b>无 <c>user_token</c></b>：本页与 get / add / update 三页一致，没有「特定请求参数」表。</para>
    /// </remarks>
    [Post("/v3.0/qualifications/delete")]
    Task<AdsQualificationDeleteResponse> DeleteAsync(
        AdsQualificationDeleteRequest request,
        CancellationToken cancellationToken = default);
}
