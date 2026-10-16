// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingBusifavor;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商家券」域 SDK（15 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径；同时有【普通商户】与【普通服务商】两页的端点，两页字段表经逐页指纹比对<b>完全同形</b>
/// ⇒ <b>一面一方法 + 双页文档引用</b>，只有字段表存在差异的端点才按「同路由多方法」分建。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>分页口径不统一</b>：本域既有 <c>offset</c>/<c>limit</c>（整型页码）也有 <c>page_size</c> + <c>offset</c> 形态，
/// 一律<b>照页</b>，不得互相套用；应答的 <c>total_count</c> 官方多为选填（<c>offset=0</c> 或结果为空时才回）。</para>
/// <para><b>PATCH 与 PUT</b>：批次/活动的改预算与改状态走 <b>PATCH</b>，终止走 <b>POST .../terminate</b>
/// ⇒ 「写操作动词」在本域不统一，照页不得合并。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MarketingBusifavor", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingBusifavorService
{
    /// <summary>
    /// 商家券回调地址。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465507"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465557"/>（普通服务商）。
    /// </summary>
    /// <param name="mchid">【商户号】商户号，不填默认查询调用方商户号（官方 query <c>mchid</c>，选填，类型 <c>string(15)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家券回调地址，见 <see cref="BusifavorCallbackQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/busifavor/callbacks</c>；query <c>mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Get("/v3/marketing/busifavor/callbacks")]
    Task<BusifavorCallbackQueryResponse> QueryBusifavorCallbackAsync(
        [Query("mchid")] string? mchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置商家券回调地址。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465431"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465425"/>（普通服务商）。
    /// </summary>
    /// <param name="request">设置商家券回调地址请求体，字段见 <see cref="BusifavorCallbackSetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>设置商家券回调地址，见 <see cref="BusifavorCallbackSetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/callbacks</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorCallbackSetResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/callbacks")]
    Task<BusifavorCallbackSetResponse> SetBusifavorCallbackAsync(
        [Body] BusifavorCallbackSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家券关联订单补贴。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465596"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465640"/>（普通服务商）。
    /// </summary>
    /// <param name="request">商家券关联订单补贴请求体，字段见 <see cref="BusifavorCouponAssociateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家券关联订单补贴，见 <see cref="BusifavorCouponAssociateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/coupons/associate</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>（官方原样）：频率：500QPS。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorCouponAssociateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 券不存在 / 400 INVALID_REQUEST 券状态非可用态 / 400 INVALID_REQUEST 券已被其他单号关联 / 400 INVALID_REQUEST 券已被其他单号关联，请用正确单号重试 / 400 INVALID_REQUEST 商户无权限操作该券。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/coupons/associate")]
    Task<BusifavorCouponAssociateResponse> AssociateBusifavorCouponAsync(
        [Body] BusifavorCouponAssociateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家券取消关联订单补贴。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465673"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465703"/>（普通服务商）。
    /// </summary>
    /// <param name="request">商家券取消关联订单补贴请求体，字段见 <see cref="BusifavorCouponDisassociateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家券取消关联订单补贴，见 <see cref="BusifavorCouponDisassociateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/coupons/disassociate</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>（官方原样）：频率：500QPS。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorCouponDisassociateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 券不存在 / 400 INVALID_REQUEST 券状态非可用态 / 400 INVALID_REQUEST 券已被其他单号关联 / 400 INVALID_REQUEST 券已被其他单号关联，请用正确单号重试 / 400 INVALID_REQUEST 商户无权限操作该券。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/coupons/disassociate")]
    Task<BusifavorCouponDisassociateResponse> DisassociateBusifavorCouponAsync(
        [Body] BusifavorCouponDisassociateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 作废商家券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465890"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465924"/>（普通服务商）。
    /// </summary>
    /// <param name="request">作废商家券请求体，字段见 <see cref="BusifavorCouponDeactivateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>作废商家券，见 <see cref="BusifavorCouponDeactivateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/coupons/deactivate</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorCouponDeactivateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：403 RULE_LIMIT 券不属于指定的批次 / 403 RULE_LIMIT 仅批次创建方有权限使券失效 / 403 RULE_LIMIT 券已被核销 / 403 RULE_LIMIT 券已过期，不能再进行其他操作 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 404 RESOURCE_NOT_EXISTS 券不存在 / 404 RESOURCE_NOT_EXISTS 券已被删除。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/coupons/deactivate")]
    Task<BusifavorCouponDeactivateResponse> DeactivateBusifavorCouponAsync(
        [Body] BusifavorCouponDeactivateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家券退款退券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465845"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465865"/>（普通服务商）。
    /// </summary>
    /// <param name="request">商家券退款退券请求体，字段见 <see cref="BusifavorCouponReturnRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家券退款退券，见 <see cref="BusifavorCouponReturnResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/coupons/return</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorCouponReturnResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 RESOURCE_ALREADY_EXISTS 已使用过此单号退券 / 403 NO_AUTH 商户无权限操作该券 / 403 RULE_LIMIT 券当前是可用的 / 403 RULE_LIMIT 券已不在使用有效期 / 404 RESOURCE_NOT_EXISTS 券不存在 / 404 RESOURCE_NOT_EXISTS 券已被删除。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/coupons/return")]
    Task<BusifavorCouponReturnResponse> ReturnBusifavorCouponAsync(
        [Body] BusifavorCouponReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 核销商家券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465292"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465359"/>（普通服务商）。
    /// </summary>
    /// <param name="request">核销商家券请求体，字段见 <see cref="BusifavorCouponUseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>核销商家券，见 <see cref="BusifavorCouponUseResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/coupons/use</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>（官方原样）：核销频率：500QPS。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorCouponUseResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/coupons/use")]
    Task<BusifavorCouponUseResponse> UseBusifavorCouponAsync(
        [Body] BusifavorCouponUseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 向用户发放商品券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015717371"/>（普通商户）。
    /// </summary>
    /// <param name="request">向用户发放商品券请求体，字段见 <see cref="BusifavorProductCouponSendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>向用户发放商品券，见 <see cref="BusifavorProductCouponSendResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/product-coupons/send</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorProductCouponSendResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID 与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 发券凭证已存在 / 400 STOCK_NOT_EXISTS 批次不存在 / 400 STOCK_STATE_ERROR 批次状态错误 / 400 STOCK_EXPIRED 批次已过期 / 400 STOCK_INSUFFICIENT 批次库存不足 / 400 STOCK_NO_PRODUCT 批次未关联商品信息 / 400 USER_ALREADY_RECEIVED 用户已领取 / 400 USER_RECEIVE_LIMIT 用户领取限制 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券批次未关联商品信息 / 403 RULE_LIMIT 品牌未开通名片功能 / 403 RULE_LIMIT 用户未开通会员功能 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/product-coupons/send")]
    Task<BusifavorProductCouponSendResponse> SendBusifavorProductCouponAsync(
        [Body] BusifavorProductCouponSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建商家券批次。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692931"/>（普通服务商）。
    /// </summary>
    /// <param name="request">创建商家券批次请求体，字段见 <see cref="BusifavorStockCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建商家券批次，见 <see cref="BusifavorStockCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/stocks</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>（官方原样）：频率限制：接口级限制1000/min。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorStockCreateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/stocks")]
    Task<BusifavorStockCreateResponse> CreateBusifavorStockAsync(
        [Body] BusifavorStockCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商家券批次详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012693047"/>（普通服务商）。
    /// </summary>
    /// <param name="stockId">【批次号】微信为每个商家券批次分配的唯一ID（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商家券批次详情，见 <see cref="BusifavorStockQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/busifavor/stocks/{stock_id}</c>；path <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Get("/v3/marketing/busifavor/stocks/{stockId}")]
    Task<BusifavorStockQueryResponse> QueryBusifavorStockAsync(
        [Path] string stockId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改商家券批次。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012696764"/>（普通服务商）。
    /// </summary>
    /// <param name="stockId">【批次号】批次号（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">修改商家券批次请求体，字段见 <see cref="BusifavorStockUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段 ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/busifavor/stocks/{stock_id}</c>；path <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 修改跳转到小程序的自定义入口请填入小程序AppID / 400 INVALID_REQUEST 修改字段中包含敏感词 / 400 INVALID_REQUEST 修改的AppID与商户号没有绑定关系 / 400 INVALID_REQUEST 修改的图片URL包含敏感内容 / 403 RULE_LIMIT 修改流程中仅允许创建方进行操作 / 403 RULE_LIMIT 修改的营销馆无权限 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 404 RESOURCE_NOT_EXISTS 修改的AppID不合法 / 404 RESOURCE_NOT_EXISTS 营销馆ID不存在。</para>
    /// </remarks>
    [Patch("/v3/marketing/busifavor/stocks/{stockId}")]
    Task UpdateBusifavorStockAsync(
        [Path] string stockId,
        [Body] BusifavorStockUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改商家券批次预算。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465739"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465809"/>（普通服务商）。
    /// </summary>
    /// <param name="stockId">【批次号】批次号（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">修改商家券批次预算请求体，字段见 <see cref="BusifavorStockBudgetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改商家券批次预算，见 <see cref="BusifavorStockBudgetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/busifavor/stocks/{stock_id}/budget</c>；path <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorStockBudgetResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：403 RULE_LIMIT 批次库存的当前值错误，请获取批次详情查看最新值 / 403 RULE_LIMIT 仅创建方有权限使券失效 / 403 RULE_LIMIT 批次正在修改中，请稍后重试 / 403 RULE_LIMIT 目标库存数目不能小于批次已发放库存数目 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 404 RESOURCE_NOT_EXISTS 批次已过期 / 429 FREQUENCY_LIMITED 调用太频繁，请降低调用接口频率。</para>
    /// </remarks>
    [Patch("/v3/marketing/busifavor/stocks/{stockId}/budget")]
    Task<BusifavorStockBudgetResponse> UpdateBusifavorStockBudgetAsync(
        [Path] string stockId,
        [Body] BusifavorStockBudgetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预上传商家券自定义券码。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487038"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012492853"/>（普通服务商）。
    /// </summary>
    /// <param name="stockId">【批次号】商家券批次号（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">预上传商家券自定义券码请求体，字段见 <see cref="BusifavorCouponCodesUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预上传商家券自定义券码，见 <see cref="BusifavorCouponCodesUploadResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/stocks/{stock_id}/couponcodes</c>；path <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BusifavorCouponCodesUploadResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/stocks/{stockId}/couponcodes")]
    Task<BusifavorCouponCodesUploadResponse> UploadBusifavorCouponCodesAsync(
        [Path] string stockId,
        [Body] BusifavorCouponCodesUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户商家券列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012693087"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户标识】Openid信息，用户在AppID下的唯一标识。 校验规则：传入的OpenID得是调用方商户号（即请求头里面的商户号）有绑定关系的AppID获取的OpenID或传入的OpenID得是归属商户号有绑定关系的AppID获取的OpenID。获取OpenID文档（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="appid">【公众账号ID】支持传入与当前调用接口商户号有绑定关系的AppID。支持小程序AppID与公众号AppID。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID 或 传入的AppID得是归属商户号有绑定关系的AppID（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次号】批次号，是否指定批次号查询（官方 query <c>stock_id</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="creatorMerchant">【创建批次的商户号】批次创建方商户号 校验规则：当调用方商户号（即请求头中的商户号）为创建批次方商户号时，该参数必传（官方 query <c>creator_merchant</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="belongMerchant">【批次归属商户号】批次归属商户号 校验规则：当调用方商户号（即请求头中的商户号）为批次归属商户号时，该参数必传（官方 query <c>belong_merchant</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="senderMerchant">【批次发放商户号】批次发放商户号 校验规则：当调用方商户号（即请求头中的商户号）为批次发放商户号时，该参数必传；委托营销关系下，请填写委托发券的商户号（官方 query <c>sender_merchant</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="offset">【分页页码】分页页码（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】分页大小（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="couponState">【券状态】商家券状态。可选取值：`SENDED` 可用 / `USED` 已核销 / `EXPIRED` 已过期 / `DELETED` 已删除 / `DEACTIVATED` 已失效（官方 query <c>coupon_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户商家券列表，见 <see cref="BusifavorUserCouponListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/busifavor/users/{openid}/coupons</c>；path <c>openid</c>，query <c>appid</c> / <c>stock_id</c> / <c>creator_merchant</c> / <c>belong_merchant</c> / <c>sender_merchant</c> / <c>offset</c> / <c>limit</c> / <c>coupon_state</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>appid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Get("/v3/marketing/busifavor/users/{openid}/coupons")]
    Task<BusifavorUserCouponListResponse> QueryBusifavorUserCouponsAsync(
        [Path] string openid,
        [Query("appid")] string appid,
        [Query("stock_id")] string? stockId = null,
        [Query("creator_merchant")] string? creatorMerchant = null,
        [Query("belong_merchant")] string? belongMerchant = null,
        [Query("sender_merchant")] string? senderMerchant = null,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("coupon_state")] string? couponState = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户商家券详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012693103"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】OpenID信息，用户在AppID下的唯一标识。 校验规则：传入的OpenID得是调用方商户号（即请求头里面的商户号）有绑定关系的AppID获取的OpenID或传入的OpenID得是归属商户号有绑定关系的AppID获取的OpenID。获取OpenID文档（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="couponCode">【券code】券的唯一标识（官方 path <c>coupon_code</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="appid">【公众账号ID】支持传入与当前调用接口商户号有绑定关系的AppID。支持小程序AppID与公众号AppID。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID或传入的AppID得是归属商户号有绑定关系的AppID（官方 path <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户商家券详情，见 <see cref="BusifavorUserCouponQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/busifavor/users/{openid}/coupons/{coupon_code}/appids/{appid}</c>；path <c>openid</c> / <c>coupon_code</c> / <c>appid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。</para>
    /// </remarks>
    [Get("/v3/marketing/busifavor/users/{openid}/coupons/{couponCode}/appids/{appid}")]
    Task<BusifavorUserCouponQueryResponse> QueryBusifavorUserCouponAsync(
        [Path] string openid,
        [Path] string couponCode,
        [Path] string appid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发放消费卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017164666"/>（【直连商户】）。
    /// </summary>
    /// <param name="cardId">【消费卡ID】 card_id，即创建消费卡后获得的唯一标识（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string[1,28]</c>）。</param>
    /// <param name="request">发放消费卡请求体，字段见 <see cref="BusifavorConsumeCardSendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发放消费卡，见 <see cref="BusifavorConsumeCardSendResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/busifavor/coupons/{card_id}/send</c>；path <c>card_id</c>，body 4 键（<c>appid</c>/<c>openid</c>/<c>out_request_no</c>/<c>send_time</c>），resp 1 键（<c>card_code</c>）。</para>
    /// <para><b>支持商户</b>【直连商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>前置警示</b>：此功能仅向指定邀约商户开放，如有需要请联系微信支付运营经理。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/busifavor/coupons/{cardId}/send")]
    Task<BusifavorConsumeCardSendResponse> SendBusifavorConsumeCardAsync(
        [Path] string cardId,
        [Body] BusifavorConsumeCardSendRequest request,
        CancellationToken cancellationToken = default);
}
