// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Papay;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「委托代扣（PAPay 服务商面预扣费通知）」域 SDK（1 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支中<b>1 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 1 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 1 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>服务商面官方只印这一支</b>：委托代扣 3.0 的服务商开放面只有「预扣费通知」 （<c>POST /v3/partner-papay/contracts/{contract_id}/notify</c>）⇒ 签约、查询、扣款各支官方<b>无服务商页</b>， 本接口<b>不得</b>按直连面镜像补齐（照页判据，缺页即缺端点）。</para>
/// <para><b>字段名与直连面不同</b>：服务商页包体为 <c>sp_mchid</c> / <c>sp_appid</c> / <c>sub_mchid</c> / <c>sub_appid</c>（多子商户两级）， 直连页为 <c>mchid</c> / <c>appid</c> ⇒ <b>不同名即不同型</b>，请求类型独立（<see cref="PartnerPapayContractNotifyRequest"/>）， 不做超集共用。</para>
/// <para><b>应答与幂等口径同直连面</b>：官方正常示例为「无数据（Http状态码为204）」⇒ 返回裸 <see cref="Task"/>， <c>RESOURCE_ALREADY_EXISTS</c> 表示通知已发送成功。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PartnerPapay", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPartnerPapayService
{
    /// <summary>
    /// 预扣费通知（服务商）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988373"/>（【服务商】）。
    /// </summary>
    /// <param name="contractId">签约成功后，微信返回的委托代扣协议id。 示例值：123124412412423431（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</param>
    /// <param name="request">预扣费通知（服务商）请求体，字段见 <see cref="PartnerPapayContractNotifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/partner-papay/contracts/{contract_id}/notify</c>；path <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：SYSTEMERROR：接口返回错误 / CONTRACT_NOT_EXIST：签约协议不存在 / PARAM_ERROR：参数错误 / APPID_MCHID_NOT_MATCH：appid和mchid不匹配 / INVALID_REQUEST：无效请求 / RULELIMIT：该请求存在风险 / RESOURCE_ALREADY_EXISTS：资源已经存在。</para>
    /// </remarks>
    [Post("/v3/partner-papay/contracts/{contractId}/notify")]
    Task SendPartnerPapayDeductNotifyAsync(
        [Path] string contractId,
        [Body] PartnerPapayContractNotifyRequest request,
        CancellationToken cancellationToken = default);
}
