// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.OpenAccount;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台（第三方平台）「开放账号管理」域 SDK
/// （创建 / 查询 / 绑定 / 解绑 / 判有 / 主体一致性——即官方「开放平台账号管理」分组下
/// 消费<b>授权方令牌</b>的六个端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌语义（官方契约，已逐端点核验）</b>：本组端点操作「某授权账号」绑定的开放平台账号，
/// 令牌为<b>授权方令牌</b>（query 参数名 <c>access_token</c>，值 <c>authorizer_access_token</c>）。
/// 调用前须经 <see cref="IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId)"/>
/// 进入目标授权方作用域，否则运行期 fail-fast（不静默回退平台令牌）。
/// </para>
/// <para>
/// MUD005 已知接受风险：开放平台官方契约强制令牌走 Query 参数，无法改用 Header；
/// URL 遥测已由组件 <c>SensitiveUrlRedactor</c> 与 <see cref="WechatOpenPlatformException"/> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "OpenAccount", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatOpenPlatformOpenAccountService
{
    /// <summary>
    /// 创建开放平台账号并绑定至授权账号。
    /// </summary>
    /// <param name="request">创建请求（授权方 <c>appid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建开放平台账号 <c>appid</c>（<c>open_appid</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/openplatform-management/createOpenAccount.html"/></para>
    /// <para>官方约束：一个授权账号只能绑定一个开放平台账号；重复创建报 <c>895030</c>（已绑定）。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40013</c> / <c>895030</c>（已绑定，先查 <see cref="HaveOpenAccountAsync"/>）。</para>
    /// </remarks>
    [Post("/cgi-bin/open/create")]
    Task<OpenPlatformOpenAppIdResponse> CreateOpenAccountAsync(
        [Body] OpenPlatformOpenAccountCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取授权账号所绑定的开放平台账号。
    /// </summary>
    /// <param name="request">查询请求（授权方 <c>appid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>绑定的开放平台账号 <c>appid</c>（<c>open_appid</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/openplatform-management/getOpenAccount.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/open/get")]
    Task<OpenPlatformOpenAppIdResponse> GetOpenAccountAsync(
        [Body] OpenPlatformOpenAccountGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将授权账号绑定至指定开放平台账号。
    /// </summary>
    /// <param name="request">绑定请求（授权方 <c>appid</c> + 开放平台 <c>open_appid</c>，均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/openplatform-management/bindOpenAccount.html"/></para>
    /// <para>官方约束：绑定后<b>不可自动解绑</b>——解绑须登录开放平台管理后台操作（官方原文），SDK 不提供解绑替代语义。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40013</c> / <c>895023</c>（open_appid 不存在）等。</para>
    /// </remarks>
    [Post("/cgi-bin/open/bind")]
    Task<OpenPlatformResponse> BindOpenAccountAsync(
        [Body] OpenPlatformOpenAccountBindRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将授权账号从开放平台账号解绑。
    /// </summary>
    /// <param name="request">解绑请求（授权方 <c>appid</c> + 开放平台 <c>open_appid</c>，均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/openplatform-management/unbindOpenAccount.html"/></para>
    /// <para><b>覆盖删除语义</b>：解绑为破坏性操作，授权账号在开放平台侧的关联关系即时失效，无法恢复。</para>
    /// </remarks>
    [Post("/cgi-bin/open/unbind")]
    Task<OpenPlatformResponse> UnbindOpenAccountAsync(
        [Body] OpenPlatformOpenAccountUnbindRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查授权账号是否已绑定开放平台账号。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>have_open</c>（0/1）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getBindOpenAccount.html"/></para>
    /// <para>官方契约：POST、请求体为空（操作对象即当前授权方上下文自身）。</para>
    /// </remarks>
    [Post("/cgi-bin/open/have")]
    Task<OpenPlatformOpenAccountHaveResponse> HaveOpenAccountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查授权账号主体与开放平台账号主体是否一致。
    /// </summary>
    /// <param name="appId">授权方 <c>appid</c>（官方 query <c>appid</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>same_entity</c>（0/1）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getBindOpenAccountEntity.html"/></para>
    /// <para>官方契约：<b>本组唯一 GET 端点</b>（其余五个均 POST）、无请求体；<c>appid</c> 走 Query。</para>
    /// </remarks>
    [Get("/cgi-bin/open/sameentity")]
    Task<OpenPlatformOpenAccountSameEntityResponse> IsSameEntityAsync(
        [Query("appid")] string appId,
        CancellationToken cancellationToken = default);
}
