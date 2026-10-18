// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductSku;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——SPU（商品）接口（8 端点）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para><b>写侧形状二分</b>：添加 <c>spu/add</c> 允许内嵌 <c>skus[]</c>（也可留空、再走 <c>sku/batch_add</c>）；
/// 更新 <c>spu/update</c> 的官方参数表<b>不含</b> <c>skus[]</c> ⇒ 两请求 DTO 分开建模
/// （<see cref="WxaProductSpuAddRequest"/> / <see cref="WxaProductSpuUpdateRequest"/>），SKU 变更走 SKU 域。</para>
/// <para><b>状态机</b>：<c>status</c>（线上状态，见 <see cref="WxaProductSpuStatuses"/>）与
/// <c>edit_status</c>（草稿审核状态，见 <see cref="WxaProductSpuEditStatuses"/>）是两个正交维度；
/// 添加 / 更新后商品进入审核流，<c>edit_status</c> 2 为审核中、3 为审核失败、4 为审核成功。</para>
/// <para><b>读侧草稿开关</b>：<c>need_edit_spu</c>（默认 0 线上数据 / 1 草稿数据）出现在
/// <c>get</c> / <c>get_list</c>，写端点无对应参数。</para>
/// <para><b>敏感面</b>：商品标题、价格与类目属于商家经营数据，<b>不得</b>写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductSku", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductSpuService
{
    /// <summary>
    /// 添加商品（<c>POST /product/spu/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/add_spu.html"/>。
    /// </summary>
    /// <param name="request">添加商品请求，见 <see cref="WxaProductSpuAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新增商品的 <c>product_id</c> / <c>out_product_id</c> / <c>create_time</c>，见 <see cref="WxaProductSpuAddResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/spu/add</c>；<c>out_product_id</c>、<c>title</c>、<c>head_img</c>、
    /// <c>brand_id</c>、<c>cats</c>、<c>express_info</c> 必填；<c>skus[]</c> 非必填，可另行通过 <c>sku/batch_add</c> 添加。</para>
    /// <para><b>类目规则</b>：<c>brand_id = 2100000000</c>（无品牌）时 <c>cat_id</c> 须来自 <c>category/get</c>；
    /// 品牌商品的三级 <c>cat_id</c> 须与 <c>brand/get</c> 中的 1/2/3 级类目一一对应。</para>
    /// <para><b>官方错误码（本端点全量）</b>：<c>-1</c> 系统异常、<c>-2</c> token 太长、<c>9401001</c> SPU 已经存在、
    /// <c>9401002</c> SPU 不存在、<c>9401020</c> 参数有误、<c>9401021</c> 无权限调用该 api、<c>9401023</c> SPU 不允许编辑、
    /// <c>9401026</c> 类目长度不对（预期三级）、<c>9401032</c> 类目非法、<c>9401033</c> 商家不具备当前类目资质、
    /// <c>9401034</c> 运费模板 ID 非法、<c>9401076</c> 按重量计价但商品重量为 0、<c>9401035</c> 标题为空、
    /// <c>9401036</c> 标题过长、<c>9401037</c> 主图为空、<c>9401038</c> 主图过多、<c>9401039</c> 副标题过长、
    /// <c>9401040</c> 详情图片过多、<c>9401041</c> 详情描述过长、<c>9401042</c> 资质图片过多、
    /// <c>9401043</c> SKU 价格过高、<c>9401044</c> <c>sku_code</c> 过长、<c>9401045</c> SKU 销售属性过长、
    /// <c>9401046</c> 同 key 属性过多、<c>9401047</c> / <c>9401048</c> 属性键值过长、<c>9401049</c> 售后说明过长、
    /// <c>9401050</c> 图片 URL 非法、<c>9401051</c> <c>out_product_id</c> 过长、<c>9401052</c> <c>out_sku_id</c> 过长、
    /// <c>9401054</c> SKU 价格为 0、<c>9401055</c> <c>sale_price</c> ≥ <c>market_price</c>、
    /// <c>9401057</c> 标题过短、<c>9401060</c> 标题字符不合规、<c>9401063</c> 品牌 ID 非法、
    /// <c>110111</c> 核销信息不完整或错误、<c>110112</c> 核销商品有效期限超限。</para>
    /// <para><b>非幂等</b>：重复提交同一 <c>out_product_id</c> 返回 <c>9401001</c>。</para>
    /// </remarks>
    [Post("/product/spu/add")]
    Task<WxaProductSpuAddResponse> AddSpuAsync(
        [Body] WxaProductSpuAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除商品（<c>POST /product/spu/del</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/del_spu.html"/>。
    /// </summary>
    /// <param name="request">商品 ID（<c>product_id</c> / <c>out_product_id</c> 二选一），见 <see cref="WxaProductSpuIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401002</c> SPU 不存在、<c>9401020</c> 参数有误、
    /// <c>9401021</c> 无权限调用该 api、<c>9401023</c> SPU 不允许编辑。</para>
    /// </remarks>
    [Post("/product/spu/del")]
    Task<WxaResponse> DeleteSpuAsync(
        [Body] WxaProductSpuIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下架商品（<c>POST /product/spu/delisting</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/up_spu_delisting.html"/>。
    /// </summary>
    /// <param name="request">商品 ID（<c>product_id</c> / <c>out_product_id</c> 二选一），见 <see cref="WxaProductSpuIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>路由反直觉点</b>：文档页文件名为 <c>up_spu_delisting.html</c>，但官方请求地址是
    /// <c>/product/spu/delisting</c>（<b>不带</b> <c>up_</c> 前缀）⇒ 以请求地址为准。</para>
    /// <para><b>错误码</b>同 <c>spu/del</c>（<c>9401002</c> / <c>9401020</c> / <c>9401021</c> / <c>9401023</c> 等）。</para>
    /// </remarks>
    [Post("/product/spu/delisting")]
    Task<WxaResponse> DelistSpuAsync(
        [Body] WxaProductSpuIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品（<c>POST /product/spu/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/get_spu.html"/>。
    /// </summary>
    /// <param name="request">查询请求（商品 ID 二选一 + 可选 <c>need_edit_spu</c>），见 <see cref="WxaProductSpuGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品详情 <c>data.spu</c>，见 <see cref="WxaProductSpuGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>嵌套形状</b>：官方响应为 <c>data.spu</c>（不是 <c>data</c> 直挂字段），与 SKU 域 <c>data</c> 直挂不同。</para>
    /// <para><b>错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401002</c> SPU 不存在、<c>9401020</c> 参数有误、
    /// <c>9401021</c> 无权限调用该 api、<c>9401023</c> SPU 不允许编辑。</para>
    /// </remarks>
    [Post("/product/spu/get")]
    Task<WxaProductSpuGetResponse> GetSpuAsync(
        [Body] WxaProductSpuGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品列表（<c>POST /product/spu/get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/get_spu_list.html"/>。
    /// </summary>
    /// <param name="request">查询请求（<c>status</c> / <c>page</c> / <c>page_size</c> 必填），见 <see cref="WxaProductSpuGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总数 <c>total_num</c> 与商品列表 <c>spus[]</c>，见 <see cref="WxaProductSpuGetListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>分页上限</b>：<c>page_size</c> 不超过 300（官方原文）。</para>
    /// <para><b>source 取值</b>：1 小商店自营（默认）、2 AMS、4 联盟、8 推荐官推荐、16 闪电购。</para>
    /// <para><b>错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c> SPU 已经存在、<c>9401002</c> SPU 不存在、
    /// <c>9401020</c> 参数有误、<c>9401021</c> 无权限调用该 api、<c>9401023</c> SPU 不允许编辑。</para>
    /// </remarks>
    [Post("/product/spu/get_list")]
    Task<WxaProductSpuGetListResponse> GetSpuListAsync(
        [Body] WxaProductSpuGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上架商品（<c>POST /product/spu/listing</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/up_spu_listing.html"/>。
    /// </summary>
    /// <param name="request">商品 ID（<c>product_id</c> / <c>out_product_id</c> 二选一），见 <see cref="WxaProductSpuIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>路由反直觉点</b>：文档页文件名为 <c>up_spu_listing.html</c>，官方请求地址为 <c>/product/spu/listing</c>
    /// （<b>不带</b> <c>up_</c> 前缀）⇒ 以请求地址为准。</para>
    /// <para><b>错误码</b>同 <c>spu/del</c>（<c>9401002</c> / <c>9401020</c> / <c>9401021</c> / <c>9401023</c> 等）。</para>
    /// </remarks>
    [Post("/product/spu/listing")]
    Task<WxaResponse> ListSpuAsync(
        [Body] WxaProductSpuIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索商品（<c>POST /product/spu/search</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/search_spu.html"/>。
    /// </summary>
    /// <param name="request">搜索请求（<c>source</c> / <c>page</c> / <c>page_size</c> / <c>keyword</c> 必填），见 <see cref="WxaProductSpuSearchRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品列表 <c>spus[]</c>（官方响应表未列 <c>total_num</c>），见 <see cref="WxaProductSpuSearchResponse"/>。</returns>
    /// <remarks>
    /// <para><b>分页上限</b>：<c>page_size</c> 不超过 <b>10000</b>（与 <c>get_list</c> 的 300 不同，照官方原文）。</para>
    /// <para><b>source 取值（本端点仅两个）</b>：1 小商店自营商品；2 带货商品。与 <c>get_list</c> 的五值集合不同。</para>
    /// <para><b>错误码</b>同 <c>get_list</c>。</para>
    /// </remarks>
    [Post("/product/spu/search")]
    Task<WxaProductSpuSearchResponse> SearchSpuAsync(
        [Body] WxaProductSpuSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新商品（<c>POST /product/spu/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/up_spu.html"/>。
    /// </summary>
    /// <param name="request">更新请求，见 <see cref="WxaProductSpuUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新后的 <c>product_id</c> / <c>out_product_id</c> / <c>update_time</c>，见 <see cref="WxaProductSpuUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>形状差异</b>：官方更新参数表<b>不含</b> <c>skus[]</c>（与添加不同）；<c>product_type</c> 须与添加时一致，
    /// 否则返回参数错误。</para>
    /// <para><b>错误码</b>：<c>9401001</c> / <c>9401002</c> / <c>9401020</c> / <c>9401021</c> / <c>9401023</c> /
    /// <c>9401026</c> / <c>9401032</c> / <c>9401033</c> / <c>9401034</c> / <c>9401076</c> / <c>9401035</c>~<c>9401063</c>
    /// 与添加同表，另含 <c>110111</c> / <c>110112</c>（核销信息）。</para>
    /// </remarks>
    [Post("/product/spu/update")]
    Task<WxaProductSpuUpdateResponse> UpdateSpuAsync(
        [Body] WxaProductSpuUpdateRequest request,
        CancellationToken cancellationToken = default);
}
