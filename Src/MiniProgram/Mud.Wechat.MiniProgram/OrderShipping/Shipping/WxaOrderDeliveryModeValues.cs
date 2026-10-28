// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 发货模式（<c>delivery_mode</c>，上传侧与查询侧同一取值表）。
/// </summary>
/// <remarks>
/// <para><b>官方给出的是枚举名而非数字描述</b>（原文附 <c>UNIFIED_DELIVERY</c> / <c>SPLIT_DELIVERY</c>），
/// 线上传的是数字 1/2 ⇒ 本表只锁数字面。</para>
/// <para><b>分拆发货仅支持物流快递</b>，且一笔支付单最多 15 个包裹。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/></para>
/// </remarks>
public static class WxaOrderDeliveryModeValues
{
    /// <summary>统一发货（<c>1</c>，UNIFIED_DELIVERY）：一笔订单统一发货，只有一个物流单号。</summary>
    public const int UnifiedDelivery = 1;

    /// <summary>分拆发货（<c>2</c>，SPLIT_DELIVERY）：一笔订单分拆发货，包括多个物流单号。</summary>
    public const int SplitDelivery = 2;
}
