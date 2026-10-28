// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>优惠券类型常量（官方 <c>type</c> 枚举值）。</summary>
public static class ChannelsCouponTypes
{
    /// <summary>商品条件折券（必填：product_ids、product_cnt、discount_num）。</summary>
    public const int GoodsConditionalDiscount = 1;

    /// <summary>商品满减券（必填：product_ids、product_price、discount_fee）。</summary>
    public const int GoodsFullReduction = 2;

    /// <summary>商品统一折扣券（必填：product_ids、discount_num）。</summary>
    public const int GoodsUniformDiscount = 3;

    /// <summary>商品直减券（必填：product_ids、discount_fee）。</summary>
    public const int GoodsDirectReduction = 4;

    /// <summary>店铺条件折扣券（必填：product_cnt、discount_num）。</summary>
    public const int ShopConditionalDiscount = 101;

    /// <summary>店铺满减券（必填：product_price、discount_fee）。</summary>
    public const int ShopFullReduction = 102;

    /// <summary>店铺统一折扣券（必填：discount_num）。</summary>
    public const int ShopUniformDiscount = 103;

    /// <summary>店铺直减券（必填：discount_fee）。</summary>
    public const int ShopDirectReduction = 104;
}
