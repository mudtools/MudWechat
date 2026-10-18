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
/// 微信开放平台「We 分析（WeData）· 登录配置」域 SDK（<b>2 端点</b>：查询登录配置 / 设置登录配置）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：本组恒 <c>component_access_token</c>（Query 参数名 <c>access_token</c>）——
/// 第三方平台以自身令牌配置 We 分析登录能力，<b>非</b>授权方令牌（官方明写「仅第三方平台可调用」）。</para>
/// <para><b>与 <see cref="IWechatOpenPlatformWeDataUserService"/> 区分</b>：用户权限 / 绑定 / 登录五端点
/// 消费授权方令牌（<c>authorizer_access_token</c>），故拆为独立接口与注册组 <c>WeData</c>（同 Ams/AmsShare 按令牌拆组先例）。</para>
/// <para><b>落位</b>：<c>Interfaces/WeData/IWechatOpenPlatformWeDataService.cs</c>；DTO 命名空间
/// <c>Mud.Wechat.OpenPlatform.DataModels.WeData</c>；<c>OpenPlatformModule.WeData</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "WeData", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformWeDataService
{
    /// <summary>
    /// 查询登录配置（<c>POST /wedata/wedata_get_login_config</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataGetLoginConfig.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务商信息、反查地址与关联小程序列表，见 <see cref="OpenPlatformWeDataLoginConfigResponse"/>。</returns>
    /// <remarks>官方约束：仅第三方平台可调用，须在服务端调用。</remarks>
    [Post("/wedata/wedata_get_login_config")]
    Task<OpenPlatformWeDataLoginConfigResponse> GetLoginConfigAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置登录配置（<c>POST /wedata/wedata_set_login_config</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataSetLoginConfig.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformWeDataSetLoginConfigRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方约束：<c>set_type = 1</c> 设置 PC 端登录的 <c>recheck_url</c>；
    /// <c>set_type = 2</c> 设置关联小程序 <c>associate_appid</c>（<b>最多 5 个</b>）。</para>
    /// <para>仅第三方平台可调用，须在服务端调用。</para>
    /// </remarks>
    [Post("/wedata/wedata_set_login_config")]
    Task<OpenPlatformResponse> SetLoginConfigAsync(
        [Body] OpenPlatformWeDataSetLoginConfigRequest request,
        CancellationToken cancellationToken = default);
}
