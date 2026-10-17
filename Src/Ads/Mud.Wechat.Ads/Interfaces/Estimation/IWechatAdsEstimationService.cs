// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Estimation;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「广告预估」域 SDK（<c>estimation</c>，1 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-17）：预估覆盖人数
/// <see href="https://developers.e.qq.com/v3.0/docs/api/estimation/get"/>。</para>
/// <para><b>所属权限</b>：官方原文 <c>ads_management</c>（与 <c>funds</c> / <c>qualifications</c> 两域的
/// <c>account_management</c> 不同权限族 ⇒ 独立成域，不并入既有域）。
/// 权限不足由应答 <c>code</c> 表达，SDK 不做本地权限预判。</para>
/// <para><b>无 <c>[Token]</c>、无 <c>user_token</c></b>（守卫 ADS-B1 / ADS-B5）：全局参数
/// <c>access_token</c> + <c>timestamp</c> + <c>nonce</c> 由 <c>AdsAuthorizationHandler</c> 成组注入；
/// 本页<b>没有</b>「特定请求参数」表，也没有请求头参数表 ⇒ 不带 <c>user_token</c>、不带 <c>X-Request-Id</c>。</para>
/// <para><b>「查询型 POST」</b>：官方动词为 <c>POST</c>、<c>Content-Type: application/json</c>，
/// 语义是只读预估、不产生任何投放资源 ⇒ 路由仍按本线口径写 <c>/v3.0/estimation/get</c>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Estimation", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsEstimationService
{
    /// <summary>
    /// 预估覆盖人数。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/estimation/get"/>。
    /// </summary>
    /// <param name="request">请求体（顶层仅 <c>account_id</c> 必填；定向/场景/载体子树复用
    /// <c>Adgroups</c> 域类型，条件必填与 ADX 禁用面见 <see cref="AdsEstimationGetRequest"/> remarks）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预估结果（<b>单结构体</b> <c>data</c>：覆盖/曝光区间 + 出价建议 + 分阶段出价建议），
    /// 见 <see cref="AdsEstimationGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/estimation/get</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>应答是区间与建议值</b>：<c>approximate_count</c> / <c>impression</c> 为「日最大值」，
    /// <c>min_users_daily</c>~<c>max_users_daily</c> 与 <c>min_exposure_daily</c>~<c>max_exposure_daily</c>
    /// 为区间上下限，<c>suggest_*</c> 为建议出价 ⇒ 都是预估值，不得当实际消耗/曝光使用。</para>
    /// </remarks>
    [Post("/v3.0/estimation/get")]
    Task<AdsEstimationGetResponse> GetEstimationAsync(
        AdsEstimationGetRequest request,
        CancellationToken cancellationToken = default);
}
