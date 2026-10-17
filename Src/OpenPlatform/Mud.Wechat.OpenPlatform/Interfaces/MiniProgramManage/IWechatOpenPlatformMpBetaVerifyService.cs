// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 试用小程序核验」子域 SDK（<b>2 端点</b>：试用小程序快速转正 / 修改试用小程序名称）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域 <c>authorizer_access_token</c>（Query 参数名 <c>access_token</c>，权限集 id 81，第三方代商家调用；2026-10-17 逐页取证）。
/// 虽官方文档树位于 <c>register-management/fast-regist-beta</c>，令牌归属与其余 <c>/wxa/*</c> 授权方面一致，故并入 <c>MiniProgramManage</c> 注册组。</para>
/// <para><b>转正流程为异步</b>：<c>verifybetaweapp</c> 下发法人扫脸/认证后，状态经 <c>notify_third_fastverifybetaapp</c> 推送回授权事件接收 URL；创建转正任务串行，重复下发报 <c>89249</c>。</para>
/// <para><b>本域不建模 2.0 旧版两支路由</b>（<c>/wxa/getmpadminauth</c>、<c>/wxa/mpverifybetaweapp</c>）：2026-10-17 逐页核验，
/// 二者旧页 <c>Third-party_Platforms/2.0/api/verify_beta_Mini_Programs/</c> HTTP 状态仍 200（无 301）但正文仅剩「优化后重新上线」，
/// 且现行 <c>openApi/OpenApiDoc</c> 目录树内无对应页 ⇒ 按「文档目录树即契约」判据不成立，属非契约路由，
/// 由守卫 <c>OpenPlatformNonContractRouteZeroDeclarationContractGuards</c>（OP-NC1/OP-NC2，追踪号 P1-OP-LEGACY20-BETAVERIFY-01）锁定零声明。
/// 注意 <c>/wxa/mpverifybetaweapp</c> 与上方已落位的 <c>/wxa/verifybetaweapp</c> 是<b>不同路由</b>，勿因名称相近而误判为已建模或误加回。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpBetaVerifyService
{
    /// <summary>
    /// 试用小程序快速转正（<c>verifybetaweapp</c>，恒 POST，企业法人人脸识别）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-regist-beta/verfifyBetaMiniprogram.html"/>。
    /// </summary>
    [Post("/wxa/verifybetaweapp")]
    Task<OpenPlatformMpVerifyBetaResponse> VerifyBetaWeappAsync(
        [Body] OpenPlatformMpVerifyBetaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改试用小程序名称（<c>setbetaweappnickname</c>，恒 POST；仅适用于试用小程序）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-regist-beta/modifyBetaMiniprogramNickName.html"/>。
    /// </summary>
    [Post("/wxa/setbetaweappnickname")]
    Task<OpenPlatformResponse> SetBetaWeappNicknameAsync(
        [Body] OpenPlatformMpSetBetaNicknameRequest request,
        CancellationToken cancellationToken = default);
}
