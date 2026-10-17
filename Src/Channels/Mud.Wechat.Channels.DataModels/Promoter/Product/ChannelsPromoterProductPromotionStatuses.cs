// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>商品带货状态常量（官方 <c>commission_info.status</c> 枚举值，<c>promoter/get_promote_product_detail</c>）。</summary>
public static class ChannelsPromoterProductPromotionStatuses
{
    /// <summary>已上架推广（可以进行推客带货）。</summary>
    public const int OnShelf = 1;

    /// <summary>已下架推广（计划已下架，原因为上游主动下架或商品失效等）。</summary>
    public const int OffShelf = 2;

    /// <summary>待选品（商家定向机构推广、尚未添加到机构合作商品列表中，可以进行推客带货）。</summary>
    public const int PendingSelection = 3;

    /// <summary>已清退（计划已下架，原因为命中联盟清退规则）。</summary>
    public const int Retired = 5;
}
