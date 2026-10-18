// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AdvancedApi;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「高级接口（订单与代付）」域公共父接口（官方 100257 ~ 100271）。
/// <para>
/// 服务商在「数据与智能专区」为企业下单购买<b>会话内容数据接口</b>（msgaudit）的订单面：
/// 下单 / 取消 / 提交余额支付 / 订单列表 / 订单详情 / 企业已购信息，共 6 支端点。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌与归属：六支端点全部以 <c>provider_access_token</c>（Query）鉴权，
/// 企业侧以 <c>custom_corpid</c> 指定 ⇒ 只落 <see cref="IWechatWorkProviderAdvancedApiService"/>
/// 一个应用类型子接口（自建 / 第三方无本面）。
/// MUD005 已知接受风险：官方契约强制令牌走 Query 参数，无法改用 Header。
/// </para>
/// <para>
/// <b>官方反直觉点</b>：列表 / 详情 / 已购信息三支<b>查询接口官方即 POST</b>（勿改成 GET）；
/// 详情里的 <c>custom_corpid</c> 返回<b>加密的 corpid</b>；<c>has_more</c> 为整型 0/1 而非布尔。
/// </para>
/// <para>
/// <b>写操作与不可逆性</b>：下单产生真实订单、提交余额支付后<b>官方禁止再变更支付方式</b> ⇒
/// 本域方法一律不做 SDK 级自动重试（官方 <c>701142</c>「购买信息已被其他请求更新，请重试」的
/// 重试决策由宿主显式承担）。
/// </para>
/// <para>
/// <b>身份信息约束</b>：<c>buyer_userid</c> / <c>payer_userid</c> 为服务商企业内成员明文 userid，
/// 属用户身份信息，不得写入日志、遥测或异常消息。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkAdvancedApiService
{
    /// <summary>
    /// 下单购买高级接口
    /// <para>服务商为<b>客户企业</b>下单购买「数据与智能专区」中的会话内容数据接口。</para>
    /// <para>
    /// <b>官方必填</b>：<c>custom_corpid</c>（客户企业 id）、<c>buyer_userid</c>（下单人，须为服务商企业内
    /// 具有「购买高级接口」或「高级接口管理」权限的成员）；<c>advanced_api_type</c> 为 1（默认）时
    /// <c>chat_archive_api</c> <b>必填</b>。
    /// </para>
    /// <para>
    /// <b>官方版本状态矩阵</b>（决定订单能否提交）：仅未到期版本可增购；仅版本到期前 60 天起可续期；
    /// 仅未到期版本 + 内外部会话版本可升级（<c>701140</c> / <c>701141</c> / <c>701149</c> / <c>701164</c>）；
    /// 升级时 <c>edition</c> 填<b>目标版本</b>、人数不可超出原版本已购人数；
    /// <c>take_effect_time</c> 若指定须 ≥ 当天 0 点且在 90 天内（<c>701163</c>）；
    /// 存在未支付或申请退款中的订单时不能下单（<c>701143</c>）；海外企业不能购买（<c>701146</c>）；
    /// 订单价格不能超过 500 万（<c>84077</c>）。
    /// </para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateAdvancedApiOrderRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单号（order_id）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100257"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制；<b>本端点为写操作，SDK 不自动重试</b>。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_api/create_order")]
    Task<CreateAdvancedApiOrderResponse> CreateOrderAsync(
        [Body] CreateAdvancedApiOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订单
    /// <para>取消「数据与智能专区」中的会话内容数据接口订单。</para>
    /// <para><b>官方约束</b>：只可取消<b>未支付且未失效</b>的订单。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CancelAdvancedApiOrderRequest"/>：order_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg（官方无业务负载，故不新建空响应 DTO）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100258"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_api/cancel_order")]
    Task<WechatWorkResponse> CancelOrderAsync(
        [Body] CancelAdvancedApiOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交余额支付订单
    /// <para>使用<b>充值账户余额</b>为订单创建支付任务；支付成功后自动扣款，且服务商可接收到支付成功回调。</para>
    /// <para>
    /// <b>不可逆写操作</b>：官方明确「提交成功后，该订单无法再变更支付方式」⇒ 务必避免由重试 /
    /// 重复请求机制无条件重复调用本方法。
    /// </para>
    /// <para>
    /// <b>官方必填与权限</b>：<c>payer_userid</c>（支付人，明文 userid，须具有「购买高级接口」或
    /// 「高级接口管理」权限，否则 <c>48001</c>）、<c>order_id</c>。
    /// 官方错误码：<c>90432</c> 充值账户未开通、<c>90433</c> 账户余额不足、
    /// <c>701084</c> 订单不是待支付状态、<c>701147</c> 余额支付超过单次支付限额上限。
    /// </para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SubmitAdvancedApiOrderPayRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg（官方无业务负载）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100259"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_api/submit_pay")]
    Task<WechatWorkResponse> SubmitOrderPayAsync(
        [Body] SubmitAdvancedApiOrderPayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单列表
    /// <para>服务商查询自己某段时间内的平台能力服务订单列表。</para>
    /// <para>
    /// <b>官方约束</b>：<c>start_time</c> 与 <c>end_time</c> <b>不能单独指定</b>（须同时给出），
    /// 下单时间区间语义为 <c>[start_time, end_time)</c>；<c>limit</c> 最大 1000、默认 500；
    /// <c>has_more</c> 为整型 0/1；<c>is_last</c> 形态在<b>本域不存在</b>（与通讯录搜索族不同）。
    /// </para>
    /// <para><b>官方即 POST</b> 的查询接口，勿「顺手修正」为 GET。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListAdvancedApiOrdersRequest"/>：全部字段可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单列表（order_list{order_id、order_type、order_status、create_time} / next_cursor / has_more）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100260"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_api/list_order")]
    Task<ListAdvancedApiOrdersResponse> ListOrdersAsync(
        [Body] ListAdvancedApiOrdersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单详情
    /// <para>查询某个订单的详情。</para>
    /// <para>
    /// <b>官方字段语义陷阱</b>：详情内 <c>custom_corpid</c> 返回<b>加密的 corpid</b>（与下单请求所传明文
    /// 客户企业 id 形态不同）；<c>chat_archive_api</c> 仅当 <c>advanced_api_type</c> 为 1 时返回，
    /// 其字段集与下单请求体<b>不同</b>（多 <c>purchase_duration_days</c> / <c>end_time</c> /
    /// <c>original_price</c>，后者仅升级订单返回）⇒ 响应与请求 DTO 不共用。
    /// </para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetAdvancedApiOrderRequest"/>：order_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情（order）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100261"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_api/get_order")]
    Task<GetAdvancedApiOrderResponse> GetOrderAsync(
        [Body] GetAdvancedApiOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业已购高级接口信息
    /// <para>查询客户企业高级接口已购信息。</para>
    /// <para>
    /// <b>官方必填</b>：<c>advanced_api_type</c>（固定为 1）与 <c>custom_corpid</c>
    /// —— 与「订单列表」端点恰好相反，那里的 <c>custom_corpid</c> 是可选过滤项。
    /// </para>
    /// <para>
    /// <b>版本枚举比下单面宽</b>：<c>edition_list.edition</c> 官方含 <c>1</c> 内部会话（历史购买的），
    /// 照抄原文、不做归一。
    /// </para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetAdvancedApiCorpBuyInfoRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>购买信息（chat_archive_api_buy_info.edition_list{edition、purchase_count、begin_time、end_time}）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100271"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_api/get_corp_buy_info")]
    Task<GetAdvancedApiCorpBuyInfoResponse> GetCorpBuyInfoAsync(
        [Body] GetAdvancedApiCorpBuyInfoRequest request,
        CancellationToken cancellationToken = default);
}
