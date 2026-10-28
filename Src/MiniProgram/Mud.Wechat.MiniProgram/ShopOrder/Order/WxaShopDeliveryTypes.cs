// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 配送方式取值（官方 <c>delivery_type</c> 四值，写侧与读侧同源）。
/// </summary>
/// <remarks>
/// <para><b>地址信息随配送方式而变</b>：官方 <c>/shop/order/add</c> 明写 <c>delivery_type = 2</c> 无需设置
/// <c>address_info</c>、<c>= 4</c> 时该地址填的是<b>自提门店地址</b>而非收件人地址 ⇒ 校验分支须由宿主实现。</para>
/// <para><b>发货侧另有约束</b>：<c>/shop/delivery/send</c> 的 <c>delivery_list</c> 官方标注「delivery_type=1 时必填」。</para>
/// </remarks>
public static class WxaShopDeliveryTypes
{
    /// <summary>1 正常快递（默认值；视频号场景唯一支持值）。</summary>
    public const int NormalExpress = 1;

    /// <summary>2 无需快递。</summary>
    public const int NoDelivery = 2;

    /// <summary>3 线下配送。</summary>
    public const int OfflineDelivery = 3;

    /// <summary>4 用户自提（此时 address_info 为门店地址）。</summary>
    public const int SelfPickup = 4;
}
