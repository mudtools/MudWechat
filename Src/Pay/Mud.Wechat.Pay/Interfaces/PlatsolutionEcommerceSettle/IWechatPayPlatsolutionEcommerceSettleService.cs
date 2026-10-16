// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceSettle;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 · 个人收款批量结算」域 SDK（3 端点）。
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
/// <para><b>三支应答逐字段同形</b>（14 行）⇒ 申请、查询、异常批次处理共用 <see cref="SettleApplyResponse"/>； 请求侧三支各异（申请 17 行、异常处理 4 行，查询与异常处理以 path <c>settle_batch_no</c> 定位）⇒ 各建一型。</para>
/// <para><b>异常处理支是 <c>POST .../{settle_batch_no}/apply-abnormal-settle</c></b>：它对批次单做的是「申请异常处理」， 与查询支同 path 不同动词 ⇒ 照页不得合并。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerceSettle", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceSettleService
{
    /// <summary>
    /// 申请个人收款批量结算。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/>（【平台商户】，官方页更新于 2026.04.24）。
    /// </summary>
    /// <param name="request">申请个人收款批量结算请求体，字段见 <see cref="SettleApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请个人收款批量结算，见 <see cref="SettleApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/settle/prepay-settle-orders</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>（官方原样）：频率限制 | 结算请求频率过高，请降低频率后重试。若要确认结算受理结果可以延迟一段时间后查询。 |</para>
    /// <para><b>应答</b>见 <see cref="SettleApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMITED 频率限制 / 400 ALREADY_EXISTS 订单已存在 / 403 USER_ERROR 用户异常 / 403 NO_AUTH 商户无权限 / 400 INVALID_REQUEST 无效请求 / 403 NOT_ENOUGH 订单异常。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/settle/prepay-settle-orders")]
    Task<SettleApplyResponse> ApplyPrepaySettleOrderAsync(
        [Body] SettleApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询个人收款批量结算信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012650326"/>（【平台商户】，官方页更新于 2026.04.24）。
    /// </summary>
    /// <param name="settleBatchNo">【微信支付结算批次单号】 微信支付系统内部的生成的结算批次单号，全局唯一对应一笔商户结算批次单号。（官方 path <c>settle_batch_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询个人收款批量结算信息，见 <see cref="SettleApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/settle/prepay-settle-orders/{settle_batch_no}</c>；path <c>settle_batch_no</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：频率限制 | 结算请求频率过高，请降低频率后重试。若要确认结算受理结果可以延迟一段时间后查询。 |</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMITED 频率限制 / 400 INVALID_REQUEST 无效请求。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/settle/prepay-settle-orders/{settleBatchNo}")]
    Task<SettleApplyResponse> QueryPrepaySettleOrderAsync(
        [Path] string settleBatchNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请异常结算批次单处理。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015198012"/>（【平台商户】，官方页更新于 2026.04.24）。
    /// </summary>
    /// <param name="settleBatchNo">【微信支付结算批次单号】 微信支付系统内部的生成的结算批次单号，全局唯一对应一笔商户结算批次单号。（官方 path <c>settle_batch_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="request">申请异常结算批次单处理请求体，字段见 <see cref="SettleApplyAbnormalRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请异常结算批次单处理，见 <see cref="SettleApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/settle/prepay-settle-orders/{settle_batch_no}/apply-abnormal-settle</c>；path <c>settle_batch_no</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：频率限制 | 请求频率过高，请降低频率后重试。 |</para>
    /// <para><b>应答</b>见 <see cref="SettleApplyResponse"/>（官方应答表逐字段建模，与 <c>settle/prepay-settle-orders</c> 的应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 400 INVALID_REQUEST 结算批次非异常状态，不支持申请 / 400 INVALID_REQUEST 发起异常结算处理申请的指令方与原始指令方不一致。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/settle/prepay-settle-orders/{settleBatchNo}/apply-abnormal-settle")]
    Task<SettleApplyResponse> ApplyAbnormalSettleAsync(
        [Path] string settleBatchNo,
        [Body] SettleApplyAbnormalRequest request,
        CancellationToken cancellationToken = default);
}
