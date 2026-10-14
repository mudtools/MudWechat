// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Repayment;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台商户统一收单·还款」域 SDK（<b>3 端点</b>：合单预下单 + 查单 + 关单）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（合作伙伴文档中心 ·「统一收单」下的还款分支，2026-10-14 逐页核验，
/// 各页「支持商户」栏恒为【平台商户】）：还款合单预下单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015124463"/>、按合单商户订单号查询
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015124483"/>、关闭还款单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015124497"/>。</para>
/// <para><b>「还款」是合单支付的一个专用面，不是 <see cref="Mud.Wechat.Pay.Extensions.PayModule.CombineTransactions"/> 的子集</b>：
/// 路由前缀 <c>/v3/repayment/combine-transactions/partner/</c>（合单面是
/// <c>/v3/combine-transactions/</c>），且<b>必填 <c>combine_mchid</c> 语义反转</b> ——
/// 官方在子单 <c>mchid</c> 一栏原文写明「此处请填写服务商(还款发起方)的商户号 <c>combine_mchid</c>」
/// ⇒ 照抄合单支付的子单填法会把钱收错账户（守卫 RP-B3 锁该字段说明）。</para>
/// <para><b>签名主体是平台商户号</b>：宿主应以平台商户配置进入调用作用域；
/// <c>combine_mchid</c> 与 <c>sub_mchid</c> 均为<b>报文参数</b>而非作用域。
/// <b>本域只借「商户号 + 私钥」这一既有承载面</b>：平台收付通的账本 / 二级商户体系
/// <b>不在本仓建模</b>（守卫 <c>WechatPayCapitalRulingContractGuards</c> C1/C2 的裁决边界）。</para>
/// <para><b>本域 <c>h5_info</c> 是「渠道信息」聚合体，不是直连面的扁平 <c>scene_info</c></b>：
/// 内含 <c>combine_appid</c> / <c>payer_client_ip</c> / <c>type</c>（Wap / iOS / Android / Harmony）
/// / <c>app_url</c> 等，字段名带 <c>combine_</c> 前缀。四个端型信息块
/// （<c>miniprogram_info</c> / <c>app_info</c> / <c>h5_info</c> / <c>wechat_web_info</c>）
/// 官方要求<b>至少传一个</b> ⇒ SDK 侧不做「四选一」编译期约束（声明式无联合类型），
/// 由宿主按 <c>h5_url</c> 是否返回判别调起方式。</para>
/// <para><b>应答是「预支付」产物，不是支付结果</b>：<c>prepay_id</c> 供端上调起、
/// <c>h5_url</c> 供网页跳转，<b>两者皆选填</b>（按传入的信息块出现）⇒
/// 拿到 200 ≠ 用户已付款，支付结果只经<b>回调通知</b>或查单得出。</para>
/// <para><b>调起支付与回调通知不在本域</b>：端上调起是 JSAPI /小程序 / APP / H5 四支
/// <b>参数签名</b>（前端形态、无 HTTP 端点），支付成功回调是<b>接收</b>面（由
/// <c>Mud.Wechat.Pay.Callback</c> 承载）⇒ 本域<b>零端点</b>（守卫 RP-B2 带正对照）。</para>
/// <para><b>关单是 204 无包体</b> ⇒ 声明式返回裸 <see cref="Task"/>（守卫 RP-B1 锁定），
/// 且定位键在 path（<c>combine_out_trade_no</c>）、包体只带待关的<b>子单</b>列表。</para>
/// <para><b><c>time_expire</c> 是「支付结束时间」而非「订单关闭时间」</b>：仅首次下单可设、
/// <b>后续修改必报错</b>；超时的订单只能<b>换新 <c>combine_out_trade_no</c> 重新下单</b>，
/// 未超时则可用原参数重取 <c>prepay_id</c>。格式为 rfc3339 字符串（支付线时间字段一律
/// <c>string</c>，不做 <c>DateTimeOffset</c> 解析）。</para>
/// <para><b><c>combine_out_trade_no</c> 须在同一商户号下唯一</b>；官方两页对合法字符集的描述
/// <b>不一致</b>（下单页 <c>_-|*</c>、查询页另含 <c>@</c>）⇒ 宿主生成单号须按<b>最窄集</b>，
/// 不要依赖查询页的宽集（守卫 RP-B3 只锁字段名，不锁字符集）。</para>
/// <para><b>子单最多 10 条</b>；<c>settle_info.subsidy_amount</c> 官方限<b>单笔 ≤ 5000 元</b>；
/// 金额单位一律<b>分</b>。</para>
/// <para><b>实名信息为密文</b>：<c>combine_payer_info.identity</c> 的 <c>number</c> / <c>name</c>
/// 传 RSA 密文、请求头 <c>Wechatpay-Serial</c> 随之<b>必填</b>；类型 <c>type</c> 有官方 7 值表
/// （见 <see cref="RepaymentIdentityTypes"/>）。查单应答的 <c>combine_payer_info</c> 形态
/// <b>与请求侧不同</b>：那里只有选填 <c>openid</c> ⇒ 两型分建（守卫 RP-B4）。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse"/> 的
/// <c>Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Repayment", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayRepaymentService
{
    /// <summary>
    /// 平台商户「还款合单预下单」：一次为多笔待还子单生成一个合单预支付会话。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015124463"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="RepaymentPrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns><c>prepay_id</c> 与 / 或 <c>h5_url</c>，见 <see cref="RepaymentPrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b>
    /// <c>/v3/repayment/combine-transactions/partner/prepay</c>；无 path / query 参数；
    /// 请求头 <c>Wechatpay-Serial</c> <b>必填</b>（官方注明填<b>微信支付公钥 ID</b>（推荐）或
    /// 平台证书序列号，敏感字段须先用其加密）。</para>
    /// <para><b>四块端型信息至少传一个</b>（<c>miniprogram_info</c> / <c>app_info</c> /
    /// <c>h5_info</c> / <c>wechat_web_info</c>）：官方以文案约束而非字段必填 ⇒ 本方法不代校验，
    /// 全不传时的报错由微信给出。</para>
    /// <para><b>子单 <c>mchid</c> 填平台商户号自身</b>（官方原文「此处请填写服务商(还款发起方)的商户号
    /// <c>combine_mchid</c>」）：这是本域相对合单支付最容易踩反的一处。</para>
    /// <para><b><c>settle_info.subsidy_amount</c> 仅在 <c>profit_sharing = true</c> 时生效</b>
    /// （官方明写），且单笔最高补差 5000 元 ⇒ 不指定分账时传补差是无效字段。</para>
    /// <para><b>受理 ≠ 支付成功</b>：应答仅给出调起凭据，结果须经回调或 <see cref="QueryByCombineOutTradeNoAsync"/>。</para>
    /// </remarks>
    [Post("/v3/repayment/combine-transactions/partner/prepay")]
    Task<RepaymentPrepayResponse> PrepayAsync(
        RepaymentPrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 平台商户「查询还款合单订单」：按合单商户订单号查合单状态与各子单支付结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015124483"/>。
    /// </summary>
    /// <param name="combineOutTradeNo">合单商户订单号（官方 path <c>combine_out_trade_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>合单概况与子单列表，见 <see cref="RepaymentQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/repayment/combine-transactions/partner/out-trade-no/{combine_out_trade_no}</c>；无 query 参数。</para>
    /// <para><b>定位键只有商家单号</b>：本域<b>没有</b>「按微信单号查单」的第二支
    /// （直连 / 服务商基础支付各有双路）⇒ 单号即唯一凭据，宿主必须持久化。</para>
    /// <para><b>子单 <c>trade_state</c> 只有三值</b>（SUCCESS / NOTPAY / CLOSED，见
    /// <see cref="RepaymentTradeStates"/>）：比基础支付的七值集合窄得多，
    /// <b>不得</b>复用基础支付的状态判定器。</para>
    /// <para><b><c>bank_type</c> 非银行卡时官方回 <c>OTHERS</c></b>（不是空值）⇒ 判空逻辑须按此写。</para>
    /// </remarks>
    [Get("/v3/repayment/combine-transactions/partner/out-trade-no/{combineOutTradeNo}")]
    Task<RepaymentQueryResponse> QueryByCombineOutTradeNoAsync(
        [Path] string combineOutTradeNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 平台商户「关闭还款单」：关闭合单下的指定子单。成功应答为 <b>204 No Content（无包体）</b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015124497"/>。
    /// </summary>
    /// <param name="combineOutTradeNo">合单商户订单号（官方 path <c>combine_out_trade_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（待关子单列表），字段见 <see cref="RepaymentCloseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回内容（204）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b>
    /// <c>/v3/repayment/combine-transactions/partner/out-trade-no/{combine_out_trade_no}/close</c>。</para>
    /// <para><b>204 无包体 ⇒ 返回裸 <see cref="Task"/></b>：本域唯一无 JSON 应答的端点
    /// （守卫 RP-B1 断言返回类型），业务失败仍以 4xx + <c>{"code","message"}</c> 呈现，
    /// 由 <c>[AllowAnyStatusCode]</c> 透传后判错。</para>
    /// <para><b>还款单号只在 path</b>：包体只有 <c>sub_orders</c>（<c>mchid</c> + <c>out_trade_no</c> 必填、
    /// <c>sub_mchid</c> 选填）⇒ 不要往包体里再塞 <c>combine_out_trade_no</c>。</para>
    /// <para><b>只能整单关闭，且五要素须与下单完全一致</b>（官方原文）：「还款单支付订单只能使用此
    /// 还款单关单 api 完成关单。还款单支付关单接口不支持关闭部分还款单，关单的还款单订单号、
    /// 还款单发起商户号、还款明细单个数、还款明细单订单号、子商户号必须与下单时完全一致」
    /// ⇒ 传<b>子集</b>（少一笔明细）会被拒，普通支付面的关单接口对本域无效。</para>
    /// </remarks>
    [Post("/v3/repayment/combine-transactions/partner/out-trade-no/{combineOutTradeNo}/close")]
    Task CloseAsync(
        [Path] string combineOutTradeNo,
        RepaymentCloseRequest request,
        CancellationToken cancellationToken = default);
}
