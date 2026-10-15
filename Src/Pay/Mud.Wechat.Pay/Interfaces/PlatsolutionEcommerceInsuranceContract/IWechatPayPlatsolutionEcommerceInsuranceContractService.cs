// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceContract;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 - InsuranceContract」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceInsuranceContractService
{
    /// <summary>
    /// 开通保险理赔功能。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012781476"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/insurance-compensation-contracts")]
    Task<InsuranceContractResponse> CreateInsuranceContractAsync(
        [Body] InsuranceContractRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询保险理赔功能开通状态。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012781480"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/insurance-compensation-contracts/sub-mchid/{sub_mchid}/check-opened")]
    Task<InsuranceContractResponse> CheckInsuranceContractOpenedAsync(
        [Path] string sub_mchid,
        CancellationToken cancellationToken = default);
}
