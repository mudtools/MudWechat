// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.TargetingTags;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「定向标签」域 SDK（<c>targeting_tags</c>，1 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-17）：获取定向标签
/// <see href="https://developers.e.qq.com/v3.0/docs/api/targeting_tags/get"/>。</para>
/// <para><b>所属权限</b>：官方原文 <c>ads_management</c>（与 <see cref="IWechatAdsEstimationService"/> 同权限族；
/// 与 <c>funds</c> / <c>qualifications</c> 两域的 <c>account_management</c> 不同）。
/// 权限不足由应答 <c>code</c> 表达，SDK 不做本地权限预判。</para>
/// <para><b>无 <c>[Token]</c>、无 <c>user_token</c></b>（守卫 ADS-B1 / ADS-B5）：全局参数
/// <c>access_token</c> + <c>timestamp</c> + <c>nonce</c> 由 <c>AdsAuthorizationHandler</c> 成组注入；
/// 本页<b>没有</b>「特定请求参数」表 ⇒ 不带 <c>user_token</c>。</para>
/// <para><b>本域是投放定向取值的供给侧</b>：<c>adgroups</c> 定向的 <c>regions</c> /
/// <c>business_districts</c> / <c>device_brand_model</c> 列表取值官方原文均标注
/// 「可通过 targeting_tags/get 接口获取」⇒ 应答 <c>id</c> 回填那些参数。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "TargetingTags", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsTargetingTagService
{
    /// <summary>
    /// 获取定向标签。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/targeting_tags/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>；
    /// 原文「有操作权限的帐号 id，不支持代理商 id」）。</param>
    /// <param name="type">标签类型（Query <c>type</c>，官方 <c>enum</c>、<b>必填</b>。本页「枚举详情」
    /// <b>未展开取值全集</b> ⇒ 不本地枚举；正文与示例可证得的取值有 <c>BEHAVIOR</c>（行为，此时
    /// <c>tag_spec.behavior_spec</c> 必填）、<c>INTEREST</c>（兴趣，此时 <c>tag_spec.interest_spec</c>
    /// 必填）、<c>REGION</c>（地域，示例形态不带 tag_spec））。</param>
    /// <param name="tagSpec">获取定向标签的条件（Query <c>tag_spec</c>，官方 <c>struct</c>，选填）。值为
    /// <b>JSON 对象字符串</b>，构造入口唯一 <see cref="AdsQueryJson.TagSpec"/>；官方 curl 原文
    /// <c>-d 'tag_spec={"interest_spec":{"query_spec":{}}}'</c>。内部条件必填链
    /// （<c>query_mode</c> 必填、三种模式下 <c>query_spec</c> 必填、ADVANCED_RECOMMEND 下
    /// <c>advanced_recommend_type</c> 必填）见 <see cref="AdsTagQuerySpec"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>标签列表（<c>data.list[]</c>，<b>无分页元信息</b>），见
    /// <see cref="AdsTargetingTagsGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/targeting_tags/get</c>。</para>
    /// <para><b>应答是树形态</b>：<c>parent_id</c> / <c>parent_name</c> 组织父子（根节点官方示例
    /// <c>parent_id=0</c>、<c>parent_name=""</c> 空串而非缺省）⇒ 组树以 <c>id</c> 为键，勿以名称为键。</para>
    /// </remarks>
    [Get("/v3.0/targeting_tags/get")]
    Task<AdsTargetingTagsGetResponse> GetAsync(
        [Query("account_id")] long accountId,
        [Query("type")] string type,
        [Query("tag_spec")] string? tagSpec = null,
        CancellationToken cancellationToken = default);
}
