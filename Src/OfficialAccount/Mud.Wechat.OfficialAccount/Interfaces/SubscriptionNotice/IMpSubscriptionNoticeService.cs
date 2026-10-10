// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「订阅通知」域 SDK（服务端 API 侧 7 端点，<b>服务号专属</b>；
/// 回调侧「订阅通知事件族」见 <see cref="Abstractions.Callback.MpSubscriptionEventTypes"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：订阅通知为<b>服务号专属</b>能力，服务端 API 文档位于服务号域
/// <see href="https://developers.weixin.qq.com/doc/service/api/"/>（/doc/service/api/notify/notify/ 前缀；
/// 2026-10-07 逐页核验确认 subscription 订阅号域无该目录——7 页均 404 后在服务号域命中）。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>适用范围</b>：<c>bizsend</c> 页为「公众号 / 服务号 —— <b>仅认证</b>」；
/// newtmpl 六页为「小程序 ✔ / 公众号 仅认证 / 服务号 仅认证 / 小游戏 ✔」（官方各页字段表原文，
/// 系小程序订阅消息文案复用的痕迹——照录；SDK 不做账号类型本地闸）。</item>
/// <item><b>一次性消耗</b>：订阅通知按<b>用户订阅次数</b>下发——用户操作订阅弹窗（accept）一次方可下发一次；
/// 发送结果经回调事件 <c>subscribe_msg_sent_event</c> 异步回执（ErrorCode = 0 表示成功）。</item>
/// <item><b>与模板消息的通路差异</b>：模板消息（<see cref="IMpTemplateService"/>）无需用户订阅、
/// 按账号日额度下发；订阅通知需先取得用户订阅（回调 <c>subscribe_msg_popup_event</c>）再调 bizsend。</item>
/// <item><b>路径前缀</b>：bizsend 在 /cgi-bin/ 下；模板管理六端点在 <b>/wxaapi/newtmpl/</b> 前缀下
/// （无 /cgi-bin 段——官方路径安排，照抄原文）。</item>
/// <item><b>第三方平台</b>：7 页均支持代商家调用（bizsend 权限集 89；newtmpl 权限集 18、89）——
/// 按 M0-R3 裁决只在 XML 记录，不扩实现面。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "SubscriptionNotice", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpSubscriptionNoticeService
{
    /// <summary>
    /// 发送订阅通知。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/notify/api_sendnewsubscribemsg.html"/>
    /// （官方接口英文名 <c>sendNewSubscribeMsg</c>）。
    /// </summary>
    /// <param name="request">发送请求（<c>touser</c>/<c>template_id</c>/<c>data</c> 必填；data 键名原样透传）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体字段见 <see cref="MpSendSubscribeNoticeRequest"/>
    /// （官方文档矛盾——miniprogram_state/lang 标必填但有默认值——已在 DTO remarks 照录）。
    /// </para>
    /// <para>
    /// <b>一次性消耗用户订阅次数</b>：下发前须确认用户已订阅（回调 subscribe_msg_popup_event 的
    /// accept 记录）；投递结果经回调 <c>subscribe_msg_sent_event</c> 回执 ⇒ 处理器须幂等。
    /// </para>
    /// <para>
    /// 本页无独立错误码表（官方指向通用错误码文档，照录）；
    /// 注意事项原文为「本接口无特殊注意事项」；全页无频率限制章节。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/subscribe/bizsend")]
    Task<MpSendSubscribeNoticeResponse> SendSubscribeNoticeAsync(
        [Body] MpSendSubscribeNoticeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 选用模板（从公共模板库选用到私有模板库）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/notify/api_addwxanewtemplate.html"/>
    /// （官方接口英文名 <c>addwxanewtemplate</c>）。
    /// </summary>
    /// <param name="request">选用请求（tid + kidList（2~5 个）+ sceneDesc（≤15 字））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>选用结果（<c>priTmplId</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpAddSubscribeTemplateRequest"/>；
    /// 官方「注意事项」：「关键词组合需 2-5 个」「服务场景描述限制 15 字」。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40400</c> / <c>200001</c> / <c>200011</c>（账号被封禁）/
    /// <c>200012</c>（<b>私有模板数已达上限</b>）/ <c>200013</c>（模板被封禁）/ <c>200014</c>（tid 参数错误）/
    /// <c>200020</c>（kidList 参数错误）/ <c>200021</c>（sceneDesc 参数错误）/ <c>200022</c>（相同标题和关键字的模板已存在）/
    /// <c>200100</c>（账号类型不合法）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxaapi/newtmpl/addtemplate")]
    Task<MpAddSubscribeTemplateResponse> AddTemplateAsync(
        [Body] MpAddSubscribeTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已有模板列表。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getwxapubnewtemplate.html"/>
    /// （官方接口英文名 <c>getwxapubnewtemplate</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>模板列表（<c>data</c>；条目含 priTmplId / type / keywordEnumValueList 等）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>、无请求体；响应条目字段为 priTmplId / title / content / example / type（2 一次性 / 3 长期）/
    /// keywordEnumValueList（<b>无 tid、无 keywordList</b>——逐页核验确认）。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>404</c> / <c>40001</c> / <c>200100</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/wxaapi/newtmpl/gettemplate")]
    Task<MpSubscribeTemplateListResponse> GetTemplateAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除模板（私有模板库）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/notify/api_delwxanewtemplate.html"/>
    /// （官方接口英文名 <c>delwxanewtemplate</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>priTmplId</c> 必填——官方请求体字段实为 priTmplId，非 template_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"priTmplId":…}</c>。</para>
    /// <para>
    /// 官方错误码：<c>20001</c> / <c>20002</c> / <c>40001</c> / <c>200001</c> / <c>200002</c> / <c>200014</c>
    /// （多行解决方案列官方原文留空——照录；200014 描述「模版 tid 参数错误」未随参数更名更新，照录）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxaapi/newtmpl/deltemplate")]
    Task<MpResponse> DeleteTemplateAsync(
        [Body] MpDeleteSubscribeTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取类目。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getcategory.html"/>
    /// （官方接口英文名 <c>getCategory</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>类目列表（<c>data</c>；元素仅 id / name 两字段）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>、无请求体；data 元素仅 <c>id</c>/<c>name</c>（无 type——逐页核验确认）。</para>
    /// <para>官方错误码：<c>404</c> / <c>20001</c> / <c>40001</c> / <c>200100</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/wxaapi/newtmpl/getcategory")]
    Task<MpSubscribeCategoryListResponse> GetCategoryAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取类目下的公共模板。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getpubnewtemplatetitles.html"/>
    /// （官方接口英文名 <c>getPubNewTemplatetitles</c>）。
    /// </summary>
    /// <param name="ids">类目 id，多个用逗号隔开（官方 <c>ids</c>，必填）。</param>
    /// <param name="start">分页起始（官方 <c>start</c>，从 0 开始计数）。</param>
    /// <param name="limit">单页条数（官方 <c>limit</c>，<b>最大 30</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>公共模板标题分页（<c>count</c> 总数 + <c>data</c> 列表）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，Query <c>ids</c>/<c>start</c>/<c>limit</c> 均必填；
    /// 响应条目 <c>tid</c>（number）/ <c>title</c> / <c>type</c>（2 一次性 / 3 长期）/ <c>categoryId</c>
    /// （类型表 number、示例字符串 "616"——矛盾照录，按示例建模）。
    /// </para>
    /// <para>官方错误码：<c>404</c> / <c>40001</c> / <c>200001</c> / <c>200016</c>（start 参数错误）/
    /// <c>200017</c>（limit 参数错误）/ <c>200018</c>（类目 ids 缺失）/ <c>200019</c>（类目 ids 不合法）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/wxaapi/newtmpl/getpubtemplatetitles")]
    Task<MpSubscribePubTemplateTitlesResponse> GetPubTemplateTitlesAsync(
        [Query("ids")] string ids,
        [Query("start")] int start,
        [Query("limit")] int limit,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取模板中的关键词。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getpubnewtemplatekeywords.html"/>
    /// （官方接口英文名 <c>getpubnewtemplatekeywords</c>）。
    /// </summary>
    /// <param name="tid">模板标题 id（官方 <c>tid</c>，必填；选用模板时需其中的 kid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>关键词列表（<c>data</c>；元素含 kid / name / example / rule）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>，Query <c>tid</c> 必填；关键词的 <c>rule</c> 即 bizsend data 值的类型前缀来源。</para>
    /// <para>官方错误码：<c>20001</c> / <c>40001</c> / <c>200002</c> / <c>200014</c>（多行解决方案列官方留空，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/wxaapi/newtmpl/getpubtemplatekeywords")]
    Task<MpSubscribePubTemplateKeywordsResponse> GetPubTemplateKeywordsAsync(
        [Query("tid")] string tid,
        CancellationToken cancellationToken = default);
}
