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
/// <para>官方文档索引：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html"/>（本域 3 支「账号-账号」授权端点，2026-10-16 逐页核验参数表与请求/应答示例）。</para>
/// <para>权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 <c>access_token</c>，Query 注入）。</para>
/// <para><b>关系模型</b>：主授权者账号（AppID）→ 被授权者账号（AppID）两维度，不含 <c>book_id</c>；授权后可把主授权账号名下作品批量放开给被授权账号。与 <see cref="IWxaNovelAuthService"/> 的「账号-小说」三维度面<b>不同形</b>。</para>
/// <para><b>分页形态差异是本域的反直觉点</b>：<c>querybookauthv2</c> 用游标（<c>cursor</c> / <c>next_cursor</c>），而 <see cref="IWxaNovelAuthService.QueryBookAuthAsync"/> 用 <c>offset</c> / <c>count</c> ⇒ 两支不得互相套用分页参数。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "Novel", TokenManage = nameof(IMpAppManager))]
public interface IWxaNovelAccountAuthService
{
    /// <summary>增加账号-账号的授权关系数据，一次最多新增 20 条。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelAddBookAuthByAppIdRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>逐条结果，见 <see cref="WxaNovelAddBookAuthByAppIdResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_addbookauthbyappid.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/addbookauthbyappid</c>；请求体是 <c>{"infos":[{grantee_appid, expire_time}]}</c> 数组包装（必填，单批上限 20 条）。</para>
    /// <para>应答外层 <c>errcode</c> / <c>errmsg</c> 之外另有 <c>results[]</c>，其内层逐项 <c>errcode</c> ⇒ <b>受理成功 ≠ 逐条成功</b>；项内不带作品维度键。</para>
    /// </remarks>
    [Post("/wxa/book/addbookauthbyappid")]
    Task<WxaNovelAddBookAuthByAppIdResponse> AddBookAuthByAppIdAsync([Body] WxaNovelAddBookAuthByAppIdRequest request, CancellationToken cancellationToken = default);

    /// <summary>查看账号的授权关系列表或被授权关系列表（游标分页，上限 100 条/次）。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelQueryBookAuthV2Request"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>被授权方列表与下一页游标，见 <see cref="WxaNovelQueryBookAuthV2Response"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_querybookauthv2.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/querybookauthv2</c>；<c>count</c> 必填且单页上限 100，<c>cursor</c> 空串即首页、后续页回填应答 <c>next_cursor</c>；结果按授权到期时间从早到晚排序。</para>
    /// <para><b>两种查询模式互斥</b>：<c>book_ids</c>（查指定作品，上限 30 条）优先级高于 <c>grantor_appid</c>，官方明写 <c>book_ids</c> 非空时 <c>grantor_appid</c> 失效 ⇒ 不得同时指望两者共同收窄。</para>
    /// </remarks>
    [Post("/wxa/book/querybookauthv2")]
    Task<WxaNovelQueryBookAuthV2Response> QueryBookAuthV2Async([Body] WxaNovelQueryBookAuthV2Request request, CancellationToken cancellationToken = default);

    /// <summary>删除账号维度授权关系（主授权账号使用）。</summary>
    /// <param name="request">请求体，字段见 <see cref="WxaNovelDelBookAuthByAppIdRequest"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_delbookauthbyappid.html"/>。</para>
    /// <para>官方契约：<b>POST</b> <c>/wxa/book/delbookauthbyappid</c>；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</para>
    /// <para>请求体只有 <c>grantee_appid</c> 一支键（单对象、非数组、无 <c>book_id</c>）⇒ 语义是「断开该账号的整体授权」，比 <see cref="IWxaNovelAuthService.DelBookAuthAsync"/> 更宽，调用前须确认影响面。</para>
    /// </remarks>
    [Post("/wxa/book/delbookauthbyappid")]
    Task<WxaResponse> DelBookAuthByAppIdAsync([Body] WxaNovelDelBookAuthByAppIdRequest request, CancellationToken cancellationToken = default);
}
