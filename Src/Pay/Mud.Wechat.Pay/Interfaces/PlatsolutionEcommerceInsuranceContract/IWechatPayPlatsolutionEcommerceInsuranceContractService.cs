// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceContract;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 · 保险理赔功能开通」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b> ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>「平台服务商」身份口径</b>：本族官方各页「支持商户」栏为【平台商户】（保险理赔功能开通族另含【普通服务商】），
/// 请求体统一以 <c>sub_mchid</c> / <c>sponsor_mchid</c> 指向被操作的二级商户 ⇒ 出资方与被赔付方是两回事，字段不得互相顶替。</para>
/// <para><b>两支应答逐字段同形</b>（<c>sub_mchid</c> / <c>opened</c> / <c>success_time</c>，3 行、必填口径一致） ⇒ 开通支与状态查询支共用 <see cref="InsuranceContractOpenResponse"/>；请求侧只有开通支有 body ⇒ 不共用。</para>
/// <para><b>状态查询支的定位键是 path 段 <c>sub_mchid</c></b>（官方路由 <c>.../sub-mchid/{sub_mchid}/check-opened</c>） ⇒ 它无请求体、以 <c>POST</c> 承载一次「检查」动作，照页不得改成 GET。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerceInsuranceContract", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceInsuranceContractService
{
    /// <summary>
    /// 开通保险理赔功能。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012781476"/>（【平台商户】 【普通服务商】，官方页更新于 2024.11.18）。
    /// </summary>
    /// <param name="request">开通保险理赔功能请求体，字段见 <see cref="InsuranceContractOpenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>开通保险理赔功能，见 <see cref="InsuranceContractOpenResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/insurance-compensation-contracts</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>（官方原样）：频率限制 | 请降低频率后重试 |</para>
    /// <para><b>应答</b>见 <see cref="InsuranceContractOpenResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 ALREADY_EXISTS 产品功能已经开通，请勿重复开通 / 403 NO_AUTH 产品权限异常，请检查 / 429 FREQUENCY_LIMITED 频率限制。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/insurance-compensation-contracts")]
    Task<InsuranceContractOpenResponse> OpenInsuranceCompensationAsync(
        [Body] InsuranceContractOpenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询保险理赔功能开通状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012781480"/>（【平台商户】 【普通服务商】，官方页更新于 2024.11.18）。
    /// </summary>
    /// <param name="subMchid">【二级商户号】 需查询保险理赔功能开通状态的保险公司商户号（官方 path <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询保险理赔功能开通状态，见 <see cref="InsuranceContractOpenResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/insurance-compensation-contracts/sub-mchid/{sub_mchid}/check-opened</c>；path <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：频率限制 | 请降低频率后重试 |</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH 产品权限异常，请检查 / 429 FREQUENCY_LIMITED 频率限制。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/insurance-compensation-contracts/sub-mchid/{subMchid}/check-opened")]
    Task<InsuranceContractOpenResponse> CheckInsuranceCompensationOpenedAsync(
        [Path] string subMchid,
        CancellationToken cancellationToken = default);
}
