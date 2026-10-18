// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 添加用户券请求体（<c>POST /shop/coupon/add_user_coupon</c>）。
/// </summary>
/// <remarks>
/// <para>顶层 <c>openid</c> + <c>user_coupon</c> 包装 + 选填 <c>recv_time</c>；四个业务键全部必填。<c>openid</c> 属敏感标识，禁入日志。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopUserCouponAddRequest
{
    /// <summary>用户 openid（敏感标识，禁落日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户券业务体（<c>user_coupon</c> 包装）。</summary>
    [JsonPropertyName("user_coupon")]
    public WxaShopUserCoupon? UserCoupon { get; set; }

    /// <summary>领取时间（秒级时间戳，选填）。</summary>
    [JsonPropertyName("recv_time")]
    public long? RecvTime { get; set; }
}
