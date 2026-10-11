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

/// <summary>优惠券推广信息（官方 <c>promote_info</c> 对象，createcoupon / updatecoupon / getcoupon 三端点字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponPromoteInfo
{
    /// <summary>获取或设置推广类型（官方 <c>promote_type</c>，必填，见 <see cref="ChannelsCouponPromoteTypes"/>）。</summary>
    [JsonPropertyName("promote_type")]
    public int PromoteType { get; set; }
}

/// <summary>优惠券优惠信息（官方 <c>discount_info</c> 对象，createcoupon / updatecoupon / getcoupon 三端点字段集一致 ⇒ 共用）。</summary>
/// <remarks>官方契约：折扣数范围 1000~10000（5000=5 折）、必须是 100 的整数倍且不可低于 2 折；
/// 直减/满减金额（分）不可超过 200 元。</remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponDiscountInfo
{
    /// <summary>获取或设置优惠门槛（官方 <c>discount_condition</c>，选填）。</summary>
    [JsonPropertyName("discount_condition")]
    public ChannelsCouponDiscountCondition? DiscountCondition { get; set; }

    /// <summary>获取或设置折扣数（官方 <c>discount_num</c>，选填，5000=5 折，范围 1000~10000 且为 100 的整数倍，不可低于 2 折）。</summary>
    [JsonPropertyName("discount_num")]
    public int? DiscountNum { get; set; }

    /// <summary>获取或设置减免金额（官方 <c>discount_fee</c>，选填，单位：分，不可超过 200 元）。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }
}

/// <summary>优惠券优惠门槛（官方 <c>discount_condition</c> 对象）。</summary>
/// <remarks>官方契约：<c>product_cnt</c> 与 <c>product_price</c> 不能同时设置；商品券 <c>product_ids</c> 必填，最多 200 个。</remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponDiscountCondition
{
    /// <summary>获取或设置商品件数门槛（官方 <c>product_cnt</c>，选填，不能和价格门槛同时设置）。</summary>
    [JsonPropertyName("product_cnt")]
    public int? ProductCnt { get; set; }

    /// <summary>获取或设置商品价格门槛（官方 <c>product_price</c>，选填，单位：分，不能和件数门槛同时设置）。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }

    /// <summary>获取或设置商品 id 列表（官方 <c>product_ids</c>，选填；商品券必填，最多 200 个）。</summary>
    [JsonPropertyName("product_ids")]
    public List<string>? ProductIds { get; set; }
}

/// <summary>优惠券领取信息（官方 <c>receive_info</c> 对象，createcoupon / updatecoupon / getcoupon 三端点字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponReceiveInfo
{
    /// <summary>获取或设置领用开始时间（官方 <c>start_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置领用结束时间（官方 <c>end_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置单人限领张数（官方 <c>limit_num_one_person</c>，必填）。</summary>
    [JsonPropertyName("limit_num_one_person")]
    public int LimitNumOnePerson { get; set; }

    /// <summary>获取或设置领用总数（官方 <c>total_num</c>，必填）。</summary>
    [JsonPropertyName("total_num")]
    public int TotalNum { get; set; }
}

