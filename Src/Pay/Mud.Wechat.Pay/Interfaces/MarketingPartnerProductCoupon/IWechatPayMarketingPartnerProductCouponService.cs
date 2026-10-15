// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingPartnerProductCoupon;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「服务商商品券」域 SDK（34 端点）。
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
[HttpClientApi(RegistryGroupName = "MarketingPartnerProductCoupon", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingPartnerProductCouponService
{
    /// <summary>
    /// 创建商品券图片生成任务。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327735"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327752"/>（普通服务商）。
    /// </summary>
    /// <param name="request">创建商品券图片生成任务请求体，字段见 <see cref="ProductCouponImageTaskCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建商品券图片生成任务，见 <see cref="ProductCouponImageTaskCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/image-generation-tasks</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponImageTaskCreateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/image-generation-tasks")]
    Task<ProductCouponImageTaskCreateResponse> CreateProductCouponImageTaskAsync(
        [Body] ProductCouponImageTaskCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商品券图片生成任务。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327739"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327753"/>（普通服务商）。
    /// </summary>
    /// <param name="taskId">【图片生成的任务ID】 服务商创建图片生成任务的请求流水号，服务商侧需保持唯一性，可使用 数字、大小写字母、下划线 _、短横线 - 组成，长度在6-40个字符之间（官方 path <c>task_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="brandId">【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="imageGenerationType">【图片生成类型】 与提交图片生成任务时所传的类型一致。可选取值：`COMBINE_IMAGE` 适合全场类优惠头图，将品牌元素与商品券优惠等信息拼接生成头图 / `CUT_OUT` 适合单品类优惠头图，用于去除图片背景，保留商品主体（官方 query <c>image_generation_type</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商品券图片生成任务，见 <see cref="ProductCouponImageTaskQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/product-coupon/image-generation-tasks/{task_id}</c>；path <c>task_id</c>，query <c>brand_id</c> / <c>image_generation_type</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 与 <c>image_generation_type</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/product-coupon/image-generation-tasks/{taskId}")]
    Task<ProductCouponImageTaskQueryResponse> QueryProductCouponImageTaskAsync(
        [Path] string taskId,
        [Query("brand_id")] string brandId,
        [Query("image_generation_type")] string imageGenerationType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置商品券通知回调。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781286"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016435719"/>（普通服务商）。
    /// </summary>
    /// <param name="request">设置商品券通知回调请求体，字段见 <see cref="ProductCouponNotifyConfigSetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>设置商品券通知回调，见 <see cref="ProductCouponNotifyConfigSetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/notify-configs</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponNotifyConfigSetResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/notify-configs")]
    Task<ProductCouponNotifyConfigSetResponse> SetProductCouponNotifyConfigAsync(
        [Body] ProductCouponNotifyConfigSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商品券通知回调。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781284"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016435718"/>（普通服务商）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商品券通知回调，见 <see cref="ProductCouponNotifyConfigQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/product-coupon/notify-configs</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/product-coupon/notify-configs")]
    Task<ProductCouponNotifyConfigQueryResponse> QueryProductCouponNotifyConfigAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建商品券（单品批次形态）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434630"/>（普通服务商）。
    /// </summary>
    /// <param name="request">创建商品券（单品批次形态）请求体，字段见 <see cref="ProductCouponCouponCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建商品券（单品批次形态），见 <see cref="ProductCouponCouponCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponCouponCreateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons")]
    Task<ProductCouponCouponCreateResponse> CreateProductCouponAsync(
        [Body] ProductCouponCouponCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建商品券（批次组合形态）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434630"/>（普通服务商）。
    /// </summary>
    /// <param name="request">创建商品券（批次组合形态）请求体，字段见 <see cref="ProductCouponBundleCouponCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建商品券（批次组合形态），见 <see cref="ProductCouponBundleCouponCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponBundleCouponCreateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons")]
    Task<ProductCouponBundleCouponCreateResponse> CreateProductCouponBundleAsync(
        [Body] ProductCouponBundleCouponCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商品券详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="brandId">【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商品券详情，见 <see cref="ProductCouponCouponQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}</c>；path <c>product_coupon_id</c>，query <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}")]
    Task<ProductCouponCouponQueryResponse> QueryProductCouponAsync(
        [Path] string productCouponId,
        [Query("brand_id")] string brandId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改商品券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434633"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">修改商品券请求体，字段见 <see cref="ProductCouponCouponUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改商品券，见 <see cref="ProductCouponCouponUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}</c>；path <c>product_coupon_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponCouponUpdateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Patch("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}")]
    Task<ProductCouponCouponUpdateResponse> UpdateProductCouponAsync(
        [Path] string productCouponId,
        [Body] ProductCouponCouponUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 作废商品券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434631"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">作废商品券请求体，字段见 <see cref="ProductCouponCouponDeactivateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>作废商品券，见 <see cref="ProductCouponCouponDeactivateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/deactivate</c>；path <c>product_coupon_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponCouponDeactivateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/deactivate")]
    Task<ProductCouponCouponDeactivateResponse> DeactivateProductCouponAsync(
        [Path] string productCouponId,
        [Body] ProductCouponCouponDeactivateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建商品券批次组合。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">创建商品券批次组合请求体，字段见 <see cref="ProductCouponStockBundleCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建商品券批次组合，见 <see cref="ProductCouponStockBundleCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles</c>；path <c>product_coupon_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockBundleCreateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles")]
    Task<ProductCouponStockBundleCreateResponse> CreateProductCouponStockBundleAsync(
        [Path] string productCouponId,
        [Body] ProductCouponStockBundleCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改商品券批次组合。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockBundleId">【批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成，本接口会同步修改批次组内所有批次的基本信息，请确保该批次组属于 product_coupon_id 对应的商品券（官方 path <c>stock_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">修改商品券批次组合请求体，字段见 <see cref="ProductCouponStockBundleUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改商品券批次组合，见 <see cref="ProductCouponStockBundleUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles/{stock_bundle_id}</c>；path <c>product_coupon_id</c> / <c>stock_bundle_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockBundleUpdateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Patch("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}")]
    Task<ProductCouponStockBundleUpdateResponse> UpdateProductCouponStockBundleAsync(
        [Path] string productCouponId,
        [Path] string stockBundleId,
        [Body] ProductCouponStockBundleUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批次组合关联门店。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280620"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockBundleId">【批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成，本接口会同步关联门店信息到批次组内所有批次。请确保该批次组属于 product_coupon_id 对应的商品券，且 store_scope 为 SPECIFIC，否则无法关联门店（官方 path <c>stock_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">批次组合关联门店请求体，字段见 <see cref="ProductCouponStockBundleAssociateStoresRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次组合关联门店，见 <see cref="ProductCouponStockBundleAssociateStoresResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles/{stock_bundle_id}/associate-stores</c>；path <c>product_coupon_id</c> / <c>stock_bundle_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockBundleAssociateStoresResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/associate-stores")]
    Task<ProductCouponStockBundleAssociateStoresResponse> AssociateProductCouponStockBundleStoresAsync(
        [Path] string productCouponId,
        [Path] string stockBundleId,
        [Body] ProductCouponStockBundleAssociateStoresRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批次组合解除门店关联。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280629"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockBundleId">【批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成，本接口会同步取消关联门店信息到批次组内所有批次。请确保该批次组属于 product_coupon_id 对应的商品券，且 store_scope 为 SPECIFIC，否则无法取消关联门店（官方 path <c>stock_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">批次组合解除门店关联请求体，字段见 <see cref="ProductCouponStockBundleDisassociateStoresRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次组合解除门店关联，见 <see cref="ProductCouponStockBundleDisassociateStoresResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles/{stock_bundle_id}/disassociate-stores</c>；path <c>product_coupon_id</c> / <c>stock_bundle_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockBundleDisassociateStoresResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/disassociate-stores")]
    Task<ProductCouponStockBundleDisassociateStoresResponse> DisassociateProductCouponStockBundleStoresAsync(
        [Path] string productCouponId,
        [Path] string stockBundleId,
        [Body] ProductCouponStockBundleDisassociateStoresRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改批次组合预算。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockBundleId">【批次组ID】 商品券批次组的唯一标识，由微信支付生成，本接口会同步修改批次组内所有批次的对应预算信息。请确保该批次组属于 product_coupon_id 对应的商品券（官方 path <c>stock_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">修改批次组合预算请求体，字段见 <see cref="ProductCouponStockBundleBudgetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改批次组合预算，见 <see cref="ProductCouponStockBundleBudgetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles/{stock_bundle_id}/update-budget</c>；path <c>product_coupon_id</c> / <c>stock_bundle_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockBundleBudgetResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/update-budget")]
    Task<ProductCouponStockBundleBudgetResponse> UpdateProductCouponStockBundleBudgetAsync(
        [Path] string productCouponId,
        [Path] string stockBundleId,
        [Body] ProductCouponStockBundleBudgetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商品券批次列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781553"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434641"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="brandId">【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="state">【批次状态】 不填默认查询所有状态的批次。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方 query <c>state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="pageSize">【分页大小】 单次拉取的数据条数上限，不填默认为20，最大值50（官方 query <c>page_size</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="pageToken">【分页Token】 分页查询时，需要传入上一次调用返回的 next_page_token，首次调用不填（官方 query <c>page_token</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="stockBundleId">【批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成。（官方 query <c>stock_bundle_id</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商品券批次列表，见 <see cref="ProductCouponStockListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks</c>；path <c>product_coupon_id</c>，query <c>brand_id</c> / <c>state</c> / <c>page_size</c> / <c>page_token</c> / <c>stock_bundle_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks")]
    Task<ProductCouponStockListResponse> QueryProductCouponStocksAsync(
        [Path] string productCouponId,
        [Query("brand_id")] string brandId,
        [Query("state")] string? state = null,
        [Query("page_size")] int? pageSize = null,
        [Query("page_token")] string? pageToken = null,
        [Query("stock_bundle_id")] string? stockBundleId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建商品券批次。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781304"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">创建商品券批次请求体，字段见 <see cref="ProductCouponStockCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建商品券批次，见 <see cref="ProductCouponStockCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks</c>；path <c>product_coupon_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockCreateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks")]
    Task<ProductCouponStockCreateResponse> CreateProductCouponStockAsync(
        [Path] string productCouponId,
        [Body] ProductCouponStockCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商品券批次详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781542"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434649"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="brandId">【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商品券批次详情，见 <see cref="ProductCouponStockQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}</c>；path <c>product_coupon_id</c> / <c>stock_id</c>，query <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}")]
    Task<ProductCouponStockQueryResponse> QueryProductCouponStockAsync(
        [Path] string productCouponId,
        [Path] string stockId,
        [Query("brand_id")] string brandId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改商品券批次。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781556"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">修改商品券批次请求体，字段见 <see cref="ProductCouponStockUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改商品券批次，见 <see cref="ProductCouponStockUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}</c>；path <c>product_coupon_id</c> / <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockUpdateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Patch("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}")]
    Task<ProductCouponStockUpdateResponse> UpdateProductCouponStockAsync(
        [Path] string productCouponId,
        [Path] string stockId,
        [Body] ProductCouponStockUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 作废商品券批次。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781532"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">作废商品券批次请求体，字段见 <see cref="ProductCouponStockDeactivateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>作废商品券批次，见 <see cref="ProductCouponStockDeactivateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/deactivate</c>；path <c>product_coupon_id</c> / <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockDeactivateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/deactivate")]
    Task<ProductCouponStockDeactivateResponse> DeactivateProductCouponStockAsync(
        [Path] string productCouponId,
        [Path] string stockId,
        [Body] ProductCouponStockDeactivateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改商品券批次预算。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781561"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">修改商品券批次预算请求体，字段见 <see cref="ProductCouponStockBudgetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改商品券批次预算，见 <see cref="ProductCouponStockBudgetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/update-budget</c>；path <c>product_coupon_id</c> / <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockBudgetResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/update-budget")]
    Task<ProductCouponStockBudgetResponse> UpdateProductCouponStockBudgetAsync(
        [Path] string productCouponId,
        [Path] string stockId,
        [Body] ProductCouponStockBudgetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批次关联门店。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781302"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券，且 store_scope 为 SPECIFIC，否则无法关联门店（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">批次关联门店请求体，字段见 <see cref="ProductCouponStockAssociateStoresRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次关联门店，见 <see cref="ProductCouponStockAssociateStoresResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/associate-stores</c>；path <c>product_coupon_id</c> / <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockAssociateStoresResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/associate-stores")]
    Task<ProductCouponStockAssociateStoresResponse> AssociateProductCouponStockStoresAsync(
        [Path] string productCouponId,
        [Path] string stockId,
        [Body] ProductCouponStockAssociateStoresRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批次解除门店关联。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781537"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券，且 store_scope 为 SPECIFIC，否则无法取消关联门店（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">批次解除门店关联请求体，字段见 <see cref="ProductCouponStockDisassociateStoresRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次解除门店关联，见 <see cref="ProductCouponStockDisassociateStoresResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/disassociate-stores</c>；path <c>product_coupon_id</c> / <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockDisassociateStoresResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/disassociate-stores")]
    Task<ProductCouponStockDisassociateStoresResponse> DisassociateProductCouponStockStoresAsync(
        [Path] string productCouponId,
        [Path] string stockId,
        [Body] ProductCouponStockDisassociateStoresRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询批次已关联门店。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781546"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434665"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="brandId">【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="pageSize">【分页大小】 单次拉取的数据条数上限，不填默认为20，最大值200（官方 query <c>page_size</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="pageToken">【分页Token】 分页查询时，需要传入上一次调用返回的 next_page_token，首次调用不填（官方 query <c>page_token</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询批次已关联门店，见 <see cref="ProductCouponStockAssociatedStoresResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/associated-stores</c>；path <c>product_coupon_id</c> / <c>stock_id</c>，query <c>brand_id</c> / <c>page_size</c> / <c>page_token</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/associated-stores")]
    Task<ProductCouponStockAssociatedStoresResponse> QueryProductCouponAssociatedStoresAsync(
        [Path] string productCouponId,
        [Path] string stockId,
        [Query("brand_id")] string brandId,
        [Query("page_size")] int? pageSize = null,
        [Query("page_token")] string? pageToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预上传商品券券码。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781572"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434668"/>（普通服务商）。
    /// </summary>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">预上传商品券券码请求体，字段见 <see cref="ProductCouponStockUploadCodesRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预上传商品券券码，见 <see cref="ProductCouponStockUploadCodesResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/upload-coupon-codes</c>；path <c>product_coupon_id</c> / <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponStockUploadCodesResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/upload-coupon-codes")]
    Task<ProductCouponStockUploadCodesResponse> UploadProductCouponStockCodesAsync(
        [Path] string productCouponId,
        [Path] string stockId,
        [Body] ProductCouponStockUploadCodesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 向用户发放券包。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280664"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">向用户发放券包请求体，字段见 <see cref="ProductCouponUserBundleSendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>向用户发放券包，见 <see cref="ProductCouponUserBundleSendResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles</c>；path <c>openid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponUserBundleSendResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles")]
    Task<ProductCouponUserBundleSendResponse> SendProductCouponUserBundleAsync(
        [Path] string openid,
        [Body] ProductCouponUserBundleSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 作废用户券包。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280658"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="userCouponBundleId">【用户券组ID】 用户券组的唯一标识，【 向用户发放商品券批次组】时由微信支付生成，本接口会同步失效用户商品券组内所有用户商品券（官方 path <c>user_coupon_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</param>
    /// <param name="request">作废用户券包请求体，字段见 <see cref="ProductCouponUserBundleDeactivateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>作废用户券包，见 <see cref="ProductCouponUserBundleDeactivateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles/{user_coupon_bundle_id}/deactivate</c>；path <c>openid</c> / <c>user_coupon_bundle_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponUserBundleDeactivateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles/{userCouponBundleId}/deactivate")]
    Task<ProductCouponUserBundleDeactivateResponse> DeactivateProductCouponUserBundleAsync(
        [Path] string openid,
        [Path] string userCouponBundleId,
        [Body] ProductCouponUserBundleDeactivateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 向用户发放商品券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">向用户发放商品券请求体，字段见 <see cref="ProductCouponUserCouponSendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>向用户发放商品券，见 <see cref="ProductCouponUserCouponSendResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupons</c>；path <c>openid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponUserCouponSendResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/users/{openid}/coupons")]
    Task<ProductCouponUserCouponSendResponse> SendProductCouponUserCouponAsync(
        [Path] string openid,
        [Body] ProductCouponUserCouponSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户商品券列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016435703"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="appid">【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后与品牌方或服务商绑定（详见 服务商商户号与AppID账号关联管理）。（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="brandId">【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成。查询指定商品券下的用户商品券列表，不传入时查询范围扩大至品牌下的所有商品券（官方 query <c>product_coupon_id</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建）。查询指定商品券批次下的用户商品券列表。（官方 query <c>stock_id</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="couponState">【用户商品券状态】 查询指定状态的用户商品券。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间…（官方 query <c>coupon_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="userCouponBundleId">【用户券组ID】 用户券组的唯一标识，【 向用户发放商品券批次组】时由微信支付生成，用于查询特定用户券组内的券（官方 query <c>user_coupon_bundle_id</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="pageSize">【分页大小】 单次拉取的数据条数上限，不填默认为20，最大值50（官方 query <c>page_size</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="pageToken">【分页Token】 分页查询时，需要传入上一次调用返回的 next_page_token，首次调用不填（官方 query <c>page_token</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询用户商品券列表，见 <see cref="ProductCouponUserCouponListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupons</c>；path <c>openid</c>，query <c>appid</c> / <c>brand_id</c> / <c>product_coupon_id</c> / <c>stock_id</c> / <c>coupon_state</c> / <c>user_coupon_bundle_id</c> / <c>page_size</c> / <c>page_token</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>appid</c> 与 <c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/product-coupon/users/{openid}/coupons")]
    Task<ProductCouponUserCouponListResponse> QueryProductCouponUserCouponsAsync(
        [Path] string openid,
        [Query("appid")] string appid,
        [Query("brand_id")] string brandId,
        [Query("product_coupon_id")] string? productCouponId = null,
        [Query("stock_id")] string? stockId = null,
        [Query("coupon_state")] string? couponState = null,
        [Query("user_coupon_bundle_id")] string? userCouponBundleId = null,
        [Query("page_size")] int? pageSize = null,
        [Query("page_token")] string? pageToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户商品券详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016435702"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="couponCode">【用户商品券Code】 券的唯一标识（官方 path <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</param>
    /// <param name="productCouponId">【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 query <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="stockId">【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 query <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="appid">【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后与品牌方或服务商绑定（详见 服务商商户号与AppID账号关联管理）。（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="brandId">【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询用户商品券详情，见 <see cref="ProductCouponUserCouponQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}</c>；path <c>openid</c> / <c>coupon_code</c>，query <c>product_coupon_id</c> / <c>stock_id</c> / <c>appid</c> / <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>product_coupon_id</c> 与 <c>stock_id</c> 与 <c>appid</c> 与 <c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}")]
    Task<ProductCouponUserCouponQueryResponse> QueryProductCouponUserCouponAsync(
        [Path] string openid,
        [Path] string couponCode,
        [Query("product_coupon_id")] string productCouponId,
        [Query("stock_id")] string stockId,
        [Query("appid")] string appid,
        [Query("brand_id")] string brandId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 确认用户商品券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016435562"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="couponCode">【用户商品券Code】 用户商品券的唯一标识（官方 path <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</param>
    /// <param name="request">确认用户商品券请求体，字段见 <see cref="ProductCouponUserCouponConfirmRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>确认用户商品券，见 <see cref="ProductCouponUserCouponConfirmResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}/confirm</c>；path <c>openid</c> / <c>coupon_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponUserCouponConfirmResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/confirm")]
    Task<ProductCouponUserCouponConfirmResponse> ConfirmProductCouponUserCouponAsync(
        [Path] string openid,
        [Path] string couponCode,
        [Body] ProductCouponUserCouponConfirmRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 退用户商品券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016435674"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="couponCode">【用户商品券Code】 用户商品券的唯一标识（官方 path <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</param>
    /// <param name="request">退用户商品券请求体，字段见 <see cref="ProductCouponUserCouponReturnRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退用户商品券，见 <see cref="ProductCouponUserCouponReturnResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}/return</c>；path <c>openid</c> / <c>coupon_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponUserCouponReturnResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/return")]
    Task<ProductCouponUserCouponReturnResponse> ReturnProductCouponUserCouponAsync(
        [Path] string openid,
        [Path] string couponCode,
        [Body] ProductCouponUserCouponReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 核销用户商品券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/>（普通服务商）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4016435636"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="couponCode">【用户商品券Code】 用户商品券的唯一标识（官方 path <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</param>
    /// <param name="request">核销用户商品券请求体，字段见 <see cref="ProductCouponUserCouponUseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>核销用户商品券，见 <see cref="ProductCouponUserCouponUseResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}/use</c>；path <c>openid</c> / <c>coupon_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponUserCouponUseResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/use")]
    Task<ProductCouponUserCouponUseResponse> UseProductCouponUserCouponAsync(
        [Path] string openid,
        [Path] string couponCode,
        [Body] ProductCouponUserCouponUseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 作废用户商品券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="couponCode">【用户商品券Code】 用户商品券的唯一标识（官方 path <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</param>
    /// <param name="request">作废用户商品券请求体，字段见 <see cref="ProductCouponUserCouponDeactivateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>作废用户商品券，见 <see cref="ProductCouponUserCouponDeactivateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}/deactivate</c>；path <c>openid</c> / <c>coupon_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="ProductCouponUserCouponDeactivateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/deactivate")]
    Task<ProductCouponUserCouponDeactivateResponse> DeactivateProductCouponUserCouponAsync(
        [Path] string openid,
        [Path] string couponCode,
        [Body] ProductCouponUserCouponDeactivateRequest request,
        CancellationToken cancellationToken = default);
}
