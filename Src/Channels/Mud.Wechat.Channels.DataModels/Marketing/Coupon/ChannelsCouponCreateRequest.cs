// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>创建优惠券（<c>createcoupon</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：必填 <c>type</c>/<c>name</c>/<c>promote_info</c>/<c>receive_info</c>；
/// 创建形态支持 <c>auto_valid_info</c>（更新形态无此对象）、<c>valid_info</c> 选填（更新形态必填）⇒ 与更新请求按端点分建。</para>
/// <para>优惠券类型 <c>type</c> 决定 <c>discount_info</c> 内必填字段组（见 <see cref="ChannelsCouponTypes"/>）；
/// 所有时间字段均为秒级时间戳。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponCreateRequest
{
    /// <summary>获取或设置优惠券类型（官方 <c>type</c>，必填，见 <see cref="ChannelsCouponTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置优惠券名称（官方 <c>name</c>，必填，最长 10 个中文字符）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置推广信息（官方 <c>promote_info</c>，必填）。</summary>
    [JsonPropertyName("promote_info")]
    public ChannelsCouponPromoteInfo PromoteInfo { get; set; } = new();

    /// <summary>获取或设置优惠信息（官方 <c>discount_info</c>，选填；商品券必填 <c>product_ids</c>，最多 200 个）。</summary>
    [JsonPropertyName("discount_info")]
    public ChannelsCouponDiscountInfo? DiscountInfo { get; set; }

    /// <summary>获取或设置领取信息（官方 <c>receive_info</c>，必填）。</summary>
    [JsonPropertyName("receive_info")]
    public ChannelsCouponReceiveInfo ReceiveInfo { get; set; } = new();

    /// <summary>获取或设置有效期信息（官方 <c>valid_info</c>，创建形态选填）。</summary>
    [JsonPropertyName("valid_info")]
    public ChannelsCouponValidInfo? ValidInfo { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>，选填）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsCouponExtInfo? ExtInfo { get; set; }

    /// <summary>获取或设置自动生效信息（官方 <c>auto_valid_info</c>，选填；仅创建形态提供，自动生效时间为 <c>receive_info.start_time</c>）。</summary>
    [JsonPropertyName("auto_valid_info")]
    public ChannelsCouponAutoValidInfo? AutoValidInfo { get; set; }
}
