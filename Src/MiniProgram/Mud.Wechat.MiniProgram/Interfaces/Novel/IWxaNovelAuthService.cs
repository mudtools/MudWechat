// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.Novel;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 小程序线「小说作品管理」域——小说授权面接口（3 端点）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_addbookauth.html"/> 等。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用。</para>
/// <para>账号-小说授权是由主授权者账号（AppID）→ 作品 ID → 被授权者账号（AppID）三维度构建的数据关系。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelAuthService
{
    /// <summary>增加账号-小说的授权关系数据，一次最多新增 20 条。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>每条授权关系的逐条结果（含内层 errcode）。</returns>
    [Post("/wxa/book/addbookauth")]
    Task<WxaNovelAddBookAuthResponse> AddBookAuthAsync([Body] WxaNovelAddBookAuthRequest request, CancellationToken cancellationToken = default);

    /// <summary>查看账号的小说授权关系列表（主授权账号使用）或被授权关系列表。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>授权关系列表。</returns>
    [Post("/wxa/book/querybookauth")]
    Task<WxaNovelQueryBookAuthResponse> QueryBookAuthAsync([Body] WxaNovelQueryBookAuthRequest request, CancellationToken cancellationToken = default);

    /// <summary>删除账号的小说授权关系（主授权账号使用）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/book/delbookauth")]
    Task<WxaResponse> DelBookAuthAsync([Body] WxaNovelDelBookAuthRequest request, CancellationToken cancellationToken = default);
}
