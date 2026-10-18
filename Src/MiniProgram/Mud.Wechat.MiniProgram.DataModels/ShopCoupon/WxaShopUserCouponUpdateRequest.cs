// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 更新用户券请求体（<c>POST /shop/coupon/update_user_coupon</c>）。
/// </summary>
/// <remarks>
/// <para>与添加侧两处差异：<c>user_coupon.status</c> 不在本页参数表内、<c>recv_time</c> 在本页<b>必填</b> ⇒ 两请求体不得合并。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopUserCouponUpdateRequest
{
    /// <summary>用户 openid（敏感标识，禁落日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户券业务体（<c>user_coupon</c> 包装）。</summary>
    [JsonPropertyName("user_coupon")]
    public WxaShopUserCoupon? UserCoupon { get; set; }

    /// <summary>领取时间（秒级时间戳，本页必填）。</summary>
    [JsonPropertyName("recv_time")]
    public long? RecvTime { get; set; }
}
