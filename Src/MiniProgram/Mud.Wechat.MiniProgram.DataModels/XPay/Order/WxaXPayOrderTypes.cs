// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 订单类型取值（官方 <c>Res.order.order_type</c> 枚举表）。
/// </summary>
/// <remarks>
/// <para><b>取值集不连续</b>（官方只给 0 / 1 / 7 / 8）：iOS 支在 7/8 而非紧邻 0/1 ⇒ <b>不得</b>因「看起来缺 2~6」而补值，也不得写 <c>value &lt;= 1</c> 这类区间判断。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_order.html"/></para>
/// </remarks>
public static class WxaXPayOrderTypes
{
    /// <summary>普通虚拟支付（<c>0</c>）</summary>
    public const int NormalPay = 0;

    /// <summary>普通退款（<c>1</c>）</summary>
    public const int NormalRefund = 1;

    /// <summary>苹果iOS支付（<c>7</c>）</summary>
    public const int IosPay = 7;

    /// <summary>苹果iOS退款（<c>8</c>）</summary>
    public const int IosRefund = 8;
}
