// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「留言管理」域 SDK（8 端点；需<b>留言功能权限</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 → 留言管理（leaving/ 前缀 8 页；2026-10-07 逐页核验，
/// subscription 订阅号域命中）。适用范围：公众号 / 服务号 —— <b>仅认证</b>。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>留言权限前置</b>：官方原文「公众号需具备留言功能权限」——无权限返回 <c>88000</c>
/// （属账号配置态问题，非调用错误；SDK 不做本地拦截）。</item>
/// <item><b>寻址形态</b>：8 端点均以 <c>msg_data_id</c>（群发返回的数据 ID）+ <c>index</c>（多图文序号，
/// 从 0 开始，不带默认第一篇）定位文章——<b>非</b> article_id 形态（方案文档预判已被核验修正）。</item>
/// <item><b>共享请求 DTO（N4 裁决）</b>：open/close 共用 <see cref="MpCommentOpenRequest"/>；
/// markelect/unmarkelect/delete/reply-delete 共用 <see cref="MpCommentRefRequest"/>（扁平三字段——
/// 方案文档预判的「嵌套 user_comment 对象」不存在）；reply/add 为扁平四字段 + content。</item>
/// <item><b>评论列表上限</b>：count「&gt;=50 会被拒绝」（88010；官方描述原文拼写「cout」照录）。</item>
/// <item><b>第三方平台</b>：8 页均支持代调用（权限集 7）——按 M0-R3 裁决只在 XML 记录。</item>
/// </list>
/// <para><b>令牌路由</b>：全部端点 Query 注入 <c>access_token</c>（MUD005 已知接受风险）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Comment", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpCommentService
{
    /// <summary>
    /// 打开已群发文章的评论。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/leaving/api_openarticlecomment.html"/>
    /// （官方接口英文名 <c>openArticleComment</c>）。
    /// </summary>
    /// <param name="request">定位请求（msg_data_id + index）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>45009</c> / <c>88000</c>（没有留言权限）/
    /// <c>88001</c>（图文不存在）/ <c>88002</c>（文章存在敏感信息）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/comment/open")]
    Task<MpResponse> OpenCommentAsync(
        [Body] MpCommentOpenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭已群发文章的评论。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/leaving/api_closecomment.html"/>
    /// （官方接口英文名 <c>closecomment</c>）。
    /// </summary>
    /// <param name="request">定位请求（与 open 共用 DTO——字段集一致）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方错误码表仅列 <c>-1</c> / <c>40001</c>（与 open 页对偶但错误码表明显更短，疑官方未列全——照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/comment/close")]
    Task<MpResponse> CloseCommentAsync(
        [Body] MpCommentOpenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查看指定文章的评论数据。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/leaving/api_listcomment.html"/>
    /// （官方接口英文名 <c>listComment</c>）。
    /// </summary>
    /// <param name="request">查询请求（msg_data_id/index/begin/count（&lt;50）/type）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>评论分页（<c>total</c> + <c>comment</c> 列表，含回复信息）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；<c>type</c> = 0 全部 / 1 普通 / 2 精选；count &gt;=50 拒绝（88010）。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>45009</c> / <c>88000</c> / <c>88010</c>
    /// （count range error，官方描述原文拼写「cout」照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/comment/list")]
    Task<MpCommentListResponse> ListCommentsAsync(
        [Body] MpCommentListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将评论标记精选。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/leaving/api_electcomment.html"/>
    /// （官方接口英文名 <c>electcomment</c>）。
    /// </summary>
    /// <param name="request">评论定位（msg_data_id/index/user_comment_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方错误码：<c>45009</c> / <c>88000</c> / <c>88001</c> / <c>88003</c>（精选评论数已达上限）/
    /// <c>88004</c>（已被用户删除，无法精选）/ <c>88008</c>（该评论不存在）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/comment/markelect")]
    Task<MpResponse> MarkCommentElectAsync(
        [Body] MpCommentRefRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将评论取消精选。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/leaving/api_unelectcomment.html"/>
    /// （官方接口英文名 <c>unelectcomment</c>）。
    /// </summary>
    /// <param name="request">评论定位（与 markelect/delete 共用 DTO——字段集一致）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>官方错误码：<c>45009</c> / <c>88000</c> / <c>88001</c> / <c>88008</c>。</remarks>
    [Post("/cgi-bin/comment/unmarkelect")]
    Task<MpResponse> UnmarkCommentElectAsync(
        [Body] MpCommentRefRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除评论。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/leaving/api_delcomment.html"/>
    /// （官方接口英文名 <c>delcomment</c>）。
    /// </summary>
    /// <param name="request">评论定位（与 markelect/unmarkelect 共用 DTO——字段集一致）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>官方错误码：<c>45009</c> / <c>88000</c> / <c>88001</c> / <c>88008</c>。</remarks>
    [Post("/cgi-bin/comment/delete")]
    Task<MpResponse> DeleteCommentAsync(
        [Body] MpCommentRefRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 回复评论。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/leaving/api_replycomment.html"/>
    /// （官方接口英文名 <c>replycomment</c>；路径 <c>comment/reply/add</c>）。
    /// </summary>
    /// <param name="request">回复请求（定位三字段 + content）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方错误码：<c>45009</c> / <c>88000</c> / <c>88001</c> / <c>88005</c>（已经回复过了）/
    /// <c>88007</c>（回复超过长度限制或为空——官方文案「或为 0」照录）/ <c>88008</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/comment/reply/add")]
    Task<MpResponse> ReplyCommentAsync(
        [Body] MpCommentReplyAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除评论的回复内容。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/leaving/api_delreplycomment.html"/>
    /// （官方接口英文名 <c>delreplycomment</c>；路径 <c>comment/reply/delete</c>）。
    /// </summary>
    /// <param name="request">评论定位（与 markelect/unmarkelect/delete 共用 DTO——字段集一致）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方错误码：<c>45009</c> / <c>87009</c>（invalid signature——本接口无签名参数，疑官方全局码表残留，照录）/
    /// <c>88000</c> / <c>88001</c> / <c>88008</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/comment/reply/delete")]
    Task<MpResponse> DeleteCommentReplyAsync(
        [Body] MpCommentRefRequest request,
        CancellationToken cancellationToken = default);
}
