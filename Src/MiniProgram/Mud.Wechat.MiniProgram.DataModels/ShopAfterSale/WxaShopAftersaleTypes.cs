// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>V1 售后类型（<c>aftersale.type</c>）。</summary>
/// <remarks>
/// <para>V1 比 V2 多一支 <c>3</c>（换货）；V2 <see cref="WxaEcAftersaleTypes"/> 只有 1/2 ⇒ 两代值域不得互用。</para>
/// </remarks>
public static class WxaShopAftersaleTypes
{
    /// <summary>1 退款（仅退款）。</summary>
    public const int Refund = 1;

    /// <summary>2 退款退货。</summary>
    public const int ReturnRefund = 2;

    /// <summary>3 换货（V1 独有）。</summary>
    public const int Exchange = 3;
}
