// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台收付通 · 分账与补差」域 SDK（<b>11 端点</b>：请求分账 + 查分账结果 + 查剩余待分金额 +
/// 分账完结 + 接收方增删 + 分账回退两支 + 补差三支）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-15 逐页逐字段核验）：请求分账
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/>、查询分账结果
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477734"/>、查询剩余待分金额
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477751"/>、分账完结
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477745"/>、添加接收方
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477758"/>、删除接收方
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477759"/>、请求分账回退
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477737"/>、查询回退结果
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477740"/>、发起补差
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477631"/>、补差回退
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477636"/>、取消补差
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477639"/>。</para>
/// <para><b>本域身份是分裂的，不是笔误</b>：分账八页标【平台商户】，而<b>补差三页标【普通服务商】</b>。
/// 补差能力由服务商侧开通、资金动作仍发生在收付通订单上 ⇒ 同一个 <c>/v3/ecommerce/subsidies/*</c> 前缀下
/// 的签名主体口径与分账不同。守卫 EC-P7 把「八支平台商户 + 三支普通服务商」钉成事实，防止后续按同域一刀切。</para>
/// <para><b>与直连分账不共用 DTO</b>：<see cref="IWechatPayProfitSharingService"/> 走 <c>/v3/profitsharing/*</c>
/// 且身份字段是 <c>receiver_account</c>+<c>mchid</c> 单层；本域全部报文带 <c>sub_mchid</c>、请求分账的接收方对象是
/// <c>receivers[].receiver_account</c>，且多出 <c>finish</c> 与 <c>description</c> 口径 ⇒ 两侧路由、字段、状态枚举都不同。</para>
/// <para><b>「受理 ≠ 结果」是本域的主线</b>：请求分账与分账完结都是<b>异步处理</b>（官方原文「先受理请求再异步处理」），
/// <see cref="EcommerceProfitSharingFinishResponse"/> 官方只回四行、<b>没有结果状态</b> ⇒ 完结成功与否只能回到
/// <see cref="QueryProfitSharingOrderAsync"/> 读 <c>state</c>（见 <see cref="EcommerceSplitOrderStatuses"/>）；
/// 而分账回退与补差三支是<b>同步处理</b>、实时返回结果。同一域内两种处理模式并存，按支区分、不能一刀切。</para>
/// <para><b>180 天是本域三条独立时限</b>，常被混为一谈：① 分账资金冻结期默认 180 天，自订单支付成功起
/// 未发起分账则待分资金<b>自动解冻给分账方</b>；② 分账回退的时限是 180 天；③ 补差重入有效期 180 天。
/// 各自起算点不同 ⇒ 过期后没有任何接口可以补救。</para>
/// <para><b>次数上限</b>：同一笔订单最多发起 <b>50</b> 次分账请求、每次最多 <b>50</b> 个接收方；
/// 同一笔分账单最多 <b>50</b> 次回退；一笔订单<b>只能补差一笔</b>、一笔微信退款单<b>只允许补差回退一次</b>；
/// 分账接收方数量上限 <b>2 万</b>（平台维度统一维护，非二级商户维度）。</para>
/// <para><b>频率上限</b>（官方逐页给出，是本线少见的多级限频）：请求分账「单个平台商户 <b>2000 QPS</b>」
/// 且「单个交易收款商户 <b>300 QPS</b>」；分账完结「单个分账接收方商户 <b>60 QPS</b>」；
/// 其余各页只列通用 429 <c>FREQUENCY_LIMITED</c>。超限即 429，官方建议拆分商户号避免集中。</para>
/// <para><b>幂等与「不得换单」</b>：请求分账官方明文「支持幂等重入」；补差与补差回退遇到 <c>SYSTEM_ERROR</c>
/// <b>必须用原参数、原单号重入</b>（官方原文「如更换金额重试，可能会导致重复扣款，系统会在 1 天后回退到原账户」）
/// ⇒ SDK 不做任何自动重试与单号改写。</para>
/// <para><b>加密面只覆盖请求分账一支</b>：只有 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/>
/// 把 <c>Wechatpay-Serial</c> 列为参数表行，密文字段是 <c>receivers[].receiver_name</c>（个人接收方姓名）⇒
/// 本接口其余十支不得支配该头（守卫 EC-P6）。分账接收方为个人时官方还要求「电商平台须确保已向微信支付
/// 传输用户身份信息做一致性校验并合法征得用户授权」⇒ 授权合规责任在宿主，SDK 不代替判断。</para>
/// <para><b>回退资格是硬条件</b>：仅支持「分账给 <c>MERCHANT_ID</c> 商户号类型」且分账单
/// <c>state=FINISHED</c>、<c>result=SUCCESS</c> 的分账单；<b>不支持对「分账给个人」的分账单发起回退</b>；
/// 商户类型接收方已自动开通回退能力。回退与退款无强耦合（余额充足时可直接退款）⇒ 是否回退由平台决定。</para>
/// <para><b>客户端与凭据</b>：走支付专用命名客户端 <c>wechat-pay</c>，签名主体是当前作用域商户号；
/// 本域<b>零 <c>[Token]</c></b>。<c>[AllowAnyStatusCode]</c> 使 APIv3 的 4xx/5xx 错误体原样落到
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Ecommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEcommerceProfitSharingService
{
    /// <summary>
    /// 「请求分账」：平台商户代二级商户把结算后的资金分给分账接收方（<b>异步受理</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/>。
    /// </summary>
    /// <param name="request">分账请求，见 <see cref="EcommerceProfitSharingOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>分账单（受理态），见 <see cref="EcommerceProfitSharingOrderResponse"/>（与查询支<b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/profitsharing/orders</c>；限频 2000 QPS（平台商户）
    /// / 300 QPS（交易收款商户）。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：<c>receivers[].receiver_name</c> 是密文字段。</para>
    /// <para><b>异步处理</b>：应答只代表受理，最终结果用 <see cref="QueryProfitSharingOrderAsync"/> 读
    /// <c>state</c> + <c>receivers[].result</c>。同一笔订单最多 50 次分账、每次最多 50 个接收方。</para>
    /// <para><b>官方支持幂等重入</b>（同参数重复请求等同一笔）⇒ 重试请保持参数原样，不要换 <c>out_order_no</c>。</para>
    /// </remarks>
    [Post("/v3/ecommerce/profitsharing/orders")]
    Task<EcommerceProfitSharingOrderResponse> CreateProfitSharingOrderAsync(
        [Body] EcommerceProfitSharingOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询分账结果」：按 <c>sub_mchid</c> + <c>transaction_id</c> + <c>out_order_no</c> 三键查分账单，
    /// 也是<b>分账完结结果的唯一观察点</b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477734"/>。
    /// </summary>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="transactionId">微信支付订单号（官方 query <c>transaction_id</c>，<b>必填</b> string(32)）。</param>
    /// <param name="outOrderNo">商户分账单号（官方 query <c>out_order_no</c>，<b>必填</b> string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与请求分账<b>同一应答类型</b>，见 <see cref="EcommerceProfitSharingOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/profitsharing/orders?sub_mchid=…&amp;transaction_id=…&amp;out_order_no=…</c>；
    /// <b>三个 query 全部必填</b>（官方没有「只给 transaction_id 查全部」的形态）。</para>
    /// <para><b>本支同时承载「完结结果」</b>：官方原文「发起分账完结请求后，可调用此接口查询分账完结的结果」⇒
    /// <see cref="FinishOrderAsync"/> 之后仍以本支读数，不要为完结另建查询。</para>
    /// </remarks>
    [Get("/v3/ecommerce/profitsharing/orders")]
    Task<EcommerceProfitSharingOrderResponse> QueryProfitSharingOrderAsync(
        [Query("sub_mchid")] string subMchId,
        [Query("transaction_id")] string transactionId,
        [Query("out_order_no")] string outOrderNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询订单剩余待分金额」：这笔微信订单还能分多少（单位分）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477751"/>。
    /// </summary>
    /// <param name="transactionId">微信支付订单号（官方 path <c>transaction_id</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>剩余待分金额，见 <see cref="EcommerceUnsplitAmountResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/profitsharing/orders/{transaction_id}/amounts</c>；
    /// 无 query、无包体。<b>本支是官方少见的「金额单值」应答</b> ⇒ 用于决定补差/分账额度，
    /// 但不代表解冻状态（解冻看 <see cref="QueryProfitSharingOrderAsync"/> 的 <c>state</c>）。</para>
    /// </remarks>
    [Get("/v3/ecommerce/profitsharing/orders/{transactionId}/amounts")]
    Task<EcommerceUnsplitAmountResponse> QueryUnsplitAmountAsync(
        [Path] string transactionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「分账完结（解冻剩余资金）」：把订单剩余金额全部解冻给二级商户（<b>异步受理、应答无状态</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477745"/>。
    /// </summary>
    /// <param name="request">完结请求，见 <see cref="EcommerceProfitSharingFinishRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>只回四行、<b>没有结果状态</b>，见 <see cref="EcommerceProfitSharingFinishResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/profitsharing/finish-order</c>；
    /// 限频「单个分账接收方商户 60 QPS」。</para>
    /// <para><b>方案里这一行重复出现两次（row 38/39）是同一端点</b> ⇒ 本域只声明一个方法，
    /// 守卫 EC-P1 锁定 <c>(POST, /v3/ecommerce/profitsharing/finish-order)</c> 唯一。</para>
    /// <para><b>不可逆</b>：完结后剩余资金全额解冻、该订单不能再分账 ⇒ 误调没有补救接口。
    /// 结果一律回到 <see cref="QueryProfitSharingOrderAsync"/> 确认。</para>
    /// </remarks>
    [Post("/v3/ecommerce/profitsharing/finish-order")]
    Task<EcommerceProfitSharingFinishResponse> FinishOrderAsync(
        [Body] EcommerceProfitSharingFinishRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「添加分账接收方」：在<b>电商平台维度</b>维护接收方列表（其他二级商户共用该列表）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477758"/>。
    /// </summary>
    /// <param name="request">添加请求，见 <see cref="EcommerceAddReceiverRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>只回 <c>type</c> + <c>account</c>，见 <see cref="EcommerceReceiverResponse"/>（增删两支<b>共用</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/profitsharing/receivers/add</c>；页内无频率数字。</para>
    /// <para><b>接收方归属平台而非二级商户</b>：官方明文「添加的分账接收方统一在电商平台维度管理，
    /// 其他二级商户均可向该列表中的接收方分账」⇒ 不要按二级商户重复添加（数量上限 2 万）。</para>
    /// <para><b>个人接收方须加密姓名并确认授权</b>：<c>name</c> 官方标选填但为个人类型时须密文；
    /// 本页参数表无 <c>Wechatpay-Serial</c> ⇒ 加密口径以官方字段说明为准（守卫 EC-P6 记录「仅请求分账页列该头」）。</para>
    /// </remarks>
    [Post("/v3/ecommerce/profitsharing/receivers/add")]
    Task<EcommerceReceiverResponse> AddReceiverAsync(
        [Body] EcommerceAddReceiverRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除分账接收方」：删除后不能再把二级商户的资金分给该接收方。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477759"/>。
    /// </summary>
    /// <param name="request">删除请求，见 <see cref="EcommerceDeleteReceiverRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与添加支<b>同一应答类型</b>，见 <see cref="EcommerceReceiverResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/profitsharing/receivers/delete</c>；页内无频率数字。</para>
    /// <para><b>删除不回溯历史分账单</b>：已完成的分账不受影响，但删除后同一 <c>account</c> 再想收款须重新添加
    /// ⇒ 与 2 万上限一起构成本域的容量治理问题。</para>
    /// </remarks>
    [Post("/v3/ecommerce/profitsharing/receivers/delete")]
    Task<EcommerceReceiverResponse> DeleteReceiverAsync(
        [Body] EcommerceDeleteReceiverRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「请求分账回退」：退款前把已分给接收方的资金退回分账方（<b>同步返回结果</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477737"/>。
    /// </summary>
    /// <param name="request">回退请求，见 <see cref="EcommerceProfitSharingReturnRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回退单，见 <see cref="EcommerceProfitSharingReturnResponse"/>（与查询支<b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/profitsharing/returnorders</c>；页内无频率数字。</para>
    /// <para><b>资格三条缺一不可</b>：接收方类型是 <c>MERCHANT_ID</c>、分账单 <c>state=FINISHED</c>、
    /// <c>result=SUCCESS</c>；<b>分账给个人的分账单不支持回退</b>。累计回退金额不得超过原分给该接收方的金额，
    /// 同一笔分账单最多 50 次回退，时限 180 天。</para>
    /// <para><b>同步处理</b>：与请求分账相反，本支实时返回结果；若仍是「处理中」则用
    /// <see cref="QueryProfitSharingReturnAsync"/> 复核。<b>回退与退款无强耦合</b>（余额充足可直接退款）⇒
    /// 是否回退由平台决定，SDK 不做隐式串联。</para>
    /// </remarks>
    [Post("/v3/ecommerce/profitsharing/returnorders")]
    Task<EcommerceProfitSharingReturnResponse> CreateProfitSharingReturnAsync(
        [Body] EcommerceProfitSharingReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询分账回退结果」：核实回退单的最终状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477740"/>。
    /// </summary>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="outReturnNo">商户回退单号（官方 query <c>out_return_no</c>，<b>必填</b> string(64)）。</param>
    /// <param name="orderId">微信分账订单号（官方 query <c>order_id</c>，选填 string(64)）。</param>
    /// <param name="outOrderNo">商户分账单号（官方 query <c>out_order_no</c>，选填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与请求回退<b>同一应答类型</b>，见 <see cref="EcommerceProfitSharingReturnResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/profitsharing/returnorders?sub_mchid=…&amp;out_return_no=…</c>；
    /// 方案里这一行也重复出现两次（row 45/47）⇒ 单方法承载，守卫 EC-P1 锁定唯一。</para>
    /// <para><b>只有 <c>sub_mchid</c> 与 <c>out_return_no</c> 必填</b>，<c>order_id</c>/<c>out_order_no</c> 是选填辅助定位
    /// ⇒ 不要要求调用方两者都传。</para>
    /// </remarks>
    [Get("/v3/ecommerce/profitsharing/returnorders")]
    Task<EcommerceProfitSharingReturnResponse> QueryProfitSharingReturnAsync(
        [Query("sub_mchid")] string subMchId,
        [Query("out_return_no")] string outReturnNo,
        [Query("order_id")] string? orderId = null,
        [Query("out_order_no")] string? outOrderNo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「发起补差」：平台向二级商户订单补入差额（<b>与普通服务商身份绑定，一笔订单只能补一次</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477631"/>。
    /// </summary>
    /// <param name="request">补差请求，见 <see cref="EcommerceCreateSubsidyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>补差单，见 <see cref="EcommerceCreateSubsidyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/subsidies/create</c>，「支持商户」【普通服务商】。</para>
    /// <para><b>时序前置</b>：必须「下单带补差标识 → 支付成功并完成结算 → 分账之前」调用；补差金额须与下单约定一致
    /// （用户退款后可小于下单金额），事后不可提额。</para>
    /// <para><b>重入有效期 180 天</b>；<c>SYSTEM_ERROR</c> <b>必须原参数重入</b>——换金额重试可能重复扣款
    /// （系统 1 天后回退到原账户）。</para>
    /// </remarks>
    [Post("/v3/ecommerce/subsidies/create")]
    Task<EcommerceCreateSubsidyResponse> CreateSubsidyAsync(
        [Body] EcommerceCreateSubsidyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「补差回退」：订单退款时把补差成功的资金退回平台（<b>同步处理，一笔退款单只允许回退一次</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477636"/>。
    /// </summary>
    /// <param name="request">回退请求，见 <see cref="EcommerceReturnSubsidyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>补差回退单，见 <see cref="EcommerceReturnSubsidyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/subsidies/return</c>，「支持商户」【普通服务商】。</para>
    /// <para><b>累计回退不得超过补差金额</b>，且<b>一笔微信退款单只允许补差回退一次</b> ⇒
    /// <c>refund_id</c> 是天然幂等边界，重试请保持原商户回退单号与原参数。</para>
    /// <para><b>本支失败拼写是 <c>FAIL</c></b>（<see cref="EcommerceSubsidyReturnResults"/>），
    /// 与回补垫付支的 <c>FAILED</c> 不同 ⇒ 两个常量类不合并、判错逻辑不得跨支复用。</para>
    /// </remarks>
    [Post("/v3/ecommerce/subsidies/return")]
    Task<EcommerceReturnSubsidyResponse> ReturnSubsidyAsync(
        [Body] EcommerceReturnSubsidyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「取消补差」：带补差标识的订单若不需补差，可在分账前取消。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477639"/>。
    /// </summary>
    /// <param name="request">取消请求，见 <see cref="EcommerceCancelSubsidyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>取消结果，见 <see cref="EcommerceCancelSubsidyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/subsidies/cancel</c>，「支持商户」【普通服务商】。</para>
    /// <para><b>前置条件是「订单发生退款」</b>（官方原文第 3 条）⇒ 未退款的订单取消补差会被拒；
    /// 取消完成后才能对未补差的订单分账。</para>
    /// </remarks>
    [Post("/v3/ecommerce/subsidies/cancel")]
    Task<EcommerceCancelSubsidyResponse> CancelSubsidyAsync(
        [Body] EcommerceCancelSubsidyRequest request,
        CancellationToken cancellationToken = default);
}
