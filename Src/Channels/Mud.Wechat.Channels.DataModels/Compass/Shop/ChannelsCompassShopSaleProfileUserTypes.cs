// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>罗盘商家版人群用户类型常量（官方 <c>type</c> 枚举值，<c>getshopsaleprofiledata</c>）。</summary>
public static class ChannelsCompassShopSaleProfileUserTypes
{
    /// <summary>商品曝光用户。</summary>
    public const int ProductExposureUser = 1;

    /// <summary>商品点击用户。</summary>
    public const int ProductClickUser = 2;

    /// <summary>购买用户。</summary>
    public const int PurchaseUser = 3;

    /// <summary>首购用户。</summary>
    public const int FirstPurchaseUser = 4;

    /// <summary>复购用户。</summary>
    public const int RepeatPurchaseUser = 5;
}
