// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 隐私接口」子域 SDK（<b>3 端点</b>：提交隐私接口审核 / 查询隐私接口列表 / 获取代码隐私信息）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载，Query 参数名恒 <c>access_token</c>（2026-10-17 逐页取证）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpPrivacyService
{
    /// <summary>
    /// 提交隐私接口审核（<c>apply_privacy_interface</c>，返回审核单 id）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-api-management/applyPrivacyInterface.html"/>。
    /// </summary>
    [Post("/wxa/security/apply_privacy_interface")]
    Task<OpenPlatformMpPrivacyApplyResponse> ApplyPrivacyInterfaceAsync(
        [Body] OpenPlatformMpPrivacyApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询隐私接口列表（<c>get_privacy_interface</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-api-management/getPrivacyInterface.html"/>。
    /// </summary>
    [Get("/wxa/security/get_privacy_interface")]
    Task<OpenPlatformMpPrivacyInterfaceListResponse> GetPrivacyInterfaceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小程序代码隐私信息（<c>get_code_privacy_info</c>，扫描代码中未声明授权用途/未配置隐私协议的隐私接口）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getCodePrivacyInfo.html"/>。
    /// </summary>
    [Get("/wxa/security/get_code_privacy_info")]
    Task<OpenPlatformMpCodePrivacyInfoResponse> GetCodePrivacyInfoAsync(CancellationToken cancellationToken = default);
}
