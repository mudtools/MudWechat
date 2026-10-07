// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「模板消息」域 SDK（7 端点，<b>服务号专属</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：模板消息为<b>服务号专属</b>能力，文档位于服务号域
/// <see href="https://developers.weixin.qq.com/doc/service/api/"/>（<b>非</b> subscription 订阅号域；
/// 2026-10-07 逐页核验确认 7 页深链全部在 /doc/service/api/notify/template/ 前缀下）。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>适用范围（7 页一致原文）</b>：「本接口支持『服务号（仅认证）』账号类型调用。
/// 其他账号类型如无特殊说明，均不可调用」——SDK 不做账号类型本地闸（官方 <c>48001</c> 表达，
/// 与既有 CM8/N1 裁决同源）。</item>
/// <item><b>频率上限（官方指南页原文）</b>：「当前每个账号的模板消息的日调用上限为 <b>10 万</b>次，
/// 单个模板没有特殊限制」；粉丝数超 10W/100W/1000W 时上限相应提升，以服务号 MP 后台开发者页面标明为准。
/// （API 页本身未列 QPS——不编造数值。）</item>
/// <item><b>行业修改（硬约束）</b>：「每月可修改行业 <b>1</b> 次」；「修改行业后，你在原有行业中的模板
/// 将会被删除」。</item>
/// <item><b>模板数量</b>：指南页「每个账号可同时使用 <b>25</b> 个模板」；「每个服务号最多 5 个类目、
/// 每月可更改 5 次，更改或删除类目后原类目模板被删除」。</item>
/// <item><b>结果回执</b>：发送结果经回调事件 <c>templatesendjobfinish</c> 异步推送
/// （<c>MpCallbackEventTypes.TemplateSendJobFinish</c> 族，见回调包）。</item>
/// <item><b>第三方平台</b>：7 页均支持代商家调用（权限集 id 7、100-101，authorizer_access_token）——
/// 按 M0-R3 裁决只在 XML 记录，不扩实现面。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Template", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpTemplateService
{
    /// <summary>
    /// 发送模板消息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/template/api_sendtemplatemessage.html"/>
    /// （官方接口英文名 <c>sendTemplateMessage</c>）。
    /// </summary>
    /// <param name="request">发送请求（<c>touser</c>/<c>template_id</c>/<c>data</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（<c>msgid</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；请求体字段见 <see cref="MpSendTemplateMessageRequest"/>
    /// （data 仅 value——历史文档的 color 字段在 2026-10-07 核验页面已不存在，按事实建模）。
    /// </para>
    /// <para>
    /// <b>发送结果经回调异步回执</b>：本响应的 msgid 只代表「受理」，投递结果经事件
    /// <c>templatesendjobfinish</c> 推送（Status 含 success / failed:user block 等）⇒ 处理器须幂等。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c> / <c>40008</c>（invalid message type）/
    /// <c>40013</c> / <c>40036</c>（invalid template_id size）/ <c>40037</c>（invalid template_id）/
    /// <c>40039</c>（invalid url size）/ <c>40249</c>（禁止发送营销内容）/ <c>43116</c>（模板被限制下发）/
    /// <c>47003</c>（参数不符合模板格式，如 thing01.DATA is invalid）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/template/send")]
    Task<MpSendTemplateMessageResponse> SendTemplateMessageAsync(
        [Body] MpSendTemplateMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置所属行业。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/template/api_setindustry.html"/>
    /// （官方接口英文名 <c>setIndustry</c>）。
    /// </summary>
    /// <param name="request">行业编号（<c>industry_id1</c>/<c>industry_id2</c> 必填；代码 1~41 字符串形态）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；<b>仅 industry_id1/2 两个字段</b>（当前页面无 industry_id3，
    /// 与旧版资料 3 参数形态不同——以核验页面为准；行业代码表见官方页 1~41）。
    /// </para>
    /// <para>
    /// <b>硬约束（官方原文）</b>：「每月可修改行业 <b>1</b> 次」；「修改行业后，你在原有行业中的模板
    /// 将会被删除」⇒ 调用前须确认模板可重建。
    /// </para>
    /// <para>官方错误码表仅列 <c>0</c>（其余参考通用错误码文档，照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/template/api_set_industry")]
    Task<MpResponse> SetIndustryAsync(
        [Body] MpSetIndustryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取行业信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/template/api_getindustry.html"/>
    /// （官方接口英文名 <c>getIndustry</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>主营 / 副营行业（<c>primary_industry</c>/<c>secondary_industry</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>、无请求体；成功响应不含 errcode（缺省 0）。
    /// </para>
    /// <para>
    /// <b>命名差异（勿互相「对齐」）</b>：本页副营行业键为 <c>secondary_industry</c>，而模板列表页
    /// 的二级行业字段名为 <c>deputy_industry</c>——同域两处「二级」命名不一致（官方原文如此）。
    /// </para>
    /// <para>官方错误码表仅列 <c>0</c>（照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/template/get_industry")]
    Task<MpGetIndustryResponse> GetIndustryAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 选用模板。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/template/api_addtemplate.html"/>
    /// （官方接口英文名 <c>addTemplate</c>）。
    /// </summary>
    /// <param name="request">选用请求（<c>template_id_short</c> + <c>keyword_name_list</c>；类目模板为纯数字 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>选用结果（<c>template_id</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；官方将行业模板库（TM** 形态）与类目模板（纯数字 id）合并在同一字段集，
    /// 无独立分支字段。<c>keyword_name_list</c> 按顺序传入；空或关键词不在模板库返回 <c>40246</c>。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾（照录）</b>：<c>keyword_name_list</c> 标注必填，但仅行业模板库形态无需关键词。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40037</c> / <c>40246</c>（invalid keyword_name_list）/
    /// <c>40247</c>（need new category template，请使用类目模板库 ID 进行添加）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/template/api_add_template")]
    Task<MpAddTemplateResponse> AddTemplateAsync(
        [Body] MpAddTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已选用模板列表。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/template/api_getalltemplates.html"/>
    /// （官方接口英文名 <c>getAllTemplates</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>模板列表（<c>template_list</c>；条目含 content 参数占位形态与 example 示例）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>、无请求体；成功响应不含 errcode（缺省 0）。
    /// </para>
    /// <para>
    /// <b>字段命名（照抄官方）</b>：条目二级行业字段名为 <c>deputy_industry</c>（2026-10-07 核验页面原文；
    /// 旧版文档与行业查询页均为 secondary_industry 形态——两处命名不一致，勿互相「对齐」）。
    /// </para>
    /// <para>官方错误码表仅列 <c>0</c>（照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/template/get_all_private_template")]
    Task<MpGetAllPrivateTemplateResponse> GetAllPrivateTemplateAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除模板。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/template/api_deletetemplate.html"/>
    /// （官方接口英文名 <c>deleteTemplate</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>template_id</c> 必填，包括类目模板 ID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"template_id":…}</c>。</para>
    /// <para>官方错误码表仅列 <c>0</c>（照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/template/del_private_template")]
    Task<MpResponse> DeletePrivateTemplateAsync(
        [Body] MpDeleteTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询拦截的模板消息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/notify/template/api_queryblocktmplmsg.html"/>
    /// （官方接口英文名 <c>queryBlockTmplMsg</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>tmpl_msg_id</c>/<c>largest_id</c>/<c>limit</c> 均必填；limit ≤ 100）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>拦截信息（<c>msginfo</c>；官方文档形态矛盾照录，见 <see cref="MpQueryBlockTmplMsgResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；<b>路径前缀为 /wxa/sec/（本域唯一非 /cgi-bin/ 路径）</b>——
    /// 官方自身路径安排，接入须照抄原文。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾（照录）</b>：①<c>msginfo.id</c> 类型标注 string、示例为数字（与请求体
    /// <c>largest_id</c> 的 number 形态矛盾）；②接口语义为分页查询但 <c>msginfo</c> 建模为单对象，
    /// 与「单页查询最大 100 条」不一致，疑似官方遗漏数组形态——SDK 照官方字段表建模。
    /// </para>
    /// <para>官方错误码表仅列 <c>0</c>（照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/sec/queryblocktmplmsg")]
    Task<MpQueryBlockTmplMsgResponse> QueryBlockTmplMsgAsync(
        [Body] MpQueryBlockTmplMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送一次性订阅消息（官方分组「一次性订阅消息」，两号通用；并入模板域——1 端点独立建域
    /// 只有注册开销而无隔离收益，与 changeopenid 并入用户域同判例）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/notify/subscribe/api_templatesubscribe.html"/>
    /// （官方接口英文名 <c>templateSubscribe</c>；路径 <c>/cgi-bin/message/template/subscribe</c>）。
    /// </summary>
    /// <param name="request">发送请求（touser/template_id/scene/title/data 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约（逐页核验 2026-10-07）：<b>POST</b> + 请求体见 <see cref="DataModels.Mass.MpOneTimeSubscribeRequest"/>
    /// （data 为固定单键 content 的 {value,color} 形态——<b>本端点有 color</b>，与 sendTemplateMessage 的
    /// value-only 形态不同，照各自页面原文）。
    /// </para>
    /// <para>
    /// 官方「注意事项」原文：①「url 和 miniprogram 都是非必填字段，若都不传则模板无跳转；
    /// 若都传会优先跳转至小程序」；②「用户已关注公众号时消息下发到公众号会话，未关注时下发到服务通知」。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40003</c> / <c>40013</c> / <c>40037</c>（模板 ID 无效）/
    /// <c>40062</c>（invalid title size，官方解决方案列留空，照录）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/template/subscribe")]
    Task<MpResponse> SendOneTimeSubscribeAsync(
        [Body] DataModels.Mass.MpOneTimeSubscribeRequest request,
        CancellationToken cancellationToken = default);
}
