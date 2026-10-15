// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingGoodsSubsidyActivity;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商品补贴活动」域 SDK（8 端点）。
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
[HttpClientApi(RegistryGroupName = "MarketingGoodsSubsidyActivity", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingGoodsSubsidyActivityService
{
    /// <summary>
    /// 设置活动门店 representative。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384386"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【零售小店活动ID】 零售小店活动ID（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="request">设置活动门店 representative请求体，字段见 <see cref="RetailStoreActRepresentativeSetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>设置活动门店 representative，见 <see cref="RetailStoreActRepresentativeSetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PUT</b> <c>/v3/marketing/goods-subsidy-activity/retail-store-act/{activity_id}/representative</c>；path <c>activity_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RetailStoreActRepresentativeSetResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 无效请求 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Put("/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representative")]
    Task<RetailStoreActRepresentativeSetResponse> SetRetailStoreRepresentativeAsync(
        [Path] string activityId,
        [Body] RetailStoreActRepresentativeSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除活动门店 representative。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384399"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【零售小店活动ID】 零售小店活动ID（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="request">删除活动门店 representative请求体，字段见 <see cref="RetailStoreActRepresentativeDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>删除活动门店 representative，见 <see cref="RetailStoreActRepresentativeDeleteResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>DELETE</b> <c>/v3/marketing/goods-subsidy-activity/retail-store-act/{activity_id}/representative</c>；path <c>activity_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RetailStoreActRepresentativeDeleteResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 无效请求 / 400 INVALID_REQUEST 参数错误 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Delete("/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representative")]
    Task<RetailStoreActRepresentativeDeleteResponse> DeleteRetailStoreRepresentativeAsync(
        [Path] string activityId,
        [Body] RetailStoreActRepresentativeDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询活动门店 representative 列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384405"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【零售小店活动ID】 零售小店活动ID（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="offset">【分页页码】 页码从0开始，默认第0页（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，默认查询10个业代（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询活动门店 representative 列表，见 <see cref="RetailStoreActRepresentativeListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/goods-subsidy-activity/retail-store-act/{activity_id}/representatives</c>；path <c>activity_id</c>，query <c>offset</c> / <c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>offset</c> 与 <c>limit</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 无效请求 / 400 INVALID_REQUEST 参数错误 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Get("/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representatives")]
    Task<RetailStoreActRepresentativeListResponse> QueryRetailStoreRepresentativesAsync(
        [Path] string activityId,
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传小店活动素材。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384395"/>（普通服务商）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 品牌ID（官方 path <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">上传小店活动素材请求体，字段见 <see cref="RetailStoreActMaterialUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>上传小店活动素材，见 <see cref="RetailStoreActMaterialUploadResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/materials</c>；path <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RetailStoreActMaterialUploadResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 无效请求 / 400 INVALID_REQUEST 参数错误 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Post("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/materials")]
    Task<RetailStoreActMaterialUploadResponse> UploadRetailStoreMaterialsAsync(
        [Path] string brandId,
        [Body] RetailStoreActMaterialUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加小店活动门店。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384421"/>（普通服务商）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 品牌ID（官方 path <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">添加小店活动门店请求体，字段见 <see cref="RetailStoreActStoreAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>添加小店活动门店，见 <see cref="RetailStoreActStoreAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/stores</c>；path <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RetailStoreActStoreAddResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 无效请求 / 400 INVALID_REQUEST 参数错误 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Post("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores")]
    Task<RetailStoreActStoreAddResponse> AddRetailStoreStoresAsync(
        [Path] string brandId,
        [Body] RetailStoreActStoreAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除小店活动门店。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384431"/>（普通服务商）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 品牌ID（官方 path <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">删除小店活动门店请求体，字段见 <see cref="RetailStoreActStoreDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>删除小店活动门店，见 <see cref="RetailStoreActStoreDeleteResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>DELETE</b> <c>/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/stores</c>；path <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RetailStoreActStoreDeleteResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 无效请求 / 400 INVALID_REQUEST 参数错误 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Delete("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores")]
    Task<RetailStoreActStoreDeleteResponse> DeleteRetailStoreStoresAsync(
        [Path] string brandId,
        [Body] RetailStoreActStoreDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询小店活动门店列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384414"/>（普通服务商）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 品牌ID（官方 path <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="offset">【分页页码】 页码从0开始，默认第0页（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，默认查询10家门店（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询小店活动门店列表，见 <see cref="RetailStoreActStoreListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/stores</c>；path <c>brand_id</c>，query <c>offset</c> / <c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>offset</c> 与 <c>limit</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 无效请求 / 400 INVALID_REQUEST 参数错误 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Get("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores")]
    Task<RetailStoreActStoreListResponse> QueryRetailStoreStoresAsync(
        [Path] string brandId,
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询小店活动门店详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384437"/>（普通服务商）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 品牌ID（官方 path <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="storeCode">【门店编码】 门店编码，可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方 path <c>store_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询小店活动门店详情，见 <see cref="RetailStoreActStoreQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/stores/{store_code}</c>；path <c>brand_id</c> / <c>store_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 无效请求 / 400 INVALID_REQUEST 参数错误 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Get("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores/{storeCode}")]
    Task<RetailStoreActStoreQueryResponse> QueryRetailStoreStoreAsync(
        [Path] string brandId,
        [Path] string storeCode,
        CancellationToken cancellationToken = default);
}
