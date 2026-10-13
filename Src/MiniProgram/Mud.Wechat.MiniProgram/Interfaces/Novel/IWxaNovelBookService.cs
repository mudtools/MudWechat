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
/// 小程序线「小说作品管理」域——作品面接口（6 端点）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_createbook.html"/> 等。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用。</para>
/// <para>官方未给出端点级频率数字（只有规模限制：单次上传章节数上限、分页 limit 上限等）。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelBookService
{
    /// <summary>创建作品（小说）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>作品 ID。</returns>
    [Post("/wxa/book/createbook")]
    Task<WxaNovelCreateBookResponse> CreateBookAsync([Body] WxaNovelCreateBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>编辑作品编辑版的基础信息、章节顺序、分卷信息。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/book/updatebook")]
    Task<WxaResponse> UpdateBookAsync([Body] WxaNovelUpdateBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>删除作品，同时作用于编辑版和发布版。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/book/deletebook")]
    Task<WxaResponse> DeleteBookAsync([Body] WxaNovelDeleteBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取所有作品列表，分为发布版列表和编辑版列表。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>作品列表。</returns>
    [Post("/wxa/book/listbook")]
    Task<WxaNovelListBookResponse> ListBookAsync([Body] WxaNovelListBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取作品详细信息，区分发布版和编辑版。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>作品信息。</returns>
    [Post("/wxa/book/getbook")]
    Task<WxaNovelGetBookResponse> GetBookAsync([Body] WxaNovelGetBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>作品提交审核。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/book/auditbook")]
    Task<WxaResponse> AuditBookAsync([Body] WxaNovelAuditBookRequest request, CancellationToken cancellationToken = default);
}
