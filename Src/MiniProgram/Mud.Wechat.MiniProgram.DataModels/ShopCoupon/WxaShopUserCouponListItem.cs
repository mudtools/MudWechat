// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 用户券列表条目（官方 <c>result_list[]</c> 元素，<b>平铺</b>）。
/// </summary>
/// <remarks>
/// <para>条目直接把 <c>openid</c> 与业务字段放在同一层，无 <c>user_coupon</c> 包装；<c>start_time</c>/<c>end_time</c> 官方示例会出现 <c>0</c>（未填有效期），不得当作合法时间戳。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopUserCouponListItem
{
    /// <summary>商家侧用户券 id。</summary>
    [JsonPropertyName("out_user_coupon_id")]
    public string? OutUserCouponId { get; set; }

    /// <summary>用户 openid（敏感标识，禁落日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>商家侧券 id。</summary>
    [JsonPropertyName("out_coupon_id")]
    public string? OutCouponId { get; set; }

    /// <summary>用户券状态（100~103）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>创建时间（秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>更新时间（秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>有效期开始（秒级时间戳，可能为 0）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>有效期结束（秒级时间戳，可能为 0）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>扩展信息（未核销时可能为空对象）。</summary>
    [JsonPropertyName("ext_info")]
    public WxaShopUserCouponExtInfo? ExtInfo { get; set; }
}
