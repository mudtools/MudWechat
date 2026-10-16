// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PasswordExemptContract;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「免密支付契约」域 SDK（6 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域六支官方页<b>全部未印</b>端点级 QPS 数字 ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>本域零 query、零 <c>brand_id</c>、零列表支</b>：预签约四支走资源段（qs 面与服务商面各两场景），
/// 协议查询与解约按 path 上的 <c>service_id</c> + <c>out_contract_code</c> 两段定位 ⇒ 官方无「按协议号查询」支。</para>
/// <para><b>两套标识体系不得互换</b>（本域红线）：<c>/contracts/qs/*</c> 是<b>从业机构（支付机构）</b>面，用 <c>appid</c> / <c>openid</c>（<c>string(128)</c>）；<c>/contracts/service-provider/*</c> 是<b>普通服务商</b>面，用 <c>sp_appid</c> / <c>sp_openid</c>（<c>string(64)</c>）⇒ 官方字段名、长度上限与应答字段集均不同，四个预签约支<b>各建一型</b>。</para>
/// <para><b>应答差异</b>：服务商面比清算机构面多 <c>sub_mchid</c>；<b>小程序</b>场景应答比<b>支付后</b>场景多 <c>sign_mp_appid</c> / <c>sign_mp_path</c>（拉起签约小程序用）⇒ 两场景不得共用应答。</para>
/// <para><b>协议查询与解约定位键</b>：<c>service_id</c>（<c>integer</c>）与 <c>out_contract_code</c>（<c>string(32)</c>）<b>同为 path 段</b>； 官方<b>无</b>按 <c>contract_id</c> 查询的支，解约后同一 <c>out_contract_code</c> <b>不可复用</b>。 两支应答亦不同形（查询 15 行、解约 3 行）⇒ 各建一型。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PasswordExemptContract", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPasswordExemptContractService
{
    /// <summary>
    /// 清算机构模式小程序场景预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471282"/>（【从业机构（支付机构）】）。
    /// </summary>
    /// <param name="request">清算机构模式小程序场景预签约请求体，字段见 <see cref="PasswordExemptQsMiniProgramPreSignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>清算机构模式小程序场景预签约，见 <see cref="PasswordExemptQsMiniProgramPreSignResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/mini-program</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【从业机构（支付机构）】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PasswordExemptQsMiniProgramPreSignResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH 商户暂无权限使用此功能。</para>
    /// </remarks>
    [Post("/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/mini-program")]
    Task<PasswordExemptQsMiniProgramPreSignResponse> CreateQsMiniProgramPreEntrustSignAsync(
        [Body] PasswordExemptQsMiniProgramPreSignRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 清算机构模式支付后场景预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471286"/>（【从业机构（支付机构）】）。
    /// </summary>
    /// <param name="request">清算机构模式支付后场景预签约请求体，字段见 <see cref="PasswordExemptQsPayRedirectPreSignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>清算机构模式支付后场景预签约，见 <see cref="PasswordExemptQsPayRedirectPreSignResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/pay-redirect-sign</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【从业机构（支付机构）】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PasswordExemptQsPayRedirectPreSignResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH 商户暂无权限使用此功能。</para>
    /// </remarks>
    [Post("/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/pay-redirect-sign")]
    Task<PasswordExemptQsPayRedirectPreSignResponse> CreateQsPayRedirectPreEntrustSignAsync(
        [Body] PasswordExemptQsPayRedirectPreSignRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商模式小程序场景预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471269"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">服务商模式小程序场景预签约请求体，字段见 <see cref="PasswordExemptSpMiniProgramPreSignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务商模式小程序场景预签约，见 <see cref="PasswordExemptSpMiniProgramPreSignResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/mini-program</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PasswordExemptSpMiniProgramPreSignResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH 商户暂无权限使用此功能。</para>
    /// </remarks>
    [Post("/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/mini-program")]
    Task<PasswordExemptSpMiniProgramPreSignResponse> CreateSpMiniProgramPreEntrustSignAsync(
        [Body] PasswordExemptSpMiniProgramPreSignRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商模式支付后场景预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471272"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">服务商模式支付后场景预签约请求体，字段见 <see cref="PasswordExemptSpPayRedirectPreSignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务商模式支付后场景预签约，见 <see cref="PasswordExemptSpPayRedirectPreSignResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/pay-redirect-sign</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PasswordExemptSpPayRedirectPreSignResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH 商户暂无权限使用此功能。</para>
    /// </remarks>
    [Post("/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/pay-redirect-sign")]
    Task<PasswordExemptSpPayRedirectPreSignResponse> CreateSpPayRedirectPreEntrustSignAsync(
        [Body] PasswordExemptSpPayRedirectPreSignRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 免密支付协议查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715321"/>（【普通服务商】【从业机构（支付机构）】【平台商户】）。
    /// </summary>
    /// <param name="serviceId">【免密支付服务ID】 免密支付服务ID（官方 path <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="outContractCode">【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性，同一个协议号解约后无法重复使用。只能是数字、大小写字母的组合。（官方 path <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>免密支付协议查询，见 <see cref="PasswordExemptContractQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/password-exempt-contract/contracts/service-id/{service_id}/out-contract-code/{out_contract_code}</c>；path <c>service_id</c> / <c>out_contract_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】【从业机构（支付机构）】【平台商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH 商户暂无权限使用此功能。</para>
    /// </remarks>
    [Get("/v3/password-exempt-contract/contracts/service-id/{serviceId}/out-contract-code/{outContractCode}")]
    Task<PasswordExemptContractQueryResponse> QueryPasswordExemptContractAsync(
        [Path] int serviceId,
        [Path] string outContractCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 免密支付协议解约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715383"/>（【普通服务商】　【从业机构（支付机构）】　【平台商户】）。
    /// </summary>
    /// <param name="serviceId">【免密支付服务ID】 免密支付服务ID（官方 path <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="outContractCode">【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性，同一个协议号解约后无法重复使用。只能是数字、大小写字母的组合。（官方 path <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">免密支付协议解约请求体，字段见 <see cref="PasswordExemptContractTerminateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>免密支付协议解约，见 <see cref="PasswordExemptContractTerminateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/password-exempt-contract/contracts/service-id/{service_id}/out-contract-code/{out_contract_code}/terminate</c>；path <c>service_id</c> / <c>out_contract_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】　【从业机构（支付机构）】　【平台商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PasswordExemptContractTerminateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH 商户暂无权限使用此功能。</para>
    /// </remarks>
    [Post("/v3/password-exempt-contract/contracts/service-id/{serviceId}/out-contract-code/{outContractCode}/terminate")]
    Task<PasswordExemptContractTerminateResponse> TerminatePasswordExemptContractAsync(
        [Path] int serviceId,
        [Path] string outContractCode,
        [Body] PasswordExemptContractTerminateRequest request,
        CancellationToken cancellationToken = default);
}