/// <summary>优惠券有效期信息（官方 <c>valid_info</c> 对象，createcoupon / updatecoupon / getcoupon 三端点字段集一致 ⇒ 共用）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：createcoupon / updatecoupon 参数表将 <c>start_time</c>/<c>end_time</c> 标注为
/// number（秒级时间戳）、getcoupon 参数表标注为 string 但示例仍为数字——按 create/update 参数表
/// （number）建模。官方契约：区间不可大于 365 天（错误码 10021077/10021078）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponValidInfo
{
    /// <summary>获取或设置有效期类型（官方 <c>valid_type</c>，必填：1=指定时间范围生效，2=生效天数）。</summary>
    [JsonPropertyName("valid_type")]
    public int ValidType { get; set; }

    /// <summary>获取或设置有效天数（官方 <c>valid_day_num</c>，选填，<c>valid_type=2</c> 时必填）。</summary>
    [JsonPropertyName("valid_day_num")]
    public int? ValidDayNum { get; set; }

    /// <summary>获取或设置有效期开始时间（官方 <c>start_time</c>，选填，秒级时间戳，<c>valid_type=1</c> 时必填）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置有效期结束时间（官方 <c>end_time</c>，选填，秒级时间戳，<c>valid_type=1</c> 时必填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}

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

/// <summary>优惠券自动生效信息（官方 <c>auto_valid_info</c> 对象，仅 createcoupon 提供）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponAutoValidInfo
{
    /// <summary>获取或设置自动生效类型（官方 <c>auto_valid_type</c>，必填：0=不启用，1=启用，自动生效时间为 <c>receive_info.start_time</c>）。</summary>
    [JsonPropertyName("auto_valid_type")]
    public int AutoValidType { get; set; }
}

/// <summary>更新优惠券内容（<c>updatecoupon</c>）请求体。</summary>
/// <remarks>
/// 官方契约：必填 <c>coupon_id</c>/<c>type</c>/<c>name</c>/<c>promote_info</c>/<c>receive_info</c>/<c>valid_info</c>；
/// 官方参数表无 <c>auto_valid_info</c> ⇒ 与创建请求按端点分建（嵌套对象共用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponUpdateRequest
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>，必填）。</summary>
    [JsonPropertyName("coupon_id")]
    public string CouponId { get; set; } = string.Empty;

    /// <summary>获取或设置优惠券类型（官方 <c>type</c>，必填，见 <see cref="ChannelsCouponTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置优惠券名称（官方 <c>name</c>，必填，最长 10 个中文字符）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置推广信息（官方 <c>promote_info</c>，必填）。</summary>
    [JsonPropertyName("promote_info")]
    public ChannelsCouponPromoteInfo PromoteInfo { get; set; } = new();

    /// <summary>获取或设置优惠信息（官方 <c>discount_info</c>，选填）。</summary>
    [JsonPropertyName("discount_info")]
    public ChannelsCouponDiscountInfo? DiscountInfo { get; set; }

    /// <summary>获取或设置领取信息（官方 <c>receive_info</c>，必填）。</summary>
    [JsonPropertyName("receive_info")]
    public ChannelsCouponReceiveInfo ReceiveInfo { get; set; } = new();

    /// <summary>获取或设置有效期信息（官方 <c>valid_info</c>，必填；更新形态比创建形态严格）。</summary>
    [JsonPropertyName("valid_info")]
    public ChannelsCouponValidInfo ValidInfo { get; set; } = new();

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>，选填）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsCouponExtInfo? ExtInfo { get; set; }
}

/// <summary>获取优惠券详情（<c>getcoupon</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetCouponRequest
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>，必填）。</summary>
    [JsonPropertyName("coupon_id")]
    public string CouponId { get; set; } = string.Empty;
}

/// <summary>获取优惠券详情（<c>getcoupon</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetCouponResponse : ChannelsResponse
{
    /// <summary>获取或设置优惠券信息（官方 <c>coupon</c>）。</summary>
    [JsonPropertyName("coupon")]
    public ChannelsCoupon? Coupon { get; set; }
}

/// <summary>优惠券信息（官方 <c>coupon</c> 对象，getcoupon 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCoupon
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置优惠券类型（官方 <c>type</c>，见 <see cref="ChannelsCouponTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置优惠券状态（官方 <c>status</c>，见 <see cref="ChannelsCouponStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置优惠券创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置优惠券更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置优惠券内容信息（官方 <c>coupon_info</c>）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsCouponInfo? CouponInfo { get; set; }

    /// <summary>获取或设置库存信息（官方 <c>stock_info</c>）。</summary>
    [JsonPropertyName("stock_info")]
    public ChannelsCouponStockInfo? StockInfo { get; set; }
}

