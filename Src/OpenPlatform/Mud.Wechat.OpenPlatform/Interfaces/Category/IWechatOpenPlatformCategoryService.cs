// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.Category;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「小程序类目管理」域 SDK（12 端点：获取全部类目 / 当前设置类目 / 按类型获取类目 /
/// 添加类目 / 删除类目 / 修改类目 / 下载扫码购文案 / 获取基础库最低版本配置 / 设置基础库最低版本配置 /
/// 获取公众号关联的小程序 / 关联小程序 / 解除关联小程序）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌语义（官方契约）</b>：本组端点的令牌<b>值</b>是平台自身 <c>component_access_token</c>，
/// 但官方 query 参数名为 <c>access_token</c> ⇒ 声明为 Query 注入 <c>access_token</c>。
/// </para>
/// <para>
/// <b>官方文档分区</b>：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management"/>
/// （类目管理 6 + 扫码购 1 + 基础库版本 2 + 公众号关联小程序 3 = 12 端点）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Category", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatOpenPlatformCategoryService
{
    /// <summary>
    /// 获取所有可设置的类目。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>全量类目列表。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/getAllCategories.html"/></para>
    /// <para>官方约束：GET 方法，无请求体；返回一级/二级类目树。</para>
    /// </remarks>
    [Get("/cgi-bin/wxopen/getallcategories")]
    Task<WxaGetAllCategoriesResponse> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已设置的类目。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>当前设置的类目列表。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/getSettingCategories.html"/></para>
    /// <para>官方约束：GET 方法，无请求体。</para>
    /// </remarks>
    [Get("/cgi-bin/wxopen/getcategory")]
    Task<WxaGetCategoryResponse> GetCategoryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据类型获取类目。
    /// </summary>
    /// <param name="request">请求体（<c>type</c> = 类目类型，1 为主营类目，2 为其他类目）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>对应类型的类目列表。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/getAllCategoriesByType.html"/></para>
    /// <para>官方约束：POST 方法，请求体含 type 字段。</para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/getcategoriesbytype")]
    Task<WxaGetCategoriesByTypeResponse> GetCategoriesByTypeAsync([Body] WxaGetCategoriesByTypeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加类目。
    /// </summary>
    /// <param name="request">添加类目请求（含类目 ID、资质信息等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/addCategory.html"/></para>
    /// <para>官方约束：POST 方法；需上传资质材料后获取 media_id 填入请求体。</para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/addcategory")]
    Task<OpenPlatformResponse> AddCategoryAsync([Body] WxaAddCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除类目。
    /// </summary>
    /// <param name="request">删除类目请求（含类目 ID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/deleteCategory.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/deletecategory")]
    Task<OpenPlatformResponse> DeleteCategoryAsync([Body] WxaDeleteCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改类目。
    /// </summary>
    /// <param name="request">修改类目请求（含类目 ID 与新信息）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/modifyCategory.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/modifycategory")]
    Task<OpenPlatformResponse> ModifyCategoryAsync([Body] WxaModifyCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载扫码购文案模板。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文案模板内容。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/jumpqrcode-config/downloadQRCodeText.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/qrcodejumpdownload")]
    Task<WxaQrCodeJumpDownloadResponse> QrCodeJumpDownloadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询小程序最低基础库版本配置。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>基础库版本配置。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getSupportVersion.html"/></para>
    /// <para>官方约束：GET 方法，无请求体。</para>
    /// </remarks>
    [Get("/cgi-bin/wxopen/getweappsupportversion")]
    Task<WxaGetWeappSupportVersionResponse> GetWeappSupportVersionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置小程序最低基础库版本。
    /// </summary>
    /// <param name="request">设置请求（含版本号）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/setSupportVersion.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/setweappsupportversion")]
    Task<OpenPlatformResponse> SetWeappSupportVersionAsync([Body] WxaSetWeappSupportVersionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取公众号关联的小程序。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>关联小程序列表。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/officalaccount-management/link-miniprogram/getLinkMiniprogram.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/wxamplinkget")]
    Task<WxaWxampLinkGetResponse> WxampLinkGetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 关联小程序。
    /// </summary>
    /// <param name="request">关联请求（含小程序 appid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/officalaccount-management/link-miniprogram/linkMiniprogram.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/wxamplink")]
    Task<OpenPlatformResponse> WxampLinkAsync([Body] WxaWxampLinkRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除关联小程序。
    /// </summary>
    /// <param name="request">解除关联请求（含小程序 appid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/officalaccount-management/link-miniprogram/unlinkMiniprogram.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/wxampunlink")]
    Task<OpenPlatformResponse> WxampUnlinkAsync([Body] WxaWxampUnlinkRequest request, CancellationToken cancellationToken = default);
}
