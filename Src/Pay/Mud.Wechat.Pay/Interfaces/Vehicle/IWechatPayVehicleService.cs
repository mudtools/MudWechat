// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Vehicle;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「车主出行（ETC 高速扣费 + 微信支付分停车）」域 SDK（10 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 10 支中<b>6 支官方只印一页</b>、<b>4 支印多页</b>（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 10 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>两族共存于一路由树、开放面不同</b>：<c>/vehicle/etc/*</c>（ETC 六支）官方<b>只印服务商页</b>【普通服务商】； <c>/vehicle/parking/*</c> 与 <c>/vehicle/transactions/parking</c> 四支各印<b>商户页 + 服务商页</b> ⇒ 停车四支按<b>并集</b>建模、一面一方法，ETC 六支<b>不得</b>按停车面镜像补齐商户页。</para>
/// <para><b>ETC 两支查单应答同形 ⇒ 共用一型</b>：<c>/vehicle/etc/transactions/id/&#42;</c> 与 <c>/vehicle/etc/transactions/out-trade-no/&#42;</c> 官方应答表 33 行<b>逐字段同形且必填口径一致</b>； 但<b>高速扣款下单的应答与查单不同形</b>（<c>device_info</c> 下单侧必填、<c>trade_type</c> 长度 16 vs 32、 <c>success_time</c> 32 vs 25）⇒ 下单应答<b>单建一型</b>，不得据「看起来一样」合并。</para>
/// <para><b>签约两支同理共用</b>：<c>/vehicle/etc/contracts/{contract_id}</c> 与 <c>/vehicle/etc/users/{openid}/contracts</c> 应答表同为 9 行同形 ⇒ 共用应答类型； 后者的定位键是 query 上的 <c>sub_mchid</c> / <c>plan_id</c> / <c>plate_number</c>（后两支必填）而 path 段是 <c>openid</c>。</para>
/// <para><b>停车「查单」与「下单」应答不共用</b>：两支应答表字段名集相同，仅 <c>amount.currency</c> 的必填口径不同 （查询页选填、下单页必填）⇒ 按「共用只发生在应答侧且须含必填口径一致」的口径<b>分建两型</b>。</para>
/// <para><b>两支都是「受理」不是「扣款成功」</b>：<c>POST /v3/vehicle/transactions/parking</c> 官方原文 「微信支付进行异步扣款，支付完成后，会将订单支付结果发送给商户」、<c>POST /v3/vehicle/etc/preopen</c> 只回 <c>preopen_id</c> ⇒ 结果由异步通知与查询支判定，<b>不得</b>据 HTTP 2xx 判定扣款或开通成功。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Vehicle", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayVehicleService
{
    /// <summary>
    /// 查询ETC签约状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715667"/>（【普通服务商】）。
    /// </summary>
    /// <param name="contractId">【商户etc绑定号】 商户etc绑定号（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【特约商户号】特约商户的商户号，由微信支付生成并下发。（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询ETC签约状态，见 <see cref="VehicleEtcContractResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/vehicle/etc/contracts/{contract_id}</c>；path <c>contract_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 404 RESOURCE_NOT_EXISTS：签约模板不存在或已停用 / 400 CONTRACT_NOT_EXIST：ETC绑定信息不存在 / 400 RESOURCE_ALREADY_EXISTS：用户已绑定该ETC设备，不可重复绑定。</para>
    /// </remarks>
    [Get("/v3/vehicle/etc/contracts/{contractId}")]
    Task<VehicleEtcContractResponse> QueryVehicleEtcContractAsync(
        [Path] string contractId,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按用户与车牌查询ETC绑定信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715623"/>（【普通服务商】）。
    /// </summary>
    /// <param name="openid">【用户标识】此参数为微信用户在商户对应AppID或sub_appid下的唯一标识（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="planId">【签约计划号】ETC授权扣费模板ID，商户申请后由微信侧提供（官方 query <c>plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="plateNumber">【车牌号】 车牌号（官方 query <c>plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【特约商户号】特约商户的商户号，由微信支付生成并下发。（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>按用户与车牌查询ETC绑定信息，见 <see cref="VehicleEtcContractResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/vehicle/etc/users/{openid}/contracts</c>；path <c>openid</c>，query <c>plan_id</c> / <c>plate_number</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>plan_id</c> 与 <c>plate_number</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 404 RESOURCE_NOT_EXISTS：签约模板不存在或已停用 / 400 CONTRACT_NOT_EXIST：ETC绑定信息不存在 / 400 RESOURCE_ALREADY_EXISTS：用户已绑定该ETC设备，不可重复绑定。</para>
    /// </remarks>
    [Get("/v3/vehicle/etc/users/{openid}/contracts")]
    Task<VehicleEtcContractResponse> QueryVehicleEtcUserContractAsync(
        [Path] string openid,
        [Query("plan_id")] string planId,
        [Query("plate_number")] string plateNumber,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ETC自动扣费预开通。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715215"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">ETC自动扣费预开通请求体，字段见 <see cref="VehicleEtcPreopenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>ETC自动扣费预开通，见 <see cref="VehicleEtcPreopenResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/vehicle/etc/preopen</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="VehicleEtcPreopenResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 404 RESOURCE_NOT_EXISTS：签约模板不存在或已停用 / 400 CONTRACT_NOT_EXIST：ETC绑定信息不存在 / 400 RESOURCE_ALREADY_EXISTS：用户已绑定该ETC设备，不可重复绑定。</para>
    /// </remarks>
    [Post("/v3/vehicle/etc/preopen")]
    Task<VehicleEtcPreopenResponse> CreateVehicleEtcPreopenAsync(
        [Body] VehicleEtcPreopenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ETC高速场景订单扣款下单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">ETC高速场景订单扣款下单请求体，字段见 <see cref="VehicleEtcHighwayTransactionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>ETC高速场景订单扣款下单，见 <see cref="VehicleEtcHighwayTransactionResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/vehicle/etc/transactions/highway</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="VehicleEtcHighwayTransactionResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH：商户号信息错误或与AppID不匹配 / 403 RULE_LIMIT：当前订单金额超过支付限额 / 400 CONTRACT_NOT_EXIST：当前车牌无绑定或已暂停ETC扣费服务 / 403 CONTRACT_ERROR：当前车牌ETC绑定状态失效 / 403 TRADE_ERROR：用户紧急冻结支付账户 / 400 INVALID_REQUEST：订单重复扣款（同一车牌在同一出场时间重复发起扣款） / 403 OUT_TRADE_NO_USED：当前商户订单号重复 / 404 ORDER_NOT_EXIST：订单不存在 / 202 USER_PAYING：当前订单正在支付中 / 403 NO_AUTH：无ETC代扣或委托代扣权限 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Post("/v3/vehicle/etc/transactions/highway")]
    Task<VehicleEtcHighwayTransactionResponse> CreateVehicleEtcHighwayTransactionAsync(
        [Body] VehicleEtcHighwayTransactionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ETC订单查询（微信订单号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715719"/>（【普通服务商】）。
    /// </summary>
    /// <param name="transactionId">【微信支付订单号】 微信支付订单号（官方 path <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【特约商户号】 特约商户号，由微信支付生成并下发。（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>ETC订单查询（微信订单号），见 <see cref="VehicleEtcTransactionResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/vehicle/etc/transactions/id/{transaction_id}</c>；path <c>transaction_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH：商户号信息错误或与AppID不匹配 / 400 INVALID_REQUEST：订单重复扣款（同一车牌在同一出场时间重复发起扣款） / 404 ORDER_NOT_EXIST：订单不存在 / 202 USER_PAYING：当前订单正在支付中 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Get("/v3/vehicle/etc/transactions/id/{transactionId}")]
    Task<VehicleEtcTransactionResponse> QueryVehicleEtcTransactionByIdAsync(
        [Path] string transactionId,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ETC订单查询（商户订单号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715845"/>（【普通服务商】）。
    /// </summary>
    /// <param name="outTradeNo">【商户订单号】 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-且在同一个商户号下唯一。重新发起一笔支付要使用原订单号，避免重复支付。（官方 path <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【特约商户号】 特约商户号，由微信支付生成并下发。（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>ETC订单查询（商户订单号），见 <see cref="VehicleEtcTransactionResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/vehicle/etc/transactions/out-trade-no/{out_trade_no}</c>；path <c>out_trade_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH：商户号信息错误或与AppID不匹配 / 400 INVALID_REQUEST：订单重复扣款（同一车牌在同一出场时间重复发起扣款） / 404 ORDER_NOT_EXIST：订单不存在 / 202 USER_PAYING：当前订单正在支付中 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Get("/v3/vehicle/etc/transactions/out-trade-no/{outTradeNo}")]
    Task<VehicleEtcTransactionResponse> QueryVehicleEtcTransactionByOutTradeNoAsync(
        [Path] string outTradeNo,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建停车入场信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533937"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012533994"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">创建停车入场信息请求体，字段见 <see cref="VehicleParkingEntryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建停车入场信息，见 <see cref="VehicleParkingEntryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/vehicle/parking/parkings</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>应答</b>见 <see cref="VehicleParkingEntryResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：商户入场ID非法，请检查商户入场ID / 403 NO_AUTH：商户未入驻 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Post("/v3/vehicle/parking/parkings")]
    Task<VehicleParkingEntryResponse> CreateVehicleParkingEntryAsync(
        [Body] VehicleParkingEntryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询车牌服务开通信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534043"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534183"/>（【普通服务商】）。
    /// </summary>
    /// <param name="appid">【公众账号ID】AppID是商户在微信申请公众号或移动应用成功后分配的账号ID，登录平台为mp.weixin.qq.com或open.weixin.qq.com（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="plateNumber">【车牌号】车牌号，仅包括省份+车牌，不包括特殊字符。（官方 query <c>plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="openid">【用户标识】用户在商户对应AppID下的唯一标识（官方 query <c>openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="plateColor">【车牌颜色】车牌颜色。可选取值：`BLUE` 蓝色 / `GREEN` 绿色 / `YELLOW` 黄色 / `BLACK` 黑色 / `WHITE` 白色 / `LIMEGREEN` 黄绿色（官方 query <c>plate_color</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="subMchid">【子商户号】微信支付分配的子商户号，服务商模式下必传（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询车牌服务开通信息，见 <see cref="VehicleParkingServiceResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/vehicle/parking/services/find</c>；query <c>appid</c> / <c>plate_number</c> / <c>openid</c> / <c>plate_color</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>appid</c> 与 <c>plate_number</c> 与 <c>openid</c> 与 <c>plate_color</c> 与 <c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：用户信息有误，请检查OpenID / 403 NO_AUTH：商户未入驻 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误 / 400 INVALID_REQUEST：子商户号不存在，请检查后再试。</para>
    /// </remarks>
    [Get("/v3/vehicle/parking/services/find")]
    Task<VehicleParkingServiceResponse> QueryVehicleParkingServiceAsync(
        [Query("appid")] string appid,
        [Query("plate_number")] string plateNumber,
        [Query("openid")] string openid,
        [Query("plate_color")] string plateColor,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 停车订单查询（商户订单号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534308"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534441"/>（【普通服务商】）。
    /// </summary>
    /// <param name="outTradeNo">【商户订单号】商户系统内部订单号，只能是数字、大小写字母，且在同一个商户号下唯一（官方 path <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【子商户号】微信支付分配的子商户号，服务商模式下必传（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>停车订单查询（商户订单号），见 <see cref="VehicleParkingTransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/vehicle/transactions/out-trade-no/{out_trade_no}</c>；path <c>out_trade_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：不支持查询非支付分停车订单，请更换单号后再试 / 400 INVALID_REQUEST：服务商和子商户没有绑定关系 / 400 PARAM_ERROR：参数错误，请检查out_trade_no / 404 NOT_FOUND：订单不存在 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Get("/v3/vehicle/transactions/out-trade-no/{outTradeNo}")]
    Task<VehicleParkingTransactionQueryResponse> QueryVehicleParkingTransactionByOutTradeNoAsync(
        [Path] string outTradeNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 停车扣费受理下单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">停车扣费受理下单请求体，字段见 <see cref="VehicleParkingTransactionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>停车扣费受理下单，见 <see cref="VehicleParkingTransactionResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/vehicle/transactions/parking</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>应答</b>见 <see cref="VehicleParkingTransactionResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 ALREADY_EXISTS：out_trade_no冲突 / 400 INVALID_REQUEST：非法商户，服务商模式下请确认子商户正确 / 400 INVALID_REQUEST：商户号和公众号没有绑定关系 / 400 INVALID_REQUEST：商户号、子商户号以及子公众号没有绑定关系 / 400 INVALID_REQUEST：用户状态异常 / 400 INVALID_REQUEST：停车入场ID不存在 / 400 INVALID_REQUEST：场景信息和停车入场ID不匹配 / 400 INVALID_REQUEST：扣款额度超出限制 / 403 NO_AUTH：当前商户无法扣款 / 403 NOT_ENOUGH：用户欠款过多，暂停交易 / 403 RULE_LIMIT：超出扣费次数限制 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Post("/v3/vehicle/transactions/parking")]
    Task<VehicleParkingTransactionResponse> CreateVehicleParkingTransactionAsync(
        [Body] VehicleParkingTransactionRequest request,
        CancellationToken cancellationToken = default);
}
