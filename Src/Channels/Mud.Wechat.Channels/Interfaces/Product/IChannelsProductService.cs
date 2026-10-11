// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「商品管理」域 SDK（43 端点：商品增改查 / 上下架 / 审核 /
/// 库存 / 赠品 / 买赠活动 / 限时抢购 / 类目辅助 / 第三方货源）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由形态（设计方案 v1 §4.3）</b>：本域全部 43 个端点走 <c>/channels/ec/product/*</c> 前缀，
/// 无 <c>/shop/*</c> 历史前缀并存。
/// </para>
/// <para>
/// <b>草稿 / 线上双份数据语义</b>：添加商品只影响草稿，需经上架 + 审核通过后草稿才覆盖线上数据
/// （<c>product/add</c>）；<c>product/get</c> 以 <c>data_type</c> 区分返回线上 / 草稿数据。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Product", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsProductService
{
    /// <summary>
    /// 添加商品（<c>product/add</c>）。
    /// </summary>
    /// <param name="request">添加请求（<c>title</c>/<c>head_imgs</c>/<c>deliver_method</c>/<c>cats</c> 或 <c>cats_v2</c>/<c>skus</c>/<c>extra_service</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品 ID 与创建时间（<c>product_id</c>/<c>create_time</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_addproduct"/></para>
    /// <para>
    /// 官方业务限制：①<b>添加商品后仅影响草稿</b>，需调上架接口并审核通过后才覆盖线上数据正式生效；
    /// ②sku 数量超过 25 个时接口会异步更新；③图片（head_img / desc_info.imgs / product_qua_infos[].qua_url[] /
    /// skus[].thumb_img 等）务必使用「上传图片」接口（resp_type=1）并回填返回的 img_url（前缀 mmecimage.cn/p/），
    /// 不接受其他格式。
    /// </para>
    /// <para>专属错误码 <c>10020052</c>（商品不存在）/ <c>10020103</c>（商品数量超限）等，详见守卫。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/add")]
    Task<ChannelsAddProductResponse> AddProductAsync(
        [Body] ChannelsAddProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新商品（<c>product/update</c>）。
    /// </summary>
    /// <param name="request">更新请求（在 <c>ChannelsAddProductRequest</c> 基础上带必填 <c>product_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品 ID 与更新时间（<c>product_id</c>/<c>update_time</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updateproduct"/></para>
    /// <para>
    /// 官方业务限制：①草稿状态非审核中（<c>edit_status != 2</c>）时可更新；②<b>上架过的商品部分字段不可修改
    /// </b>（类目 / SKU 维度等），请查阅官方文档；③更新后草稿不会自动铺到线上。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/update")]
    Task<ChannelsUpdateProductResponse> UpdateProductAsync(
        [Body] ChannelsUpdateProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品（<c>product/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填；<c>data_type</c> 缺省 1）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品线上 / 草稿数据（<c>product</c>/<c>edit_product</c>）及售卖上限提醒 / 信息质量 / 高价预警 / 审核信息。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproduct"/></para>
    /// <para>官方业务限制：<c>data_type</c>：1 获取线上数据，2 获取草稿数据，3 同时获取线上和草稿数据（上架过的商品才有线上数据）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/get")]
    Task<ChannelsGetProductResponse> GetProductAsync(
        [Body] ChannelsGetProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品列表（<c>product/list/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填；<c>status</c> 不填默认拉全部商品（不含草稿与回收站））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品 id 列表与翻页上下文（<c>product_ids</c>/<c>next_key</c>/<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductlist"/></para>
    /// <para>
    /// 官方业务限制：<c>status</c>：0 初始值，5 上架，6 回收站（仅传入 6 才返回回收站），11 所有下架商品
    /// （含自主下架 11 / 违规下架 15 等）；<c>page_size</c> 默认 10、不超过 30。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/list/get")]
    Task<ChannelsProductIdListResponse> GetProductListAsync(
        [Body] ChannelsProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商品上架（<c>product/listing</c>）。
    /// </summary>
    /// <param name="request">上架请求（<c>product_id</c> 必填；赠品 ID 亦可）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_listingproduct"/></para>
    /// <para>官方业务限制：上架触发商品审核；审核中商品不可重复上架。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/listing")]
    Task<ChannelsResponse> ListingProductAsync(
        [Body] ChannelsProductIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商品下架（<c>product/delisting</c>）。
    /// </summary>
    /// <param name="request">下架请求（<c>product_id</c> 必填；赠品 ID 亦可）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_delistingproduct"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/delisting")]
    Task<ChannelsResponse> DelistingProductAsync(
        [Body] ChannelsProductIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除商品（<c>product/delete</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>product_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_deleteproduct"/></para>
    /// <para>官方业务限制：删除为<b>逻辑删除（移入回收站）</b>，可从回收站恢复（内部机制）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/delete")]
    Task<ChannelsResponse> DeleteProductAsync(
        [Body] ChannelsProductIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消商品提审（<c>product/audit/cancel</c>）。
    /// </summary>
    /// <param name="request">取消提审请求（<c>product_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_cancelaudit"/></para>
    /// <para>官方业务限制：审核中（<c>edit_status==2</c>）的商品可取消提审，取消后回到草稿态。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/audit/cancel")]
    Task<ChannelsResponse> CancelProductAuditAsync(
        [Body] ChannelsProductIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新商品免审（<c>product/auditfree</c>）—— 部分字段更新（sku / 库存 / 运费 / 限购等）免审。
    /// </summary>
    /// <param name="request">更新请求（<c>product_id</c> 与 <c>skus</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updateproductauditfree"/></para>
    /// <para>
    /// 官方业务限制：仅对<b>曾经上架成功过的商品</b>适用（草稿状态非审核中，<c>edit_status != 2</c>）；
    /// 本地生活商品不受该规则约束。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/auditfree")]
    Task<ChannelsResponse> UpdateProductAuditFreeAsync(
        [Body] ChannelsUpdateProductAuditFreeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取上架策略（<c>product/auditstrategy/get</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>上架策略（<c>audit_strategy</c>：隐藏商品信息上架 / 可上架相似品 / 命中低风险规则可上架）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getauditstrategy"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/auditstrategy/get")]
    Task<ChannelsGetProductAuditStrategyResponse> GetProductAuditStrategyAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置上架策略（<c>product/auditstrategy/set</c>）。
    /// </summary>
    /// <param name="request">设置请求（<c>audit_strategy</c> 必填；各 flag 0 不修改 / 1 打开 / 2 关闭）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_setauditstrategy"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/auditstrategy/set")]
    Task<ChannelsResponse> SetProductAuditStrategyAsync(
        [Body] ChannelsSetProductAuditStrategyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取提审配额（<c>product/getauditquota</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核限额（<c>audit_quota</c>：是否限制 / 可用配额 / 总配额 / 不限判断 / 新增商品配额）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getauditquota"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体；<c>block_status==1</c> 时提审商品会变为提审 quota 不足状态。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/getauditquota")]
    Task<ChannelsGetProductAuditQuotaResponse> GetProductAuditQuotaAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品受限信息（<c>product/getproductrestrictedinfo</c>，上架 / 展示受限策略查询）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>受限数据（<c>restricted_data</c>：受限场景 / 来源 / 描述）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductrestrictedinfo"/></para>
    /// <para>官方业务限制：受限场景枚举见 <see cref="ChannelsProductRestrictedScenes"/>，
    /// 受限策略来源枚举见 <see cref="ChannelsProductRestrictedSources"/>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/getproductrestrictedinfo")]
    Task<ChannelsGetProductRestrictedInfoResponse> GetProductRestrictedInfoAsync(
        [Body] ChannelsGetProductRestrictedInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 类目「一物一码」鉴定 / 图片分类（<c>product/category/classify</c>）。
    /// </summary>
    /// <param name="request">鉴定请求（<c>cat_ids</c> 候选叶子类目与 <c>img_urls</c> 主图必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>类目鉴定结果（<c>cats</c>：命中类目列表）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getcategoryclassify"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/category/classify")]
    Task<ChannelsProductClassifyResponse> ClassifyProductCategoryAsync(
        [Body] ChannelsProductClassifyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 类目预检（<c>product/categoryprecheck</c>，判断商品信息是否满足类目要求）。
    /// </summary>
    /// <param name="request">预检请求（<c>cat_id</c> 与商品信息必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预检结果（<c>all_pass</c> 是否可用 + <c>fail_reasons</c> 不通过原因）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_categoryprecheck"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/categoryprecheck")]
    Task<ChannelsProductCategoryPrecheckResponse> CategoryPrecheckAsync(
        [Body] ChannelsProductCategoryPrecheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增第三方货源商品（<c>product/addproductthirdpartysource</c>，分销铺货 / 商品搬家 / 其他场景）。
    /// </summary>
    /// <param name="request">新增请求（<c>scene_value</c>/<c>supplier</c>/<c>product_source_info</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>货源 id（<c>third_party_source_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_addproductthirdpartysource"/></para>
    /// <para>官方业务限制：场景值 <see cref="ChannelsThirdPartySourceScenes"/>（1 分销铺货，2 商品搬家，3 其他场景）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/addproductthirdpartysource")]
    Task<ChannelsAddProductThirdPartySourceResponse> AddProductThirdPartySourceAsync(
        [Body] ChannelsAddProductThirdPartySourceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌推荐（<c>product/productbrandrecommend</c>，根据类目与商品信息推荐品牌）。
    /// </summary>
    /// <param name="request">推荐请求（<c>cat_id</c>/<c>head_imgs</c>/<c>title</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推荐品牌（<c>brand_id</c>/<c>brand_name_chinese</c>/<c>brand_name_english</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_productbrandrecommend"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/productbrandrecommend")]
    Task<ChannelsProductBrandRecommendResponse> RecommendProductBrandAsync(
        [Body] ChannelsProductBrandRecommendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 外部商品属性映射（<c>product/externalproductmapping</c>）。
    /// </summary>
    /// <param name="request">映射请求（<c>cat_id</c>/<c>external_attribute_name</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>外部属性到内部属性的映射（<c>external_attribute_name</c>/<c>external_attribute_value</c>/
    /// <c>internal_attribute_name</c>/<c>internal_attribute_value</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_externalproductmapping"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/externalproductmapping")]
    Task<ChannelsExternalProductMappingResponse> MapExternalProductAttributeAsync(
        [Body] ChannelsExternalProductMappingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 外部商品属性映射（新版，<c>product/externalproductmappingnew</c>）。
    /// </summary>
    /// <param name="request">映射请求（<c>cat_id</c>/<c>head_imgs</c>/<c>title</c>/<c>external_attributes</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>映射属性结果列表（<c>attributes</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_externalproductmappingnew"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/externalproductmappingnew")]
    Task<ChannelsExternalProductMappingNewResponse> MapExternalProductAttributeNewAsync(
        [Body] ChannelsExternalProductMappingNewRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开始定时开售（<c>product/begintimingsale</c>）。
    /// </summary>
    /// <param name="request">开售请求（<c>product_id</c> 与 <c>task_id</c> 必填，task_id 来自商品里的字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_begintimingsale"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/begintimingsale")]
    Task<ChannelsResponse> BeginTimingSaleAsync(
        [Body] ChannelsBeginTimingSaleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消定时开售（<c>product/canceltimingsale</c>）。
    /// </summary>
    /// <param name="request">取消请求（<c>product_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_canceltimingsale"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/canceltimingsale")]
    Task<ChannelsResponse> CancelTimingSaleAsync(
        [Body] ChannelsProductIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品微信口令（<c>product/taglink/get</c>，只支持微信内打开）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填；可带企微关联账号 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品微信口令（<c>product_taglink</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproducttaglink"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/taglink/get")]
    Task<ChannelsGetProductTagLinkResponse> GetProductTagLinkAsync(
        [Body] ChannelsProductTagLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品二维码 / 物料（<c>product/qrcode/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填；<c>qrcode_type</c> 缺省 1）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品二维码链接（<c>product_qrcode</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductqrcode"/></para>
    /// <para>官方业务限制：<c>qrcode_type</c>：1 二维码，2 标准物料，3 送礼物物料。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/qrcode/get")]
    Task<ChannelsGetProductQrcodeResponse> GetProductQrcodeAsync(
        [Body] ChannelsGetProductQrcodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品 H5 短链（<c>product/h5url/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填；可带企微关联账号 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品 H5 短链（<c>product_h5url</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproducth5url"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/h5url/get")]
    Task<ChannelsGetProductH5UrlResponse> GetProductH5UrlAsync(
        [Body] ChannelsGetProductH5UrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品跳转 scheme（<c>product/scheme/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c>/<c>from_appid</c>/<c>expire</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品跳转 scheme 码（<c>openlink</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductscheme"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/scheme/get")]
    Task<ChannelsGetProductSchemeResponse> GetProductSchemeAsync(
        [Body] ChannelsGetProductSchemeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取库存（<c>product/stock/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 与 <c>sku_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>库存数据（<c>data</c>：通用库存 / 库存总量 / 区域库存列表）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getstock"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/stock/get")]
    Task<ChannelsGetStockResponse> GetStockAsync(
        [Body] ChannelsGetStockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新库存（<c>product/stock/update</c>）。
    /// </summary>
    /// <param name="request">更新请求（<c>product_id</c>/<c>sku_id</c>/<c>diff_type</c>/<c>num</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updatestock"/></para>
    /// <para>
    /// 官方业务限制：<c>diff_type</c>：1 增加，2 减少，3 设置；建议使用 1 或 2，
    /// 不建议使用 3（高并发场景可能出现预期外表现）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/stock/update")]
    Task<ChannelsResponse> UpdateStockAsync(
        [Body] ChannelsUpdateStockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取库存（<c>product/stock/batchget</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填，上限 50；<c>stock_type</c> 不填默认 0）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>SPU 库存列表（<c>data.spu_stock_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_batchgetstock"/></para>
    /// <para>
    /// 官方业务限制：<c>stock_type==1</c>（达人专属计划营销库存）时 <c>finder_id</c> 必填；
    /// <c>stock_type</c> 不为 0 且不为 1 时 <c>stock_type_id</c> 必填。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/stock/batchget")]
    Task<ChannelsBatchGetStockResponse> BatchGetStockAsync(
        [Body] ChannelsBatchGetStockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取库存流水（<c>product/stock/getflow</c>，库存变动对账）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c>/<c>sku_id</c>/<c>stock_type</c>/<c>begin_time</c>/<c>end_time</c>/<c>page_size</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>库存流水与翻页上下文（<c>data.stock_flow_info_list</c>/<c>data.next_key</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getstockflow"/></para>
    /// <para>官方业务限制：事件类型枚举见 <see cref="ChannelsStockFlowOpTypes"/>、库存子类型见
    /// <see cref="ChannelsStockSubTypes"/>；翻页回传 <c>next_key</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/stock/getflow")]
    Task<ChannelsGetStockFlowResponse> GetStockFlowAsync(
        [Body] ChannelsGetStockFlowRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增赠品（<c>product/gift/add</c>，非卖商品）。
    /// </summary>
    /// <param name="request">新增请求（<c>title</c>/<c>head_imgs</c>/<c>cats_v2</c>/<c>skus</c> 必填；仅支持单 sku）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>非卖商品 id 与创建时间（<c>product_id</c>/<c>create_time</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_addgift"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/gift/add")]
    Task<ChannelsAddGiftProductResponse> AddGiftProductAsync(
        [Body] ChannelsAddGiftProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取赠品（<c>product/gift/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填；<c>data_type</c> 缺省 1）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>赠品线上 / 草稿数据（<c>product</c>/<c>edit_product</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getgift"/></para>
    /// <para>官方业务限制：<c>data_type</c>：1 线上数据，2 草稿数据，3 同时获取线上和草稿数据；
    /// 赠品类型见 <see cref="ChannelsGiftProductDetail.ProductType"/>（4 在售赠品只读，5 非卖赠品）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/gift/get")]
    Task<ChannelsGetGiftProductResponse> GetGiftProductAsync(
        [Body] ChannelsGetGiftProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取赠品列表（<c>product/gift/list/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（与商品列表字段集一致 ⇒ 复用 <see cref="ChannelsProductListRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>赠品 id 列表与翻页上下文（<c>product_ids</c>/<c>next_key</c>/<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getgiftlist"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/gift/list/get")]
    Task<ChannelsProductIdListResponse> GetGiftProductListAsync(
        [Body] ChannelsProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置赠品上架（<c>product/gift/onsale/set</c>）。
    /// </summary>
    /// <param name="request">设置请求（<c>product_id</c> 原始商品 ID 与 <c>skus</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 赠品 id 列表（<c>product_ids</c>/<c>next_key</c>/<c>total_num</c>；官方文档返回形态与商品列表一致，照录）。
    /// </returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_setgiftonsale"/></para>
    /// <para>官方业务限制：目前仅支持将<b>单品商品</b>设置为赠品；划拨库存直接从原始商品库存扣除、不参与售卖。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/gift/onsale/set")]
    Task<ChannelsProductIdListResponse> SetGiftOnsaleAsync(
        [Body] ChannelsSetGiftOnsaleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新赠品库存（<c>product/gift/stock/update</c>）。
    /// </summary>
    /// <param name="request">更新请求（与商品库存更新字段集一致 ⇒ 复用 <see cref="ChannelsUpdateStockRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updategiftstock"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/gift/stock/update")]
    Task<ChannelsResponse> UpdateGiftStockAsync(
        [Body] ChannelsUpdateStockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新赠品（<c>product/gift/update</c>，非卖商品）。
    /// </summary>
    /// <param name="request">更新请求（<c>product_id</c>/<c>title</c>/<c>head_imgs</c>/<c>skus</c>/<c>cats_v2</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>小店内部非卖商品 ID 与更新时间（<c>product_id</c>/<c>update_time</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updategift"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/gift/update")]
    Task<ChannelsUpdateGiftProductResponse> UpdateGiftProductAsync(
        [Body] ChannelsUpdateGiftProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建买赠活动（<c>product/activity/add</c>）。
    /// </summary>
    /// <param name="request">创建请求（<c>title</c>/<c>start_time</c>/<c>end_time</c>/<c>detail</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>买赠活动 ID（<c>activity_id</c>，创建成功后返回）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_addgiftactivity"/></para>
    /// <para>
    /// 官方业务限制：① <c>start_time</c> 只能取大于等于当前时间、且距离当前时间不得超过 30 天；
    /// ② <c>end_time</c> 必须大于当前时间以及 <c>start_time</c>，活动持续时间需 ≥10 分钟、≤30 天；
    /// ③<b>当前 Open API 仅支持传 0（全场景）</b>，<c>show_scene==1</c> 可能无法生效。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/activity/add")]
    Task<ChannelsAddGiftActivityResponse> AddGiftActivityAsync(
        [Body] ChannelsAddGiftActivityRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除买赠活动（<c>product/activity/del</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>activity_id</c> 必填，官方类型为 number）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_delgiftactivity"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/activity/del")]
    Task<ChannelsResponse> DeleteGiftActivityAsync(
        [Body] ChannelsGiftActivityDelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止买赠活动（<c>product/activity/stop</c>）。
    /// </summary>
    /// <param name="request">停止请求（<c>activity_id</c> 必填，官方类型为 string，与 activity/del 的 number 不同）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_stopgiftactivity"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/activity/stop")]
    Task<ChannelsResponse> StopGiftActivityAsync(
        [Body] ChannelsGiftActivityStopRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建限时抢购任务（<c>product/limiteddiscounttask/add</c>）。
    /// </summary>
    /// <param name="request">创建请求（<c>product_id</c>/<c>start_time</c>/<c>end_time</c>/<c>limited_discount_skus</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>限时抢购任务 ID（<c>task_id</c>，创建成功后返回）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_add"/></para>
    /// <para>
    /// 官方业务限制：① <c>start_time</c> 只能取大于等于当前时间（允许最多十分钟误差）、且距离当前时间不得超过
    /// 一年（365 天）；② 抢购价格必须小于原价（原价为 1 分钱的商品无法创建抢购任务）；
    /// ③ 参与抢购的库存必须小于等于现有库存。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/limiteddiscounttask/add")]
    Task<ChannelsAddLimitedDiscountTaskResponse> AddLimitedDiscountTaskAsync(
        [Body] ChannelsAddLimitedDiscountTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新限时抢购任务（<c>product/limiteddiscounttask/update</c>）。
    /// </summary>
    /// <param name="request">更新请求（全字段必填；<c>status</c> 为乐观锁校验快照）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>限时抢购任务 ID 与活动名称（<c>task_id</c>/<c>title</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_update"/></para>
    /// <para>官方业务限制：<c>status</c>：0 待开始，1 进行中（传入时校验与实际状态是否一致，乐观锁）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/limiteddiscounttask/update")]
    Task<ChannelsUpdateLimitedDiscountTaskResponse> UpdateLimitedDiscountTaskAsync(
        [Body] ChannelsUpdateLimitedDiscountTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除限时抢购任务（<c>product/limiteddiscounttask/delete</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>task_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_delete"/></para>
    /// <para>官方业务限制：任务进行中（status==1）或创建完成未开始（status==0）时不能直接调用删除接口。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/limiteddiscounttask/delete")]
    Task<ChannelsResponse> DeleteLimitedDiscountTaskAsync(
        [Body] ChannelsLimitedDiscountTaskIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止限时抢购任务（<c>product/limiteddiscounttask/stop</c>）。
    /// </summary>
    /// <param name="request">停止请求（<c>task_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_stop"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/limiteddiscounttask/stop")]
    Task<ChannelsResponse> StopLimitedDiscountTaskAsync(
        [Body] ChannelsLimitedDiscountTaskIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取限时抢购任务列表（<c>product/limiteddiscounttask/list/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填；<c>status</c> 不填获取所有状态）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>限时抢购任务列表、翻页上下文与总数（<c>limited_discount_tasks</c>/<c>next_key</c>/<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_listget"/></para>
    /// <para>官方业务限制：状态枚举见 <see cref="ChannelsLimitedDiscountTaskStatuses"/>；<c>page_size</c> 默认 10、不超过 50。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/limiteddiscounttask/list/get")]
    Task<ChannelsGetLimitedDiscountTaskListResponse> GetLimitedDiscountTaskListAsync(
        [Body] ChannelsGetLimitedDiscountTaskListRequest request,
        CancellationToken cancellationToken = default);
}