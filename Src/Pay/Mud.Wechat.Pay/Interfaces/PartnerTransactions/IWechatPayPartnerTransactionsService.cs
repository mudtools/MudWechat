// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PartnerTransactions;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「服务商基础支付 / 分期」域 SDK（服务商下单六族 + 双路查单 + 关单 + 撤销 + 分期曝光咨询 +
/// 分付曝光建议 + 从业机构分付曝光，<b>13 端点</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（服务商文档中心，2026-10-14 逐页核验）：JSAPI/小程序下单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714911"/>、Native 下单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714902"/>、APP 下单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714669"/>、H5 下单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714759"/>、扫码支付下单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012492476"/>、订单号查询
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738964"/>、商户订单号查询
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012739008"/>、关闭订单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012739019"/>、分期曝光咨询
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016196820"/>。
/// 除曝光咨询外，各端点另有产品族分身页（同一契约、不同接入形态章节），逐端点在方法 remarks 内列出。</para>
/// <para><b>2026-10-25 增补四端点（EP-3 批次，官方 .md 直抓逐页核验）</b>：付款码支付下单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012382179"/>、撤销订单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012382181"/>、服务商分付曝光
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4014326522"/>、从业机构分付曝光
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4014326541"/>。前三者于 2026-10-14 曾按
/// 「V2 指引页 / 页面下架」裁定不落（守卫 PT-B2 旧零声明断言），官方现行 `.md` 正文均已提供完整
/// APIv3 字段表 ⇒ <b>翻案落地</b>（裁定可复核、以最新官方面为准）。</para>
/// <para><b>与直连域（<see cref="IWechatPayTransactionsService"/>）的形态差异</b>，本域纪律「表相同则共用，
/// 表不同则分建」下的三条判据（守卫 PT-B1/PT-B3/PT-B4 锁定）：
/// ① 商户标识<b>四分</b> —— <c>sp_appid</c>/<c>sp_mchid</c>/<c>sub_appid</c>/<c>sub_mchid</c>，
/// 直连的 <c>appid</c>/<c>mchid</c> 在本域<b>不存在</b>；
/// ② <c>payer</c> 形态按族分列（JSAPI 与扫码支付为 sp/sub 双 openid；付款码为 <c>auth_code</c>；
/// 分付曝光为三标识选填其一）；
/// ③ <c>support_fapiao</c> 在 JSAPI/Native/APP/H5 四族位于 <c>settle_info</c> 内、在<b>扫码支付与付款码两族位于请求顶层</b>。
/// 两套 DTO <b>不共用</b>（跨产品线共用会把直连域的序列化上下文登记牵进本域）。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 <c>PAY-B1</c> fail-closed）：APIv3 全程以商户 API 证书做
/// RSA-SHA256 请求签名，本接口<b>不声明 <c>[Token]</c></b>（违反即 MUD005 与 Query 白名单漂移）。
/// 签名在传输层 <c>WechatPayAuthorizationHandler</c> 完成，端点方法只管路由与报文。
/// <b>签名主体是服务商商户号</b>（<c>sp_mchid</c>），子商户号只出现在报文里 —— 服务商<b>不得</b>用子商户证书调本域。</para>
/// <para><b>客户端</b>：走支付专用 <c>IWechatPayHttpClient</c>（命名客户端 <c>wechat-pay</c>，
/// BaseAddress 恒为 <c>https://api.mch.weixin.qq.com</c>）——不复用 <c>IEnhancedHttpClient</c> 默认实例，
/// 否则会与企微线的 <c>qyapi.weixin.qq.com</c> 撞 BaseUrl。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 <b>HTTP 4xx/5xx</b> +
/// <c>{"code","message"}</c>。若不加本特性，组件会在非 2xx 直接抛 <c>ApiException</c>、把官方业务码丢掉；
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c>/<c>Message</c>，调用方经
/// <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>不做运行时多态</b>：各下单族各自一个端点方法 + 各自请求 DTO（设计方案 §2.2）——
/// AOT 下源生成按声明类型序列化，族间差异（有无 <c>payer</c>、<c>scene_info</c> 形态、
/// <c>support_fapiao</c> 位置）用类型精确表达，不做「一个方法接枚举参数」的运行期分派。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PartnerTransactions", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPartnerTransactionsService
{
    /// <summary>
    /// 服务商 JSAPI / 小程序下单，获取 <c>prepay_id</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714911"/>
    /// （同契约产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738519"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012759974"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714678"/>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="PartnerJsapiPrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>prepay_id</c>（2 小时有效）的应答，见 <see cref="PartnerPrepayIdResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/partner/transactions/jsapi</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① <c>out_trade_no</c> 同一商户号下唯一，重复提交报
    /// <c>OUT_TRADE_NO_USED</c>(403)；② <c>sp_appid</c> 须与 <c>sp_mchid</c> 绑定、<c>sub_appid</c> 须与
    /// <c>sub_mchid</c> 绑定，错配报 <c>APPID_MCHID_NOT_MATCH</c>(400)；③ <c>payer</c> 内
    /// <c>sp_openid</c> 与 <c>sub_openid</c> <b>必须要填一个</b>，传 <c>sub_openid</c> 则 <c>sub_appid</c> 必填，
    /// 归属错配报 <c>OPENID_MISMATCH</c>；④ <c>time_expire</c> 须在下单时间 15 天内且不得早于下单后 1 分钟，
    /// 不传则默认 7 天未支付失效；⑤ <c>amount.total</c> 单位为分、必须大于 0。</para>
    /// <para><b>补差金额</b>：<c>settle_info.subsidy_amount</c> 仅当 <c>profit_sharing = true</c> 时生效，
    /// 官方限制单笔订单最高补差 5000 元。</para>
    /// <para><b>频率</b>：超限返回 <c>FREQUENCY_LIMITED</c>(429)，官方要求降低请求频率。</para>
    /// <para><b>官方业务错误码表（逐条核验）</b>：<c>400 PARAM_ERROR</c> / <c>400 INVALID_REQUEST</c> /
    /// <c>401 SIGN_ERROR</c> / <c>400 APPID_MCHID_NOT_MATCH</c> / <c>400 MCH_NOT_EXISTS</c> /
    /// <c>400 ORDER_CLOSED</c>（订单已关闭，须重新下单）/ <c>403 ACCOUNT_ERROR</c>（用户账号异常，
    /// 官方明确<b>无需更多操作</b>，不得重试）/ <c>403 NO_AUTH</c> / <c>403 OUT_TRADE_NO_USED</c> /
    /// <c>403 RULE_LIMIT</c> / <c>403 TRADE_ERROR</c> / <c>404 ORDER_NOT_EXIST</c> /
    /// <c>429 FREQUENCY_LIMITED</c> / <c>500 SYSTEM_ERROR</c> / <c>500 BANK_ERROR</c>（银行系统异常，
    /// 官方要求<b>用相同参数重新调用</b>）/ <c>500 INVALID_TRANSACTIONID</c> / <c>500 OPENID_MISMATCH</c>。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/jsapi")]
    Task<PartnerPrepayIdResponse> CreateJsapiOrderAsync(
        PartnerJsapiPrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商 Native（扫码支付）下单，获取支付二维码链接 <c>code_url</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714902"/>
    /// （同契约产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738659"/>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="PartnerNativePrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>code_url</c> 的应答，见 <see cref="PartnerNativePrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/partner/transactions/native</c>；无 path / query 参数。</para>
    /// <para><b>与 <see cref="CreateJsapiOrderAsync"/> 的根本差异</b>：请求<b>无 <c>payer</c></b>
    /// （下单时不存在支付者标识），应答<b>只有 <c>code_url</c></b> —— 支付由用户<b>扫码</b>发起，
    /// 服务端须自行把 <c>code_url</c> 生成二维码展示。</para>
    /// <para><b><c>code_url</c> 不得拼接改写</b>；失效后须<b>重新调用本接口</b>取新链接。
    /// 业务限制（订单号唯一性 / AppID-商户号绑定 / <c>time_expire</c> 窗口 / 金额单位为分）同 JSAPI 族口径。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/native")]
    Task<PartnerNativePrepayResponse> CreateNativeOrderAsync(
        PartnerNativePrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商 APP 下单，获取 <c>prepay_id</c>（供移动 SDK 调起）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714669"/>
    /// （同契约产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013080231"/>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="PartnerAppPrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>prepay_id</c>（2 小时有效）的应答，见 <see cref="PartnerPrepayIdResponse"/>
    /// （官方 APP 页应答表与 JSAPI 逐项一致 ⇒ <b>共用</b>同一类型）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/partner/transactions/app</c>；无 path / query 参数。</para>
    /// <para><b>AppID 形态收窄</b>：<c>sp_appid</c> / <c>sub_appid</c> 须为微信开放平台<b>移动应用</b> AppID，
    /// 且各自须与对应商户号有绑定关系。</para>
    /// <para><b>与直连域 APP 下单的差异</b>：官方服务商 APP 页字段表<b>没有</b> <c>subsidy_info</c>（补贴节）
    /// ⇒ <see cref="PartnerAppPrepayRequest"/> 不提供该字段，不得据直连形态补。</para>
    /// <para><b>调起支付在客户端侧</b>：<c>prepay_id</c> 回传 APP 后由移动 SDK 二次签名调起，
    /// <b>服务端不得</b>替客户端组装该签名串；调起参数注入须按 <c>appid</c> + <c>partner_id</c>
    /// 识别服务商形态。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/app")]
    Task<PartnerPrepayIdResponse> CreateAppOrderAsync(
        PartnerAppPrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商 H5（移动端网页）下单，获取可直接跳转的支付链接 <c>h5_url</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714759"/>
    /// （同契约产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738604"/>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="PartnerH5PrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>h5_url</c> 的应答，见 <see cref="PartnerH5PrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/partner/transactions/h5</c>；无 path / query 参数。</para>
    /// <para><b>⚠️ 本族最容易漏的一处</b>：<c>scene_info</c> <b>整体必填</b>且类型是
    /// <see cref="PartnerH5SceneInfo"/>（比其它族多一节必填的 <see cref="PartnerH5Info"/>）——
    /// 从 JSAPI / Native 请求体复制会<b>传不了</b> <c>h5_info</c>。请求另<b>无</b> <c>payer</c>。</para>
    /// <para><b>拿到 <c>h5_url</c> 后</b>须按官方《H5 调起支付》指引跳转（含 <c>Referer</c> 等要求），
    /// <b>严禁</b>改写或拼装该链接。</para>
    /// <para><b>仅在微信外浏览器场景可用</b>：微信内置浏览器场景须走 <see cref="CreateJsapiOrderAsync"/>。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/h5")]
    Task<PartnerH5PrepayResponse> CreateH5OrderAsync(
        PartnerH5PrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商扫码支付（被扫）下单，获取 <c>prepay_id</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012492476"/>。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="PartnerScannedPosPrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>prepay_id</c>（2 小时有效）的应答，见 <see cref="PartnerPrepayIdResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/partner/transactions/scannedpos</c>；无 path / query 参数。</para>
    /// <para><b>本族字段约束与其余四族处处相反</b>（守卫 PT-B4 逐条锁定）：
    /// ① <c>sp_appid</c> 与 <c>sub_appid</c> <b>二选一必填</b>（其余四族 <c>sp_appid</c> 恒必填）；
    /// ② <c>notify_url</c> <b>选填</b>（其余四族必填）；③ <c>support_fapiao</c> 在<b>请求顶层</b>；
    /// ④ <c>settle_info</c> 只有 <c>profit_sharing</c>（<see cref="PartnerScannedPosSettleInfo"/>）；
    /// ⑤ <c>scene_info</c> <b>必填</b>、<c>payer_client_ip</c> <b>选填</b>、<c>store_info</c> <b>必填</b>
    /// 且只有 <c>id</c>/<c>out_id</c>（<see cref="PartnerScannedPosSceneInfo"/>）。</para>
    /// <para><b>本族不是 APIv2 的付款码支付</b>：官方 <c>codepay</c> 与 <c>reverse</c> 两页现行正文是 V2 指引、
    /// 无 APIv3 字段表 ⇒ 本域<b>零端点</b>，V2 归 P0-8 批次；守卫 PT-B2 断言这两条路由在本线零声明。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/scannedpos")]
    Task<PartnerPrepayIdResponse> CreateScannedPosOrderAsync(
        PartnerScannedPosPrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商通过<b>微信支付订单号</b>查询订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012738964"/>
    /// （同契约产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738969"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738971"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738973"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760565"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013080234"/>）。
    /// </summary>
    /// <param name="transactionId">微信支付订单号（路径参数，string(32)，必填）。</param>
    /// <param name="spMchId">服务商户号（Query <c>sp_mchid</c>，必填 string(32)）。</param>
    /// <param name="subMchId">子商户号（Query <c>sub_mchid</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单详情，见 <see cref="PartnerTransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/pay/partner/transactions/id/{transaction_id}</c>；
    /// Query <c>sp_mchid</c> 与 <c>sub_mchid</c> <b>皆必填</b>（直连域只需 <c>mchid</c> 一个）；
    /// 必带 <c>Accept: application/json</c>。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① 只能查<b>本服务商</b>下子商户的订单；
    /// ② 查单<b>不可代替支付结果通知</b>，须以 <c>trade_state</c> 为准；③ 订单不存在返回
    /// <c>ORDER_NOT_EXIST</c>；④ 频率超限返回 <c>FREQUENCY_LIMITED</c>(429)。</para>
    /// <para><b>与商户订单号查询共用应答</b>：两路由官方应答字段表<b>完全一致</b> ⇒ 同一
    /// <see cref="PartnerTransactionQueryResponse"/>，仅查询键不同。</para>
    /// </remarks>
    [Get("/v3/pay/partner/transactions/id/{transactionId}")]
    Task<PartnerTransactionQueryResponse> QueryByTransactionIdAsync(
        [Path] string transactionId,
        [Query("sp_mchid")] string spMchId,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商通过<b>商户订单号</b>查询订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012739008"/>
    /// （同契约产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012759661"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012759714"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760115"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760568"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013080235"/>）。
    /// </summary>
    /// <param name="outTradeNo">商户订单号（路径参数，string(32)，必填）。</param>
    /// <param name="spMchId">服务商户号（Query <c>sp_mchid</c>，必填 string(32)）。</param>
    /// <param name="subMchId">子商户号（Query <c>sub_mchid</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单详情，见 <see cref="PartnerTransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/pay/partner/transactions/out-trade-no/{out_trade_no}</c>；
    /// Query <c>sp_mchid</c> 与 <c>sub_mchid</c> <b>皆必填</b>；必带 <c>Accept: application/json</c>。</para>
    /// <para>业务限制同 <see cref="QueryByTransactionIdAsync"/>（查单不可代替支付结果通知；
    /// 订单不存在报 <c>ORDER_NOT_EXIST</c>）。</para>
    /// </remarks>
    [Get("/v3/pay/partner/transactions/out-trade-no/{outTradeNo}")]
    Task<PartnerTransactionQueryResponse> QueryByOutTradeNoAsync(
        [Path] string outTradeNo,
        [Query("sp_mchid")] string spMchId,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商关闭订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012739019"/>
    /// （同契约产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012759669"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012759725"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760108"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760574"/>、
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013080236"/>）。
    /// </summary>
    /// <param name="outTradeNo">商户订单号（路径参数，string(32)，必填）。</param>
    /// <param name="request">关单请求体（<c>sp_mchid</c> + <c>sub_mchid</c>），见 <see cref="PartnerCloseOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/partner/transactions/out-trade-no/{out_trade_no}/close</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>；成功应答 <c>204</c>。</para>
    /// <para><b>请求体只有两个字段</b>：<c>sp_mchid</c> / <c>sub_mchid</c>，<b>不含任何 AppID</b>（守卫 PT-B1）。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① 订单<b>已支付成功不能关闭</b>（须走退款，否则
    /// <c>403 TRADE_ERROR</c>）；② 关单后用户无法再支付该订单；③ 关单<b>异步</b>生效，须经查单确认；
    /// ④ 已超时未支付订单官方自动关闭。</para>
    /// <para><b>官方错误码表</b>：<c>400 PARAM_ERROR</c> / <c>401 SIGN_ERROR</c> / <c>500 SYSTEM_ERROR</c> /
    /// <c>400 MCH_NOT_EXISTS</c> / <c>403 RULE_LIMIT</c> / <c>403 TRADE_ERROR</c> / <c>429 FREQUENCY_LIMITED</c>。</para>
    /// <para><b>本域没有撤销（reverse）端点</b>：官方 <c>…/reverse</c> 页现行正文是 APIv2 付款码支付指引
    /// ⇒ 归 P0-8，守卫 PT-B2 零声明锁定。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/out-trade-no/{outTradeNo}/close")]
    Task CloseOrderAsync(
        [Path] string outTradeNo,
        PartnerCloseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商分期支付「曝光咨询」：查询某用户是否推荐曝光分期入口，及各档期有无息费减免券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016196820"/>。
    /// </summary>
    /// <param name="request">咨询请求体，字段见 <see cref="PartnerInstalExposureRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>曝光建议，见 <see cref="PartnerInstalExposureResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/instal/partner/exposure</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>路由前缀例外</b>：本端点在 <c>/v3/pay/instal/</c> 下，<b>不在</b>
    /// <c>/v3/pay/partner/transactions/</c> 前缀内 —— 守卫 PT-B2 按「8 + 1」形态钉住。</para>
    /// <para><b>频率上限 200 次/秒</b>（服务商商户号维度），超限须降频。</para>
    /// <para><b>用户标识形态与下单族不同</b>：本请求的 <c>user_id</c> 是<b>对象</b>，
    /// <c>user_id_type</c> 必填（四值见 <see cref="PartnerUserIdTypes"/>），
    /// 不是下单族 <c>payer</c> 的平铺双 openid。</para>
    /// <para><b>应答不含金额与费率</b>：只有 <c>recommend_exposure</c> 与逐档期的
    /// <c>exist_cash_off_coupon</c>（档期取值见 <see cref="PartnerRepaymentTypes"/>）。</para>
    /// <para><b>SKIT 清单里的 <c>/pay/partner/transactions/exposure</c> 不落端点</b>：官方页面已下架
    /// （302 到 50x，两次独立核验），曝光能力只由本方法承载。</para>
    /// </remarks>
    [Post("/v3/pay/instal/partner/exposure")]
    Task<PartnerInstalExposureResponse> GetInstalExposureAdviceAsync(
        PartnerInstalExposureRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商付款码支付（被扫）下单，实时扣款。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012382179"/>。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="PartnerCodepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>交易结果（<c>transaction_id</c> / <c>trade_state</c> 等），见
    /// <see cref="PartnerTransactionQueryResponse"/>（官方本页应答表与查单逐项一致 ⇒ 同表共用）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/partner/transactions/codepay</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>业务特征</b>：收银员扫码设备读取用户付款码后由商户后台发起<b>实时扣款</b>——
    /// 与 JSAPI/Native/APP/H5 下单不同，无需预支付 ID、无需用户二次确认；
    /// <c>payer.auth_code</c>（用户付款码）与 <c>scene_info</c>（含门店 <c>id</c>/<c>out_id</c> 二选一）
    /// 官方均<b>必填</b>（线下条码支付合规要求）。</para>
    /// <para><b>2026-10-25 翻案说明</b>：本端点于 2026-10-14 曾按「官方正文是 V2 指引页」裁定不落
    /// （守卫 PT-B2 旧零声明断言）；官方现行 <c>.md</c> 正文已提供完整 APIv3 字段表（页头更新时间
    /// 2026.07.14）⇒ 翻案落地，详见类型级 remarks。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/codepay")]
    Task<PartnerTransactionQueryResponse> CreateCodepayOrderAsync(
        PartnerCodepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商撤销订单（支付失败 / 系统超时的纠错出口）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012382181"/>。
    /// </summary>
    /// <param name="outTradeNo">商户订单号（路径参数，string(32)，必填）。</param>
    /// <param name="request">撤销请求体（四分商户标识），见 <see cref="PartnerReverseOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>撤销结果（四分商户标识 + 订单号），见 <see cref="PartnerReverseOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b>
    /// <c>/v3/pay/partner/transactions/out-trade-no/{out_trade_no}/reverse</c>；成功应答 <c>200</c>（非 204）。</para>
    /// <para><b>官方语义</b>：「支付交易返回失败或支付系统超时，调用该接口撤销交易。如果此订单用户支付失败，
    /// 微信支付系统会将此订单关闭；如果用户支付成功，微信支付系统会将此订单资金退还给用户。」
    /// —— 撤销是付款码支付的配套纠错动作（用户支付中态 <c>USERPAYING</c> 的收尾手段）。</para>
    /// <para><b>请求体与关单不同表</b>：本体含选填 <c>sp_appid</c>/<c>sub_appid</c>
    /// （关单体 <see cref="PartnerCloseOrderRequest"/> 无任何 AppID），不可互抄。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/out-trade-no/{outTradeNo}/reverse")]
    Task<PartnerReverseOrderResponse> ReverseOrderAsync(
        [Path] string outTradeNo,
        PartnerReverseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商查询微信分付曝光建议（下单前调用）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014326522"/>。
    /// </summary>
    /// <param name="request">曝光咨询请求体，字段见 <see cref="PartnerTransactionExposureRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>曝光建议，见 <see cref="PartnerTransactionExposureResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/partner/transactions/exposure</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>与分期曝光族（<see cref="GetInstalExposureAdviceAsync"/>）并存非替代</b>：本族查
    /// 「是否建议曝光微信分付」（<c>fqf_exposure</c> + 调起用 <c>market_exposure_token</c>），
    /// 分期族查分期入口与档期息费减免券；两者路由、字段表、业务时机均不同。</para>
    /// <para><b>2026-10-25 翻案说明</b>：2026-10-14 曾以「页面下架（302→50x）」裁定不落；
    /// 官方现行 <c>.md</c> 正文在册且字段表完整（页头更新时间 2026.03.26）⇒ 翻案落地。</para>
    /// </remarks>
    [Post("/v3/pay/partner/transactions/exposure")]
    Task<PartnerTransactionExposureResponse> GetFqfExposureAdviceAsync(
        PartnerTransactionExposureRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构查询微信分付曝光建议。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014326541"/>。
    /// </summary>
    /// <param name="request">曝光咨询请求体（从业机构字段面），见 <see cref="AcquiringBankTransactionExposureRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>曝光建议，见 <see cref="PartnerTransactionExposureResponse"/>（两页应答表逐项一致 ⇒ 同表共用）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/acquiring-bank/transactions/exposure</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>支持商户类型为从业机构（银行 / 支付机构）</b>：请求体是
    /// <c>acquiring_bank_id</c>/<c>merchant_id</c>/<c>channel_id</c> 三方标识形态，与服务商族不同表 ⇒
    /// DTO 独立。本端点是本接口上的<b>第二个前缀例外</b>（不在 <c>/v3/pay/partner/transactions/</c> 下），
    /// 从业机构族后续扩域时随迁独立域。</para>
    /// <para><b>签名主体</b>：仍为当前作用域商户的 API 证书（从业机构 / 服务商自有商户号），见类型级 remarks。</para>
    /// </remarks>
    [Post("/v3/pay/acquiring-bank/transactions/exposure")]
    Task<PartnerTransactionExposureResponse> GetAcquiringBankFqfExposureAdviceAsync(
        AcquiringBankTransactionExposureRequest request,
        CancellationToken cancellationToken = default);
}
