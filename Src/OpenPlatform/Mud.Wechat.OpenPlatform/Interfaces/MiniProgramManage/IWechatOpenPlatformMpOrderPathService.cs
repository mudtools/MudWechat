// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 订单路径」子域 SDK（<b>1 端点</b>：查询订单路径信息）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载，Query 参数名恒 <c>access_token</c>（2026-10-17 逐页取证）。</para>
/// <para><b>动词陷阱</b>：<c>getorderpathinfo</c> 名义为查询、官方实为 <b>POST</b>（带 JSON 请求体），勿望文生义写成 GET。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpOrderPathService
{
    /// <summary>
    /// 查询订单路径信息（<c>getorderpathinfo</c>；<b>注意官方动词为 POST</b>，非 GET）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getOrderPathInfo.html"/>。
    /// </summary>
    [Post("/wxa/security/getorderpathinfo")]
    Task<OpenPlatformMpOrderPathInfoResponse> GetOrderPathInfoAsync(
        [Body] OpenPlatformMpOrderPathGetRequest request,
        CancellationToken cancellationToken = default);
}
