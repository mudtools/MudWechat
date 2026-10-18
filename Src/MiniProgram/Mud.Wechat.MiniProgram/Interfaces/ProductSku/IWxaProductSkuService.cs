// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductSku;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——SKU 接口（7 端点）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para><b>商品与 SKU 的两种身份</b>：所有端点同时接受「小商店内部 ID」（<c>product_id</c> / <c>sku_id</c>，number）
/// 与「商家自定义 ID」（<c>out_product_id</c> / <c>out_sku_id</c>，string），同组二选一；
/// <c>out_sku_id</c> 添加成功后<b>不可修改</b>，最多 32 字符。</para>
/// <para><b>金额单位一律为分</b>，<c>sale_price</c> / <c>market_price</c> 均不超过 1000000000（1000 万元），
/// 且 <c>market_price</c> 必须严格大于 <c>sale_price</c>（否则 <c>9401055</c>）。</para>
/// <para><b>幂等性</b>：读端点（<c>get</c> / <c>get_list</c>）可重试；写端点中 <c>add</c> / <c>batch_add</c> 重放会命中
/// <c>9401005 SKU 已经存在</c>，<c>del</c> 重放由 <c>9401006 SKU 不存在</c> 兜底，<c>update</c> / <c>update_price</c>
/// 是幂等的「覆盖为给定值」语义（官方未提供版本号或条件更新）。</para>
/// <para><b>读草稿 / 真实库存</b>：<c>need_real_stock</c>（0 线上数据 / 1 草稿数据）与 <c>need_edit_sku</c>
/// （0 线上数据 / 1 草稿数据）只在读端点出现，写端点无对应参数。</para>
/// <para><b>敏感面</b>：商品与价格数据属于商家经营数据，<b>不得</b>写入日志、遥测或异常消息（尤其 <c>barcode</c> / <c>sku_code</c>）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductSku", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductSkuService
{
    /// <summary>
    /// 添加 SKU（<c>POST /product/sku/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/add_sku.html"/>。
    /// </summary>
    /// <param name="request">添加 SKU 请求，见 <see cref="WxaProductSkuAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新增 SKU 的 <c>sku_id</c> 与 <c>create_time</c>，见 <see cref="WxaProductSkuAddResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/sku/add</c>；<c>product_id</c> / <c>out_product_id</c> 二选一，
    /// <c>out_sku_id</c>、<c>sale_price</c>、<c>stock_num</c>、<c>sku_attrs[]</c> 必填。</para>
    /// <para><b>属性约束</b>：<c>sku_attrs[].attr_key</c> / <c>attr_value</c> 各最多 40 字符；同一 <c>attr_key</c> 下
    /// 不能超过 100 个不同 <c>attr_value</c>；每个 SPU 下<b>第一个 SKU</b> 的 <c>sku_attrs</c> 顺序决定该 SPU
    /// 在 PC 端商品详情页的属性排序。</para>
    /// <para><b>域内错误码</b>：<c>-1</c> 系统异常、<c>-2</c> token 太长、<c>9401020</c> 参数有误、<c>9401005</c> SKU 已经存在、
    /// <c>9401043</c> SKU 价格过高、<c>9401044</c> <c>sku_code</c> 过长、<c>9401045</c> SKU 销售属性过长、
    /// <c>9401046</c> 同 key 属性个数过多、<c>9401047</c> <c>attr_key</c> 过长、<c>9401048</c> <c>attr_value</c> 过长、
    /// <c>9401050</c> 图片 URL 非法（域名须为 <c>store.mp.video.tencent-cloud.com</c> 且前缀为
    /// <c>mmbizurl.cn/p/</c> 或 <c>mmecimage.cn/p/</c>）、<c>9401052</c> <c>out_sku_id</c> 过长、
    /// <c>9401054</c> 价格为 0、<c>9401055</c> <c>sale_price</c> ≥ <c>market_price</c>。</para>
    /// <para><b>非幂等</b>：重复提交同一 <c>out_sku_id</c> 返回 <c>9401005</c>，调用方须按「先查 <c>sku/get</c> 再决定是否新增」编排。</para>
    /// </remarks>
    [Post("/product/sku/add")]
    Task<WxaProductSkuAddResponse> AddSkuAsync(
        [Body] WxaProductSkuAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加 SKU（<c>POST /product/sku/batch_add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/batch_add_sku.html"/>。
    /// </summary>
    /// <param name="request">批量添加请求（<c>skus[]</c>，元素形状同 <see cref="WxaProductSkuAddRequest"/>），见 <see cref="WxaProductSkuBatchAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐项结果 <c>data[]</c>，见 <see cref="WxaProductSkuBatchAddResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/sku/batch_add</c>；请求体 <c>skus[]</c> 每项与单条添加同形。</para>
    /// <para><b>官方文档形状不一致（照原文留档）</b>：响应参数表写作 <c>data.sku_key.sku_id</c>，而响应示例为
    /// <c>data[]</c> 扁平数组且每项含 <c>sku_id</c> / <c>out_sku_id</c> / <c>create_time</c>；本 SDK 按<b>示例</b>建模
    /// （<see cref="WxaProductSkuBatchAddItem"/>），并在字段注释保留差异。</para>
    /// <para><b>错误码</b>与单条添加一致（<c>9401005</c> / <c>9401043</c> / <c>9401050</c> / <c>9401054</c> / <c>9401055</c> 等）。</para>
    /// </remarks>
    [Post("/product/sku/batch_add")]
    Task<WxaProductSkuBatchAddResponse> BatchAddSkuAsync(
        [Body] WxaProductSkuBatchAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除 SKU（<c>POST /product/sku/del</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/del_sku.html"/>。
    /// </summary>
    /// <param name="request">删除请求（商品 ID 二选一 + SKU ID 二选一），见 <see cref="WxaProductSkuDelRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>错误码</b>：<c>-1</c> 系统异常、<c>-2</c> token 太长、<c>9401006</c> SKU 不存在、<c>9401020</c> 参数有误。</para>
    /// <para><b>幂等性</b>：删除为「置为假删除」语义；重复删除返回 <c>9401006</c>，调用方可视为已达成目标。</para>
    /// </remarks>
    [Post("/product/sku/del")]
    Task<WxaResponse> DeleteSkuAsync(
        [Body] WxaProductSkuDelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 SKU（<c>POST /product/sku/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/get_sku.html"/>。
    /// </summary>
    /// <param name="request">查询请求（<c>sku_id</c> / <c>out_sku_id</c> 二选一，可附 <c>need_real_stock</c> / <c>need_edit_sku</c>），见 <see cref="WxaProductSkuGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>SKU 明细 <c>data</c>，见 <see cref="WxaProductSkuGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>status 枚举（本端点）</b>：5 上架中、21 假删除（见 <see cref="WxaProductSkuStatuses"/>；
    /// <c>get_list</c> 额外出现 0 未上架）。</para>
    /// <para><b>错误码</b>：<c>-1</c> 系统异常、<c>-2</c> token 太长、<c>9401006</c> SKU 不存在、<c>9401020</c> 参数有误。</para>
    /// </remarks>
    [Post("/product/sku/get")]
    Task<WxaProductSkuGetResponse> GetSkuAsync(
        [Body] WxaProductSkuGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 SKU 列表（<c>POST /product/sku/get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/get_sku_list.html"/>。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填），见 <see cref="WxaProductSkuGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>SKU 列表 <c>skus[]</c>，见 <see cref="WxaProductSkuGetListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>仅接受内部商品 ID</b>：官方参数表只列 <c>product_id</c>（必填），<b>不</b>接受 <c>out_product_id</c>。</para>
    /// <para><b>status 枚举（本端点）</b>：0 未上架、5 上架中、21 假删除（见 <see cref="WxaProductSkuStatuses"/>）。</para>
    /// <para><b>错误码</b>：<c>-1</c> 系统异常、<c>-2</c> token 太长、<c>9401020</c> 参数有误、<c>9401002</c> SPU 不存在。</para>
    /// </remarks>
    [Post("/product/sku/get_list")]
    Task<WxaProductSkuGetListResponse> GetSkuListAsync(
        [Body] WxaProductSkuGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新 SKU（<c>POST /product/sku/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/up_sku.html"/>。
    /// </summary>
    /// <param name="request">更新请求，见 <see cref="WxaProductSkuUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新后的 <c>sku_id</c> 与 <c>update_time</c>，见 <see cref="WxaProductSkuUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/sku/update</c>；<c>sale_price</c> 与 <c>stock_num</c> 必填，
    /// <c>sku_attrs</c> 必填（整体覆盖，不是增量合并）。</para>
    /// <para><b>错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401020</c> 参数有误、<c>9401006</c> SKU 不存在、
    /// <c>9401025</c> SKU 不允许编辑、<c>9401043</c> 价格过高、<c>9401044</c> <c>sku_code</c> 过长、
    /// <c>9401046</c> 同 key 属性过多、<c>9401047</c> / <c>9401048</c> 属性键值过长、
    /// <c>9401050</c> 图片 URL 非法、<c>9401054</c> 价格为 0、<c>9401055</c> <c>sale_price</c> ≥ <c>market_price</c>。</para>
    /// </remarks>
    [Post("/product/sku/update")]
    Task<WxaProductSkuUpdateResponse> UpdateSkuAsync(
        [Body] WxaProductSkuUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新 SKU 价格（<c>POST /product/sku/update_price</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/up_sku_price.html"/>。
    /// </summary>
    /// <param name="request">价格更新请求（商品 ID 二选一 + SKU ID 二选一 + <c>sale_price</c>），见 <see cref="WxaProductSkuUpdatePriceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新后的 <c>sku_id</c> 与 <c>update_time</c>，见 <see cref="WxaProductSkuUpdatePriceResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/sku/update_price</c>；只改价格，不动库存与属性。</para>
    /// <para><b>错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401020</c> 参数有误、<c>9401006</c> SKU 不存在、
    /// <c>9401025</c> SKU 不允许编辑、<c>9401054</c> 价格为 0、<c>9401055</c> <c>sale_price</c> ≥ <c>market_price</c>。</para>
    /// </remarks>
    [Post("/product/sku/update_price")]
    Task<WxaProductSkuUpdatePriceResponse> UpdateSkuPriceAsync(
        [Body] WxaProductSkuUpdatePriceRequest request,
        CancellationToken cancellationToken = default);
}
