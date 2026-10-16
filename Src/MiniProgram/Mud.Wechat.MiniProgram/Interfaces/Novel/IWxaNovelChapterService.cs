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
/// <para>官方文档索引：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html"/>（本域 8 支章节端点，2026-10-16 逐行核验路由与索引表逐字一致）。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 <c>access_token</c>，Query 注入）。</para>
/// <para><b>形态口径</b>：8 支官方一律 <b>POST + JSON 请求体</b>、<b>零业务 Query 参数</b>；官方<b>未给出端点级频率数字</b>，只给规模限制（单次批量上传章数上限、章节内容长度上限等）⇒ 不得凭第三方转述补数。</para>
/// <para><b>一律作用编辑版</b>：章节的增删改须再走作品提审才会进入发布版；两支排序端点（<c>reorderchapter</c> / <c>updatechapterseq</c>）按作品 <c>chapter_sort_type</c> 二分生效，互斥。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelChapterService
{
    /// <summary>上传章节到作品编辑版信息里。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelCreateChapterRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>章节 ID，见 <see cref="WxaNovelCreateChapterResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_createchapter.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/createchapter</c>；单章上传，章节正文由 <c>chapter.content</c> 内联承载。</para>
    /// </remarks>
    [Post("/wxa/book/createchapter")]
    Task<WxaNovelCreateChapterResponse> CreateChapterAsync([Body] WxaNovelCreateChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>批量上传章节到作品编辑版信息里，单次最多 10 章。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelBatchCreateChapterRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>章节 ID 列表与冲突的提供方主键列表，见 <see cref="WxaNovelBatchCreateChapterResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_batchcreatechapter.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/batchcreatechapter</c>；<c>chapter_list</c> 单次上限 10 章（照页传参，不得按「批量」想象放大）。</para>
    /// <para><b>部分成功语义</b>：应答同时返回 <c>chapter_id_list</c> 与 <c>conflict_original_id_list</c>（服务商自有 ID 冲突项）⇒ 外层 <c>errcode == 0</c> 不等于全部落库，须逐条核对。</para>
    /// </remarks>
    [Post("/wxa/book/batchcreatechapter")]
    Task<WxaNovelBatchCreateChapterResponse> BatchCreateChapterAsync([Body] WxaNovelBatchCreateChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>删除作品编辑版里的章节。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelDeleteChapterRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_deletechapter.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/deletechapter</c>；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</para>
    /// </remarks>
    [Post("/wxa/book/deletechapter")]
    Task<WxaResponse> DeleteChapterAsync([Body] WxaNovelDeleteChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>替换作品编辑版里的已有章节内容，会生成新的章节 ID。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelReplaceChapterRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>新章节 ID，见 <see cref="WxaNovelReplaceChapterResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_replacechapter.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/replacechapter</c>；定位键为旧 <c>chapter_id</c>，应答返回 <c>new_chapter_id</c> ⇒ <b>替换即换 ID</b>，宿主侧缓存的旧章节 ID 同批作废。</para>
    /// </remarks>
    [Post("/wxa/book/replacechapter")]
    Task<WxaNovelReplaceChapterResponse> ReplaceChapterAsync([Body] WxaNovelReplaceChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取章节信息列表，区分编辑版和发布版。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelListChapterRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>章节列表与总数，见 <see cref="WxaNovelListChapterResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_listchapter.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/listchapter</c>；<c>need_edited_data</c> 判别编辑版/发布版，分页为 <c>limit</c> / <c>offset</c>，可按 <c>volume_index</c> 限定分卷。</para>
    /// <para>本端点返回的是章节<b>元信息</b>列表（含 <c>word_cnt</c> / <c>audit_info</c> / <c>ban_status</c>），不含正文；取正文须走 <see cref="GetChapterAsync"/>。</para>
    /// </remarks>
    [Post("/wxa/book/listchapter")]
    Task<WxaNovelListChapterResponse> ListChapterAsync([Body] WxaNovelListChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取章节详细信息，区分编辑版和发布版。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelGetChapterRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>章节信息（含正文），见 <see cref="WxaNovelGetChapterResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_getchapter.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/getchapter</c>；应答为单个 <c>chapter</c> 对象（非列表），正文由该对象的 <c>content</c> 承载。</para>
    /// </remarks>
    [Post("/wxa/book/getchapter")]
    Task<WxaNovelGetChapterResponse> GetChapterAsync([Body] WxaNovelGetChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>调整作品编辑版信息中的章节顺序（仅当章节排序方式为"追加"时生效）。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelReorderChapterRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_reorderchapter.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/reorderchapter</c>；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</para>
    /// <para><b>与 <see cref="UpdateChapterSeqAsync"/> 互斥</b>：本端点用 <c>target_chapter_id</c> + <c>operation</c> 做相对移动，只在「追加」排序作品上生效；<c>seq</c> 递增作品须改走 <see cref="UpdateChapterSeqAsync"/>。</para>
    /// </remarks>
    [Post("/wxa/book/reorderchapter")]
    Task<WxaResponse> ReorderChapterAsync([Body] WxaNovelReorderChapterRequest request, CancellationToken cancellationToken = default);

    /// <summary>调整作品编辑版信息中的章节相对顺序（仅当章节排序方式为"seq 递增"时生效）。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelUpdateChapterSeqRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_updatechapterseq.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/updatechapterseq</c>；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</para>
    /// <para>全量重排语义：<c>chapter_seq_list</c> 一次性给出每章的新 <c>seq</c>，与 <see cref="ReorderChapterAsync"/> 的「单章相对移动」不同形，不得互换调用。</para>
    /// </remarks>
    [Post("/wxa/book/updatechapterseq")]
    Task<WxaResponse> UpdateChapterSeqAsync([Body] WxaNovelUpdateChapterSeqRequest request, CancellationToken cancellationToken = default);
}
