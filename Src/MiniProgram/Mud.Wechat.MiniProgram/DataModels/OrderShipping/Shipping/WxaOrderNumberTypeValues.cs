// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 订单单号类型（上传侧 <c>order_key.order_number_type</c>）。
/// </summary>
/// <remarks>
/// <para><b>与「订单状态」不是一回事</b>（后者见 <see cref="WxaOrderStateValues"/>，1~6）；
/// 且<b>没有取值 3</b>。合单场景下主单与子单必须取同一值（官方原文）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/></para>
/// </remarks>
public static class WxaOrderNumberTypeValues
{
    /// <summary>使用下单商户号和商户侧单号（<c>1</c>），即 <c>mchid</c> + <c>out_trade_no</c>。</summary>
    public const int MerchantSide = 1;

    /// <summary>使用微信支付单号（<c>2</c>），即 <c>transaction_id</c>。</summary>
    public const int WechatPaySide = 2;
}
