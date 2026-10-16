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
/// <para>官方文档索引：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html"/>（「小说作品管理接口」全集 23 支，2026-10-16 逐行核验本域 20 支路由与索引表逐字一致）。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 <c>access_token</c>，Query 注入）。</para>
/// <para><b>形态口径</b>：本域 20 支官方一律 <b>POST + JSON 请求体</b>、<b>零业务 Query 参数</b>；官方<b>未给出端点级频率数字</b>（只有规模上限：作品/章节列表分页、批量上传单批条数等，见各 DTO 属性注释）⇒ 不得凭第三方转述补数。</para>
/// <para><b>编辑版 vs 发布版</b>：<c>updatebook</c> / <c>listbook</c> / <c>getbook</c> 的 <c>need_edited_data</c> 与
/// <c>auditbook</c> 构成「编辑 → 提审 → 发布」两段式；<c>deletebook</c> 官方明写「同时作用于编辑版和发布版」⇒ 不可逆。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelBookService
{
    /// <summary>创建作品（小说）。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelCreateBookRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>作品 ID，见 <see cref="WxaNovelCreateBookResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_createbook.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/createbook</c>；请求体为 JSON 对象，无 Query 参数、无端点级频率数字。</para>
    /// </remarks>
    [Post("/wxa/book/createbook")]
    Task<WxaNovelCreateBookResponse> CreateBookAsync([Body] WxaNovelCreateBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>编辑作品编辑版的基础信息、章节顺序、分卷信息。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelUpdateBookRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_updatebook.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/updatebook</c>；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</para>
    /// <para><b>只作用编辑版</b>：改动须再走 <see cref="AuditBookAsync"/> 提审才会进入发布版。</para>
    /// </remarks>
    [Post("/wxa/book/updatebook")]
    Task<WxaResponse> UpdateBookAsync([Body] WxaNovelUpdateBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>删除作品，同时作用于编辑版和发布版。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelDeleteBookRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_deletebook.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/deletebook</c>；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</para>
    /// <para><b>不可逆</b>：官方明写删除同时作用于编辑版与发布版（连带章节），不得当「下架」理解。</para>
    /// </remarks>
    [Post("/wxa/book/deletebook")]
    Task<WxaResponse> DeleteBookAsync([Body] WxaNovelDeleteBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取所有作品列表，分为发布版列表和编辑版列表。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelListBookRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>作品列表，见 <see cref="WxaNovelListBookResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_listbook.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/listbook</c>；分页是 <c>limit</c> / <c>offset</c> 与游标 <c>last_id</c> 两套并存 ⇒ 照页传参，不得只留一种。</para>
    /// </remarks>
    [Post("/wxa/book/listbook")]
    Task<WxaNovelListBookResponse> ListBookAsync([Body] WxaNovelListBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>获取作品详细信息，区分发布版和编辑版。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelGetBookRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>作品信息，见 <see cref="WxaNovelGetBookResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_getbook.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/getbook</c>；定位键 <c>book_id</c> 与 <c>original_id</c>（服务商自有 ID）两种皆可 ⇒ 请求体二选一，非同时必填。</para>
    /// </remarks>
    [Post("/wxa/book/getbook")]
    Task<WxaNovelGetBookResponse> GetBookAsync([Body] WxaNovelGetBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>作品提交审核。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelAuditBookRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_auditbook.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/auditbook</c>；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ <b>提审结果须回 <see cref="GetBookAsync"/> 读 <c>audit_info</c></b>，不得把受理当审核通过。</para>
    /// </remarks>
    [Post("/wxa/book/auditbook")]
    Task<WxaResponse> AuditBookAsync([Body] WxaNovelAuditBookRequest request, CancellationToken cancellationToken = default);
}
