// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.Auth;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「小程序微信认证 / 安全认证」域 SDK（<b>5 端点</b>：身份选项树 1 支 GET + 提审 / 重新提审 / 进度查询 3 支 POST + 补充材料上传 1 支 multipart POST）。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 5 条路由恒为 <c>/wxa/sec/*</c>（2026-10-17 依官方 weapp-wxverify 5 页逐页核验）。</para>
/// <para><b>落位判据（为何属开放平台线，而非小程序线）</b>：本域 5 页只落在
/// <c>doc/oplatform/…/miniprogram-management/weapp-wxverify/</c> 文档树，令牌面是
/// <c>authorizer_access_token</c>（第三方平台代授权方小程序发起认证，Query 参数名仍为 <c>access_token</c>）⇒ 属开放平台线。
/// 该归属由留档 <c>MP-D-BUCKET-RULING-01</c>（<c>.docs/MudWechat-小程序D桶归属终判留档-2026-10-13.md</c>）与小程序线守卫
/// <c>NC2</c>（<c>Src/MiniProgram</c> 对 <c>/wxa/sec/authidentitytree</c> 等认证路由零声明）双向锁定。</para>
/// <para><b>官方反直觉点（照抄不修正）</b>：① <c>/wxa/sec/authidentitytree</c> 官方动词是 <b>GET</b>（清单误记 POST）；
/// ② <c>wxaauth</c>（首次提审）与 <c>reauth</c>（重新提审）请求体 <c>auth_data</c> 子树与应答逐字段相同 ⇒ 共用
/// <see cref="OpenPlatformAuthSubmitRequest"/> / <see cref="OpenPlatformAuthSubmitResponse"/>，官方 <c>reauth</c> 页示例 URL 仍写 <c>/wxa/sec/wxaauth</c>。</para>
/// <para><b>落位</b>：<c>Interfaces/Auth/IWechatOpenPlatformAuthService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.Auth</c>；
/// <c>OpenPlatformModule.Auth</c>，注册组 <c>Auth</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Auth", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformAuthService
{
    /// <summary>
    /// 查询个人认证身份选项列表（<c>authidentitytree</c>，递归身份树）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_authidentitytree.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAuthIdentityTreeResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b> <c>/wxa/sec/authidentitytree</c>；Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）；<b>无请求体</b>（不得造请求 DTO）。</para>
    /// <para>应答为递归 <c>identity_tree_list</c>（节点自指子级 <c>node_list</c>）。</para>
    /// </remarks>
    [Get("/wxa/sec/authidentitytree")]
    Task<OpenPlatformAuthIdentityTreeResponse> GetAuthIdentityTreeAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起小程序认证提审（<c>wxaauth</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_wxaauth.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>auth_data</c> 子树），见 <see cref="OpenPlatformAuthSubmitRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAuthSubmitResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/sec/wxaauth</c>；JSON 请求体为 <c>{"auth_data": {...}}</c>，Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）。</para>
    /// <para>与 <see cref="ReauthAsync"/> 的 <c>auth_data</c> 子树与应答逐字段相同 ⇒ 共用请求 / 应答 DTO。</para>
    /// </remarks>
    [Post("/wxa/sec/wxaauth")]
    Task<OpenPlatformAuthSubmitResponse> SubmitAuthAsync(
        [Body] OpenPlatformAuthSubmitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 小程序认证重新提审（<c>reauth</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_reauth.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>auth_data</c> 子树），见 <see cref="OpenPlatformAuthSubmitRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAuthSubmitResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/sec/reauth</c>；请求体与 <see cref="SubmitAuthAsync"/> 逐字相同（同为 <c>{"auth_data": {...}}</c>）⇒ 共用 <see cref="OpenPlatformAuthSubmitRequest"/>。Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）。</para>
    /// <para><b>官方页面示例 URL 仍写 <c>/wxa/sec/wxaauth</c></b>（正文端点却是 <c>reauth</c>）——属官方页面示例瑕疵，路由以本节请求地址为准。</para>
    /// </remarks>
    [Post("/wxa/sec/reauth")]
    Task<OpenPlatformAuthSubmitResponse> ReauthAsync(
        [Body] OpenPlatformAuthSubmitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 小程序认证进度查询（<c>queryauth</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_queryauth.html"/>。
    /// </summary>
    /// <param name="request">请求体（仅 <c>taskid</c>，必填），见 <see cref="OpenPlatformAuthQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAuthQueryResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/sec/queryauth</c>；查询类端点官方仍用 POST + JSON 请求体（仅 <c>taskid</c>），Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）。</para>
    /// <para>应答 <c>task_status</c> / <c>apply_status</c> / <c>orderid</c> 官方标 number、其余标 string（2026-10-17 逐页核验返回参数表）。</para>
    /// </remarks>
    [Post("/wxa/sec/queryauth")]
    Task<OpenPlatformAuthQueryResponse> QueryAuthAsync(
        [Body] OpenPlatformAuthQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 小程序认证上传补充材料（<c>uploadauthmaterial</c>，<c>multipart/form-data</c>，图片文件字段名固定 <c>media</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_uploadauthmaterial.html"/>。
    /// </summary>
    /// <param name="formData">
    /// multipart 表单内容（文件字段名必须为 <c>media</c>；由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>；
    /// 官方约束：图片 ≤2M，支持 PNG / JPEG / JPG / GIF）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAuthUploadMaterialResponse"/>（回填 <c>mediaid</c> 进 <c>auth_data</c> 各材料数组）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/sec/uploadauthmaterial</c> + <c>multipart/form-data</c>；官方 <c>Content-Type</c> 为 multipart，JSON DTO 通路无法承载二进制字段 ⇒ 表单由宿主拼装，守卫 AUTH-E5 断言本端点无 JSON 请求 DTO。</para>
    /// <para>Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）。</para>
    /// </remarks>
    [Post("/wxa/sec/uploadauthmaterial")]
    Task<OpenPlatformAuthUploadMaterialResponse> UploadAuthMaterialAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);
}
