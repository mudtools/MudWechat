// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券库存信息（<c>coupon.stock_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>余量字段名反直觉（照录）</b>：官方将 <c>issued_num</c> 说明为「优惠券剩余量」、
/// <c>receive_num</c> 说明为「领用量」；<c>get</c> 页示例中 <c>stock_info</c> 为空对象 ⇒ 各字段均可空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponStockInfo
{
    /// <summary>优惠券剩余量（官方原文）。</summary>
    [JsonPropertyName("issued_num")]
    public long? IssuedNum { get; set; }

    /// <summary>优惠券领用量。</summary>
    [JsonPropertyName("receive_num")]
    public long? ReceiveNum { get; set; }

    /// <summary>优惠券已用量。</summary>
    [JsonPropertyName("used_num")]
    public long? UsedNum { get; set; }
}
