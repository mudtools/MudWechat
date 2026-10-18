// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 被展示组件（订阅）」子域 SDK（<b>3 端点</b>：查被展示状态 / 查可展示小程序列表 / 设置被展示）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载，Query 参数名恒 <c>access_token</c>（2026-10-17 逐页核验）。</para>
/// <para><b>动词</b>：两支查询为 GET，设置支为 POST。<c>getwxamplinkforshow</c> 以 Query <c>page</c>/<c>num</c>（每页最多 20）分页。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpSubscribeService
{
    /// <summary>
    /// 获取小程序被展示组件状态（<c>getshowwxaitem</c>，官方为 GET，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/subscribe-component/getShowItem.html"/>。
    /// </summary>
    [Get("/wxa/getshowwxaitem")]
    Task<OpenPlatformMpShowItemResponse> GetShowItemAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可展示小程序列表（<c>getwxamplinkforshow</c>，官方为 GET，Query <c>page</c>/<c>num</c> 必填，<c>num</c> 最大 20）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/subscribe-component/getLinkForShow.html"/>。
    /// </summary>
    [Get("/wxa/getwxamplinkforshow")]
    Task<OpenPlatformMpLinkForShowResponse> GetLinkForShowAsync(
        [Query("page")] long page,
        [Query("num")] long num,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置小程序被展示组件（<c>updateshowwxaitem</c>，<c>appid</c> 取自 <c>getwxamplinkforshow</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/subscribe-component/setShowItem.html"/>。
    /// </summary>
    [Post("/wxa/updateshowwxaitem")]
    Task<OpenPlatformResponse> UpdateShowItemAsync(
        [Body] OpenPlatformMpShowItemFlagRequest request,
        CancellationToken cancellationToken = default);
}
