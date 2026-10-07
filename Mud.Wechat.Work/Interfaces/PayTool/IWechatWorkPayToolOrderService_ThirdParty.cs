// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.PayTool;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「收银台」模块收款工具域第三方应用 / 服务商代开发 SDK
/// （创建收款订单 / 取消收款订单 / 获取收款订单列表 / 获取收款订单详情）。
/// <para>官方在第三方应用开发与服务商代开发两棵文档树开放本族端点（共享同一端点页），
/// 全部 4 个端点声明于本接口；企业自建应用官方不开放，不设对应子接口。</para>
/// </summary>
/// <remarks>
/// <para>消费服务商级 provider_access_token（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。</para>
/// <para>官方权限口径：创建与取消收款订单要求「服务商需有在收银台完成商户号注册」（支付方式为「免支付」的订单可不受此限制）；
/// 获取订单列表与订单详情页面标注「无特殊权限」。</para>
/// <para>官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。</para>
/// <para>MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "PayTool",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayToolOrderService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyPayToolOrderService : IWechatWorkPayToolOrderService
{
    /// <summary>
    /// 创建收款订单
    /// <para>服务商可以使用该接口创建各种业务的收款订单。</para>
    /// <para>官方权限：服务商需有在收银台完成商户号注册（支付方式为「免支付」的订单可以不受此限制）。</para>
    /// <para>官方约束：请求体须携带 nonce_str / ts / sig 三要素，签名算法见官方
    /// <see href="https://developer.work.weixin.qq.com/document/path/98768">path 98768</see>；
    /// ts 为 unix 时间戳（中国时区，精确到秒），<b>业务系统的机器时间与腾讯的时间相差不能超过 15 分钟</b>；
    /// nonce_str 长度在 32 字节以内且<b>需保证 15 分钟内不能重复</b>。</para>
    /// <para>官方约束：product_list 按 business_type 三选一 —— 1 普通第三方应用填 third_app、2 代开发应用填 customized_app、
    /// 3 行业解决方案填 promotion_case；buy_info_list 可填充个数 1 ~ 20。</para>
    /// <para>官方约束：notify_custom_corp 开启后，服务商代支付与免支付订单将向企业管理员推送确认提醒，
    /// <b>超时（72 小时）未确认会自动确认</b>；客户支付订单创建后将向企业管理员推送支付提醒；
    /// 指定的免支付订单创建后默认推送订单通知、不可取消。</para>
    /// <para>官方说明：bank_receipt_media_id 需通过「服务商上传临时素材」上传（须指定 attachment_type=3 专用于收银台）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreatePayToolOrderRequest"/>：business_type 业务类型 /
    /// custom_corpid 客户企业 corpid / pay_type 支付方式 / bank_receipt_media_id 银行收款回单凭证 media_id /
    /// creator 订单创建人 userid / product_list 购买商品 / nonce_str 随机字符串 / ts 时间戳 / sig 签名）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>收款订单号（order_id）/ 收款订单链接（order_url）/ 原价（origin_price）/ 折后价（paid_price）。
    /// <para>官方口径：order_url 对客户支付订单为支付链接、对服务商代支付与免支付订单为订单确认链接；
    /// 两个价格仅部分可以确定价格的请求下会返回，代开发应用的原价和折后价一致。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98045">path 98045 创建收款订单</see></para>
    /// <para><b>服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/99358">path 99358 创建收款订单</see></para>
    /// <para><b>签名算法</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98768">path 98768 签名算法</see>（代开发树同文：<see href="https://developer.work.weixin.qq.com/document/path/99362">path 99362</see>）</para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/paytool/open_order")]
    Task<CreatePayToolOrderResponse> CreateOrderAsync(
        [Body] CreatePayToolOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消收款订单
    /// <para>服务商可以使用该接口取消指定的收款订单。</para>
    /// <para>官方权限：服务商需有在收银台完成商户号注册。</para>
    /// <para>官方约束：请求体须携带 nonce_str / ts / sig 三要素，签名算法见官方
    /// <see href="https://developer.work.weixin.qq.com/document/path/98768">path 98768</see>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ClosePayToolOrderRequest"/>：order_id 收款订单号 /
    /// nonce_str 随机字符串 / ts 时间戳 / sig 签名）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 errcode / errmsg（官方本端点无业务负载）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98046">path 98046 取消收款订单</see></para>
    /// <para><b>服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/99359">path 99359 取消收款订单</see></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/paytool/close_order")]
    Task<WechatWorkResponse> CloseOrderAsync(
        [Body] ClosePayToolOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取收款订单列表
    /// <para>服务商可以使用该接口查询指定时间段内创建的各种业务的收款订单列表。</para>
    /// <para>官方权限：本端点页面标注「无特殊权限」。</para>
    /// <para>官方约束：limit 为官方必填，取值范围 1 ~ 2000；cursor 由上一次调用返回，首次调用不填；
    /// 请求体须携带 nonce_str / ts / sig 三要素。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetPayToolOrderListRequest"/>：business_type 业务类型 /
    /// start_time 起始时间 / end_time 结束时间 / cursor 分页游标 / limit 分页条数 /
    /// nonce_str 随机字符串 / ts 时间戳 / sig 签名）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分页游标（next_cursor）/ 是否还有更多（has_more）/ 订单列表（pay_order_list）。
    /// <para>列表项含 order_id / create_time / custom_corpid / buy_content / origin_price / paid_price /
    /// order_status / order_from / creator / pay_type。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98053">path 98053 获取收款订单列表</see></para>
    /// <para><b>服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/99360">path 99360 获取收款订单列表</see></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/paytool/get_order_list")]
    Task<GetPayToolOrderListResponse> GetOrderListAsync(
        [Body] GetPayToolOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取收款订单详情
    /// <para>服务商可以使用该接口查询指定收款订单的详情。</para>
    /// <para>官方权限：本端点页面标注「无特殊权限」。</para>
    /// <para>官方约束：请求体须携带 nonce_str / ts / sig 三要素。</para>
    /// <para>官方契约陷阱：返回参数表同时列出 pay_order.pay_from 与 pay_order.pay_type，而返回示例只用
    /// <c>pay_type</c>，本 SDK 照官方返回示例承载 <c>pay_type</c>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetPayToolOrderDetailRequest"/>：order_id 订单号 /
    /// nonce_str 随机字符串 / ts 时间戳 / sig 签名）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情（pay_order）。
    /// <para>含 order_id / create_time / custom_corpid / buy_content / origin_price / paid_price /
    /// order_status / order_from / creator / pay_type / custom_corp_name / pay_channel / channel_order_id /
    /// paid_time / business_type / income_type / income_time / income_amount / product_list 购买明细。</para>
    /// <para>官方口径：pay_channel、channel_order_id、paid_time 未支付时为空；
    /// income_type、income_time、income_amount 收入未到账时为空。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98054">path 98054 获取收款订单详情</see></para>
    /// <para><b>服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/99361">path 99361 获取收款订单详情</see></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/paytool/get_order_detail")]
    Task<GetPayToolOrderDetailResponse> GetOrderDetailAsync(
        [Body] GetPayToolOrderDetailRequest request,
        CancellationToken cancellationToken = default);
}