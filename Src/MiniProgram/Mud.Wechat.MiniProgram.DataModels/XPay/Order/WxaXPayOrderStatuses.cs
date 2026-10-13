// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 订单状态取值（官方 <c>Res.order.status</c> 枚举表，0~10 共 11 值）。
/// </summary>
/// <remarks>
/// <para><b><c>5</c> 与 <c>8</c> 是同一笔退款的两侧</b>：5 = 商户侧扣款完成、8 = 用户侧收款完成，中间态 <c>7</c> = 退款失败 ⇒ 退款「成功」的官方口径是 8 而不是 5。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_order.html"/></para>
/// </remarks>
public static class WxaXPayOrderStatuses
{
    /// <summary>订单初始化（未创建成功，不可用于支付）（<c>0</c>）</summary>
    public const int Initialized = 0;

    /// <summary>订单创建成功（<c>1</c>）</summary>
    public const int Created = 1;

    /// <summary>订单已经支付，待发货（<c>2</c>）</summary>
    public const int PaidWaitProvide = 2;

    /// <summary>订单发货中（<c>3</c>）</summary>
    public const int Providing = 3;

    /// <summary>订单已发货（<c>4</c>）</summary>
    public const int Provided = 4;

    /// <summary>订单退款：商户侧扣款完成（<c>5</c>）</summary>
    public const int RefundMerchantDeducted = 5;

    /// <summary>订单已经关闭（不可再使用）（<c>6</c>）</summary>
    public const int Closed = 6;

    /// <summary>订单退款失败（<c>7</c>）</summary>
    public const int RefundFailed = 7;

    /// <summary>订单退款：用户侧收款完成（<c>8</c>）</summary>
    public const int RefundUserReceived = 8;

    /// <summary>回收广告金完成（<c>9</c>）</summary>
    public const int AdvertRecovered = 9;

    /// <summary>分账回退完成（<c>10</c>）</summary>
    public const int SplitFallbackDone = 10;
}
