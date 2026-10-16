// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Recharge;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「银行转账充值（商户账户充值）」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 2 支中<b>2 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 2 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>两支官方全部只印商户页</b>（「支持商户：【普通商户】」）⇒ 无服务商镜像、无双页归并问题。</para>
/// <para><b>申领支是充值流程的前置闸</b>：官方原文「商户系统<b>须通过调用此接口获取银行转账账号，随后发起充值流程</b>」⇒ <c>POST .../apply</c> 的应答 <c>transfer_in_account</c>（<c>bank_name</c> / <c>bank_address_code</c> / <c>bank_code</c> / <c>bank_account_name</c> / <c>bank_account_no</c>）是后续<b>线下银行转账</b>的收款账号，HTTP 成功不等于到账。</para>
/// <para><b>两支应答不同形 ⇒ 各建一型，不得按「字段重合」合并</b>：申领支应答 6 键（<c>recharge_id</c> + <c>out_recharge_no</c> + <c>transfer_in_account</c> 子树），查询支应答 17 键（另含 <c>mchid</c>、<c>account_type</c>、 <c>recharge_state</c>、<c>bank_transfer_info</c> 子树与 <c>accept_time</c> / <c>success_time</c> / <c>close_time</c>）⇒ 两个响应类型独立建模。</para>
/// <para><b>定位键只有商户单号</b>：查询支把 <c>out_recharge_no</c> 放在 <b>path</b> 段、<b>无</b> query； 官方分配的 <c>recharge_id</c> 长度是罕见的 <c>string(27)</c> ⇒ 照页，不用微信单号作查询键。</para>
/// <para><b>退回是异步态</b>：查询应答的 <c>bank_transfer_info</c> 里 <c>return_time</c> / <c>return_reason</c> / <c>bank_card_tail</c> 均为选填，配合 <c>recharge_state</c> 与 <c>recharge_state_desc</c> 表达「已退回」等中间态 ⇒ <b>不得</b>据 <c>success_time</c> 缺失判定失败，也不得自造 <c>state</c> 枚举闭集。</para>
/// <para><b>频率限制</b>：两支官方页<b>未印</b>端点级数字，错误码给 <c>429 FREQUENCY_LIMITED</c> 与 <c>429 FREQUENCY_LIMIT_EXCEED</c> <b>两种</b>拼写（申领支前者、查询支后者，照页各归各支）⇒ 不得「统一」成一条。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Recharge", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayRechargeService
{
    /// <summary>
    /// 申请银行转账充值。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585425"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">申请银行转账充值请求体，字段见 <see cref="RechargeBankTransferApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请银行转账充值，见 <see cref="RechargeBankTransferApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/recharge/bank-transfer-recharges/apply</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RechargeBankTransferApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Post("/v3/recharge/bank-transfer-recharges/apply")]
    Task<RechargeBankTransferApplyResponse> ApplyRechargeBankTransferAsync(
        [Body] RechargeBankTransferApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询银行转账充值结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585430"/>（【普通商户】）。
    /// </summary>
    /// <param name="outRechargeNo">【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方 path <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询银行转账充值结果，见 <see cref="RechargeBankTransferQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/recharge/bank-transfer-recharges/out-recharge-no/{out_recharge_no}</c>；path <c>out_recharge_no</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数格式校验失败 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 400 INVALID_REQUEST：充值单不存在 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMIT_EXCEED：频率超限 / 500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Get("/v3/recharge/bank-transfer-recharges/out-recharge-no/{outRechargeNo}")]
    Task<RechargeBankTransferQueryResponse> QueryRechargeBankTransferAsync(
        [Path] string outRechargeNo,
        CancellationToken cancellationToken = default);
}
