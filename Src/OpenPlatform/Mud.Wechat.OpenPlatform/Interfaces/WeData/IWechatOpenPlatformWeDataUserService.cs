// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.WeData;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「We 分析（WeData）· 用户权限与登录」域 SDK
/// （<b>5 端点</b>：权限集列表 / 设置用户权限 / 查询绑定列表 / 用户解绑 / PC 端登录）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：本组恒 <c>authorizer_access_token</c>（Query 参数名 <c>access_token</c>）——
/// 第三方平台代授权方小程序管理 We 分析用户，调用前须经
/// <see cref="Mud.Wechat.OpenPlatform.Abstractions.Authentication.IComponentAppContextSwitcher.UseAuthorizerScope(string)"/>
/// 进入目标授权方作用域。</para>
/// <para><b>官方约束</b>：小程序的权限集 ID 为 <b>18</b>；全部端点在服务端调用；
/// <c>set_user_perm</c> 不可修改管理员 / 数据管理员；<c>unbind_user</c> 会删除成员身份（管理员与数据管理员除外）。</para>
/// <para><b>与 <see cref="IWechatOpenPlatformWeDataService"/> 区分</b>：本组消费授权方令牌，登录配置两端口消费
/// <c>component_access_token</c>；令牌归属相反故拆分两接口，但共用注册组 <c>WeData</c>（同 Ams/AmsShare 拆组先例）。</para>
/// <para><b>落位</b>：<c>Interfaces/WeData/IWechatOpenPlatformWeDataUserService.cs</c>；DTO 命名空间
/// <c>Mud.Wechat.OpenPlatform.DataModels.WeData</c>；<c>OpenPlatformModule.WeData</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "WeData", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformWeDataUserService
{
    /// <summary>
    /// 获取商家 We 分析权限集列表（<c>POST /wedata/wedata_get_perm_list</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataGetPermList.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>权限集列表，见 <see cref="OpenPlatformWeDataPermissionListResponse"/>。</returns>
    /// <remarks>官方约束：权限集 ID <c>18</c>；仅第三方平台代调用，须在服务端调用。</remarks>
    [Post("/wedata/wedata_get_perm_list")]
    Task<OpenPlatformWeDataPermissionListResponse> GetPermissionListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置用户 We 分析权限集（<c>POST /wedata/wedata_set_user_perm</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataSetUserPerm.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>uid</c> + 权限集列表），见 <see cref="OpenPlatformWeDataSetUserPermissionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方约束：只能设置普通用户权限集，<b>管理员与数据管理员不可通过本接口修改</b>；权限集 ID <c>18</c>。</remarks>
    [Post("/wedata/wedata_set_user_perm")]
    Task<OpenPlatformResponse> SetUserPermissionAsync(
        [Body] OpenPlatformWeDataSetUserPermissionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户绑定列表（<c>POST /wedata/wedata_query_bind_list</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataQueryBindList.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户绑定信息列表，见 <see cref="OpenPlatformWeDataBindListResponse"/>。</returns>
    /// <remarks>官方约束：仅第三方平台可调用，权限集 ID <c>18</c>，须在服务端调用。</remarks>
    [Post("/wedata/wedata_query_bind_list")]
    Task<OpenPlatformWeDataBindListResponse> QueryBindListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户解绑（<c>POST /wedata/wedata_unbind_user</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataUnbindUser.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>uid</c>），见 <see cref="OpenPlatformWeDataUnbindUserRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方约束：解绑会删除该用户的成员身份（管理员与数据管理员除外），操作不可撤销。</remarks>
    [Post("/wedata/wedata_unbind_user")]
    Task<OpenPlatformResponse> UnbindUserAsync(
        [Body] OpenPlatformWeDataUnbindUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户 PC 端登录（<c>POST /wedata/wedata_login</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/wedata_login.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>uid</c> + <c>user_session</c> + 可选环境信息），见 <see cref="OpenPlatformWeDataLoginRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>登录链接与过期时间（信封为 <c>base_resp</c>），见 <see cref="OpenPlatformWeDataLoginResponse"/>。</returns>
    /// <remarks>
    /// <para>官方约束：须先完成权限集与登录配置设置，须在服务端调用。</para>
    /// <para>登录链接 <see cref="OpenPlatformWeDataLoginResponse.RedirectUrl"/> 为一次性凭据，<b>不得</b>落日志或遥测；
    /// <c>client_ip</c>/<c>user_agent</c> 属用户环境信息，同样不得落日志。</para>
    /// </remarks>
    [Post("/wedata/wedata_login")]
    Task<OpenPlatformWeDataLoginResponse> LoginAsync(
        [Body] OpenPlatformWeDataLoginRequest request,
        CancellationToken cancellationToken = default);
}
