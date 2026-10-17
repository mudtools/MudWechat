// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.CustomAudiences;

namespace Mud.Wechat.Ads;

/// <summary>
/// 人群数据文件上传通道（<c>POST /v3.0/custom_audience_files/add</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不在 <see cref="IWechatAdsCustomAudienceService"/> 里</b>：官方本支
/// <c>Content-Type</c> 为 <c>multipart/form-data</c>（文件字段 <c>file</c>），
/// 声明式 <c>[HttpClientApi]</c> 面承载不了文件流 ⇒ 走手写
/// <see cref="Mud.HttpUtils.Clients.IEnhancedHttpClient"/> 传输（形态照素材两域的
/// <c>AdsMaterialUploadService</c>）：注入带令牌 Handler 的 <see cref="IAdsHttpClient"/>，
/// <c>access_token</c> / <c>timestamp</c> / <c>nonce</c> 由传输层成组注入（守卫 ADS-B1 语义不变）。
/// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/custom_audience_files/add"/>（2026-10-17 核验；
/// 本页未另列 <c>user_token</c>）。
/// </para>
/// <para><b>文件约束</b>（官方原文）：txt 或 csv 文件、每行一个号码，或文件压缩后的 zip 包，
/// <b>压缩后的 zip 包不超过 100M</b>；<c>audience_id</c> 只能是 CUSTOMER_FILE 类人群。
/// 大小判定不依赖本地拦截（流长度语义因流类型而异），越界由应答 <c>code</c> 表达。</para>
/// </remarks>
public interface IWechatAdsCustomAudienceFileUploadService
{
    /// <summary>上传人群数据文件（号码包）。</summary>
    /// <param name="accountId">账户 id（表单字段 <c>account_id</c>，必填）。</param>
    /// <param name="audienceId">人群 id（表单字段 <c>audience_id</c>，必填；只能是 CUSTOMER_FILE 类人群）。</param>
    /// <param name="userIdType">号码包用户 id 类型（表单字段 <c>user_id_type</c>，必填，<c>enum</c>；可选值
    /// <c>{ HASH_IDFA, HASH_IMEI, HASH_MOBILE_PHONE, IDFA, IMEI, WX_OPENID, WX_UNIONID, WECHAT_OPENID,
    /// SALTED_HASH_IMEI, SALTED_HASH_IDFA, OAID, HASH_OAID, SHA256_MOBILE_PHONE, MD5_SHA256_IMEI,
    /// MD5_SHA256_IDFA, MD5_SHA256_OAID, CAID }</c>）。</param>
    /// <param name="file">号码包文件流（表单字段 <c>file</c>，必填；txt/csv 每行一个号码或压缩后的 zip 包；
    /// 本方法<b>不</b>关闭调用方传入的流）。</param>
    /// <param name="fileName">文件名（<c>file</c> 部分的 filename；必填）。</param>
    /// <param name="operationType">文件操作类型（表单字段 <c>operation_type</c>，选填，<c>enum</c>；
    /// 不填默认 <c>APPEND</c>；可选值 <c>{ APPEND, REDUCE }</c>）。</param>
    /// <param name="openAppId">微信 appid（表单字段 <c>open_app_id</c>，选填；官方原文：当且仅当数据文件中的
    /// 用户 id 类型为 WECHAT_OPENID（WX_OPENID）时有效）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>上传结果（<c>data.custom_audience_file_id</c>，数据文件 id）。</returns>
    Task<AdsCustomAudienceFileUploadResponse> UploadAsync(
        long accountId,
        long audienceId,
        string userIdType,
        Stream file,
        string fileName,
        string? operationType = null,
        string? openAppId = null,
        CancellationToken cancellationToken = default);
}
