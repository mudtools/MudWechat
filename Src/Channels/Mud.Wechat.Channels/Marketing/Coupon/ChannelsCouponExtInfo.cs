// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>优惠券扩展信息（官方 <c>ext_info</c> 对象，createcoupon / updatecoupon / getcoupon 三端点共用）。</summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾照录（jump_product_id 类型）</b>：createcoupon 参数表标注为 number，
/// updatecoupon / getcoupon 参数表标注为 string，且商品 id 全域均为字符串形态（如 <c>product_ids</c>）——
/// 按 string 建模。
/// </para>
/// <para><c>valid_time</c>/<c>invalid_time</c> 仅 getcoupon 响应返回（create/update 参数表未登记，照官方 getcoupon 表补全为超集）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponExtInfo
{
    /// <summary>获取或设置领取后跳转的商品 id（官方 <c>jump_product_id</c>，选填；商品折扣券使用）。</summary>
    [JsonPropertyName("jump_product_id")]
    public string? JumpProductId { get; set; }

    /// <summary>获取或设置备注信息（官方 <c>notes</c>，选填）。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>获取或设置优惠券有效时间（官方 <c>valid_time</c>，秒级时间戳；仅 getcoupon 响应返回）。</summary>
    [JsonPropertyName("valid_time")]
    public long? ValidTime { get; set; }

    /// <summary>获取或设置优惠券失效时间（官方 <c>invalid_time</c>，秒级时间戳；仅 getcoupon 响应返回）。</summary>
    [JsonPropertyName("invalid_time")]
    public long? InvalidTime { get; set; }
}