/// <summary>优惠券内容信息（官方 <c>coupon_info</c> 对象，getcoupon 响应；嵌套对象与创建/更新请求共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponInfo
{
    /// <summary>获取或设置优惠券名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置推广信息（官方 <c>promote_info</c>）。</summary>
    [JsonPropertyName("promote_info")]
    public ChannelsCouponPromoteInfo? PromoteInfo { get; set; }

    /// <summary>获取或设置优惠信息（官方 <c>discount_info</c>）。</summary>
    [JsonPropertyName("discount_info")]
    public ChannelsCouponDiscountInfo? DiscountInfo { get; set; }

    /// <summary>获取或设置领取信息（官方 <c>receive_info</c>）。</summary>
    [JsonPropertyName("receive_info")]
    public ChannelsCouponReceiveInfo? ReceiveInfo { get; set; }

    /// <summary>获取或设置有效期信息（官方 <c>valid_info</c>）。</summary>
    [JsonPropertyName("valid_info")]
    public ChannelsCouponValidInfo? ValidInfo { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>，含仅响应返回的 <c>valid_time</c>/<c>invalid_time</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsCouponExtInfo? ExtInfo { get; set; }
}

/// <summary>优惠券库存信息（官方 <c>stock_info</c> 对象，getcoupon 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponStockInfo
{
    /// <summary>获取或设置优惠券剩余量（官方 <c>issued_num</c>）。</summary>
    [JsonPropertyName("issued_num")]
    public int? IssuedNum { get; set; }

    /// <summary>获取或设置优惠券领用量（官方 <c>receive_num</c>）。</summary>
    [JsonPropertyName("receive_num")]
    public int? ReceiveNum { get; set; }

    /// <summary>获取或设置优惠券已用量（官方 <c>used_num</c>）。</summary>
    [JsonPropertyName("used_num")]
    public int? UsedNum { get; set; }
}

/// <summary>获取优惠券 ID 列表（<c>getcouponlist</c>）请求体。</summary>
/// <remarks>官方契约：<c>page_ctx</c> 第一次请求填空、后续请求填上次返回值；页码间隔不能超过 10，一页最大 200 条。</remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetCouponListRequest
{
    /// <summary>获取或设置优惠券状态（官方 <c>status</c>，必填，见 <see cref="ChannelsCouponStatuses"/>，含官方复合值 200=过期或作废）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>获取或设置页码（官方 <c>page</c>，必填，最小填 1）。</summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，不超过 200）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>，必填；第一次请求填空，后续请求填上次返回值）。</summary>
    [JsonPropertyName("page_ctx")]
    public string PageCtx { get; set; } = string.Empty;
}

/// <summary>优惠券 ID 条目（官方 <c>coupons</c> 数组元素，getcouponlist 响应）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：官方参数表将 <c>coupon_id</c> 类型标注为 number，官方示例为字符串
/// <c>"123456789"</c>（getcoupon 侧同名字段为 string）——按参数表（number）建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponIdItem
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>；参数表类型 number，示例为字符串——矛盾照录）。</summary>
    [JsonPropertyName("coupon_id")]
    public long? CouponId { get; set; }
}

/// <summary>获取优惠券 ID 列表（<c>getcouponlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetCouponListResponse : ChannelsResponse
{
    /// <summary>获取或设置优惠券列表（官方 <c>coupons</c>）。</summary>
    [JsonPropertyName("coupons")]
    public List<ChannelsCouponIdItem>? Coupons { get; set; }

    /// <summary>获取或设置优惠券总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>）。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}

/// <summary>获取用户优惠券详情（<c>getusercoupon</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetUserCouponRequest
{
    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>，必填）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public string UserCouponId { get; set; } = string.Empty;

    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;
}

/// <summary>用户优惠券详情（官方 <c>user_coupon</c> 对象，getusercoupon 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsUserCoupon
{
    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public string? UserCouponId { get; set; }

    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置优惠券状态（官方 <c>status</c>，见 <see cref="ChannelsUserCouponStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置优惠券派发时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置优惠券更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置优惠券生效时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置优惠券失效时间（官方 <c>end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置优惠券使用的订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsUserCouponExtInfo? ExtInfo { get; set; }
}

/// <summary>用户优惠券扩展信息（官方 <c>user_coupon.ext_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsUserCouponExtInfo
{
    /// <summary>获取或设置优惠券核销时间（官方 <c>use_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("use_time")]
    public long? UseTime { get; set; }
}

/// <summary>获取用户优惠券详情（<c>getusercoupon</c>）响应。</summary>
/// <remarks>官方变更日志（2026-06-03）：响应参数 <c>user_coupon_list</c> 修正为 <c>user_coupon</c>——SDK 按修正后字段名建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetUserCouponResponse : ChannelsResponse
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户 unionid（官方 <c>unionid</c>；小店接入 open 平台后生成的用户券会返回）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置用户优惠券详情（官方 <c>user_coupon</c>）。</summary>
    [JsonPropertyName("user_coupon")]
    public ChannelsUserCoupon? UserCoupon { get; set; }
}

/// <summary>获取用户优惠券 ID 列表（<c>getusercouponlist</c>）请求体。</summary>
/// <remarks>官方契约：<c>page_ctx</c> 第一次请求填空、后续请求填上次返回值；页码间隔不能超过 10，一页最大 200 条。</remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetUserCouponListRequest
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置优惠券状态（官方 <c>status</c>，选填，不填时获取所有状态，见 <see cref="ChannelsUserCouponStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置页码（官方 <c>page</c>，必填）。</summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>，必填；第一次请求填空，后续请求填上次返回值）。</summary>
    [JsonPropertyName("page_ctx")]
    public string PageCtx { get; set; } = string.Empty;
}

