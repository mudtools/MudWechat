// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.ProfitSharing;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「分账」域 SDK（APIv3 分账，<b>本批 3 端点</b>：添加接收方 / 请求分账 / 查询分账结果）。
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
/// <b>本域尚未覆盖的端点</b>（官方「分账 → API 列表」中确有此页，但本批未实现，须先逐页核验再增量）：
/// 请求分账回退、查询分账回退结果、解冻剩余资金、查询剩余待分金额、删除分账接收方、申请分账账单、
/// 分账动态通知（回调，归回调包）。
/// <b>不要凭推断补路由</b>——官方新文档站不暴露兄弟页 <c>docId</c>，每个端点须按接口名检索到其页面再核验。
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
}
