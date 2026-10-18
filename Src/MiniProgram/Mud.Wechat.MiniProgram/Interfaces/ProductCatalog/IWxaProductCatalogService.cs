// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——类目与品牌面接口（2 端点：获取类目树 / 获取品牌列表）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>，
/// 不新增令牌类型；第三方代调用形态由宿主以小程序应用配置供给令牌。
/// </para>
/// <para><b>路由前缀</b>：本族全部为 <c>/product/*</c>（V1 基础交易工具面），与小店线 <c>/channels/ec/*</c> 无任何交叠（守卫 PC7 锁定）。</para>
/// <para><b>形态口径</b>：2 支官方一律 <b>POST</b>；<c>category/get</c> 有单字段请求体、<c>brand/get</c> 请求参数表为空（零业务体）。</para>
/// <para><b>频率</b>：官方两页均未给出端点级频率数字（仅通用错误码 <c>-1</c>/<c>-2</c>）⇒ 不得凭记忆补数。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCatalog", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductCatalogService
{
    /// <summary>
    /// 获取类目（<c>POST /product/category/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/cat/get_cat_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（唯一字段 <c>f_cat_id</c>，必填），见 <see cref="WxaProductCategoryGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>类目列表，见 <see cref="WxaProductCategoryGetResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/category/get</c>；请求体只有 <c>f_cat_id</c>（number，必填）——<b>填 <c>0</c> 获取根类目</b>，
    /// 再逐层以父类目 id 下钻。应答 <c>cat_list[]</c> 每项含 <c>cat_id</c> / <c>f_cat_id</c> / <c>name</c>。</para>
    /// <para><b>类目树层级口径</b>：本端点返回的是<b>单层子节点</b>（父 id → 直接子类目），不是一次性返回整棵三层树 ⇒
    /// 宿主须按 <c>f_cat_id</c> 逐级下钻自行组装层级；<c>brand/get</c> 的 <c>first/second/third_cat_id</c> 即该三层口径。</para>
    /// <para><b>错误码</b>：<c>9401020</c> 参数有误、<c>9401021</c> 无权限调用该 api（另有通用 <c>-1</c>/<c>-2</c>）。</para>
    /// </remarks>
    [Post("/product/category/get")]
    Task<WxaProductCategoryGetResponse> GetCategoryListAsync(
        [Body] WxaProductCategoryGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取品牌（<c>POST /product/brand/get</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/cat/get_brand.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>品牌列表，见 <see cref="WxaProductBrandGetResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/brand/get</c>；请求参数表<b>零字段</b>（仅 <c>access_token</c> 由 <c>[Token]</c> 注入）⇒ 不设请求 DTO、方法无 <c>[Body]</c> 参数。</para>
    /// <para>应答 <c>brands[]</c> 每项按类目三层给出 <c>first_cat_id</c> / <c>second_cat_id</c> / <c>third_cat_id</c>，
    /// 品牌本体在 <c>brand_info</c>（<c>brand_id</c> / <c>brand_name</c>）——品牌是<b>挂在具体三级类目下</b>的，宿主按类目过滤取用。</para>
    /// </remarks>
    [Post("/product/brand/get")]
    Task<WxaProductBrandGetResponse> GetBrandListAsync(
        CancellationToken cancellationToken = default);
}
