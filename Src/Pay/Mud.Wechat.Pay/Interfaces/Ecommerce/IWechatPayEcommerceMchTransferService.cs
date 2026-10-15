// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台收付通 · 二级商户商家转账」域 SDK（<b>6 端点</b>：授权申请/查询 + 发起转账 + 两支查单 + 撤销）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-15 逐页逐字段核验）：申请授权
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504208"/>、查询授权状态
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504209"/>、发起转账
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504211"/>、按商户单号查单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504213"/>、撤销转账
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504212"/>、按微信单号查单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504214"/>。</para>
/// <para><b>授权是转账的前置闸门，且申请成功不等于已授权</b>：官方在授权状态里给了中间态
/// <see cref="EcommerceTransferAuthorizationStates.AuthorizeProcessing"/>（「授权申请正在处理中，请稍后查询授权结果」）
/// ⇒ 必须先 <see cref="ApplyTransferAuthorizationAsync"/> 再 <see cref="QueryTransferAuthorizationAsync"/> 轮询到
/// <c>AUTHORIZED</c> 才能发起转账。<b>本域的授权状态是三值 <c>authorization_state</c>，与个人收款方签约面的
/// 两值 <c>auth_state</c>（见 <see cref="EcommerceAuthStates"/>）字段名与值集都不得互换</b>（守卫 EC-M1）。</para>
/// <para><b>两支查单同应答、与受理应答不同</b>：<c>4013504213</c> 与 <c>4013504214</c> 应答表 14 行<b>逐行相同</b>
/// ⇒ 共用 <see cref="EcommerceMchTransferBillQueryResponse"/>；而发起转账的受理应答（7 行）比查单少
/// <c>update_time</c> 却多出 <c>package_info</c>，撤销应答（5 行）只回 <c>state</c> 两值 ⇒ 三支三型，不做超集。</para>
/// <para><b><c>package_info</c> 只出现在受理应答</b>：它是「拉起微信收款确认页面」所需的参数，官方在两支查单应答里
/// <b>不返回</b>该字段 ⇒ 错过受理应答就再也取不到，须自行持久化（守卫 EC-M2）。</para>
/// <para><b>转账是「用户确认收款」型资金动作，状态机有八个值</b>：见 <see cref="EcommerceMchTransferBillStates"/>
/// （<c>ACCEPTED</c> → <c>PROCESSING</c> / <c>WAIT_USER_CONFIRM</c> → <c>TRANSFERING</c> → <c>SUCCESS</c> / <c>FAIL</c>，
/// 撤销走 <c>CANCELING</c> → <c>CANCELLED</c>）。官方对 <c>PROCESSING</c> 的原文提示是
/// 「如一直处于此状态，建议检查账户余额是否足够」⇒ 长时间 <c>PROCESSING</c> 是资金问题而不是网络问题。</para>
/// <para><b>撤销受理 ≠ 撤销完成</b>：官方原文「该接口返回成功仅表示撤销请求已受理，系统会异步处理退款等操作，
/// 以最终查询单据返回状态为准」，且「在<b>用户确认收款之前</b>可以通过该接口撤销付款」⇒ 用户已确认后撤销必失败；
/// 撤销应答的 <c>state</c> 只有 <c>CANCELING</c>/<c>CANCELLED</c> 两值，须回查 <see cref="QueryMchTransferBillByOutBillNoAsync"/> 收敛。</para>
/// <para><b>本域加密面恰一支</b>：只有 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504211"/> 把
/// <c>Wechatpay-Serial</c> 列为参数表行，密文入参是 <c>user_name</c>（string(1024)，标准 RSA 与国密均支持，
/// 公钥由微信侧提供）⇒ 其余五支不得支配该头（守卫 EC-M3）。该字段是收款人真实姓名，
/// <b>绝不写入日志、遥测或异常消息</b>；加密属宿主证书资产，SDK 不代为加密。</para>
/// <para><b>条件必填的阈值以元表述、金额以分传递</b>：官方原文「转账金额 &gt;= 2,000 元时，该笔明细必须填写
/// <c>user_name</c>」⇒ 判断须换算为 <c>200000</c> 分；传入后微信支付校验收款用户与姓名是否一致并提供电子回单。
/// SDK 不加本地闸门（阈值可能随官方调整，硬编码即造出误拒）。</para>
/// <para><b><c>out_bill_no</c> 字符集比本域退款单号严格</b>：官方要求「只能由数字、大小写字母组成」，
/// 不含 <c>- _ | * @</c>；同域 <c>out_refund_no</c> 却允许这些符号 ⇒ 复用同一套单号生成器会在此报错。</para>
/// <para><b><c>appid</c> 必须是 <c>openid</c> 所属的那个 AppID</b>（官方：「请填写收款用户 OpenID 对应的 AppID」）
/// ⇒ AppID 与 OpenID 不匹配时官方直接校验失败；<c>transfer_scene_id</c> 须在「平台商家转账 - 产品设置」中已获取，
/// <c>notify_url</c> 须公网可访问、<b>必须 HTTPS</b> 且不能携带参数。</para>
/// <para><b>各页未给出频率数字</b>：本域六页只列通用错误码表（含 500 <c>SYSTEM_ERROR</c>「系统异常，请稍后重试」）
/// ⇒ SDK 侧不做限流与自动重试，重入策略由宿主按单号幂等保证。</para>
/// <para><b>客户端与凭据</b>：走支付专用命名客户端 <c>wechat-pay</c>，签名主体是当前作用域商户号；
/// 本域<b>零 <c>[Token]</c></b>。<c>[AllowAnyStatusCode]</c> 使 APIv3 的 4xx/5xx 错误体原样落到
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Ecommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEcommerceMchTransferService
{
    /// <summary>
    /// 「申请二级商户商家转账授权」：上送已线下签署的转账授权协议清单（<b>受理语义，回 200 不等于已授权</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504208"/>。
    /// </summary>
    /// <param name="request">授权申请，见 <see cref="EcommerceTransferAuthorizationRequest"/>（<c>agreement_list</c> 承载协议编号/版本/签约机构/签约时间）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权状态，见 <see cref="EcommerceTransferAuthorizationResponse"/>（与 <see cref="QueryTransferAuthorizationAsync"/> 共用）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/mch-transfer/authorizations</c>；无加密头。</para>
    /// <para><b>须再查单收敛</b>：官方值集含 <c>AUTHORIZE_PROCESSING</c> ⇒ 拿到本应答后须用
    /// <see cref="QueryTransferAuthorizationAsync"/> 轮询到 <c>AUTHORIZED</c> 才能发起转账。</para>
    /// <para><b><c>accept_time</c> 只在本页文档化</b>（查询页未列出）⇒ 勿以该字段是否出现来判断走的是哪一支。</para>
    /// </remarks>
    [Post("/v3/ecommerce/mch-transfer/authorizations")]
    Task<EcommerceTransferAuthorizationResponse> ApplyTransferAuthorizationAsync(
        [Body] EcommerceTransferAuthorizationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询二级商户商家转账授权状态」：转账发起前的前置校验入口。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504209"/>。
    /// </summary>
    /// <param name="subMchId">二级商户号（官方 path <c>sub_mchid</c>，必填 string(32)）：需授权商家转账功能的商户号。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与申请支<b>共用同一应答类型</b>，见 <see cref="EcommerceTransferAuthorizationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/mch-transfer/authorizations/{sub_mchid}</c>；
    /// 二级商户号走 <b>path</b>（申请支走 body）⇒ 同一凭据两种传法，照页实现。</para>
    /// <para><b><c>UNAUTHORIZED</c> 是「未授权」而非「授权失败」</b>：三值语义见 <see cref="EcommerceTransferAuthorizationStates"/>。</para>
    /// </remarks>
    [Get("/v3/ecommerce/mch-transfer/authorizations/{subMchId}")]
    Task<EcommerceTransferAuthorizationResponse> QueryTransferAuthorizationAsync(
        [Path] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「发起商家转账」：平台商户代二级商户向用户 openid 转账，等待用户在微信收款确认页面确认收款。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504211"/>。
    /// </summary>
    /// <param name="request">转账请求，见 <see cref="EcommerceTransferBillCreateRequest"/>（<c>user_name</c> 为<b>密文</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账受理单，见 <see cref="EcommerceTransferBillCreateResponse"/>（含<b>仅此支返回</b>的 <c>package_info</c>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/mch-transfer/transfer-bills</c>。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：本方法是本域唯一需要该头的端点，<c>user_name</c>
    /// 须以微信支付公钥（推荐）或平台证书公钥加密；转账金额 &gt;= 2,000 元（即 200000 分）时该字段<b>必填</b>。</para>
    /// <para><b>前置：授权须已是 <c>AUTHORIZED</c></b>（<see cref="QueryTransferAuthorizationAsync"/>）。</para>
    /// <para><b><c>package_info</c> 须自行持久化</b>：用于拉起微信收款确认页面，两支查单<b>不再返回</b>该字段。</para>
    /// </remarks>
    [Post("/v3/ecommerce/mch-transfer/transfer-bills")]
    Task<EcommerceTransferBillCreateResponse> CreateTransferBillAsync(
        [Body] EcommerceTransferBillCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按商户单号查询商家转账订单」：转账终态与失败原因的唯一权威来源。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504213"/>。
    /// </summary>
    /// <param name="outBillNo">商户单号（官方 path <c>out_bill_no</c>，必填 string(32)）：只能是数字与大小写字母，商户系统内唯一。</param>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账单全量（14 行），见 <see cref="EcommerceMchTransferBillQueryResponse"/>（与 <see cref="QueryMchTransferBillByTransferBillNoAsync"/><b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}?sub_mchid=</c>；
    /// <c>sub_mchid</c> 是<b>必填 query</b>，漏传即 400 <c>PARAM_ERROR</c>。</para>
    /// <para><b><c>fail_reason</c> 是选填</b>：官方只在失败态给出，缺省不代表无失败；
    /// <c>PROCESSING</c> 长期不收敛时官方建议检查账户余额。</para>
    /// </remarks>
    [Get("/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}")]
    Task<EcommerceMchTransferBillQueryResponse> QueryMchTransferBillByOutBillNoAsync(
        [Path] string outBillNo,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「撤销转账」：在<b>用户确认收款之前</b>撤销付款请求（<b>异步受理</b>，返回成功只表示撤销请求已受理）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504212"/>。
    /// </summary>
    /// <param name="outBillNo">商户单号（官方 path <c>out_bill_no</c>，必填 string(32)）：待撤销的转账单。</param>
    /// <param name="request">撤销请求，见 <see cref="EcommerceTransferBillCancelRequest"/>（官方入参只有 <c>sub_mchid</c> 一行）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>撤销受理结果（<c>state</c> 只有 <c>CANCELING</c>/<c>CANCELLED</c> 两值），见 <see cref="EcommerceTransferBillCancelResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}/cancel</c>；
    /// <c>sub_mchid</c> 走 <b>body</b>（两支查单走 query）⇒ 同一凭据三种传法，照页实现。</para>
    /// <para><b>窗口只在用户确认收款之前</b>：官方原文「在用户确认收款之前可以通过该接口撤销付款」⇒
    /// 状态已是 <c>TRANSFERING</c>/<c>SUCCESS</c> 时撤销必失败。</para>
    /// <para><b>受理 ≠ 完成</b>：官方原文「系统会异步处理退款等操作，以最终查询单据返回状态为准」⇒
    /// 须回查 <see cref="QueryMchTransferBillByOutBillNoAsync"/> 直到 <c>CANCELLED</c>。</para>
    /// </remarks>
    [Post("/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel")]
    Task<EcommerceTransferBillCancelResponse> CancelTransferBillAsync(
        [Path] string outBillNo,
        [Body] EcommerceTransferBillCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按微信转账单号查询商家转账订单」：与 <see cref="QueryMchTransferBillByOutBillNoAsync"/> 同一应答类型，只是定位键不同。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504214"/>。
    /// </summary>
    /// <param name="transferBillNo">微信转账单号（官方 path <c>transfer_bill_no</c>，必填 string(64)）。</param>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按商户单号查单<b>共用同一应答类型</b>，见 <see cref="EcommerceMchTransferBillQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/mch-transfer/transfer-bills/transfer-bill-no/{transfer_bill_no}?sub_mchid=</c>。</para>
    /// <para><b>两页应答表 14 行逐行相同</b>（2026-10-15 比对）⇒ 共用一个类型；
    /// 本域不含「转账列表」面，官方无按商户号枚举转账单的端点。</para>
    /// </remarks>
    [Get("/v3/ecommerce/mch-transfer/transfer-bills/transfer-bill-no/{transferBillNo}")]
    Task<EcommerceMchTransferBillQueryResponse> QueryMchTransferBillByTransferBillNoAsync(
        [Path] string transferBillNo,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);
}
