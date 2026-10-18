// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopSpu;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 自定义交易组件 V2——商品（SPU）接口（9 端点：增删改查与上下架 6 + 撤回审核 1 + 免审更新 1 + 列表 1）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para><b>与 V1 <c>/product/spu/*</c> 是两代契约</b>：V2 请求体以 <c>third_cat_id</c> + <c>brand_id</c> + <c>skus[]</c> 为核心，
/// 且 <c>scene_group_list</c> 为<b>必填</b>（组件未接入场景时传空数组）；V1 的 <c>cats[]</c> / <c>express_info</c> 不在此域出现。</para>
/// <para><b>审核流</b>：添加 / 更新提交后进入审核，<c>edit_status</c> 见 <see cref="WxaShopSpuEditStatuses"/>；
/// <c>update_without_audit</c> 用于不触发重新审核的更新，<c>del_audit</c> 用于撤回审核。</para>
/// <para><b>敏感面</b>：商品标题、价格、库存与类目属于商家经营数据，<b>不得</b>写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopSpu", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopSpuService
{
    /// <summary>
    /// 添加商品（<c>POST /shop/spu/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/add_spu.html"/>。
    /// </summary>
    /// <param name="request">添加请求，见 <see cref="WxaShopSpuAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>平台商品 ID、自定义商品 ID 与 SKU 回显，见 <see cref="WxaShopSpuAddResponse"/>。</returns>
    /// <remarks>
    /// <para><c>out_product_id</c>、<c>title</c>、<c>path</c>、<c>head_img</c>、<c>third_cat_id</c>、<c>brand_id</c>、
    /// <c>skus[]</c>、<c>scene_group_list</c> 均为必填。</para>
    /// <para><b>非幂等</b>：重复添加同一 <c>out_product_id</c> 会命中 <c>9401001 SPU 已经存在</c>。</para>
    /// <para><b>通用错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c>、<c>9401002</c>、<c>9401020</c> 参数有误、
    /// <c>9401021</c> 无权限调用该 api、<c>9401023</c> SPU 不允许编辑。</para>
    /// </remarks>
    [Post("/shop/spu/add")]
    Task<WxaShopSpuAddResponse> AddSpuAsync(
        [Body] WxaShopSpuAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除商品（<c>POST /shop/spu/del</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/del_spu.html"/>。
    /// </summary>
    /// <param name="request">商品 ID（二选一），见 <see cref="WxaShopSpuIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>通用错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c>、<c>9401002</c>、<c>9401020</c>、<c>9401021</c>、<c>9401023</c>。</para>
    /// </remarks>
    [Post("/shop/spu/del")]
    Task<WxaResponse> DeleteSpuAsync(
        [Body] WxaShopSpuIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤回商品审核（<c>POST /shop/spu/del_audit</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/del_spu_audit.html"/>。
    /// </summary>
    /// <param name="request">商品 ID（二选一），见 <see cref="WxaShopSpuIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>仅对处于审核中的商品有意义；撤回后 <c>edit_status</c> 回到可编辑状态，需重新提交审核。</para>
    /// <para><b>通用错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c>、<c>9401002</c>、<c>9401020</c>、<c>9401021</c>、<c>9401023</c>。</para>
    /// </remarks>
    [Post("/shop/spu/del_audit")]
    Task<WxaResponse> DeleteSpuAuditAsync(
        [Body] WxaShopSpuIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下架商品（<c>POST /shop/spu/delisting</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/delisting_spu.html"/>。
    /// </summary>
    /// <param name="request">商品 ID（二选一），见 <see cref="WxaShopSpuIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>路由反直觉点</b>：文档页文件名为 <c>delisting_spu.html</c>，请求地址为 <c>/shop/spu/delisting</c>（以请求地址为准）。</para>
    /// <para><b>通用错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c>、<c>9401002</c>、<c>9401020</c>、<c>9401021</c>、<c>9401023</c>。</para>
    /// </remarks>
    [Post("/shop/spu/delisting")]
    Task<WxaResponse> DelistSpuAsync(
        [Body] WxaShopSpuIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品（<c>POST /shop/spu/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/get_spu.html"/>。
    /// </summary>
    /// <param name="request">查询请求（商品 ID 二选一 + 可选 <c>need_edit_spu</c>），见 <see cref="WxaShopSpuGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品详情（顶层 <c>spu</c>），见 <see cref="WxaShopSpuGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>应答形状</b>：官方参数表与示例均为顶层 <c>spu</c>（<b>不是</b> <c>data.spu</c>），与 V1 的 <c>data.spu</c> 不同。</para>
    /// <para><b>状态枚举</b>：<c>status</c> 见 <see cref="WxaShopSpuStatuses"/>；<c>edit_status</c> 见 <see cref="WxaShopSpuEditStatuses"/>
    /// （本页枚举自 1 起：1 未审核 / 2 审核中 / 3 审核失败 / 4 审核成功）。</para>
    /// <para><b>通用错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c>、<c>9401002</c>、<c>9401020</c>、<c>9401021</c>、<c>9401023</c>。</para>
    /// </remarks>
    [Post("/shop/spu/get")]
    Task<WxaShopSpuGetResponse> GetSpuAsync(
        [Body] WxaShopSpuGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品列表（<c>POST /shop/spu/get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/get_spu_list.html"/>。
    /// </summary>
    /// <param name="request">查询请求（<c>page</c> / <c>page_size</c> 必填），见 <see cref="WxaShopSpuGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总数与商品列表（顶层 <c>total_num</c> / <c>spus[]</c>），见 <see cref="WxaShopSpuGetListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方表与示例冲突（留档）</b>：参数表把 <c>status</c> 标为必填，示例注释却写「选填，不填时获取所有状态商品」
    /// ⇒ 本仓建模为可空，按示例语义调用。</para>
    /// <para><b>分页上限</b>：<c>page_size</c> 不超过 100。</para>
    /// <para><b>状态枚举</b>：本页 <c>edit_status</c> 含 0（初始值）；<c>get</c> 页从 1 起 —— 同一字段两页枚举不同，照官方留档。</para>
    /// </remarks>
    [Post("/shop/spu/get_list")]
    Task<WxaShopSpuGetListResponse> GetSpuListAsync(
        [Body] WxaShopSpuGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上架商品（<c>POST /shop/spu/listing</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/listing_spu.html"/>。
    /// </summary>
    /// <param name="request">商品 ID（二选一），见 <see cref="WxaShopSpuIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>路由反直觉点</b>：文档页文件名为 <c>listing_spu.html</c>，请求地址为 <c>/shop/spu/listing</c>（以请求地址为准）。</para>
    /// <para><b>通用错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c>、<c>9401002</c>、<c>9401020</c>、<c>9401021</c>、<c>9401023</c>。</para>
    /// </remarks>
    [Post("/shop/spu/listing")]
    Task<WxaResponse> ListSpuAsync(
        [Body] WxaShopSpuIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新商品（<c>POST /shop/spu/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/update_spu.html"/>。
    /// </summary>
    /// <param name="request">更新请求（整体覆盖，<c>skus[]</c> 必填），见 <see cref="WxaShopSpuUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>平台商品 ID、自定义商品 ID、更新时间与 SKU 回显，见 <see cref="WxaShopSpuUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>覆盖式写入</b>：官方请求体是整体覆盖（非 JSON Patch），漏传字段即失去原值 ⇒ 更新前必须先 <c>get</c> 读回再改再写。</para>
    /// <para><b>ID 二选一的官方表述矛盾</b>：参数表把 <c>product_id</c> 与 <c>out_product_id</c> 都标为「是」且写「二选一」
    /// ⇒ 本 SDK 按二选一建模（两者均可空），由调用方保证恰传其一。</para>
    /// <para><b>通用错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c>、<c>9401002</c>、<c>9401020</c>、<c>9401021</c>、<c>9401023</c>。</para>
    /// </remarks>
    [Post("/shop/spu/update")]
    Task<WxaShopSpuUpdateResponse> UpdateSpuAsync(
        [Body] WxaShopSpuUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 免审更新商品（<c>POST /shop/spu/update_without_audit</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/SPU/update_spu.html"/>（与本域 <c>update</c> 共用同一官方页）。
    /// </summary>
    /// <param name="request">更新请求（与 <see cref="UpdateSpuAsync"/> 同形），见 <see cref="WxaShopSpuUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果，见 <see cref="WxaShopSpuUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方页未单列本路由</b>：工作单给出 <c>/shop/spu/update_without_audit</c> 与 <c>update_spu.html</c> 的映射；
    /// 该页参数表只描述 <c>/shop/spu/update</c> ⇒ 本端点的字段集与 <c>update</c> 相同，差异仅在「不触发重新审核」的服务端行为。</para>
    /// <para><b>通用错误码</b>：<c>-1</c>、<c>-2</c>、<c>9401001</c>、<c>9401002</c>、<c>9401020</c>、<c>9401021</c>、<c>9401023</c>。</para>
    /// </remarks>
    [Post("/shop/spu/update_without_audit")]
    Task<WxaShopSpuUpdateResponse> UpdateSpuWithoutAuditAsync(
        [Body] WxaShopSpuUpdateRequest request,
        CancellationToken cancellationToken = default);
}
