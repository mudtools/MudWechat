// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券扩展信息（<c>ext_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方 create / update 页写时间戳、get / get_list 回包写字符串</b>：请求侧
/// <c>valid_time</c> / <c>invalid_time</c> 为 Unix 秒级时间戳（number），回包侧同名字段为
/// <c>yyyy-MM-dd HH:mm:ss</c> 字符串 ⇒ 读写不同型，故本请求类与 <see cref="WxaProductCouponInfo"/>
/// 内的同名结构分开建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponExtInfo
{
    /// <summary>商品折扣券领取后跳转的商品 id。</summary>
    [JsonPropertyName("jump_product_id")]
    public long? JumpProductId { get; set; }

    /// <summary>备注信息。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>优惠券有效时间戳（请求侧，秒级）。</summary>
    [JsonPropertyName("valid_time")]
    public long? ValidTime { get; set; }

    /// <summary>优惠券失效时间戳（请求侧，秒级）。</summary>
    [JsonPropertyName("invalid_time")]
    public long? InvalidTime { get; set; }
}
