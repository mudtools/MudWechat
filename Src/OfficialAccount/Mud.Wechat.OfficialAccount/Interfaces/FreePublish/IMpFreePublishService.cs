// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「发布能力」域 SDK（5 端点；<c>PUBLISHJOBFINISH</c> 事件未建模——官方 API 页仅文字提及）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 → 发布能力（public/ 前缀 5 页；2026-10-07 逐页核验，
/// subscription 订阅号域命中）。适用范围：公众号 / 服务号 —— <b>仅认证</b>；第三方平台代调用权限集 7。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>提交 ≠ 完成</b>：官方原文「此时只意味着发布任务提交成功，并不意味着此时发布已经完成」——
/// 发布结果经 <c>PUBLISHJOBFINISH</c> 事件推送（官方 API 页仅文字提及事件名与 publish_status 语义，
/// 无独立 XML 报文页 ⇒ 事件暂不建模，见 .docs 实施记录）。</item>
/// <item><b>publish_status 为标量</b>（0 成功 / 1 发布中 / 2 原创失败 / 3 常规失败 / 4 平台审核不通过 /
/// 5 成功后用户删除所有文章 / 6 成功后系统封禁所有文章）——方案文档「数组」预判已被核验修正。</item>
/// <item><b>id 形态</b>：发布走 publish_id；发布成功的文章以 article_id 寻址（与草稿 media_id、群发
/// msg_data_id 三套 id 并存，勿混用）。</item>
/// <item><b>列表上限</b>：freepublish/batchget 的 count 官方原文「取值在 1 到 20 之间」
/// （<b>非</b>方案文档预判的 ≤100，已双验）；响应条目键为 <c>article_id</c>（非 item_id）。</item>
/// </list>
/// <para><b>令牌路由</b>：全部端点 Query 注入 <c>access_token</c>（MUD005 已知接受风险）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "FreePublish", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpFreePublishService
{
    /// <summary>
    /// 发布草稿。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublish_submit.html"/>
    /// （官方接口英文名 <c>freepublish_submit</c>）。
    /// </summary>
    /// <param name="request">发布请求（<c>media_id</c> 为草稿 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提交结果（<c>publish_id</c>；<c>msg_data_id</c> 官方返回表有、示例缺失，矛盾照录）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"media_id":…}</c>。</para>
    /// <para>官方错误码：<c>48001</c> / <c>53503</c>（该草稿未通过发布检查）/ <c>53504</c>（需前往公众平台官网使用草稿）/
    /// <c>53505</c>（请手动保存成功后再发表）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/freepublish/submit")]
    Task<MpFreePublishSubmitResponse> SubmitAsync(
        [Body] MpFreePublishSubmitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询发布任务状态。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublish_get.html"/>
    /// （官方接口英文名 <c>freepublish_get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>publish_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发布状态（<c>publish_status</c> 0~6 标量 + 成功时的 article_id / article_detail / fail_idx）。</returns>
    /// <remarks>官方错误码：<c>40001</c> / <c>40002</c>（invalid argument）/ <c>48001</c>。</remarks>
    [Post("/cgi-bin/freepublish/get")]
    Task<MpFreePublishGetResponse> GetPublishStatusAsync(
        [Body] MpFreePublishGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除发布文章（<b>此操作不可逆</b>）。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublishdelete.html"/>
    /// （官方接口英文名 <c>freepublishDelete</c>，官方大小写混杂照录）。
    /// </summary>
    /// <param name="request">删除请求（article_id 必填；index 第一篇编号 1，不填或 0 删除全部）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>官方错误码：<c>0</c> / <c>48001</c>。</remarks>
    [Post("/cgi-bin/freepublish/delete")]
    Task<MpResponse> DeletePublishAsync(
        [Body] MpFreePublishDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过 article_id 获取已发布图文信息。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublishgetarticle.html"/>
    /// （官方接口英文名 <c>freepublishGetarticle</c>）。
    /// </summary>
    /// <param name="request">获取请求（<c>article_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已发布图文（<c>news_item</c>，含 is_deleted 删除标记与临时链接）。</returns>
    /// <remarks>
    /// <para>官方错误码：<c>0</c> / <c>48001</c> / <c>53600</c>（Article ID 无效）。</para>
    /// <para>官方文档矛盾（照录）：请求体说明误写「要获取的草稿的 article_id」；字段表无 show_cover_pic 行、示例却含该字段。</para>
    /// </remarks>
    [Post("/cgi-bin/freepublish/getarticle")]
    Task<MpFreePublishGetArticleResponse> GetArticleAsync(
        [Body] MpFreePublishGetArticleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已成功发布的消息列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublish_batchget.html"/>
    /// （官方接口英文名 <c>freepublish_batchget</c>）。
    /// </summary>
    /// <param name="request">分页请求（offset / count（1~20）/ no_content）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发布分页（<c>total_count</c>/<c>item_count</c>/<c>item</c>，条目键为 article_id）。</returns>
    /// <remarks>官方错误码：<c>0</c> / <c>48001</c>。</remarks>
    [Post("/cgi-bin/freepublish/batchget")]
    Task<MpFreePublishBatchGetResponse> BatchGetPublishedAsync(
        [Body] MpFreePublishBatchGetRequest request,
        CancellationToken cancellationToken = default);
}
