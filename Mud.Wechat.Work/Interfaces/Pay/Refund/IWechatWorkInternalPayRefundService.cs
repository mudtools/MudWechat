// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Pay;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「企业支付」模块退款域企业自建应用 SDK（申请退款 + 查询退款）。
/// <para>
/// 官方仅向自建应用开放本域 2 个端点（代开发应用与第三方应用均暂不支持），
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
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayRefundService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalPayRefundService : IWechatWorkPayRefundService
{
    /// <summary>
    /// 申请退款
    /// <para>提交退款申请，支持单笔交易分多次退款；接口返回仅代表受理情况，
    /// 退款是否成功须通过「查询退款」接口或退款通知回调确认。</para>
    /// <para>官方业务限制：交易时间超过一年的订单无法提交退款；
    /// 总退款金额不能超过订单金额，每个支付订单的部分退款次数不能超过 50 次；
    /// 退款失败重试须沿用原退款单号（同一退款单号多次请求只退一笔）；
    /// 请求频率限制 150 qps，单笔订单 1 qpm（每分钟申请退款不超过 1 次）。</para>
    /// </summary>
    /// <param name="request">退款请求体（<see cref="ApplyPayRefundRequest"/>：
    /// mchid / appid / out_trade_no / out_refund_no / amount，reason 与 funds_account 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退款受理结果（out_refund_no / amount / promotion_detail）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97333"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/refund")]
    Task<ApplyPayRefundResponse> ApplyRefundAsync(
        [Body] ApplyPayRefundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询退款
    /// <para>按商户退款单号查询退款单的受理与到账状态。</para>
    /// <para>官方业务限制：退款有一定延时，用零钱支付的退款 20 分钟内到账，
    /// 银行卡支付的退款 3 个工作日后重新查询退款状态。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPayRefundDetailRequest"/>：mchid + out_refund_no）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退款单详情（refund_id / status：SUCCESS / CLOSE / PROCESSING / ABNORMAL、
    /// channel / user_received_account / amount / promotion_detail 等）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97352"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/get_refund_detail")]
    Task<GetPayRefundDetailResponse> GetRefundDetailAsync(
        [Body] GetPayRefundDetailRequest request,
        CancellationToken cancellationToken = default);
}
