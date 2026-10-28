// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>优惠券类型（create / get / get_list 的 <c>type</c>）。</summary>
/// <remarks>
/// <para>官方枚举按「商品券 1~4 / 店铺券 101~104」分两组；每型的必填字段组合见官方 create 页枚举表。</para>
/// </remarks>
public static class WxaProductCouponTypes
{
    /// <summary>1 商品条件折扣券（product_ids + product_cnt + discount_num）。</summary>
    public const int ProductConditionDiscount = 1;

    /// <summary>2 商品满减券（product_ids + product_price + discount_fee）。</summary>
    public const int ProductFullReduction = 2;

    /// <summary>3 商品统一折扣券（product_ids + discount_num）。</summary>
    public const int ProductUniformDiscount = 3;

    /// <summary>4 商品直减券（product_ids + discount_fee）。</summary>
    public const int ProductDirectReduction = 4;

    /// <summary>101 店铺条件折扣券（product_cnt + discount_num）。</summary>
    public const int ShopConditionDiscount = 101;

    /// <summary>102 店铺满减券（product_price + discount_fee）。</summary>
    public const int ShopFullReduction = 102;

    /// <summary>103 店铺统一折扣券（discount_num）。</summary>
    public const int ShopUniformDiscount = 103;

    /// <summary>104 店铺直减券（discount_fee）。</summary>
    public const int ShopDirectReduction = 104;
}
