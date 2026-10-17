// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 名称与搜索」子域 SDK（<b>5 端点</b>：修改名字 / 查名称审核状态 / 改可被搜索 / 查可被搜索 / 数据预拉取配置）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载，Query 参数名 <c>access_token</c>（2026-10-17 逐页核验）。</para>
/// <para><b>字段名差异</b>：名称审核用 <c>audit_id</c>（带下划线），代码审核用 <c>auditid</c>（无下划线），各照官方原文不「对齐」。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpBasicInfoService
{
    /// <summary>
    /// 修改小程序名字（<c>setnickname</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/setNickName.html"/>。
    /// </summary>
    [Post("/wxa/setnickname")]
    Task<OpenPlatformMpSetNicknameResponse> SetNicknameAsync(
        [Body] OpenPlatformMpSetNicknameRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询名称审核状态（<c>api_wxa_querynickname</c>，按名称审核单号 <c>audit_id</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getNickNameStatus.html"/>。
    /// </summary>
    [Post("/wxa/api_wxa_querynickname")]
    Task<OpenPlatformMpQueryNicknameResponse> QueryNicknameStatusAsync(
        [Body] OpenPlatformMpQueryNicknameRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改可被搜索状态（<c>changewxasearchstatus</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/setSearchStatus.html"/>。
    /// </summary>
    [Post("/wxa/changewxasearchstatus")]
    Task<OpenPlatformResponse> ChangeWxaSearchStatusAsync(
        [Body] OpenPlatformMpSearchStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可被搜索状态（<c>getwxasearchstatus</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getSearchStatus.html"/>。
    /// </summary>
    [Get("/wxa/getwxasearchstatus")]
    Task<OpenPlatformMpSearchStatusResponse> GetWxaSearchStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 数据预拉取 / 周期性运营数据配置（<c>fetchdatasetting</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getFetchdataSetting.html"/>。
    /// </summary>
    [Post("/wxa/fetchdatasetting")]
    Task<OpenPlatformMpFetchDataSettingResponse> FetchDataSettingAsync(
        [Body] OpenPlatformMpFetchDataSettingRequest request,
        CancellationToken cancellationToken = default);
}
