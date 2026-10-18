// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 类目」子域 SDK（<b>1 端点</b>：获取所有可提交类目名称）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：<c>authorizer_access_token</c>，Query 参数名 <c>access_token</c>。</para>
/// <para><b>与第三方类目域区分</b>：本端点 <c>/wxa/get_category</c> 返回授权方小程序「可提交的服务类目」，与 <see cref="IWechatOpenPlatformCategoryService"/>（<c>/cgi-bin/wxopen/*</c>，第三方平台自身的类目库）不同。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpCategoryService
{
    /// <summary>
    /// 获取小程序所有类目名称（<c>get_category</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/getAllCategoryName.html"/>。
    /// </summary>
    [Get("/wxa/get_category")]
    Task<OpenPlatformMpCategoryListResponse> GetCategoryAsync(
        CancellationToken cancellationToken = default);
}
