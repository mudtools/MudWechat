// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MerchantSettlement;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商户结算规则申请」域 SDK（<b>2 端点</b>：创建结算规则申请单 + 查询申请单状态）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-14 逐字段核验）：创建
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697924"/>、查询状态
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012698574"/>。</para>
/// <para><b>本域是从业机构面</b>：两支页面的「支持商户」都是【从业机构（银行）】【从业机构（支付机构）】
/// ⇒ 报文用 <c>acquiring_bank_id</c> + <c>channel_id</c> + <c>sub_mchid</c> 三层标识，
/// <b>没有</b> <c>sp_mchid</c>/<c>sub_appid</c> 那套服务商前缀，也不与
/// <see cref="IWechatPayBankTransferService"/> 的六层标识互换（守卫 MSET-B3 锁字段名）。</para>
/// <para><b>两支同前缀 <c>/v3/merchant-settlement/merchant-settle-rule-applications</c></b>：
/// 创建是 POST 无尾段、查询是 GET 带 <c>/{application_id}</c> ⇒ 两条不同 (方法, 路由)（守卫 MSET-B1）。</para>
/// <para><b>两支应答表逐字段相同</b>（8 键、同名同类型同必填性）⇒ 共用
/// <see cref="MerchantSettlementRuleApplicationResponse"/>（守卫 MSET-B6）。</para>
/// <para><b>创建 ≠ 生效</b>：<c>application_state</c> 三值里只有 <c>PASSED</c> / <c>FAILED</c> 是终态，
/// 创建应答就可能是 <c>PROCESSING</c> ⇒ 唯一闭环路径是轮询查询支；<c>application_process_info</c>
/// 是给人看的描述串（string(256)），<b>不是</b>结构化进度节点，不得据它判状态。</para>
/// <para><b><c>settle_rule_id</c> 官方是 integer</b>（请求与应答两侧同为整型，请求侧必填）
/// ⇒ 以 <c>long</c> 承载，传字符串会落到 400 <c>PARAM_ERROR</c> 而不是编译期提示。</para>
/// <para><b><c>update_time</c> 是 rfc3339 字符串</b>（string(128)）⇒ 本包全域时间字段一律 <c>string</c>，
/// 不在 DTO 里做 <c>DateTimeOffset</c> 转换（两套官方时间格式并存的口径见
/// <see cref="Mud.Wechat.Pay.Extensions.PayModule"/> 各域注释）。</para>
/// <para><b>两支页内无端点级频率数字</b>（两轮复核一致）。<b>业务错误码</b>为 APIv3 公共码
/// （400 <c>PARAM_ERROR</c> / 400 <c>INVALID_REQUEST</c> / 401 <c>SIGN_ERROR</c> / 500 <c>SYSTEM_ERROR</c>），
/// 原样透传至 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>。</para>
/// <para><b>本章不含「结算规则查询」与「默认规则设置」</b>：官方在 <c>商户结算规则</c> 章节内只文档这两支
/// HTTP 面（创建 + 按申请单号查），无「直接查生效规则」的读接口 ⇒ 零端点由守卫 MSET-B2 带正对照锁定。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）；商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，
/// 三层标识是<b>报文参数</b>而非作用域；本域<b>零 <c>[Token]</c></b>（守卫 PAY-B1）、
/// 两页均未把 <c>Wechatpay-Serial</c> 列入参数表 ⇒ <b>无加密字段</b>。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MerchantSettlement", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMerchantSettlementService
{
    /// <summary>
    /// 「创建商户结算规则申请单」：为特约商户提交一条结算规则，返回申请单号与当前状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697924"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantSettlementRuleApplicationCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请单号与状态，见 <see cref="MerchantSettlementRuleApplicationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-settlement/merchant-settle-rule-applications</c>；
    /// 支持商户【从业机构（银行）】【从业机构（支付机构）】；无 path / query 参数。</para>
    /// <para><b>四个字段全必填</b>：<c>acquiring_bank_id</c> / <c>channel_id</c> / <c>sub_mchid</c> /
    /// <c>settle_rule_id</c> 缺一即 400 <c>PARAM_ERROR</c> ⇒ 官方没有「按当前登录商户推断」的缺省。</para>
    /// <para><b>受理不等于生效</b>：应答的 <c>application_state</c> 可能是 <c>PROCESSING</c>，
    /// 须轮询 <see cref="QuerySettleRuleApplicationAsync"/> 到 <c>PASSED</c> / <c>FAILED</c>。</para>
    /// </remarks>
    [Post("/v3/merchant-settlement/merchant-settle-rule-applications")]
    Task<MerchantSettlementRuleApplicationResponse> CreateSettleRuleApplicationAsync(
        MerchantSettlementRuleApplicationCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询商户结算规则申请单状态」：按创建应答的 <c>application_id</c> 轮询，应答与创建支同型。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698574"/>。
    /// </summary>
    /// <param name="applicationId">申请单号（官方 path <c>application_id</c>，必填 string）：创建应答返回。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与创建支<b>同一应答类型</b>，见 <see cref="MerchantSettlementRuleApplicationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/merchant-settlement/merchant-settle-rule-applications/{application_id}</c>；无 query 参数、无包体。</para>
    /// <para><b>只有申请单号一条定位路径</b>：官方未提供「按商户号查最新申请单」的接口 ⇒
    /// 宿主必须持久化创建应答的 <c>application_id</c>，丢了就没有查询通道（守卫 MSET-B2）。</para>
    /// <para><b>终态后应答仍带全套请求字段</b>：<c>acquiring_bank_id</c> / <c>channel_id</c> /
    /// <c>sub_mchid</c> / <c>settle_rule_id</c> 在查询侧回显（官方标选填）⇒ 以回显值与本地台账比对，
    /// 不以「查到了」代替「查对了」。</para>
    /// </remarks>
    [Get("/v3/merchant-settlement/merchant-settle-rule-applications/{applicationId}")]
    Task<MerchantSettlementRuleApplicationResponse> QuerySettleRuleApplicationAsync(
        [Path] string applicationId,
        CancellationToken cancellationToken = default);
}
