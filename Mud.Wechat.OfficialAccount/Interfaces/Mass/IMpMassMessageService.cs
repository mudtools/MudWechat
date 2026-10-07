// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「群发消息」域 SDK（7 端点；<c>uploadimg</c> 归素材域、<c>uploadnews</c> 已废弃不建模）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 → 群发消息（notify/message/ 前缀 8 页；其中 uploadimg 与素材域
/// 同端点只建模一次、uploadnews 官方标注「该能力已更新为草稿箱」按废弃迁移指引处理不建模——N5 裁决）。
/// 深链均为逐页核验过的真实页面（2026-10-07；mass/send 与 qrcode/create 两页在服务号域命中，
/// 其余在订阅号域命中）。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>归属差异</b>：<c>mass/sendall</c> / <c>preview</c> / delete / get / speed 双端点适用范围为
/// 「公众号 / 服务号 —— 仅认证」；<c>mass/send</c> 为<b>服务号专属</b>（官方原文「本接口支持
/// 『服务号（仅认证）』账号类型调用」）——SDK 不做本地拦截，官方 48001 表达（N1 裁决）。</item>
/// <item><b>提交 ≠ 完成</b>：官方原文「在返回成功时，意味着群发任务提交成功，并不意味着此时群发已经结束」
/// ——最终结果经回调事件 <c>masssendjobfinish</c>（MASSSENDJOBFINISH）异步推送（Status 含
/// send success / send fail / err(num) 审核失败码族）。</item>
/// <item><b>原创校验</b>：图文群发前进行原创校验；<c>send_ignore_reprint</c>（默认 0）控制被判为转载时
/// 是否继续群发（官方两页参数表形态不一致，已在 DTO remarks 照录）。</item>
/// <item><b>防重</b>：<c>clientmsgid</c>（≤32 字节）24 小时内防重；45065 返回已存在群发任务的 msgid。</item>
/// <item><b>media_id 生命周期</b>：官方原文「如果 media_id 是通过草稿箱/新建草稿接口来得到的，
/// 那么在群发成功后 media_id 会失效，后台草稿也会被自动删除」。</item>
/// <item><b>频次（官方群发指南页原文，2026-10-07 核验）</b>：「群发接口<b>每分钟限制请求 60 次</b>，
/// 超过限制的请求会被拒绝」；用户侧接收限制「<b>用户每月只能接收 4 条</b>群发消息，多于 4 条的群发
/// 将对该用户发送失败」（认证服务号；无论公众平台网站或接口群发）。API 页本身无次数上限数值，
/// 45028「当前周期内的发表次数已用完」由官方表达。</item>
/// <item><b>API 群发安全保护</b>：官方原文「群发全部用户后需要等待管理员进行确认，如管理员拒绝或
/// 30 分钟内没有确认，该次群发失败」（错误码 89504/89505）。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Mass", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpMassMessageService
{
    /// <summary>
    /// 根据标签群发消息。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/notify/message/api_sendall.html"/>
    /// （官方接口英文名 <c>sendAll</c>）。
    /// </summary>
    /// <param name="request">群发请求（filter + msgtype + 分支对象其一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提交结果（<c>msg_id</c>；<c>msg_data_id</c> 仅图文群发时出现）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；请求体字段见 <see cref="MpMassSendAllRequest"/>
    /// （六个分支 DTO 参照客服消息域「分支 DTO 复用」裁决——N4）。
    /// </para>
    /// <para>
    /// 官方错误码（节选，全表见官方页）：<c>40001</c> / <c>40007</c> / <c>40008</c> / <c>40152</c>
    /// （tag_id 不存在）/ <c>42010</c>（操作太快）/ <c>45028</c>（当前周期内的发表次数已用完）/
    /// <c>45062</c>（已接广告不支持 api 群发）/ <c>45065</c> / <c>45066</c> / <c>45067</c> /
    /// <c>45113</c>（已不支持商品消息与卡券发表）/ <c>48001</c> / <c>48021</c>（系统自动保存的草稿不允许群发）/
    /// <c>48022</c>（api 上传的视频不允许用 api 发表）/ <c>89504</c> / <c>89505</c> / <c>268488001</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/mass/sendall")]
    Task<MpMassSendResponse> SendAllAsync(
        [Body] MpMassSendAllRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 OpenID 群发消息（<b>服务号专属</b>）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/message/api_masssend.html"/>
    /// （官方接口英文名 <c>massSend</c>）。
    /// </summary>
    /// <param name="request">群发请求（touser 2~10000 个 + msgtype + 分支对象其一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提交结果（<c>msg_id</c>；<c>msg_data_id</c> 仅图文群发时出现）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；<c>touser</c> OpenID 列表<b>最少 2 个、最多 10000 个</b>。
    /// 服务号专属归属写入官方原文（「本接口支持『服务号（仅认证）』账号类型调用」）——
    /// SDK 不做本地拦截（官方 48001 表达）。
    /// </para>
    /// <para>
    /// 官方错误码（节选）：<c>40031</c> / <c>40032</c>（openid 列表长度）/
    /// <c>40130</c>（入参至少需要 2 个 openid）/ <c>45162</c>（msgtype 参数错误）/ <c>9500002</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/mass/send")]
    Task<MpMassSendResponse> SendAsync(
        [Body] MpMassSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预览消息（在手机端查看消息的样式和排版）。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/notify/message/api_preview.html"/>
    /// （官方接口英文名 <c>preview</c>）。
    /// </summary>
    /// <param name="request">预览请求（touser / towxname 二选一 + msgtype + 分支对象其一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；分支对象比群发页多 music 与单图 image（共用 <see cref="MpMassMediaMessage"/>）；
    /// wxcard 带 card_ext。
    /// </para>
    /// <para>
    /// 官方注意事项原文：「为了满足第三方平台开发者的需求，在保留对 openID 预览能力的同时，
    /// 增加了对指定微信号发送预览的能力，但该能力每日调用次数有限制（<b>100 次</b>），请勿滥用」。
    /// </para>
    /// <para>官方错误码表仅列 <c>0</c>（照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/mass/preview")]
    Task<MpResponse> PreviewAsync(
        [Body] MpMassPreviewRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除群发消息。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/notify/message/api_deletemassmsg.html"/>
    /// （官方接口英文名 <c>deleteMassMsg</c>）。
    /// </summary>
    /// <param name="request">删除请求（msg_id / url 二选一 + article_idx）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>可删除条件（官方原文，勿弱化）</b>：「只有通过 api 发送的并且已经发送成功的消息才能删除」；
    /// 「删除群发消息只能删除<b>文章和视频</b>消息，其他类型的消息一经发送，无法删除」；
    /// 「删除消息是将消息的正文内容失效，已经收到的用户，还是能在其本地看到消息卡片」；
    /// 「如果多次群发发送的是一个文章，那么删除其中一次群发，就会删除掉这个内容，导致所有群发都失效」。
    /// </para>
    /// <para>
    /// 官方错误码：<c>40059</c>（msg_id 传参有误）/ <c>40060</c>（article_idx 不合法）/
    /// <c>40065</c>（文章状态异常不允许删除）/ <c>40204</c>（不允许删除洗稿文或谣言文）/ <c>9500001</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/mass/delete")]
    Task<MpResponse> DeleteMassMsgAsync(
        [Body] MpMassDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询群发消息发送状态。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/notify/message/api_massmsgget.html"/>
    /// （官方接口英文名 <c>massmsgget</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>msg_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送状态（<c>msg_status</c>：SEND_SUCCESS / SENDING / SEND_FAIL / DELETE）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"msg_id":…}</c>。
    /// </para>
    /// <para>
    /// <b>响应仅 msg_id / msg_status 两字段</b>（逐页核验 2026-10-07）——
    /// TotalCount/FilterCount/SentCount/ErrorCount 只在 <c>masssendjobfinish</c> 事件推送 XML 中，
    /// 不在本查询接口响应里（方案文档预判已按核验修正）。
    /// </para>
    /// <para>官方错误码：<c>40059</c> / <c>89504</c> / <c>89505</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/mass/get")]
    Task<MpMassStatusResponse> GetMassMsgStatusAsync(
        [Body] MpMassStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置群发速度。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/notify/message/api_setspeed.html"/>
    /// （官方接口英文名 <c>setSpeed</c>）。
    /// </summary>
    /// <param name="request">速度设置（<c>speed</c>：0=80w/分钟、1=60w/分钟、2=45w/分钟、3=30w/分钟、4=10w/分钟）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"speed":…}</c>（0~4 档位含义为官方请求体说明原文）。</para>
    /// <para>官方错误码：<c>45083</c>（speed 不在 0~4）/ <c>45084</c>（没有设置 speed）/
    /// <c>89504</c> / <c>89505</c>（官方两行解决方案列留空，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/mass/speed/set")]
    Task<MpResponse> SetMassSpeedAsync(
        [Body] MpMassSpeedSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取群发速度。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/notify/message/api_getspeed.html"/>
    /// （官方接口英文名 <c>getSpeed</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>当前速度（<c>speed</c> 档位 + <c>realspeed</c> 真实值万/分钟）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> 且<b>无请求体</b>（官方参数表原文「无」；页面代码示例给 {"speed":3} 属矛盾，照录——
    /// SDK 按参数表建模）。speed 与 realspeed 对照表（官方注意事项原文）：
    /// 0→80w/分钟、1→60w/分钟、2→45w/分钟、3→30w/分钟、4→10w/分钟。
    /// </para>
    /// <para>官方错误码：<c>45083</c> / <c>45084</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/mass/speed/get")]
    Task<MpMassSpeedResponse> GetMassSpeedAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>群发消息类型（官方 <c>msgtype</c> 取值；预览页另有 music，未在此列——预览请求直接传字符串）。</summary>
public static class MpMassMsgTypes
{
    /// <summary>图文消息（media_id 来自草稿箱/临时素材）。</summary>
    public const string MpNews = "mpnews";

    /// <summary>文本消息。</summary>
    public const string Text = "text";

    /// <summary>语音消息。</summary>
    public const string Voice = "voice";

    /// <summary>图片消息（群发页分支对象名为 images）。</summary>
    public const string Image = "image";

    /// <summary>视频消息。</summary>
    public const string MpVideo = "mpvideo";

    /// <summary>卡券消息（官方 45113：已不支持商品消息与卡券发表——保留词表供判错语义参考）。</summary>
    public const string WxCard = "wxcard";
}
