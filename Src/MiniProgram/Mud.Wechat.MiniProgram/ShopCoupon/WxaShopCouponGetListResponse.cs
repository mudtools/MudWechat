// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 获取优惠券列表应答（<c>POST /shop/coupon/get_list</c>）。
/// </summary>
/// <remarks>
/// <para>顶层 <c>total_num</c> + <c>result_list</c>，<b>无</b> <c>result</c> 包装；与单查应答形态不同，两类型不得互抄。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponGetListResponse : WxaResponse
{
    /// <summary>总数。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>券条目列表。</summary>
    [JsonPropertyName("result_list")]
    public List<WxaShopCouponListItem>? ResultList { get; set; }
}
