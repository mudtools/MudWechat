// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceRecharge;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 · 二级商户充值」域 SDK（3 端点）。
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
/// <para><b>查询与关闭应答逐字段同形</b>（36 行，含 <c>available_recharge_channels</c> 数组） ⇒ 共用 <see cref="RechargeQueryResponse"/>；两支都要求 <c>sub_mchid</c> 为<b>必填 query</b>。</para>
/// <para><b>本域两支官方路由未被本 SDK 声明</b>：<c>POST /v3/platsolution/ecommerce/bank-transfer-recharges/apply</c> 与 <c>.../bank-transfer-recharges/out-recharge-no/{out_recharge_no}</c> 的官方页已不可达（取证结论 NO-PAGE） ⇒ 零声明，由非契约路由零声明守卫第八条锁定，改动须重新逐页取证。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerceRecharge", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceRechargeService
{
    /// <summary>
    /// 申请二级商户充值。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/>（【平台商户】，官方页更新于 2025.07.29）。
    /// </summary>
    /// <param name="request">申请二级商户充值请求体，字段见 <see cref="RechargeApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请二级商户充值，见 <see cref="RechargeApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/recharges/apply</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RechargeApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 入账账户不存在 / 400 INVALID_REQUEST 单笔充值金额超过上限 / 400 INVALID_REQUEST 该业务充值单号已存在 / 403 NO_AUTH 服务商未开通平台收付通 / 403 NO_AUTH 平台充值权限被处罚，无法发起充值 / 403 NO_AUTH 商户号出入金权限被管控 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/recharges/apply")]
    Task<RechargeApplyResponse> ApplyRechargeAsync(
        [Body] RechargeApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询二级商户充值结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/>（【平台商户】，官方页更新于 2025.07.29）。
    /// </summary>
    /// <param name="outRechargeNo">【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方 path <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询二级商户充值结果，见 <see cref="RechargeQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/recharges/out-recharge-no/{out_recharge_no}</c>；path <c>out_recharge_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 充值单不存在 / 403 NO_AUTH 服务商未开通平台收付通 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/recharges/out-recharge-no/{outRechargeNo}")]
    Task<RechargeQueryResponse> QueryRechargeByOutRechargeNoAsync(
        [Path] string outRechargeNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭二级商户充值。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762895"/>（【平台商户】，官方页更新于 2025.07.29）。
    /// </summary>
    /// <param name="outRechargeNo">【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方 path <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>关闭二级商户充值，见 <see cref="RechargeQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/recharges/out-recharge-no/{out_recharge_no}/close</c>；path <c>out_recharge_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 充值单不存在 / 403 NO_AUTH 服务商未开通平台收付通 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/recharges/out-recharge-no/{outRechargeNo}/close")]
    Task<RechargeQueryResponse> CloseRechargeAsync(
        [Path] string outRechargeNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);
}
