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
/// 小程序线「小说作品管理」域——章节面接口（8 端点）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_createchapter.html"/> 等。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用。</para>
/// <para>官方未给出端点级频率数字（只有规模限制：单次最多上传 10 章、章节内容长度上限等）。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelChapterService
{
    /// <summary>上传章节到作品编辑版信息里。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>章节 ID。</returns>
    [Post("/wxa/book/createchapter")]
    Task<WxaNovelCreateChapterResponse> CreateChapterAsync([Body] WxaNovelCreateChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>批量上传章节到作品编辑版信息里，单次最多 10 章。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>章节 ID 列表与冲突的提供方主键列表。</returns>
    [Post("/wxa/book/batchcreatechapter")]
    Task<WxaNovelBatchCreateChapterResponse> BatchCreateChapterAsync([Body] WxaNovelBatchCreateChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>删除作品编辑版里的章节。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/book/deletechapter")]
    Task<WxaResponse> DeleteChapterAsync([Body] WxaNovelDeleteChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>替换作品编辑版里的已有章节内容，会生成新的章节 ID。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>新章节 ID。</returns>
    [Post("/wxa/book/replacechapter")]
    Task<WxaNovelReplaceChapterResponse> ReplaceChapterAsync([Body] WxaNovelReplaceChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取章节信息列表，区分编辑版和发布版。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>章节列表。</returns>
    [Post("/wxa/book/listchapter")]
    Task<WxaNovelListChapterResponse> ListChapterAsync([Body] WxaNovelListChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取章节详细信息，区分编辑版和发布版。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>章节信息。</returns>
    [Post("/wxa/book/getchapter")]
    Task<WxaNovelGetChapterResponse> GetChapterAsync([Body] WxaNovelGetChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>调整作品编辑版信息中的章节顺序（仅当章节排序方式为"追加"时生效）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/book/reorderchapter")]
    Task<WxaResponse> ReorderChapterAsync([Body] WxaNovelReorderChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>调整作品编辑版信息中的章节相对顺序（仅当章节排序方式为"seq 递增"时生效）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [Post("/wxa/book/updatechapterseq")]
    Task<WxaResponse> UpdateChapterSeqAsync([Body] WxaNovelUpdateChapterSeqRequest request, CancellationToken cancellationToken = default);
}
