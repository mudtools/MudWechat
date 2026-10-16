// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.AbnormalFundProcessing;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「在途异常资金处理」域 SDK（3 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域三支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 RATELIMIT_EXCEEDED</c> /
/// <c>FREQUENCY_LIMITED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>本域零 query、零列表支</b>：三支全部以 path 定位（付款单与指令发起用 <c>receipt_id</c>，指令查询再加
/// <c>out_instruction_no</c>）⇒ 不存在分页口径可套用，他域的 <c>offset</c>/<c>limit</c> 模板不得搬进来。</para>
/// <para><b>官方只开放查询与指令两支</b>：<c>GET /receipts/{receipt_id}</c> 查付款单、<c>POST /receipts/{receipt_id}/transfer-instructions</c> 发起指令， 执行结果查询支（<c>GET .../transfer-instructions/{out_instruction_no}</c>）为第三支 ⇒ 三支应答<b>不同形</b>，各建一型。</para>
/// <para><b>指令发起支官方未印业务错误码表</b>：只在通用错误码里给出 <c>429 RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "AbnormalFundProcessing", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayAbnormalFundProcessingService
{
    /// <summary>
    /// 在途异常资金付款单查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408031"/>（【平台商户】　【普通服务商】）。
    /// </summary>
    /// <param name="receiptId">【微信支付在途异常资金付款单号】 微信支付返回的在途异常资金付款单号（官方 path <c>receipt_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>在途异常资金付款单查询，见 <see cref="AbnormalFundProcessingReceiptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/abnormal-fund-processing/receipts/{receipt_id}</c>；path <c>receipt_id</c>。</para>
    /// <para><b>支持商户</b>【平台商户】　【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 404 NOT_FOUND 在途异常资金付款单不存在。</para>
    /// </remarks>
    [Get("/v3/abnormal-fund-processing/receipts/{receiptId}")]
    Task<AbnormalFundProcessingReceiptResponse> QueryAbnormalFundReceiptAsync(
        [Path] string receiptId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 在途异常资金付款指令发起。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408022"/>（【平台商户】　【普通服务商】）。
    /// </summary>
    /// <param name="receiptId">【微信支付在途异常资金付款单号】 微信支付在途异常资金付款业务的唯一标识（官方 path <c>receipt_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="request">在途异常资金付款指令发起请求体，字段见 <see cref="AbnormalFundProcessingInstructionCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>在途异常资金付款指令发起，见 <see cref="AbnormalFundProcessingInstructionCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/abnormal-fund-processing/receipts/{receipt_id}/transfer-instructions</c>；path <c>receipt_id</c>。</para>
    /// <para><b>支持商户</b>【平台商户】　【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="AbnormalFundProcessingInstructionCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 400 ALREADY_EXISTS 当前在途异常资金付款单已在处理中或处理成功。</para>
    /// </remarks>
    [Post("/v3/abnormal-fund-processing/receipts/{receiptId}/transfer-instructions")]
    Task<AbnormalFundProcessingInstructionCreateResponse> CreateAbnormalFundTransferInstructionAsync(
        [Path] string receiptId,
        [Body] AbnormalFundProcessingInstructionCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 在途异常资金付款指令执行结果查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016636882"/>（【平台商户】　【普通服务商】）。
    /// </summary>
    /// <param name="receiptId">【微信支付在途异常资金付款单号】 微信支付在途异常资金付款单号（官方 path <c>receipt_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="outInstructionNo">【商户侧指令编号】 发起在途异常资金付款指令时传入的商户侧指令编号（官方 path <c>out_instruction_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>在途异常资金付款指令执行结果查询，见 <see cref="AbnormalFundProcessingInstructionResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/abnormal-fund-processing/receipts/{receipt_id}/transfer-instructions/{out_instruction_no}</c>；path <c>receipt_id</c> / <c>out_instruction_no</c>。</para>
    /// <para><b>支持商户</b>【平台商户】　【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 404 NOT_FOUND 在途异常资金付款指令单不存在。</para>
    /// </remarks>
    [Get("/v3/abnormal-fund-processing/receipts/{receiptId}/transfer-instructions/{outInstructionNo}")]
    Task<AbnormalFundProcessingInstructionResponse> QueryAbnormalFundTransferInstructionAsync(
        [Path] string receiptId,
        [Path] string outInstructionNo,
        CancellationToken cancellationToken = default);
}
