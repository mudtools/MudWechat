// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 更新优惠券状态请求体（<c>POST /shop/coupon/update_status</c>）。
/// </summary>
/// <remarks>
/// <para>字段集取自官方请求示例（本页无请求参数表）；状态机单向不可逆：1→2 不可回退，3/4 只可由 2 进入。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponStatusUpdateRequest
{
    /// <summary>商家侧券 id。</summary>
    [JsonPropertyName("out_coupon_id")]
    public string? OutCouponId { get; set; }

    /// <summary>目标状态（1 未生效编辑中 / 2 生效 / 3 已过期 / 4 已作废）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
