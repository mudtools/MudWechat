// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>优惠券类型常量（官方 <c>type</c> 枚举值，<c>promoter/get_coupon_detail</c>）。</summary>
public static class ChannelsPromoterCouponTypes
{
    /// <summary>商品条件折券，<c>discount_condition.product_ids</c>、<c>discount_condition.product_cnt</c>、<c>discount_info.discount_num</c> 必填。</summary>
    public const int ProductConditionDiscount = 1;

    /// <summary>商品满减券，<c>discount_condition.product_ids</c>、<c>discount_condition.product_price</c>、<c>discount_info.discount_fee</c> 必填。</summary>
    public const int ProductFullReduce = 2;

    /// <summary>商品统一折扣券，<c>discount_condition.product_ids</c>、<c>discount_info.discount_num</c> 必填。</summary>
    public const int ProductUniformDiscount = 3;

    /// <summary>商品直减券，<c>discount_condition.product_ids</c>、<c>discount_fee</c> 必填；低于可用商品最小价格时提醒。</summary>
    public const int ProductDirectReduce = 4;

    /// <summary>店铺条件折扣券，<c>discount_condition.product_cnt</c>、<c>discount_info.discount_num</c> 必填。</summary>
    public const int ShopConditionDiscount = 101;

    /// <summary>店铺满减券，<c>discount_condition.product_price</c>、<c>discount_info.discount_fee</c> 必填。</summary>
    public const int ShopFullReduce = 102;

    /// <summary>店铺统一折扣券，<c>discount_info.discount_num</c> 必填。</summary>
    public const int ShopUniformDiscount = 103;

    /// <summary>店铺直减券，<c>discount_fee</c> 必填；低于可用商品最小价格时提醒。</summary>
    public const int ShopDirectReduce = 104;
}
