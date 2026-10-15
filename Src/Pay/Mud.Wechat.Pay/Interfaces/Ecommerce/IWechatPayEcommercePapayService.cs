// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台收付通 · 委托代扣合单」域 SDK（<b>6 端点</b>：预签约 + 查签约 + 解约 + 合单扣款 + 合单查询 + 合单撤销）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-15 逐页逐字段核验，六页全标【平台商户】）：预签约
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012881608"/>、查询签约信息
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884060"/>、解约
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884072"/>、合单下单（免密扣款）
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>、合单查询
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884130"/>、合单撤销
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884126"/>。</para>
/// <para><b>设计文档表格给的是旧版 wiki 链接</b>（<c>/wiki/doc/apiv3_partner/Offline/apis/chapter5_5_*.shtml</c>）：
/// 本轮按现行文档页 <c>4012881608</c> 等六页逐字段核验，字段表与旧版一致但 URL 以现行页为准。</para>
/// <para><b>三支产品能力前置，缺一不可</b>：官方逐页给出前置条件 ⇒ ① 预签约「使用前请确认已开通委托代扣能力」；
/// ② 合单扣款「调用前请确认电商服务商已经开通<b>合单</b>委托代扣能力并已经和用户签约，<b>且签约的模板有合单委托代扣能力</b>」
/// —— 能力开通是「服务商级 + 模板级」两级，模板没有合单能力时即使服务商已开通也会失败；
/// ③ 解约「调用前请确认对应的签约协议是<b>已生效状态</b>」⇒ 对未生效或已解约的协议解约必失败（守卫 EC-S1）。</para>
/// <para><b>签约会话只有 2 小时</b>：官方原文「商户获取的签约会话有效期为 2 小时」，
/// <c>pre_entrustweb_id</c> 的描述同文「两个小时内有效」⇒ 取到后必须在窗口内拉起微信客户端，
/// 过期即需重新预签约（重新预签约会换会话 ID，不复用旧值）。</para>
/// <para><b>签约结果走异步通知且「放弃签约不通知」</b>：官方原文「用户在微信的页面中完成委托代扣签约后，
/// 微信会同时将签约信息通过异步通知的方式通知给商户后台。<b>如果用户放弃签约或签约失败则不通知</b>」⇒
/// 没有通知不等于失败，只有 <see cref="QueryPapayContractAsync"/> 的 <c>contract_state</c> 是权威；
/// 因此回调后必须回查，而不是把「未收到通知」当状态使用。</para>
/// <para><b>预签约应答的两个条件字段决定调起方式</b>：<c>miniprogram_username</c> / <c>miniprogram_path</c>
/// 只在「模板为 2025-09-23 及之后申请」或「2025-09-23 之前但申请了 <c>WXLaunchMiniProgram</c> 权限」时返回 ⇒
/// 返回则用 LaunchMiniProgram SDK 调起签约（字段名与值<b>原样填入</b>），未返回则该模板只能通过
/// OpenBusinessWebview SDK 调起 —— 官方以「是否返回该字段」判别可用调起通道，SDK 不代为选择。</para>
/// <para><b>扣款是同步模式且窗口只有 1 分钟</b>：官方原文「当前扣费接口为<b>同步扣费模式</b>，
/// 商户超时时间为 <b>1 分钟</b>，超过 1 分钟未能返回结果时，商户可主动发起撤销」⇒
/// 这是本域唯一由「商户侧计时」决定是否撤销的端点：1 分钟内无结果即应走 <see cref="ReversePapayCombineTransactionAsync"/>，
/// 不要盲目重下（合单扣款官方未声明幂等，重复下单以 <c>combine_out_trade_no</c> 定位的既有单为准）。</para>
/// <para><b>撤销支官方零应答字段</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884126"/> 的应答节
/// 只有 <c>200 OK</c>、<b>参数表为空</b>（2026-10-15 核验：应答 0 行）⇒ 本方法返回裸 <see cref="Task"/>，
/// 撤销结果必须回 <see cref="QueryPapayCombineTransactionAsync"/> 读子单状态，不得伪造应答类型（守卫 EC-S2）。</para>
/// <para><b>扣款与查询两支共用同一应答类型</b>：<c>4012884121</c> 与 <c>4012884130</c> 的应答表 21 行
/// <b>逐行相同</b>（2026-10-15 比对为 IDENTICAL）⇒ 共用 <see cref="EcommercePapayCombineTransactionResponse"/>；
/// 两支的<b>请求</b>形态则完全不同（下单整棵合单树、查询只有 path 上的 <c>combine_out_trade_no</c>）。</para>
/// <para><b>本域应答的交易类型只有一个值</b>：<c>sub_orders[].trade_type</c> 官方枚举仅 <c>PAP</c>（委托代扣，
/// 见 <see cref="EcommercePapayTradeTypes"/>），与直连合单的 <c>JSAPI</c>/<c>NATIVE</c>/<c>APP</c> 形态不同；
/// 子单状态族见 <see cref="EcommercePapayTradeStates"/>（六个值，无 <c>REVOKED</c>）⇒
/// 撤销后的收敛态须以查询应答的实际值判读，不得臆造第七个值。</para>
/// <para><b>签约定位键是「模板 + 商户签约协议号」二元组</b>：查询与解约两支都走
/// <c>/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}</c>，<c>plan_id</c> 官方类型为
/// <b>integer</b>（本方法用 <see cref="long"/> 承载）而 <c>out_contract_code</c> 是 string(32) 且
/// 「商户侧需保证唯一性」⇒ 与 <see cref="EcommercePapayPreEntrustSignRequest"/> 里的同一字段是同一个键，
/// 签约、查签、解约三支共用；扣款用的是 <c>contract_id</c>（微信侧签约协议号），两者不可互换。</para>
/// <para><b>本域零加密面</b>：六页 Header 参数表只有 <c>Authorization</c> / <c>Accept</c> / <c>Content-Type</c>，
/// <b>没有任何一页把 <c>Wechatpay-Serial</c> 列为参数</b>（2026-10-15 逐页核验）⇒ 本接口六支一律不得支配该头
/// （守卫 EC-S3）。签约链上的 <c>openid</c> 在官方口径里是明文标识符，不是敏感字段；
/// 但 <c>contract_display_account</c>（协议展示账号）会出现在用户可见的签约页面，属业务文案而非凭据。</para>
/// <para><b>各页均未给出频率数字</b>：只列通用错误码表 ⇒ SDK 不做限流与自动重试；撤销与扣款的时序由宿主按
/// 1 分钟窗口掌握。</para>
/// <para><b>客户端与凭据</b>：走支付专用命名客户端 <c>wechat-pay</c>，签名主体是当前作用域商户号；
/// 本域<b>零 <c>[Token]</c></b>。<c>[AllowAnyStatusCode]</c> 使 APIv3 的 4xx/5xx 错误体原样落到
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Ecommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEcommercePapayService
{
    /// <summary>
    /// 「预签约」：外部 App 拉起微信客户端发起委托代扣签约前，须后台先调本接口换取签约会话 ID（<b>2 小时有效</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012881608"/>。
    /// </summary>
    /// <param name="request">预签约请求，见 <see cref="EcommercePapayPreEntrustSignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>签约会话，见 <see cref="EcommercePapayPreEntrustSignResponse"/>（含 <c>pre_entrustweb_id</c> 与两个条件字段）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/combine-papay/contracts/pre-entrust-sign</c>；无加密头。</para>
    /// <para><b>前置</b>：「使用前请确认已开通委托代扣能力」。</para>
    /// <para><b>会话 2 小时过期</b>：取到 <c>pre_entrustweb_id</c> 后须在窗口内拉起客户端；过期只能重新预签约。</para>
    /// <para><b><c>miniprogram_username</c>/<c>miniprogram_path</c> 决定调起 SDK</b>：返回则用 LaunchMiniProgram
    /// 且字段名与值原样填入，未返回则该模板只能走 OpenBusinessWebview。</para>
    /// </remarks>
    [Post("/v3/ecommerce/combine-papay/contracts/pre-entrust-sign")]
    Task<EcommercePapayPreEntrustSignResponse> PreEntrustSignAsync(
        [Body] EcommercePapayPreEntrustSignRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询签约协议」：签约状态的<b>唯一权威来源</b>（用户放弃签约时官方不通知）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884060"/>。
    /// </summary>
    /// <param name="planId">委托代扣协议模板 ID（官方 path <c>plan_id</c>，必填 integer）。</param>
    /// <param name="outContractCode">商户签约协议号（官方 path <c>out_contract_code</c>，必填 string(32)）：商户侧唯一。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>签约协议详情（15 行），见 <see cref="EcommercePapayContractQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/combine-papay/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}</c>。</para>
    /// <para><b>前置</b>：「商户已经成功获取过委托代扣的签约」⇒ 未签约时本支报错，不返回空对象。</para>
    /// <para><b>「未收到签约通知」不等于失败</b>：官方只对成功的签约下发异步通知 ⇒ 判状态须用
    /// <c>contract_state</c>（见 <see cref="EcommercePapayContractStates"/>）。</para>
    /// <para><b>解约信息是条件子表</b>：<c>contract_terminate_info</c> 只在已解约时返回，
    /// 其 <c>contract_termination_mode</c> 见 <see cref="EcommercePapayTerminationModes"/>。</para>
    /// </remarks>
    [Get("/v3/ecommerce/combine-papay/contracts/plan-id/{planId}/out-contract-code/{outContractCode}")]
    Task<EcommercePapayContractQueryResponse> QueryPapayContractAsync(
        [Path] long planId,
        [Path] string outContractCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「解约」：对<b>已生效</b>的委托代扣签约协议发起解约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884072"/>。
    /// </summary>
    /// <param name="planId">委托代扣协议模板 ID（官方 path <c>plan_id</c>，必填 integer）。</param>
    /// <param name="outContractCode">商户签约协议号（官方 path <c>out_contract_code</c>，必填 string(32)）。</param>
    /// <param name="request">解约请求，见 <see cref="EcommercePapayContractTerminateRequest"/>（官方入参只有 <c>contract_termination_remark</c> 一行且必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>解约后的定位键（3 行），见 <see cref="EcommercePapayContractTerminateResponse"/>（<b>不回状态</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/combine-papay/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}/terminate</c>。</para>
    /// <para><b>前置</b>：「调用前请确认对应的签约协议是已生效状态的签约协议」⇒ 未生效/已解约的协议调用必失败。</para>
    /// <para><b><c>contract_termination_remark</c> 官方标必填</b>：会体现在解约信息子表里，属留证文本。</para>
    /// <para><b>应答只有三个定位键、没有状态字段</b>：解约结果须回 <see cref="QueryPapayContractAsync"/> 读
    /// <c>contract_state</c> 与 <c>contract_terminate_info</c> 确认。</para>
    /// </remarks>
    [Post("/v3/ecommerce/combine-papay/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate")]
    Task<EcommercePapayContractTerminateResponse> TerminatePapayContractAsync(
        [Path] long planId,
        [Path] string outContractCode,
        [Body] EcommercePapayContractTerminateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「合单免密扣款」：凭已生效的签约协议对多笔子单一并发起委托代扣（<b>同步扣费，商户侧超时窗口 1 分钟</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>。
    /// </summary>
    /// <param name="request">合单扣款请求，见 <see cref="EcommercePapayCombineTransactionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>合单结果（21 行），见 <see cref="EcommercePapayCombineTransactionResponse"/>（与 <see cref="QueryPapayCombineTransactionAsync"/><b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/combine-papay/transactions</c>；无加密头。</para>
    /// <para><b>两级前置</b>：服务商须开通<b>合单</b>委托代扣能力、用户已签约，<b>且签约的模板本身具备合单代扣能力</b>。</para>
    /// <para><b>同步扣费 + 1 分钟窗口</b>：官方原文「商户超时时间为 1 分钟，超过 1 分钟未能返回结果时，
    /// 商户可主动发起撤销」（<see cref="ReversePapayCombineTransactionAsync"/>）⇒ 窗口归宿主计时。</para>
    /// <para><b>应答的 <c>sub_orders[].trade_type</c> 官方只有 <c>PAP</c> 一个值</b>（见 <see cref="EcommercePapayTradeTypes"/>），
    /// 与直连合单的 <c>JSAPI</c>/<c>NATIVE</c>/<c>APP</c> 形态不同。</para>
    /// </remarks>
    [Post("/v3/ecommerce/combine-papay/transactions")]
    Task<EcommercePapayCombineTransactionResponse> CreatePapayCombineTransactionAsync(
        [Body] EcommercePapayCombineTransactionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询合单扣款」：按商户合单号读合单与子单状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884130"/>。
    /// </summary>
    /// <param name="combineOutTradeNo">商户合单总订单号（官方 path <c>combine_out_trade_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与扣款支<b>共用同一应答类型</b>（21 行逐字段相同），见 <see cref="EcommercePapayCombineTransactionResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/combine-papay/transactions/{combine_out_trade_no}</c>。</para>
    /// <para><b>前置</b>：「调用前请商户确认已经发起了免密的合单支付」。</para>
    /// <para><b>撤销与扣款的结果都收敛到本方法</b>：撤销支零应答 ⇒ 判撤销是否生效只能读这里的子单状态族
    /// （<see cref="EcommercePapayTradeStates"/>）。</para>
    /// </remarks>
    [Get("/v3/ecommerce/combine-papay/transactions/{combineOutTradeNo}")]
    Task<EcommercePapayCombineTransactionResponse> QueryPapayCombineTransactionAsync(
        [Path] string combineOutTradeNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「撤销合单扣款」：对已发起的免密合单支付按子单粒度撤销（<b>官方应答零业务字段</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884126"/>。
    /// </summary>
    /// <param name="combineOutTradeNo">商户合单总订单号（官方 path <c>combine_out_trade_no</c>，必填 string(32)）：待撤销的合单。</param>
    /// <param name="request">撤销请求，见 <see cref="EcommercePapayReverseRequest"/>（<c>combine_appid</c> + 待撤销子单列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方只回 <c>200 OK</c> 且<b>应答参数表为空</b> ⇒ 裸 <see cref="Task"/>，撤销结果须回 <see cref="QueryPapayCombineTransactionAsync"/> 确认。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/combine-papay/transactions/{combine_out_trade_no}/reverse</c>。</para>
    /// <para><b>前置</b>：「调用前请商户确认已经发起了免密的合单支付」；扣款超过 1 分钟无结果时官方指引即走本支。</para>
    /// <para><b>不建应答类型</b>：本页应答节只有 <c>200 OK</c>、无任何参数行 ⇒
    /// 伪造一个「看起来合理」的应答类型即造出官方永不返回的字段（守卫 EC-S2）。</para>
    /// <para><b>200 不等于撤销成功</b>：本支不回状态，须查单读子单 <c>trade_state</c> 收敛。</para>
    /// </remarks>
    [Post("/v3/ecommerce/combine-papay/transactions/{combineOutTradeNo}/reverse")]
    Task ReversePapayCombineTransactionAsync(
        [Path] string combineOutTradeNo,
        [Body] EcommercePapayReverseRequest request,
        CancellationToken cancellationToken = default);
}
