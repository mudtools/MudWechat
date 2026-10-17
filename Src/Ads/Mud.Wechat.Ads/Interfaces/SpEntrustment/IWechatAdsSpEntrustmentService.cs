// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.SpEntrustment;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「服务商授权」域 SDK（<c>sp_entrustment</c>，2 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-16）：添加授权 <see href="https://developers.e.qq.com/v3.0/docs/api/sp_entrustment/add"/>、
/// 查询授权 <see href="https://developers.e.qq.com/v3.0/docs/api/sp_entrustment/get"/>。
/// 请求域 <c>https://api.e.qq.com/v3.0/</c>。</para>
/// <para><b>无 <c>[Token]</c></b>：全局参数 <c>access_token</c> + <c>timestamp</c> + <c>nonce</c>
/// 由 <c>AdsAuthorizationHandler</c> 成组注入，方法签名不出现这三者（同广告线其他接口）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "SpEntrustment", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsSpEntrustmentService
{
    /// <summary>
    /// 添加服务商授权关系。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/sp_entrustment/add"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅标准信封（code/message/message_cn），成功时 code=0。</returns>
    /// <remarks>
    /// <para><b>官方约束</b>：本接口用于建立服务商与广告主的授权关系，调用方需具备相应权限。</para>
    /// </remarks>
    [Post("/v3.0/sp_entrustment/add")]
    Task<AdsSpEntrustmentAddResponse> AddSpEntrustmentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询服务商授权关系。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/sp_entrustment/get"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权关系信息（data.entrusted 表示是否有授权）。</returns>
    /// <remarks>
    /// <para><b>官方约束</b>：返回当前调用方与指定广告主的授权状态。</para>
    /// </remarks>
    [Get("/v3.0/sp_entrustment/get")]
    Task<AdsSpEntrustmentGetResponse> GetSpEntrustmentAsync(CancellationToken cancellationToken = default);
}
