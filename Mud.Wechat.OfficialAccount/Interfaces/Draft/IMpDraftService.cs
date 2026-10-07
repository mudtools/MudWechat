// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「草稿管理」域 SDK（6 端点；<c>draft/switch</c> 官方已废弃，不实现）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 → 草稿管理（draftbox/draftmanage/ 前缀 6 页；2026-10-07 逐页核验，
/// subscription 订阅号域命中）。适用范围：公众号 ✔ / 服务号 ✔（非仅认证）。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>生命周期</b>：官方原文「上传到草稿箱中的素材被<b>群发或发布</b>后，该素材将从草稿箱中移除」
/// ——群发/发布成功后 media_id 失效。</item>
/// <item><b>双形态不一致（照录）</b>：draft_add 的 <c>articles</c> 为数组、draft_update 的为单对象
/// ——SDK 按各自页面建模（<see cref="MpDraftAddRequest.Articles"/> / <see cref="MpDraftUpdateRequest.Articles"/>）。</item>
/// <item><b>文章类型</b>：<c>article_type</c> = news（图文消息，默认）/ newspic（图片消息；
/// image_info 必填、最多 20 张、首张为封面；content 仅支持纯文本与部分特殊功能标签如商品，商品 ≤ 50 个）。</item>
/// <item><b>content 图片来源</b>：官方原文「涉及图片 url 必须来源『上传图文消息内的图片获取URL』接口获取，
/// 外部图片 url 将被过滤」（即 <see cref="IMpMediaService.UploadImageAsync"/>）。</item>
/// <item><b>列表上限</b>：draft/batchget 的 count 取值 1~20；官方页面无「草稿总数上限」与「单篇图文 8 条上限」表述
/// （后者仅第三方转述，SDK 不编造）。</item>
/// <item><b>第三方平台</b>：6 页均支持代调用（权限集 11、100）——按 M0-R3 裁决只在 XML 记录。</item>
/// </list>
/// <para><b>令牌路由</b>：全部端点 Query 注入 <c>access_token</c>（MUD005 已知接受风险）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Draft", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpDraftService
{
    /// <summary>
    /// 新建草稿。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_add.html"/>
    /// （官方接口英文名 <c>draft_add</c>）。
    /// </summary>
    /// <param name="request">草稿请求（<c>articles</c> 数组；字段约束见 <see cref="DataModels.Draft.MpDraftArticle"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>草稿 media_id（≤128 字符）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"articles":[…]}</c>。</para>
    /// <para>官方错误码：<c>53404</c>（账号已被限制带货能力）/ <c>53405</c>（插入商品信息有误）/
    /// <c>53406</c>（请先开通带货能力）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/draft/add")]
    Task<MpDraftAddResponse> AddDraftAsync(
        [Body] MpDraftAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改草稿。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_update.html"/>
    /// （官方接口英文名 <c>draft_update</c>）。
    /// </summary>
    /// <param name="request">更新请求（media_id + index + 单对象 articles——与 add 页数组形态不同，照官方页面）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；<c>index</c> 为要更新的文章位置（多图文时有意义，第一篇为 0）。</para>
    /// <para>官方错误码：<c>40114</c>（invalid index value）/ <c>41039</c>（invalid content_source_url）/
    /// <c>45166</c>（invalid content）/ <c>53404~53406</c> / <c>88000</c>（官方解决方案列多行留空，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/draft/update")]
    Task<MpResponse> UpdateDraftAsync(
        [Body] MpDraftUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取草稿详情。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_getdraft.html"/>
    /// （官方接口英文名 <c>getDraft</c>）。
    /// </summary>
    /// <param name="request">获取请求（<c>media_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>草稿详情（<c>news_item</c>；含草稿临时链接 url）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"media_id":…}</c>。</para>
    /// <para>官方错误码：<c>40007</c>（invalid media_id，本页仅此一行）。</para>
    /// <para>官方文档矛盾（照录）：响应示例含 show_cover_pic 但字段表无该行——SDK 按字段表建模。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/draft/get")]
    Task<MpDraftGetResponse> GetDraftAsync(
        [Body] MpDraftGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除草稿。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_delete.html"/>
    /// （官方接口英文名 <c>draft_delete</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>media_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"media_id":…}</c>；<b>此操作不可逆</b>（官方注意事项原文「此操作无法撤销，请谨慎操作」）。</para>
    /// <para>官方错误码：<c>-1</c> / <c>0</c> / <c>40007</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/draft/delete")]
    Task<MpResponse> DeleteDraftAsync(
        [Body] MpDraftDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取草稿总数。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_count.html"/>
    /// （官方接口英文名 <c>draft_count</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>草稿总数（<c>total_count</c>；只统计数量不返回内容）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>、无请求体。</para>
    /// <para>官方错误码表仅列 <c>0</c>（官方两列文案错位，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/draft/count")]
    Task<MpDraftCountResponse> GetDraftCountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取草稿列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_batchget.html"/>
    /// （官方接口英文名 <c>draft_batchget</c>）。
    /// </summary>
    /// <param name="request">分页请求（offset / count（1~20）/ no_content）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>草稿分页（<c>total_count</c>/<c>item_count</c>/<c>item</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"offset":…,"count":…,"no_content":…}</c>；
    /// <c>no_content</c> = 1 表示不返回 content 字段（0 正常返回，默认 0）。
    /// </para>
    /// <para>官方错误码表仅列 <c>0</c>（官方两列文案错位，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/draft/batchget")]
    Task<MpDraftBatchGetResponse> BatchGetDraftsAsync(
        [Body] MpDraftBatchGetRequest request,
        CancellationToken cancellationToken = default);
}
