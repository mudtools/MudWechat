// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——店铺信息面接口（2 端点：获取店铺信息 / 获取店铺商品分类）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para><b>两支均为只读查询</b>：请求参数表为空（零业务体），幂等，可安全重试；官方均未给端点级频率数字。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCatalog", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductStoreService
{
    /// <summary>
    /// 获取店铺信息（<c>POST /product/store/get_info</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/store/get_store_info.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>店铺信息，见 <see cref="WxaProductStoreInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/store/get_info</c>；请求参数表<b>零字段</b> ⇒ 不设请求 DTO。</para>
    /// <para>应答包在 <c>data</c> 下（<c>store_name</c> / <c>logo</c> / <c>type</c>），<c>type</c> 取值 <c>1</c>=企业店、<c>2</c>=个人店。</para>
    /// </remarks>
    [Post("/product/store/get_info")]
    Task<WxaProductStoreInfoResponse> GetStoreInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺的商品分类（<c>POST /product/store/get_shopcat</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/store/get_shopcat.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>店铺商品分类列表，见 <see cref="WxaProductShopCatResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/store/get_shopcat</c>；请求参数表<b>零字段</b> ⇒ 不设请求 DTO。</para>
    /// <para>
    /// <b>与 <c>category/get</c> 的区别</b>：本端点返回的是<b>商家自建</b>的店铺商品分类（<c>shopcat_list[]</c>，含 <c>shopcat_id</c> /
    /// <c>shopcat_name</c> / <c>f_shopcat_id</c> / <c>cat_level</c>），是店铺维度的分类树；
    /// <see cref="IWxaProductCatalogService.GetCategoryListAsync"/> 返回的是<b>官方平台类目</b>（商品发布时选择的类目）。两者不得混用。
    /// </para>
    /// <para><b>路由取证纠正</b>：官方文档页文件名是 <c>get_shopcat.html</c>（<c>get_shop_cat.html</c> 实测 404），请求地址即
    /// <c>/product/store/get_shopcat</c> ⇒ 本仓照官方请求地址落地。</para>
    /// </remarks>
    [Post("/product/store/get_shopcat")]
    Task<WxaProductShopCatResponse> GetShopCategoryListAsync(
        CancellationToken cancellationToken = default);
}
