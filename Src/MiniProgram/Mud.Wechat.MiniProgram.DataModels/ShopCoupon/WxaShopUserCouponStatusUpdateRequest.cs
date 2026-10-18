// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 更新用户券状态请求体（<c>POST /shop/coupon/update_usercoupon_status</c>）。
/// </summary>
/// <remarks>
/// <para>平铺四支、无包装（字段集取自官方请求示例，本页无参数表）；置 102（已经使用）时须成对补记 <c>use_time</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopUserCouponStatusUpdateRequest
{
    /// <summary>用户 openid（敏感标识，禁落日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>商家侧券 id。</summary>
    [JsonPropertyName("out_coupon_id")]
    public string? OutCouponId { get; set; }

    /// <summary>商家侧用户券 id。</summary>
    [JsonPropertyName("out_user_coupon_id")]
    public string? OutUserCouponId { get; set; }

    /// <summary>目标状态（100~103）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