/// <summary>用户优惠券 ID 条目（官方 <c>user_coupon_list</c> 数组元素，getusercouponlist 响应）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：官方参数表将 <c>user_coupon_id</c>/<c>coupon_id</c> 类型标注为 number，
/// 官方示例为字符串（<c>"109694341"</c> / <c>"123456789"</c>）——按参数表（number）建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsUserCouponIdItem
{
    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>；参数表类型 number，示例为字符串——矛盾照录）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public long? UserCouponId { get; set; }

    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>；参数表类型 number，示例为字符串——矛盾照录）。</summary>
    [JsonPropertyName("coupon_id")]
    public long? CouponId { get; set; }
}

/// <summary>获取用户优惠券 ID 列表（<c>getusercouponlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetUserCouponListResponse : ChannelsResponse
{
    /// <summary>获取或设置用户优惠券列表（官方 <c>user_coupon_list</c>）。</summary>
    [JsonPropertyName("user_coupon_list")]
    public List<ChannelsUserCouponIdItem>? UserCouponList { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>）。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}

/// <summary>更新优惠券状态（<c>updatecouponstatus</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponUpdateStatusRequest
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>，必填）。</summary>
    [JsonPropertyName("coupon_id")]
    public string CouponId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置目标状态（官方 <c>status</c>，必填）——官方流转约束：创建后 <c>status=1</c>（编辑中），
    /// 仅可流转到 2（生效）、4（作废）、5（删除），是 <see cref="ChannelsCouponStatuses"/> 的子集。
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}

/// <summary>创建 / 更新优惠券响应（官方 <c>data.coupon_id</c> 形态；createcoupon 与 updatecoupon 响应字段集完全一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponDataResponse : ChannelsResponse
{
    /// <summary>获取或设置返回数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCouponData? Data { get; set; }
}

/// <summary>优惠券操作返回数据（官方 <c>data</c> 对象，createcoupon / updatecoupon 响应共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponData
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }
}

/// <summary>优惠券类型常量（官方 <c>type</c> 枚举值）。</summary>
public static class ChannelsCouponTypes
{
    /// <summary>商品条件折券（必填：product_ids、product_cnt、discount_num）。</summary>
    public const int GoodsConditionalDiscount = 1;

    /// <summary>商品满减券（必填：product_ids、product_price、discount_fee）。</summary>
    public const int GoodsFullReduction = 2;

    /// <summary>商品统一折扣券（必填：product_ids、discount_num）。</summary>
    public const int GoodsUniformDiscount = 3;

    /// <summary>商品直减券（必填：product_ids、discount_fee）。</summary>
    public const int GoodsDirectReduction = 4;

    /// <summary>店铺条件折扣券（必填：product_cnt、discount_num）。</summary>
    public const int ShopConditionalDiscount = 101;

    /// <summary>店铺满减券（必填：product_price、discount_fee）。</summary>
    public const int ShopFullReduction = 102;

    /// <summary>店铺统一折扣券（必填：discount_num）。</summary>
    public const int ShopUniformDiscount = 103;

    /// <summary>店铺直减券（必填：discount_fee）。</summary>
    public const int ShopDirectReduction = 104;
}

/// <summary>优惠券推广类型常量（官方 <c>promote_info.promote_type</c> 枚举值）。</summary>
public static class ChannelsCouponPromoteTypes
{
    /// <summary>小店内推广。</summary>
    public const int InShop = 1;

    /// <summary>会员券。</summary>
    public const int MemberCoupon = 9;

    /// <summary>会员开卡礼券。</summary>
    public const int MemberCardGift = 10;

    /// <summary>小程序会员券。</summary>
    public const int MiniProgramMemberCoupon = 50;

    /// <summary>小程序会员开卡礼券。</summary>
    public const int MiniProgramMemberCardGift = 51;
}

/// <summary>优惠券状态常量（官方 <c>status</c> 枚举值，getcoupon 响应与 getcouponlist 筛选）。</summary>
public static class ChannelsCouponStatuses
{
    /// <summary>编辑中（创建后初始状态）。</summary>
    public const int Editing = 1;

    /// <summary>已生效。</summary>
    public const int Active = 2;

    /// <summary>已过期。</summary>
    public const int Expired = 3;

    /// <summary>已作废。</summary>
    public const int Invalid = 4;

    /// <summary>已删除。</summary>
    public const int Deleted = 5;

    /// <summary>过期或作废（官方 getcouponlist 筛选复合值）。</summary>
    public const int ExpiredOrInvalid = 200;
}

/// <summary>用户优惠券状态常量（官方 <c>user_coupon.status</c> 枚举值，getusercoupon / getusercouponlist）。</summary>
public static class ChannelsUserCouponStatuses
{
    /// <summary>生效中。</summary>
    public const int Active = 100;

    /// <summary>已过期。</summary>
    public const int Expired = 101;

    /// <summary>已使用。</summary>
    public const int Used = 102;
}
