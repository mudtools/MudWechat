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
/// 小程序线「小说作品管理」域——账号授权面接口（3 端点）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_addbookauthbyappid.html"/> 等。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用。</para>
/// <para>账号授权是由主授权者账号（AppID）→ 被授权者账号（AppID）构建的数据关系，授权后可将主授权账号的小说批量授权给被授权账号。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelAccountAuthService
{
    /// <summary>增加账号-账号的授权关系数据，一次最多新增 20 条。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>每条授权关系的逐条结果（含内层 errcode）。</returns>
    [Post("/wxa/book/addbookauthbyappid")]
    Task<WxaNovelAddBookAuthByAppIdResponse> AddBookAuthByAppIdAsync([Body] WxaNovelAddBookAuthByAppIdRequest request, CancellationToken cancellationToken = default);

    /// <summary>查看账号的授权关系列表或被授权关系列表（游标分页，上限 100 条/次）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>被授权方列表与下一页游标。</returns>
    [Post("/wxa/book/querybookauthv2")]
    Task<WxaNovelQueryBookAuthV2Response> QueryBookAuthV2Async([Body] WxaNovelQueryBookAuthV2Request request, CancellationToken cancellationToken = default);

    /// <summary>删除账号维度授权关系（主授权账号使用）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/book/delbookauthbyappid")]
    Task<WxaResponse> DelBookAuthByAppIdAsync([Body] WxaNovelDelBookAuthByAppIdRequest request, CancellationToken cancellationToken = default);
}
