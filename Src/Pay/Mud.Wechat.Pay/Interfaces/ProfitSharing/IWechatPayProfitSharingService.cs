// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Bill;
using Mud.Wechat.Pay.DataModels.ProfitSharing;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「分账」域 SDK（APIv3 分账，<b>7 端点</b>：添加接收方 / 请求分账 / 查询分账结果 /
/// 请求分账回退 / 查询分账回退结果 / 解冻剩余资金 / 查询剩余待分金额）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（普通商户文档中心，2026-10-09 逐页核验，页面更新时间均为 2025.09.29）：
/// 请求分账 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524936"/>、
/// 添加分账接收方 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528995"/>、
/// 查询分账结果 <see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_2.shtml"/>。
/// 支持商户类型：<b>普通商户</b>（三页均标注）。
/// </para>
/// <para>
/// <b>✅ 本域现已 9 端点</b>（原「尚未覆盖」清单已随增量关闭）：请求分账 · 添加 / <b>删除</b>分账接收方 ·
/// 查询分账结果 · <b>请求分账回退</b> · <b>查询分账回退结果</b> · <b>解冻剩余资金</b> ·
/// <b>查询剩余待分金额</b> · <b>申请分账账单</b>。
/// </para>
/// <para>
/// <b>两个资源族别混</b>：「分账单」在 <c>/v3/profitsharing/orders…</c>，「回退单」在
/// <c>/v3/profitsharing/return-orders…</c> —— 回退<b>不是</b>分账单的子资源（实测纠正了先验预判）。
/// </para>
/// <para>
/// <b>回调面</b>：分账动态通知已支持（归回调包），且<b>必须</b>靠
/// <c>resource.original_type = profitsharing</c> 与其它产品线区分（分账复用交易的
/// <c>event_type</c>，不像支付分那样自带前缀）——这是本仓回调面已经踩实的一处形态差异。
/// </para>
/// <para>
/// <b>不要凭推断补路由</b>——官方新文档站不暴露兄弟页 <c>docId</c>，每个端点须按接口名检索到其页面再核验
/// （已实测可行路径：<c>/wiki/doc/apiv3/apis/chapter8_1_N.shtml</c> 家族逐页核对标题）。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "ProfitSharing", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayProfitSharingService
{
    /// <summary>
    /// 添加分账接收方。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528995"/>。
    /// </summary>
    /// <param name="request">添加接收方请求体，见 <see cref="ProfitSharingAddReceiverRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>已建立的接收方关系（官方应答与请求同字段），见 <see cref="ProfitSharingReceiver"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/profitsharing/receivers/add</c>；无 path / query 参数；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>幂等与上限</b>：官方明示「同一接收方重复添加」由微信侧处理；商户接收方数量<b>上限 2 万</b>，
    /// 达上限须先删除未使用的接收方。</para>
    /// <para><b>错误码</b>：<c>PARAM_ERROR</c> / <c>INVALID_REQUEST</c> / <c>SIGN_ERROR</c> /
    /// <c>NO_AUTH</c> / <c>FREQUENCY_LIMITED</c>（添加接收方频率过高）/ <c>SYSTEM_ERROR</c>。</para>
    /// <para><b>敏感字段</b>：<c>name</c> 须按官方用<b>微信支付公钥</b>（推荐）或平台证书公钥以
    /// <b>RSAES-OAEP</b> 加密，并把 <c>Wechatpay-Serial</c> 指向对应公钥 —— 该头由传输层按配置写入。</para>
    /// </remarks>
    [Post("/v3/profitsharing/receivers/add")]
    Task<ProfitSharingReceiver> AddReceiverAsync(
        [Body] ProfitSharingAddReceiverRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求分账。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524936"/>。
    /// </summary>
    /// <param name="request">请求分账请求体，见 <see cref="ProfitSharingOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>分账单（<b>异步受理</b>：通常先返回 <c>PROCESSING</c>），见 <see cref="ProfitSharingOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/profitsharing/orders</c>；无 path / query 参数。</para>
    /// <para>
    /// <b>异步处理模式（官方原文）</b>：接口采用异步处理，受理后先返回，最终结果须经
    /// <see cref="QueryOrderAsync"/> 查询或分账动态通知获取 ⇒ <b>调用方不得把受理成功当成分账到账</b>。
    /// </para>
    /// <para><b>幂等</b>：官方明示「同一分账单号（<c>out_order_no</c>）多次请求等同一次」。</para>
    /// <para><b>错误码</b>：<c>RULE_LIMIT</c>（超出最大分账比例）/ <c>NOT_ENOUGH</c>（分账金额不足）/
    /// <c>NO_AUTH</c> / <c>FREQUENCY_LIMITED</c>（同笔订单分账频率过高）等。</para>
    /// </remarks>
    [Post("/v3/profitsharing/orders")]
    Task<ProfitSharingOrderResponse> CreateOrderAsync(
        [Body] ProfitSharingOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询分账结果。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_2.shtml"/>。
    /// </summary>
    /// <param name="outOrderNo">商户分账单号（官方 path <c>out_order_no</c>，必填 string(64)）。</param>
    /// <param name="transactionId">微信支付订单号（官方 query <c>transaction_id</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>分账结果（与请求分账<b>同一应答形态</b>），见 <see cref="ProfitSharingOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/profitsharing/orders/{out_order_no}?transaction_id=…</c>；
    /// path 必带 <c>out_order_no</c>、query 必带 <c>transaction_id</c>（官方两参数均标注必填）。</para>
    /// <para>
    /// <b>占位符写法</b>：路由占位符用的是 <b>C# 参数名</b>（<c>{outOrderNo}</c>）而非官方 <c>out_order_no</c> ——
    /// 生成器<b>强制</b>「占位符 ↔ <c>[Path]</c> 参数名」一一对应（违反即 <c>HTTPCLIENT013</c>），
    /// 且占位符名<b>不出现在报文里</b>（只做替换），故对线上契约零影响。与交易域既有端点同款。
    /// </para>
    /// <para><b>一接口两用途（官方原文）</b>：既查分账结果，也查<b>解冻剩余资金</b>的执行结果
    /// （解冻请求本身走另一端点，本批未实现）。</para>
    /// <para><b>错误码</b>：含 <c>404 RESOURCE_NOT_EXISTS</c>（记录不存在 ⇒ 检查单号）、
    /// <c>429 FREQUENCY_LIMITED</c>（查询频率过高）。</para>
    /// </remarks>
    [Get("/v3/profitsharing/orders/{outOrderNo}")]
    Task<ProfitSharingOrderResponse> QueryOrderAsync(
        [Path] string outOrderNo,
        [Query("transaction_id")] string transactionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求分账回退。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_3.shtml"/>。
    /// </summary>
    /// <param name="request">回退请求体，见 <see cref="ProfitSharingReturnOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回退单（异步受理：通常先 <c>PROCESSING</c>），见 <see cref="ProfitSharingReturnOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/profitsharing/return-orders</c>；<b>无</b> path / query 参数。</para>
    /// <para><b>⚠️ 路由纠偏</b>：回退是<b>独立资源族</b>，不是 <c>/orders/{out_order_no}/return</c>
    /// （官方页面实测如此，勿凭直觉补）。</para>
    /// <para><b>二选一定位</b>：<c>order_id</c>（微信分账单号）与 <c>out_order_no</c>（商户分账单号）
    /// <b>二选一</b>填写，两者皆选填但不可都不填。</para>
    /// <para><b>回退方约束</b>：<c>return_mchid</c> <b>只能</b>是原分账单中某个接收方的商户号；
    /// <c>amount</c> 不得超过原分账金额。</para>
    /// </remarks>
    [Post("/v3/profitsharing/return-orders")]
    Task<ProfitSharingReturnOrderResponse> CreateReturnOrderAsync(
        [Body] ProfitSharingReturnOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询分账回退结果。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_4.shtml"/>。
    /// </summary>
    /// <param name="outReturnNo">商户回退单号（官方 path <c>out_return_no</c>，必填 string(64)）。</param>
    /// <param name="outOrderNo">商户分账单号（官方 query <c>out_order_no</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回退结果（与请求分账回退<b>同一应答形态</b>），见 <see cref="ProfitSharingReturnOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/profitsharing/return-orders/{out_return_no}?out_order_no=…</c>；
    /// path 必带 <c>out_return_no</c>、query 必带 <c>out_order_no</c>。</para>
    /// <para><b>终态判定</b>：<c>result</c> 取 <c>PROCESSING</c>（非终态）/ <c>SUCCESS</c> / <c>FAILED</c>；
    /// 与分账单的 <c>state</c>（<c>PROCESSING</c>/<c>FINISHED</c>）是<b>两套枚举</b>，勿混用。</para>
    /// </remarks>
    [Get("/v3/profitsharing/return-orders/{outReturnNo}")]
    Task<ProfitSharingReturnOrderResponse> QueryReturnOrderAsync(
        [Path] string outReturnNo,
        [Query("out_order_no")] string outOrderNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解冻剩余资金。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_5.shtml"/>。
    /// </summary>
    /// <param name="request">解冻请求体，见 <see cref="ProfitSharingUnfreezeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>分账单（<b>应答形态与请求分账相同</b> ⇒ 复用 <see cref="ProfitSharingOrderResponse"/>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/profitsharing/orders/unfreeze</c>；无 path / query 参数。</para>
    /// <para>
    /// <b>与「请求分账」的关系</b>：<c>unfreeze_unsplit=false</c> 允许对同一笔订单<b>多次</b>分账；
    /// 分完后调用本端点把<b>剩余</b>未分金额全部解冻给发起方 —— 一旦解冻即<b>不可再次分账</b>。
    /// 查结果用 <see cref="QueryOrderAsync"/>（官方该页注明它兼查解冻结果）。
    /// </para>
    /// <para><b>幂等</b>：官方原文「同一 <c>out_order_no</c> 多次请求等同一次」。</para>
    /// </remarks>
    [Post("/v3/profitsharing/orders/unfreeze")]
    Task<ProfitSharingOrderResponse> UnfreezeAsync(
        [Body] ProfitSharingUnfreezeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询剩余待分金额。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_6.shtml"/>。
    /// </summary>
    /// <param name="transactionId">微信支付订单号（官方 path <c>transaction_id</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>剩余待分金额（<c>unsplit_amount</c>，单位分），见 <see cref="ProfitSharingAmountsResponse"/>。</returns>
    /// <remarks>
    /// <b>官方契约</b>：<b>GET</b> <c>/v3/profitsharing/transactions/{transaction_id}/amounts</c>；
    /// 仅 path 参数 <c>transaction_id</c>，无 body / query。<b>用途</b>：分账前先探余额，
    /// 避免 <c>NOT_ENOUGH</c>（分账金额不足）类拒绝（官方常见问题即以此接口确认剩余可分金额）。
    /// </remarks>
    [Get("/v3/profitsharing/transactions/{transactionId}/amounts")]
    Task<ProfitSharingAmountsResponse> QueryUnsplitAmountAsync(
        [Path] string transactionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请分账账单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529628"/>。
    /// </summary>
    /// <param name="billDate">账单日期（官方 query <c>bill_date</c>，必填 string(10)，格式 <c>yyyy-MM-dd</c>）。</param>
    /// <param name="tarType">压缩类型（官方 query <c>tar_type</c>，选填，取值 <c>GZIP</c>；不传则返回原始文件）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>账单下载信息（<c>hash_type</c> / <c>hash_value</c> / <c>download_url</c>），见 <see cref="BillDownloadInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/profitsharing/bills?bill_date=…[&amp;tar_type=…]</c>；
    /// 无 path / body 参数（2026-10-09 核验，更新时间 2025.09.29）。</para>
    /// <para>
    /// <b>应答为何复用 <see cref="BillDownloadInfoResponse"/></b>：官方本页应答字段表为
    /// <c>hash_type</c> / <c>hash_value</c> / <c>download_url</c>，与「申请交易账单」「申请资金账单」
    /// <b>逐项相同</b> —— 三者本就是同一个「账单下载地址」协议，另设类型只会让同一份事实在两处漂移。
    /// </para>
    /// <para>
    /// <b>文件下载复用账单通道</b>：<c>download_url</c> 指向的文件仍须经
    /// <c>IWechatPayBillDownloadService</c> 下载（它带<b>主机白名单前置闸</b>，防 <c>download_url</c> 被替换）。
    /// 该服务随 <c>PayModule.Bill</c> 注册 —— 只用分账域而需下载文件的宿主，须一并启用账单域
    /// （或自行注册该端口）。<b>本端点只返回地址，不下载</b>。
    /// </para>
    /// <para><b>时限</b>：官方原文「只能下载<b>三个月以内</b>的账单」；金额单位为<b>元</b>。</para>
    /// </remarks>
    [Get("/v3/profitsharing/bills")]
    Task<BillDownloadInfoResponse> GetBillAsync(
        [Query("bill_date")] string billDate,
        [Query("tar_type")] string? tarType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除分账接收方。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529590"/>。
    /// </summary>
    /// <param name="request">删除请求体，见 <see cref="ProfitSharingDeleteReceiverRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>被删除的接收方身份（<b>仅</b> <c>type</c> / <c>account</c>），见 <see cref="ProfitSharingDeleteReceiverResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/profitsharing/receivers/delete</c>；无 path / query 参数。</para>
    /// <para><b>⚠️ 参数必填性</b>：本接口 <c>appid</c> <b>必填</b>（与「请求分账」中它的选填语义<b>不同</b>，
    /// 官方原样，勿统一）。</para>
    /// <para><b>业务含义</b>：删除后不再支持把该商户结算后的资金分给该接收方；商户接收方上限 2 万，
    /// 达上限须先删除未使用项。</para>
    /// </remarks>
    [Post("/v3/profitsharing/receivers/delete")]
    Task<ProfitSharingDeleteReceiverResponse> DeleteReceiverAsync(
        [Body] ProfitSharingDeleteReceiverRequest request,
        CancellationToken cancellationToken = default);
}
