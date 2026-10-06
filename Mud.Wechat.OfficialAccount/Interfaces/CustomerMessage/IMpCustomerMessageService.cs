// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「客服消息 → 客服消息」子分组 SDK（3 端点：发送客服消息 + 客服输入状态 + 获取聊天记录）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/subscription/api/"/>
/// → 客服消息 → 客服消息。深链均为逐页核验过的真实页面。
/// </para>
/// <para>
/// <b>本子分组的核心业务约束（官方明文，SDK 用户最常踩的坑）</b>：
/// </para>
/// <list type="table">
/// <item><description><b>下发额度</b>：用户发消息 → <b>5 条 / 48 小时</b>；点击自定义菜单 → 3 条 / 1 分钟；
/// 关注公众号 → 3 条 / 1 分钟；扫描二维码 → 3 条 / 1 分钟</description></item>
/// <item><description><b>菜单触发面极窄</b>：仅 <c>click</c>、<c>scancode_push</c>、<c>scancode_waitmsg</c>
/// 三类菜单会触发客服接口（其余菜单类型<b>不产生</b>下发额度）</description></item>
/// <item><description><b>输入状态前置条件</b>：需 30 秒内与该用户有过消息交互，且不可重复下发</description></item>
/// <item><description><b>聊天记录限制</b>：查询时间段 ≤ 24 小时、每次 ≤ 10000 条；
/// 且需已开通/升级新版客服功能（否则 <c>65400</c>）</description></item>
/// </list>
/// <para>
/// <b>账号适用性与「服务号专属」的澄清（重要）</b>：官方本子分组三端点适用范围均为
/// 「公众号 —— 仅认证 / 服务号 —— 仅认证」（即<b>认证的订阅号亦可调用</b>），
/// <b>并非服务号专属</b> ⇒ 本 SDK <b>不引入</b> <c>MpAccountType</c> 配置项与本地能力闸
/// （无真实消费点；账号类型不符时官方以 <c>40200 invalid account type</c> / <c>48001</c> 明确表达）。
/// </para>
/// <para>
/// <b>令牌路由</b>：3 端点均消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入 <c>access_token</c>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CustomerMessage", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpCustomerMessageService
{
    /// <summary>
    /// 发送客服消息。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/message/api_sendcustommessage.html"/>
    /// （官方接口英文名 <c>sendCustomMessage</c>）。
    /// </summary>
    /// <param name="request">消息体（<c>touser</c> + <c>msgtype</c> + 对应的分支对象）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>下发额度（官方明文）</b>：用户发送消息 → <b>5 条 / 48 小时</b>；点击自定义菜单 → 3 条 / 1 分钟；
    /// 关注公众号 → 3 条 / 1 分钟；扫描二维码 → 3 条 / 1 分钟。且<b>仅</b> <c>click</c>、
    /// <c>scancode_push</c>、<c>scancode_waitmsg</c> 三类菜单会触发客服接口。
    /// </para>
    /// <para>
    /// <b>各分支字段（官方分支字段表）</b>：<c>text.content</c>；<c>image/voice/mpnews</c> 仅 <c>media_id</c>；
    /// <c>video</c> 需 <c>media_id</c> + <c>thumb_media_id</c>（<c>title</c>/<c>description</c> 可选）；
    /// <c>music</c> 需 <c>title</c> + <c>description</c> + <c>musicurl</c> + <c>thumb_media_id</c>；
    /// <c>news.articles</c>（<b>1 条以内</b>）；<c>mpnewsarticle.article_id</c>；
    /// <c>msgmenu.{head_content?, list[{id,content}], tail_content?}</c>；<c>wxcard.card_id</c>；
    /// <c>miniprogrampage.{title, appid, pagepath, thumb_media_id}</c>。
    /// </para>
    /// <para>
    /// <b>官方文档内部不一致</b>：该页 <c>msgtype</c> 说明只列 4 种类型（text/image/link/miniprogrampage），
    /// 但分支字段表覆盖 10+ 分支 ⇒ SDK 以分支字段表为准（常量见 <c>MpCustomMessageTypes</c>）。
    /// </para>
    /// <para>
    /// <b>已弃用分支</b>：<c>mpnews</c> 官方标注「草稿灰度完成后不再支持」⇒ 应改用
    /// <c>mpnewsarticle</c>（<c>article_id</c> 来自「发布」系列接口）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40013</c> / <c>45008</c>（图文条数超限）/
    /// <c>70000</c>（为保护未成年人权益，该条消息发送失败）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/custom/send")]
    Task<MpResponse> SendCustomMessageAsync(
        [Body] MpSendCustomMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 客服输入状态。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/message/api_typing.html"/>
    /// （官方接口英文名 <c>typing</c>）。
    /// </summary>
    /// <param name="request">输入状态请求（<c>touser</c> + <c>command</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>硬性前置条件（官方明文）</b>：下发输入状态需<b>之前 30 秒内与该用户有过消息交互</b>
    /// （否则 <c>45080 need sending message to user or recving message from user in the last 30 seconds
    /// before typing</c>）；已处于输入状态时<b>不可重复下发</b>（<c>45081 you are already typing</c>）。
    /// </para>
    /// <para>
    /// <b>路径差异</b>：本接口对公众号 / 服务号为 <c>/cgi-bin/message/custom/typing</c>；
    /// 小程序 / 小游戏为 <c>/cgi-bin/message/custom/business/typing</c>（本 SDK 仅覆盖公众号形态）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40200</c>（账号类型不符）/ <c>45072</c>（command 取值不对）/
    /// <c>45080</c> / <c>45081</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/custom/typing")]
    Task<MpResponse> SetTypingAsync(
        [Body] MpTypingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取聊天记录。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/message/api_getmsglist.html"/>
    /// （官方接口英文名 <c>getMsgList</c>）。
    /// </summary>
    /// <param name="request">查询区间与分页（<c>starttime</c>/<c>endtime</c>/<c>msgid</c>/<c>number</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>聊天记录分页（<c>recordlist</c> + <c>number</c> + <c>msgid</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方路径无 <c>/cgi-bin</c> 前缀</b>：<c>POST /customservice/msgrecord/getmsglist</c>
    /// （与其他端点形态不同，勿「顺手补齐」前缀）。
    /// </para>
    /// <para>
    /// <b>官方文档缺陷</b>：该页字段区标注「请求体：无」，但请求示例携带 4 个字段 ⇒ SDK 按示例建模
    /// （见 <c>MpGetMsgListRequest</c> remarks）。
    /// </para>
    /// <para>
    /// <b>上限（官方明文）</b>：查询时间段<b>不能超过 24 小时</b>；每次最多获取 <b>10000 条</b>记录。
    /// </para>
    /// <para>
    /// 官方错误码：<c>65400</c>（没有开通或升级到新版客服功能）/ <c>65416</c>（查询参数不合法）/
    /// <c>65417</c>（查询时间段超出限制）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/customservice/msgrecord/getmsglist")]
    Task<MpGetMsgListResponse> GetMsgListAsync(
        [Body] MpGetMsgListRequest request,
        CancellationToken cancellationToken = default);
}
