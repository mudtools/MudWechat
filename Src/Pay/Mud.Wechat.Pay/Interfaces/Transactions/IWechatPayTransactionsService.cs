// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「基础交易」域 SDK（APIv3 下单 / 查单 / 关单，7 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（普通商户文档中心，2026-10-09 逐页核验）：
/// JSAPI/小程序下单 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>（同内容另有 <c>4012791856</c>）、
/// 微信支付订单号查询 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791899"/>、
/// 商户订单号查询 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791900"/>、
/// 关闭订单 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791901"/>。</para>
/// <para><b>下单四族</b>：JSAPI（<c>4012791897</c>，已核验）/ Native（<c>4012791877</c>）/
/// APP（<c>4013070347</c>）/ H5（<c>4012791834</c>）—— 后三者的路由与字段表以本地 SKIT
/// <c>TenpayV3</c> 为对齐基准（2026-10-10），<b>官方逐页核验待补</b>（文档中心为 SPA，正文不可达）。
/// 四族<b>各自一个方法 + 各自请求 DTO</b>，差异见各族 DTO 的 remarks。</para>
/// <para><b>与其它产品线的根本差异 —— 无 access_token</b>：APIv3 全程以商户 API 证书做
/// RSA-SHA256 请求签名，故本接口<b>不声明 <c>[Token]</c></b>（守卫 PAY-B1 fail-closed 锁定，
/// 违反即 MUD005 与 Query 白名单漂移）。签名在传输层
/// <c>WechatPayAuthorizationHandler</c> 完成，端点方法只管路由与报文。</para>
/// <para><b>客户端</b>：走支付专用 <c>IWechatPayHttpClient</c>（命名客户端 <c>wechat-pay</c>，
/// BaseAddress 恒为 <c>https://api.mch.weixin.qq.com</c>）——不复用 <c>IEnhancedHttpClient</c>
/// 默认实例，否则会与企微线的 <c>qyapi.weixin.qq.com</c> 撞 BaseUrl 并给企微请求套上商户签名头。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 <b>HTTP 4xx/5xx</b> + <c>{"code","message"}</c>。
/// 若不加本特性，组件会在非 2xx 直接抛 <c>ApiException</c>、把官方业务码丢掉。加上后错误体原样落到
/// <c>WechatPayResponse.Code</c>/<c>Message</c>，调用方经 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>不做运行时多态</b>：每个产品族各自一个端点方法 + 各自请求 DTO（设计方案 §2.2）。
/// APP / H5 / Native / 小程序 / 合单各族的差异只体现在<b>报文字段</b>上，用类型精确表达，
/// 不做「一个方法接枚举参数」的运行期分派 —— AOT 下源生成按声明类型序列化，多态无从谈起。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Transactions", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayTransactionsService
{
    /// <summary>
    /// JSAPI / 小程序下单，获取 <c>prepay_id</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>
    /// （官方接口名 <c>DirectAPIv3JsapiPrepay</c>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="JsapiPrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>prepay_id</c>（2 小时有效）的应答，见 <see cref="PrepayIdResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/transactions/jsapi</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① <c>out_trade_no</c> 须 6-32 字符、
    /// 仅数字与大小写字母 <c>_-|*</c>、同一商户号下唯一，重复提交报 <c>OUT_TRADE_NO_USED</c>(403)；
    /// ② <c>appid</c> 须与 <c>mchid</c> 有绑定关系，否则 <c>APPID_MCHID_NOT_MATCH</c>(400)；
    /// ③ <c>time_expire</c> 须在下单时间 15 天内、且不得早于下单后 1 分钟（官方会自动调整），
    /// 超时须先调关闭订单接口再以新商户订单号重下；④ 未指定 <c>time_expire</c> 时默认 7 天未支付即失效。</para>
    /// <para><b>频率</b>：超限返回 <c>FREQUENCY_LIMITED</c>(429)，官方要求降低请求频率。</para>
    /// <para><b>金额</b>：<c>amount.total</c> 单位为分、必须大于 0（1 元填 100）。</para>
    /// </remarks>
    [Post("/v3/pay/transactions/jsapi")]
    Task<PrepayIdResponse> CreateJsapiOrderAsync(
        JsapiPrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Native（扫码支付）下单，获取支付二维码链接 <c>code_url</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791877"/>
    /// （docId 由本地 SKIT <c>TenpayV3</c> 登记，2026-10-10 对齐；<b>官方逐页核验待补</b>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="NativePrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>code_url</c>（<b>2 小时有效</b>）的应答，见 <see cref="NativePrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/transactions/native</c>；无 path / query 参数；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>与 JSAPI 下单的两点根本差异</b>（勿混用 DTO）：① 请求<b>无支付者信息</b>（无 <c>payer</c>，
    /// 下单时不存在支付者标识）；② 应答<b>只有 <c>code_url</c></b>（无 <c>prepay_id</c>）——
    /// 支付由用户<b>扫码</b>发起，服务端须自行把 <c>code_url</c> 生成二维码展示。</para>
    /// <para><b><c>code_url</c> 有效期 2 小时</b>：失效后须<b>重新调用本接口</b>取新链接，
    /// 不得对旧链接做任何拼接或改写。</para>
    /// <para>业务限制（<c>out_trade_no</c> 唯一性 / <c>appid</c>-<c>mchid</c> 绑定 / <c>time_expire</c> 窗口 /
    /// 金额单位为分）与 <see cref="CreateJsapiOrderAsync"/> 同族口径；<b>这些限制是否逐条见诸 Native 页，
    /// 待官方逐页核验</b>。</para>
    /// </remarks>
    [Post("/v3/pay/transactions/native")]
    Task<NativePrepayResponse> CreateNativeOrderAsync(
        NativePrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// APP 下单，获取 <c>prepay_id</c>（供移动 SDK 调起）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013070347"/>
    /// （docId 由本地 SKIT <c>TenpayV3</c> 登记，2026-10-10 对齐；<b>官方逐页核验待补</b>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="AppPrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>prepay_id</c>（<b>2 小时有效</b>）的应答，见 <see cref="PrepayIdResponse"/>
    /// （官方 APP 页应答表与 JSAPI 逐项一致 ⇒ <b>共用</b>同一类型，本线纪律「表相同则共用」）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/transactions/app</c>；无 path / query 参数；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>与 JSAPI 下单的差异</b>：① 请求<b>无</b> <c>payer</c>（APP 由客户端 SDK 调起，
    /// 服务端拿不到支付者标识）；② 请求<b>多</b> <c>subsidy_info</c>（补贴详情，Native / H5 无）；
    /// ③ <c>appid</c> 须为<b>移动应用</b> AppID。</para>
    /// <para><b>应答与 JSAPI 共用</b> <see cref="PrepayIdResponse"/>：官方两页应答表逐项一致
    /// （均只有 <c>prepay_id</c>）⇒ 按「表相同则共用」不另建类型；Native / H5 的应答形态不同，各自独立。</para>
    /// <para><b>调起支付在客户端侧</b>：<c>prepay_id</c> 回传 APP 后由移动 SDK 二次签名调起，
    /// <b>服务端不得</b>替客户端组装该签名串。</para>
    /// </remarks>
    [Post("/v3/pay/transactions/app")]
    Task<PrepayIdResponse> CreateAppOrderAsync(
        AppPrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// H5（移动端网页）下单，获取可直接跳转的支付链接 <c>h5_url</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791834"/>
    /// （docId 由本地 SKIT <c>TenpayV3</c> 登记，2026-10-10 对齐；<b>官方逐页核验待补</b>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="H5PrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>h5_url</c> 的应答，见 <see cref="H5PrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/transactions/h5</c>；无 path / query 参数；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>⚠️ 本族最容易漏的一处</b>：<see cref="H5SceneInfo"/> 比其它下单族的场景信息<b>多</b>
    /// <c>h5_info</c> 一节 ⇒ 从 JSAPI / Native 复制请求体会<b>传不了</b>该字段（故 <c>scene_info</c>
    /// 不复用 <see cref="JsapiSceneInfo"/>）。请求顶层另<b>无</b> <c>payer</c>。</para>
    /// <para><b>拿到 <c>h5_url</c> 后</b>须按官方《H5 调起支付》指引跳转（含 <c>Referer</c> 等要求），
    /// <b>严禁</b>自行改写或拼装该链接。</para>
    /// <para><b>仅在微信外浏览器场景可用</b>：微信内置浏览器场景须走 JSAPI 下单。</para>
    /// </remarks>
    [Post("/v3/pay/transactions/h5")]
    Task<H5PrepayResponse> CreateH5OrderAsync(
        H5PrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过<b>微信支付订单号</b>查询订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791899"/>
    /// （官方接口名 <c>DirectAPIv3Transactions/id</c>；页面更新时间 2024.12.27）。
    /// </summary>
    /// <param name="transactionId">微信支付订单号（路径参数，string(32)）。</param>
    /// <param name="mchId">商户号（Query <c>mchid</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单详情，见 <see cref="TransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/pay/transactions/id/{transaction_id}</c>；
    /// Query <c>mchid</c> 必填；必带 <c>Accept: application/json</c>。</para>
    /// <para><b>与「商户订单号查询」的关系</b>：两路由应答字段<b>完全一致</b>（共用
    /// <see cref="TransactionQueryResponse"/>），仅查询键不同；商户订单号场景请用
    /// <see cref="QueryByOutTradeNoAsync"/>。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① 只能查<b>本商户号</b>下的订单；② 查单<b>不可代替支付结果通知</b>，
    /// 须以订单状态为准；③ 订单不存在时返回 <c>ORDER_NOT_EXIST</c>（4xx）。</para>
    /// <para><b>交易状态</b>：<c>trade_state</c> 取值 SUCCESS / REFUND / NOTPAY / CLOSED / REVOKED /
    /// USERPAYING / PAYERROR（枚举语义见 <see cref="TransactionQueryResponse.TradeState"/>）。</para>
    /// </remarks>
    [Get("/v3/pay/transactions/id/{transactionId}")]
    Task<TransactionQueryResponse> QueryByTransactionIdAsync(
        [Path] string transactionId,
        [Query("mchid")] string mchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过<b>商户订单号</b>查询订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791900"/>
    /// （官方接口名 <c>DirectAPIv3Transactions/out-trade-no</c>；页面更新时间 2024.12.27）。
    /// </summary>
    /// <param name="outTradeNo">商户订单号（路径参数，string(32)）。</param>
    /// <param name="mchId">商户号（Query <c>mchid</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单详情，见 <see cref="TransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/pay/transactions/out-trade-no/{out_trade_no}</c>；
    /// Query <c>mchid</c> 必填；必带 <c>Accept: application/json</c>。</para>
    /// <para>业务限制同 <see cref="QueryByTransactionIdAsync"/>（查询不可代替支付结果通知；订单不存在报
    /// <c>ORDER_NOT_EXIST</c>）。</para>
    /// </remarks>
    [Get("/v3/pay/transactions/out-trade-no/{outTradeNo}")]
    Task<TransactionQueryResponse> QueryByOutTradeNoAsync(
        [Path] string outTradeNo,
        [Query("mchid")] string mchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791901"/>
    /// （官方接口名 <c>DirectAPIv3Transactions/out-trade-no/close</c>；页面更新时间 2024.12.11）。
    /// </summary>
    /// <param name="outTradeNo">商户订单号（路径参数，string(32)）。</param>
    /// <param name="request">关单请求体（<c>mchid</c>），见 <see cref="CloseOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/transactions/out-trade-no/{out_trade_no}/close</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>；
    /// 成功应答 <c>204 No Content</c>。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① 订单<b>已支付成功不能关闭</b>（须走退款）；
    /// ② 关单后用户<b>无法再支付</b>、也无法继续支付该订单；③ 关单为<b>异步</b>生效，
    /// 关单后应经查单确认；④ 已超时（<c>time_expire</c>）未支付的订单官方自动关闭。</para>
    /// </remarks>
    [Post("/v3/pay/transactions/out-trade-no/{outTradeNo}/close")]
    Task CloseOrderAsync(
        [Path] string outTradeNo,
        CloseOrderRequest request,
        CancellationToken cancellationToken = default);
}
