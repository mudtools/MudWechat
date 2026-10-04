// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Pay;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「企业支付」模块普通支付域企业自建应用 SDK
/// （小程序下单 + 查询订单 + 关闭订单 + 获取支付签名）。
/// <para>
/// 官方仅向自建应用开放本域 4 个端点（代开发应用与第三方应用均暂不支持），
/// 全部声明于本接口（形态对齐 <see cref="IWechatWorkInternalPayMchApplyService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// 官方权限口径：须先在企微管理端开通对外收款能力，商户号为「由企业微信生成并下发」的二级商户号。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Pay",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayOrderService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalPayOrderService : IWechatWorkPayOrderService
{
    /// <summary>
    /// 小程序下单（普通支付）
    /// <para>商户系统内部订单号在同一商户号下唯一；返回预支付交易会话标识 prepay_id
    /// （有效期 2 小时），配合「获取支付签名」接口供小程序侧拉起支付。</para>
    /// <para>官方业务限制：境内商户号仅支持人民币（CNY）；scenekey 用于统计企微成员
    /// 发出小程序的交易业绩（展示在「对外收款 - 成员业绩」），不传则不统计。</para>
    /// </summary>
    /// <param name="request">下单请求体（<see cref="CreatePayOrderRequest"/>：
    /// appid / mchid / out_trade_no / description / amount / payer / scene_info 等完整下单要素）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预支付交易会话标识（prepay_id，有效期 2 小时）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97322"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/create_order")]
    Task<CreatePayOrderResponse> CreateOrderAsync(
        [Body] CreatePayOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询订单（普通支付）
    /// <para>查询单笔订单的支付状态与交易详情，用于未收到支付通知、支付接口返回
    /// 系统错误或未知状态、以及调关单/撤销前确认支付状态的场景。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPayOrderRequest"/>：mchid + out_trade_no）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情（trade_state / trade_state_desc / payer / transaction_id /
    /// amount / promotion_detail 等；trade_state：SUCCESS / REFUND / NOTPAY / CLOSED /
    /// REVOKED / USERPAYING / PAYERROR / ACCEPT）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97323"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/get_order")]
    Task<GetPayOrderResponse> GetOrderAsync(
        [Body] GetPayOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭订单（普通支付）
    /// <para>关闭指定订单，订单生成后不能马上调用关单接口，最短调用时间间隔为 5 分钟。</para>
    /// </summary>
    /// <param name="request">关单请求体（<see cref="ClosePayOrderRequest"/>：mchid + out_trade_no）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>关闭结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97324"/></para>
    /// <para>官方业务限制：订单生成后不能马上调用关单接口，最短调用时间间隔为 5 分钟；
    /// 代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/close_order")]
    Task<WechatWorkResponse> CloseOrderAsync(
        [Body] ClosePayOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取支付签名（普通支付）
    /// <para>使用字段 appid、timestamp、nonce、prepay_id 计算支付签名值（pay_sign），
    /// 供小程序侧拉起支付时使用。</para>
    /// <para>官方业务限制：仅支持下单两小时内的 prepay_id；随机字符串不长于 32 位
    /// 且仅支持数字、大小写字母；签名方式默认 RSA，仅支持 RSA。</para>
    /// </summary>
    /// <param name="request">签名请求体（<see cref="GetPaySignRequest"/>：
    /// appid / prepay_id / nonce / timestamp，sign_type 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支付签名值（pay_sign）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98130"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/get_sign")]
    Task<GetPaySignResponse> GetSignAsync(
        [Body] GetPaySignRequest request,
        CancellationToken cancellationToken = default);
}
