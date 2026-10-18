// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 获取优惠券列表请求体（<c>POST /shop/coupon/get_list</c>）。
/// </summary>
/// <remarks>
/// <para>偏移式翻页：<c>page_size</c> 必填（官方最大 200）、<c>offset</c> 从 0 开始。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponGetListRequest
{
    /// <summary>每页条数（必填，最大 200）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>偏移量（从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
}
