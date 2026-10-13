// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「交易保障 · 评价管理」域 SDK（9 端点：评价列表 / 详情 / 评论回复列表 3 查 +
/// 评论增删 2 + 回复增删 2 + 确认和解 + 重置 API 客服 quota，官方「交易保障」分组 → 评价管理，
/// 路由族 <c>/wxaapi/comment/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三层对象模型（官方语义，决定端点定位键）</b>：官方把该域对象分为
/// <b>评价</b>（<c>commentId</c>，用户对订单的打分）→ <b>评论</b>（<c>replyId</c>，评价下的第一条内容）
/// → <b>回复</b>（<c>commentReplyId</c>，评论之后的所有追加内容）。因此
/// <c>reply/*</c> 两端点操作「评论」、<c>commentreply/*</c> 两端点操作「回复」，
/// 且 <c>replyandcommentreplylist/get</c> 一次返回两层 —— 命名易混，
/// 本 SDK 的方法名与请求 DTO 名一律<b>照官方路由段</b>命名，不做「语义更正」。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>；第三方平台代商家调用为<b>权限集 id 151</b>（9 端点逐页一致），
/// 以 <c>authorizer_access_token</c> 代调用。适用范围均为「小程序」账号类型。
/// </para>
/// <para>
/// <b>GET 端点的「请求体」表建模裁决</b>：本域 3 个查询端点官方动词为 GET，却在页面给出
/// 「请求体 Request Payload」表；官方<b>代码示例</b>一致地以查询串发送
/// （如 <c>mpcommentlist/get?filterType=1&amp;offset=0&amp;limit=8&amp;startTime=...&amp;endTime=...&amp;access_token=xxx</c>）
/// ⇒ 本 SDK 按示例落地为 <c>[Query]</c>，不为 GET 声明请求体（全仓零 GET + <c>[Body]</c> 先例）。
/// </para>
/// <para>
/// <b>本域公共错误码（9 端点逐页同一张表，照录）</b>：<c>-10403</c> 无权限操作该评价/评论/回复、
/// <c>-10008</c> 服务异常、<c>-10007</c> 无权限调用接口、<c>-10003</c> 参数不匹配、
/// <c>-10001</c> 参数异常、<c>10000</c> 和解场景为「不是差评 / 发送过模板消息」，其它场景为参数异常。
/// <b>注意本域错误码为负数族</b>，与全局 <c>4xxxxx</c> 令牌失效码不同源，
/// 不属令牌自愈触发集。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "TransactionGuarantee", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaTradeCommentService
{
    /// <summary>
    /// 查询评价列表（<c>GET /wxaapi/comment/mpcommentlist/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getccommentlist.html"/>。
    /// </summary>
    /// <param name="startTime">查询时间段开始时间（官方参数表类型 string，秒级时间戳字符串，必填）。</param>
    /// <param name="endTime">查询时间段结束时间（官方参数表类型 string，秒级时间戳字符串，必填）。</param>
    /// <param name="filterType">过滤的数据类型（选填；取值见 <see cref="DataModels.TransactionGuarantee.WxaCommentFilterType"/>）。</param>
    /// <param name="offset">偏移数（选填，官方原文「默认值为 0 代表首页拉取」）。</param>
    /// <param name="limit">每页数量（选填，官方原文「默认值为 8」）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>评价列表（<c>offset</c> / <c>total</c> / <c>commentList[]</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>；Query 携带 <c>access_token</c> 及本页参数表的五个业务参数（照官方示例以查询串发送）。</para>
    /// <para>
    /// <b>列表与详情的字段类型口径差异（官方原文如此）</b>：本端点 <c>commentList[]</c> 的
    /// <c>amount</c> / <c>score</c> 参数表类型为 <c>number</c>，而「查询评价详情」的
    /// <c>info.content.amount</c> / <c>score</c> 类型为 <c>string</c> ⇒ 两个 DTO 各照本页参数表落地，
    /// 宿主不得假设两端类型一致。
    /// </para>
    /// <para>
    /// <b>媒体互斥（官方原文）</b>：<c>content.media[]</c> 中「视频跟图片只能存在一种，不同时存在；
    /// 图片可以有多张，视频只会有一个」⇒ <c>img</c> 与 <c>video</c>/<c>videoCover</c>/<c>videoDuration</c> 互斥。
    /// </para>
    /// </remarks>
    [Get("/wxaapi/comment/mpcommentlist/get")]
    Task<DataModels.TransactionGuarantee.WxaCommentListResponse> GetCommentListAsync(
        [Query("startTime")] string startTime,
        [Query("endTime")] string endTime,
        [Query("filterType")] long? filterType = null,
        [Query("offset")] long? offset = null,
        [Query("limit")] long? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询评价详情（<c>GET /wxaapi/comment/commentinfo/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getcommentinfo.html"/>。
    /// </summary>
    /// <param name="commentId">评价 id（官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>评价详情（<c>info</c> / <c>processInfo</c> / <c>oldComment</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>；Query 携带 <c>access_token</c> 与 <c>commentId</c>（官方示例 <c>?commentId=123456&amp;access_token=xxx</c>）。</para>
    /// <para>
    /// <b>两个可选块的官方限定（照录）</b>：<c>processInfo</c>「只有差评才会有」——
    /// 其 <c>actionList[]</c> 为进度节点（<c>type</c>：<c>1</c> 发表差评 / <c>2</c> 开发者处理 /
    /// <c>3</c> 用户调研 / <c>4</c> 用户改评），官方定位当前状态的规则是
    /// 「<b>从数组最后往前数，有 <c>updateTime</c> 的就是当前状态</b>」；
    /// <c>oldComment</c>「只有改评的新评价才有」，其内容字段名为 <c>ext</c>（而非列表侧的 <c>txt</c>）。
    /// </para>
    /// </remarks>
    [Get("/wxaapi/comment/commentinfo/get")]
    Task<DataModels.TransactionGuarantee.WxaCommentInfoResponse> GetCommentInfoAsync(
        [Query("commentId")] string commentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询评论列表（<c>GET /wxaapi/comment/replyandcommentreplylist/get</c>，官方一次返回「评论 + 回复」两层）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getcommentreplylist.html"/>。
    /// </summary>
    /// <param name="commentId">评价 id（官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>评论与回复（<c>list.reply</c> + <c>list.commentReplyList[]</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentReplyListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>；Query 携带 <c>access_token</c> 与 <c>commentId</c>（官方示例 <c>?commentId=123456&amp;access_token=xxx</c>）。</para>
    /// <para>
    /// <b><c>list</c> 为对象而非数组（官方原文）</b>：官方把 <c>list</c> 建为单个对象，内含
    /// <c>reply</c>（评论，即「评论回复第一条内容」）与 <c>commentReplyList</c>
    /// （回复，即「评论回复第二条及之后的所有集合」）⇒ 本 SDK 不得把 <c>list</c> 改成列表。
    /// </para>
    /// <para>
    /// <b>第一层的定位键是 <c>replyId</c>、第二层是 <c>commentReplyId</c></b>，
    /// 分别对应 <see cref="AddCommentReplyAsync"/> / <see cref="DeleteCommentReplyAsync"/> 的入参。
    /// </para>
    /// </remarks>
    [Get("/wxaapi/comment/replyandcommentreplylist/get")]
    Task<DataModels.TransactionGuarantee.WxaCommentReplyListResponse> GetReplyAndCommentReplyListAsync(
        [Query("commentId")] string commentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建评论（<c>POST /wxaapi/comment/reply/add</c>，评价下的第一层内容）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_addreply.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>commentId</c> / <c>content</c> 必填），见 <see cref="DataModels.TransactionGuarantee.WxaCommentAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>操作结果（<c>success</c> + <c>errcode</c>/<c>errmsg</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentOpResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON（<c>commentId</c>、<c>content</c>）；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>与 <see cref="AddCommentReplyAsync"/> 的分层区别</b>：本端点在<b>评价</b>下创建<b>评论</b>
    /// （只需 <c>commentId</c>），回复的回复走 <c>commentreply/add</c>（还需 <c>replyId</c>）。
    /// 删除同理：<see cref="DeleteReplyAsync"/> / <see cref="DeleteCommentReplyAsync"/>。
    /// </para>
    /// </remarks>
    [Post("/wxaapi/comment/reply/add")]
    Task<DataModels.TransactionGuarantee.WxaCommentOpResponse> AddReplyAsync(
        [Body] DataModels.TransactionGuarantee.WxaCommentAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除评论（<c>POST /wxaapi/comment/reply/delete</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_deletereply.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>commentId</c> / <c>replyId</c> 必填），见 <see cref="DataModels.TransactionGuarantee.WxaCommentDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>操作结果（<c>success</c> + <c>errcode</c>/<c>errmsg</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentOpResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON（<c>commentId</c>、<c>replyId</c>）；Query 携带 <c>access_token</c>。</para>
    /// <para><b>删除为不可逆操作</b>：官方无回收站/恢复端点，定位键 <c>replyId</c> 须先经查询评论列表取得。</para>
    /// </remarks>
    [Post("/wxaapi/comment/reply/delete")]
    Task<DataModels.TransactionGuarantee.WxaCommentOpResponse> DeleteReplyAsync(
        [Body] DataModels.TransactionGuarantee.WxaCommentDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建回复（<c>POST /wxaapi/comment/commentreply/add</c>，评论下的追加回复）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_addcommentreply.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>commentId</c> / <c>replyId</c> / <c>content</c> 必填），见 <see cref="DataModels.TransactionGuarantee.WxaCommentReplyAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>操作结果（<c>success</c> + <c>errcode</c>/<c>errmsg</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentOpResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON（<c>commentId</c>、<c>replyId</c>、<c>content</c>）；Query 携带 <c>access_token</c>。</para>
    /// <para>本端点创建的对象在官方模型里是「回复」（<c>commentReplyId</c> 标识），归属某条<b>评论</b>（<c>replyId</c>）。</para>
    /// </remarks>
    [Post("/wxaapi/comment/commentreply/add")]
    Task<DataModels.TransactionGuarantee.WxaCommentOpResponse> AddCommentReplyAsync(
        [Body] DataModels.TransactionGuarantee.WxaCommentReplyAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除回复（<c>POST /wxaapi/comment/commentreply/delete</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_deletecommentreply.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>commentId</c> / <c>replyId</c> / <c>commentReplyId</c> 三者必填），见 <see cref="DataModels.TransactionGuarantee.WxaCommentReplyDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>操作结果（<c>success</c> + <c>errcode</c>/<c>errmsg</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentOpResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON（三个 id 全必填）；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>定位键为三层全链（官方原文）</b>：本端点要求同时传 <c>commentId</c>、<c>replyId</c>、
    /// <c>commentReplyId</c>，而非仅最内层 id ⇒ 宿主需保留父层 id，或先调查询评论列表重取。
    /// </para>
    /// </remarks>
    [Post("/wxaapi/comment/commentreply/delete")]
    Task<DataModels.TransactionGuarantee.WxaCommentOpResponse> DeleteCommentReplyAsync(
        [Body] DataModels.TransactionGuarantee.WxaCommentReplyDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 确认和解（<c>POST /wxaapi/comment/confirmcompromise</c>，差评挽回流程的商家动作）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_confirmcompromise.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>commentId</c> / <c>picList</c> / <c>content</c> 必填），见 <see cref="DataModels.TransactionGuarantee.WxaCommentCompromiseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>操作结果（<c>success</c> + <c>errcode</c>/<c>errmsg</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentOpResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON（<c>commentId</c>、<c>picList</c>、<c>content</c>）；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b><c>picList</c> 为 mediaId 集合（官方原文「和解的图片 mediaId 集合，可参考素材管理」）</b>
    /// ⇒ 图片须先上传取得 mediaId，不能直接传 URL。
    /// </para>
    /// <para>
    /// <b>业务前置（由错误码反推，官方原文）</b>：<c>10000</c> 在和解场景的含义是
    /// 「不是差评 / 发送过模板消息」⇒ 仅<b>差评</b>且<b>尚未发起过差评客服会话模板消息</b>的评价可确认和解；
    /// 差评可和解的判定口径亦可用列表侧 <c>extInfo.isAlreadySendTmpl</c> 预判。
    /// </para>
    /// </remarks>
    [Post("/wxaapi/comment/confirmcompromise")]
    Task<DataModels.TransactionGuarantee.WxaCommentOpResponse> ConfirmCompromiseAsync(
        [Body] DataModels.TransactionGuarantee.WxaCommentCompromiseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重置 API 客服 quota（<c>POST /wxaapi/comment/apikfquota/reset</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_resetapikfquota.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>commentId</c> 必填，单字段），见 <see cref="DataModels.TransactionGuarantee.WxaCommentKfQuotaResetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>操作结果（<c>success</c> + <c>errcode</c>/<c>errmsg</c>），见 <see cref="DataModels.TransactionGuarantee.WxaCommentOpResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON（仅 <c>commentId</c>）；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>与差评客服会话的关联</b>：官方把本端点归入评价管理，语义为按评价重置其 API 客服会话额度；
    /// 客服侧额度与消息规则属 Kf 域（<c>AddKfApi()</c>），本 SDK 不跨域推断，调用方须按官方页面语义使用。
    /// </para>
    /// </remarks>
    [Post("/wxaapi/comment/apikfquota/reset")]
    Task<DataModels.TransactionGuarantee.WxaCommentOpResponse> ResetApiKfQuotaAsync(
        [Body] DataModels.TransactionGuarantee.WxaCommentKfQuotaResetRequest request,
        CancellationToken cancellationToken = default);
}
