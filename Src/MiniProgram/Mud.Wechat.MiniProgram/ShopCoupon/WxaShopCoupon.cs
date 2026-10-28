// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 优惠券本体（官方 <c>Object coupon</c>，添加 / 更新 / 详情 / 列表四族共用同一份参数表定义）。
/// </summary>
/// <remarks>
/// <para>
/// <b>一型承载请求与应答两向</b>：官方 <c>add_coupon</c> / <c>update_coupon</c> 两页的参数表逐行一致
/// （仅更新侧多出选填的 <c>status</c>），<c>get_coupon</c> / <c>get_coupon_list</c> 的回包参数表是同一张表的超集
/// （多出 <c>status</c> / <c>create_time</c> / <c>update_time</c> / <c>appid</c> 四支回显字段）⇒ 本类按<b>可空并集</b>建模，
/// 请求侧未填的字段由 <c>WhenWritingNull</c> 自动省略，不得为省字段拆出两套互相抄写的类型。
/// </para>
/// <para>
/// <b>覆盖式写入</b>：官方 <c>coupon</c> 请求体是<b>整体覆盖</b>（非 JSON Patch），漏传字段即失去原值
/// ⇒ 更新前必须先经 <c>/shop/coupon/get</c> 读回本对象再改再写。
/// </para>
/// <para>
/// <b><c>out_coupon_id</c> 是商家侧自拟主键</b>：官方不回服务端 id，详情 / 更新 / 库存 / 状态四支全以它定位
/// ⇒ 必须跨批次唯一且不可复用；重复添加属覆盖语义、<b>非幂等安全</b>。
/// </para>
/// <para><b>敏感面</b>：<c>appid</c> 是商家小程序标识、<c>promote_info.finder.nickname</c> 是推广视频号昵称 ⇒ 禁写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCoupon
{
    /// <summary>商家侧优惠券 ID（<c>out_coupon_id</c>，必填）。</summary>
    [JsonPropertyName("out_coupon_id")]
    public string? OutCouponId { get; set; }

    /// <summary>优惠券类型（<c>type</c>，必填），取值见 <see cref="WxaShopCouponTypes"/>；类型决定 <see cref="CouponInfo"/> 内的必填集。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>优惠券推广类型（<c>promote_type</c>，必填），官方枚举目前仅 <c>4</c>（视频号直播）；须与 <see cref="WxaShopCouponPromoteInfo.PromoteType"/> 同值双传。</summary>
    [JsonPropertyName("promote_type")]
    public int? PromoteType { get; set; }

    /// <summary>券面信息（<c>coupon_info</c>，必填），见 <see cref="WxaShopCouponInfo"/>。</summary>
    [JsonPropertyName("coupon_info")]
    public WxaShopCouponInfo? CouponInfo { get; set; }

    /// <summary>优惠券状态（<c>status</c>）：1 未生效编辑中 / 2 生效 / 3 已过期 / 4 已作废，取值见 <see cref="WxaShopCouponStatuses"/>。<b>请求侧仅更新页承认该字段且选填</b>，新建券恒为 1；改状态主入口是 <c>/shop/coupon/update_status</c>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>创建时间（<c>create_time</c>，秒级时间戳）。<b>只在应答里出现</b>，请求侧不得填。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>更新时间（<c>update_time</c>，秒级时间戳）。<b>只在应答里出现</b>，请求侧不得填。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>小程序 AppID（<c>appid</c>）。<b>只在列表应答里出现</b>；商家标识信息，禁落日志。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
