// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 体验成员」子域 SDK（<b>3 端点</b>：绑定体验者 / 解绑体验者 / 获取体验者列表）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载，Query 参数名恒 <c>access_token</c>（2026-10-17 逐页核验）。</para>
/// <para><b>动词</b>：三支官方均为 POST；<c>memberauth</c> 以请求体 <c>action=get_experiencer</c> 分派（清单里的 <c>?action=</c> 是文档简写，实为 JSON body）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpMemberService
{
    /// <summary>
    /// 绑定体验者（<c>bind_tester</c>，微信号必填；上限 85002，重复绑定 85004）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/member-management/bindTester.html"/>。
    /// </summary>
    [Post("/wxa/bind_tester")]
    Task<OpenPlatformMpBindTesterResponse> BindTesterAsync(
        [Body] OpenPlatformMpBindTesterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解绑体验者（<c>unbind_tester</c>，以 <c>wechatid</c> 或 <c>userstr</c> 任一标识解绑）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/member-management/unbindTester.html"/>。
    /// </summary>
    [Post("/wxa/unbind_tester")]
    Task<OpenPlatformResponse> UnbindTesterAsync(
        [Body] OpenPlatformMpUnbindTesterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取体验者列表（<c>memberauth</c>，官方为 POST、请求体 <c>action=get_experiencer</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/member-management/getTester.html"/>。
    /// </summary>
    [Post("/wxa/memberauth")]
    Task<OpenPlatformMpTesterListResponse> GetTesterAsync(
        [Body] OpenPlatformMpMemberAuthRequest request,
        CancellationToken cancellationToken = default);
}
