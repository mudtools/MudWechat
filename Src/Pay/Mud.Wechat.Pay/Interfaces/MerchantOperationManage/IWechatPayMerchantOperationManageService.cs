// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MerchantOperationManage;

namespace Mud.Wechat.Pay;

/// <summary>
/// 商户运营管控情况查询服务接口（1 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方开放面</b>：单页逐字段核验（更新于 2025.07.18），支持商户【普通服务商】。
/// </para>
/// <para>
/// <b>同前缀零声明守卫</b>：同前缀另有尽调族三支为【平台商户】专属 ⇒ 本包零声明（守卫 MOM-B2），
/// 正对照取本端点证明扫描机制生效。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MerchantOperationManage", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMerchantOperationManageService
{
    /// <summary>
    /// 查询子商户管控情况（<c>GET /v3/mch-operation-manage/merchant-limitations/sub-mchid/{sub_mchid}</c>）。
    /// </summary>
    /// <param name="subMchId">子商户号（path 参数）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>管控措施列表及总数。</returns>
    /// <remarks>
    /// <para><b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012803072">4012803072</see></para>
    /// <para><b>成功应答</b>：200 + JSON（含 <c>limitation_list</c> 数组）。</para>
    /// </remarks>
    [Get("/v3/mch-operation-manage/merchant-limitations/sub-mchid/{subMchId}")]
    Task<MerchantLimitationsQueryResponse> GetMerchantLimitationsAsync(
        [Path] string subMchId,
        CancellationToken cancellationToken = default);
}
