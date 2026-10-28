// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>供货单取消原因常量（官方 <c>cancel_scene</c> 枚举值，供货 <c>supply_order_product_info</c>）。</summary>
/// <remarks><b>共用裁决（负向）</b>：代发侧 <c>cancel_scene</c> 同值不同义（1 售后 / 2 商家取消代发）⇒ 分建 <see cref="ChannelsDropshipCancelScenes"/>。</remarks>
public static class ChannelsSupplyCancelScenes
{
    /// <summary>用户售后。</summary>
    public const int UserAfterSale = 1;

    /// <summary>平台砍单。</summary>
    public const int PlatformCutOrder = 2;
}
