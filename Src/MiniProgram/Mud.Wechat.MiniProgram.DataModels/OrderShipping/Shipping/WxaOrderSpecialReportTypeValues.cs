// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 特殊发货报备类型（<c>opspecialorder.type</c>，仅两值）。
/// </summary>
/// <remarks>
/// <para><c>1</c>（预售商品订单）时 <c>delay_to</c> 必填，且时间窗为支付时间后 15~330 天；
/// <c>2</c>（测试订单）报备后无需发货，退款须去商户号后台另行操作。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_opspecialorder.html"/></para>
/// </remarks>
public static class WxaOrderSpecialReportTypeValues
{
    /// <summary>预售商品订单（<c>1</c>）。</summary>
    public const int PresaleOrder = 1;

    /// <summary>测试订单（<c>2</c>）。</summary>
    public const int TestOrder = 2;
}
