// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.Embedded;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「半屏小程序 / 嵌入式小程序管理」域 SDK（<b>6 端点</b>：添加 / 删除 / 取消授权 / 设置授权方式 4 支 POST + 调用列表 / 授权列表 2 支 GET）。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 6 条路由恒为 <c>/wxaapi/wxaembedded/*</c> 前缀（2026-10-17 依官方 embedded-management 6 页逐页核验）。
/// <b>路由拼写照官方正文，不得据页面文件名推测</b>：请求路径是 <c>add_embedded</c> / <c>get_list</c> / <c>del_authorize</c> / <c>set_authorize</c>，
/// 而官方页面文件名是 <c>addEmbedded</c> / <c>getEmbeddedList</c> 等驼峰形态，两者不一致。</para>
/// <para><b>落位判据（为何属开放平台线，而非小程序线）</b>：本域 6 页只落在
/// <c>doc/oplatform/…/miniprogram-management/embedded-management/</c> 文档树，令牌面是
/// <c>authorizer_access_token</c>（第三方平台代授权方小程序调用，Query 参数名仍为 <c>access_token</c>）⇒ 属开放平台线。
/// 该归属由留档 <c>MP-D-BUCKET-RULING-01</c>（<c>.docs/MudWechat-小程序D桶归属终判留档-2026-10-13.md</c>）与小程序线守卫
/// <c>NC2</c>（<c>Src/MiniProgram</c> 对 <c>/wxaapi/wxaembedded/*</c> 零声明）双向锁定；小程序线声明之即令牌串号（P3-R2）。</para>
/// <para><b>权限集</b>：官方「添加半屏小程序」页给出权限集 <b>18</b>（「开发与数据分析」）；其余端点权限集见逐接口注释，未给数字的一律留空、不凭记忆补。</para>
/// <para><b>频率</b>：官方仅「添加半屏小程序」页给出端点级数字（单授权方 50 次/天、同一目标小程序 3 次/24h、单目标小程序每日被添加 10 次），
/// 其余端点未给频率数字 ⇒ 注释照官方留空。</para>
/// <para><b>落位</b>：<c>Interfaces/Embedded/IWechatOpenPlatformEmbeddedService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.Embedded</c>；
/// <c>OpenPlatformModule.Embedded</c>，注册组 <c>Embedded</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Embedded", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformEmbeddedService
{
    /// <summary>
    /// 「添加半屏小程序」（<c>add_embedded</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/embedded-management/addEmbedded.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformEmbeddedAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxaapi/wxaembedded/add_embedded</c>；JSON 请求体（<c>appid</c> + <c>apply_reason</c>），Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）。</para>
    /// <para><b>频率</b>：官方本页给出「同一授权方小程序最多添加 50 次/天；同一目标小程序最多被添加 3 次/24h；单个目标小程序每日被添加 10 次」。</para>
    /// <para><b>权限集</b>：官方给出权限集 18（「开发与数据分析」）。</para>
    /// </remarks>
    [Post("/wxaapi/wxaembedded/add_embedded")]
    Task<OpenPlatformResponse> AddEmbeddedAsync(
        [Body] OpenPlatformEmbeddedAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除半屏小程序」（<c>del_embedded</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/embedded-management/deleteEmbedded.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>appid</c>），见 <see cref="OpenPlatformEmbeddedAppidRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxaapi/wxaembedded/del_embedded</c>；JSON 请求体仅 <c>appid</c>，Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）。官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/wxaapi/wxaembedded/del_embedded")]
    Task<OpenPlatformResponse> DeleteEmbeddedAsync(
        [Body] OpenPlatformEmbeddedAppidRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「取消授权小程序」（<c>del_authorize</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/embedded-management/deleteAuthorizedEmbedded.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>appid</c>），见 <see cref="OpenPlatformEmbeddedAppidRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxaapi/wxaembedded/del_authorize</c>；请求体与 <c>del_embedded</c> 逐字相同（仅 <c>appid</c>）⇒ 共用 <see cref="OpenPlatformEmbeddedAppidRequest"/>。Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）。官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/wxaapi/wxaembedded/del_authorize")]
    Task<OpenPlatformResponse> DeleteAuthorizedEmbeddedAsync(
        [Body] OpenPlatformEmbeddedAppidRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「设置授权方式」（<c>set_authorize</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/embedded-management/setAuthorizedEmbedded.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>flag</c>），见 <see cref="OpenPlatformEmbeddedSetAuthorizeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxaapi/wxaembedded/set_authorize</c>；JSON 请求体仅 <c>flag</c>（number），Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）。官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/wxaapi/wxaembedded/set_authorize")]
    Task<OpenPlatformResponse> SetAuthorizeAsync(
        [Body] OpenPlatformEmbeddedSetAuthorizeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取半屏小程序调用列表」（<c>get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/embedded-management/getEmbeddedList.html"/>。
    /// </summary>
    /// <param name="start">分页起始偏移（Query <c>start</c>）。</param>
    /// <param name="num">分页数量（Query <c>num</c>，官方上限 1000）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformEmbeddedListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b> <c>/wxaapi/wxaembedded/get_list</c>；Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>）+ <c>start</c> + <c>num</c>；<b>无请求体</b>（不得造请求 DTO）。应答含 <c>embedded_flag</c> 与 <c>wxa_embedded_list</c>。</para>
    /// </remarks>
    [Get("/wxaapi/wxaembedded/get_list")]
    Task<OpenPlatformEmbeddedListResponse> GetEmbeddedListAsync(
        [Query("start")] long? start = null,
        [Query("num")] long? num = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取半屏小程序授权列表」（<c>get_own_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/embedded-management/getOwnList.html"/>。
    /// </summary>
    /// <param name="start">分页起始偏移（Query <c>start</c>）。</param>
    /// <param name="num">分页数量（Query <c>num</c>，官方上限 1000）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformEmbeddedListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b> <c>/wxaapi/wxaembedded/get_own_list</c>；Query 参数与应答形状与 <c>get_list</c> 逐字相同 ⇒ 两支列表共用 <see cref="OpenPlatformEmbeddedListResponse"/>。</para>
    /// </remarks>
    [Get("/wxaapi/wxaembedded/get_own_list")]
    Task<OpenPlatformEmbeddedListResponse> GetOwnEmbeddedListAsync(
        [Query("start")] long? start = null,
        [Query("num")] long? num = null,
        CancellationToken cancellationToken = default);
}
