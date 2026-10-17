// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>商品状态常量（官方 <c>product_info.status</c> 枚举值，<c>promoter/get_promote_product_detail</c>）。</summary>
public static class ChannelsPromoterProductStatuses
{
    /// <summary>初始值。</summary>
    public const int Initial = 0;

    /// <summary>上架。</summary>
    public const int OnSale = 5;

    /// <summary>回收站。</summary>
    public const int RecycleBin = 6;

    /// <summary>彻底删除，商品无法再进行任何操作。</summary>
    public const int Deleted = 9;

    /// <summary>自主下架。</summary>
    public const int SelfOffShelf = 11;

    /// <summary>违规下架/风控系统下架。</summary>
    public const int ViolationOffShelf = 13;

    /// <summary>保证金不足下架。</summary>
    public const int DepositInsufficient = 14;

    /// <summary>品牌过期下架。</summary>
    public const int BrandExpired = 15;

    /// <summary>商品被封禁。</summary>
    public const int Banned = 20;
}
