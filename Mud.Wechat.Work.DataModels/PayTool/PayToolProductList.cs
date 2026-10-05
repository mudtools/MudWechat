// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 购买商品集合（<c>product_list</c>，收银台收款工具请求与订单详情响应共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方按<b>业务类型</b>三选一填充：<c>business_type=1</c>（普通第三方应用）填 <see cref="ThirdApp"/>、
/// <c>2</c>（代开发应用）填 <see cref="CustomizedApp"/>、<c>3</c>（行业解决方案）填 <see cref="PromotionCase"/>；
/// 其余两个为 <see langword="null"/>。本 SDK 照官方原文保留三个互斥分支的独立形态，
/// 不做「合成为一个超集对象」的归一（避免丢失各分支的必填口径）。
/// </para>
/// <para>
/// <b>官方契约陷阱</b>：官方代开发应用请求示例中首个 <c>total_price</c> 之后保留了逗号，
/// 该 JSON 语法无效，实际调用须删除。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolProductList
{
    /// <summary>
    /// 获取或设置普通第三方应用购买详情（官方 <c>product_list.third_app</c>）。
    /// <para>官方必填口径：<b>当业务类型是「普通第三方应用」时必填</b>。</para>
    /// </summary>
    [JsonPropertyName("third_app")]
    public PayToolThirdAppProduct? ThirdApp { get; set; }

    /// <summary>
    /// 获取或设置代开发应用购买详情（官方 <c>product_list.customized_app</c>）。
    /// <para>官方必填口径：<b>当业务类型是「代开发应用」时必填</b>。</para>
    /// </summary>
    [JsonPropertyName("customized_app")]
    public PayToolCustomizedAppProduct? CustomizedApp { get; set; }

    /// <summary>
    /// 获取或设置行业解决方案购买详情（官方 <c>product_list.promotion_case</c>）。
    /// <para>官方必填口径：<b>当业务类型是「行业解决方案」时必填</b>。</para>
    /// </summary>
    [JsonPropertyName("promotion_case")]
    public PayToolPromotionCaseProduct? PromotionCase { get; set; }
}

/// <summary>
/// 普通第三方应用购买详情（<c>product_list.third_app</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolThirdAppProduct
{
    /// <summary>
    /// 获取或设置购买类型（官方必填）：<c>0</c>-新购 / <c>1</c>-扩容 / <c>2</c>-续期。
    /// <para>官方特例：第三方应用有剩余有效时长且不超过 1 年时，支持以「新购」购买类型下单。</para>
    /// </summary>
    [JsonPropertyName("order_type")]
    public int OrderType { get; set; }

    /// <summary>
    /// 获取或设置购买应用列表（官方必填，可填充个数 1 ~ 20）。
    /// </summary>
    [JsonPropertyName("buy_info_list")]
    public List<PayToolBuyInfo> BuyInfoList { get; set; } = new List<PayToolBuyInfo>();

    /// <summary>
    /// 获取或设置是否推送确认提醒（官方可选）：<c>0</c>-否 / <c>1</c>-是，不填默认是。
    /// <para>官方限制：有指定企业且指定企业与服务商存在应用授权关系才可通知成功。</para>
    /// </summary>
    [JsonPropertyName("notify_custom_corp")]
    public int? NotifyCustomCorp { get; set; }
}

/// <summary>
/// 代开发应用购买详情（<c>product_list.customized_app</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolCustomizedAppProduct
{
    /// <summary>
    /// 获取或设置购买类型（官方必填）：<c>0</c>-新购 / <c>1</c>-扩容 / <c>2</c>-续期。
    /// <para>官方特例：代开发应用有剩余有效时长且不超过 1 年时，支持以「新购」购买类型下单。</para>
    /// </summary>
    [JsonPropertyName("order_type")]
    public int OrderType { get; set; }

    /// <summary>
    /// 获取或设置购买应用列表（官方必填，可填充个数 1 ~ 20）。
    /// </summary>
    [JsonPropertyName("buy_info_list")]
    public List<PayToolBuyInfo> BuyInfoList { get; set; } = new List<PayToolBuyInfo>();

    /// <summary>
    /// 获取或设置是否推送确认提醒（官方可选）：<c>0</c>-否 / <c>1</c>-是，不填默认是。
    /// <para>官方限制：指定的免支付订单创建后默认向企业管理员推送订单通知，<b>不可取消</b>。</para>
    /// </summary>
    [JsonPropertyName("notify_custom_corp")]
    public int? NotifyCustomCorp { get; set; }
}

/// <summary>
/// 行业解决方案购买详情（<c>product_list.promotion_case</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolPromotionCaseProduct
{
    /// <summary>
    /// 获取或设置购买类型（官方必填）：<c>0</c>-新购 / <c>1</c>-扩容 / <c>2</c>-续期。
    /// </summary>
    [JsonPropertyName("order_type")]
    public int OrderType { get; set; }

    /// <summary>
    /// 获取或设置行业方案 ID（官方必填，不多于 64 字节）。
    /// </summary>
    [JsonPropertyName("case_id")]
    public string CaseId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置行业方案版本名（官方必填，不多于 128 字节）。
    /// </summary>
    [JsonPropertyName("promotion_edition_name")]
    public string PromotionEditionName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置应用的购买时长（单位天）。
    /// <para>官方必填口径：当购买类型是新购或续期时必填，取值范围 1 ~ 1825。</para>
    /// </summary>
    [JsonPropertyName("duration_days")]
    public int? DurationDays { get; set; }

    /// <summary>
    /// 获取或设置生效日期（格式如 20221212，不多于 8 字节）。
    /// <para>官方限制：只能是当天之后的日期，最迟不能超过一年；不填表示立即生效。</para>
    /// </summary>
    [JsonPropertyName("take_effect_date")]
    public string? TakeEffectDate { get; set; }

    /// <summary>
    /// 获取或设置购买应用列表（可填充个数 1 ~ 20）。
    /// </summary>
    [JsonPropertyName("buy_info_list")]
    public List<PayToolBuyInfo>? BuyInfoList { get; set; }

    /// <summary>
    /// 获取或设置是否推送确认提醒（官方可选）：<c>0</c>-否 / <c>1</c>-是，不填默认是。
    /// </summary>
    [JsonPropertyName("notify_custom_corp")]
    public int? NotifyCustomCorp { get; set; }
}

/// <summary>
/// 购买应用明细（<c>buy_info_list</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方按业务类型给出<b>不同字段子集</b>（普通第三方应用有 <c>edition_id</c> 与 <c>discount_info</c>、
/// 代开发应用有 <c>total_price</c>、行业解决方案只有 <c>suiteid</c>/<c>appid</c>/<c>user_count</c>/<c>take_effect_date</c>），
/// 本 SDK 以一份可空超集承载：未使用的字段保持 <see langword="null"/>，字段名一律照抄官方原文。
/// </para>
/// <para>
/// <b>方向差异</b>：<see cref="DiscountInfo"/>（优惠信息）与 <see cref="TotalPrice"/>（应用总价）仅出现在
/// <b>请求</b>；<see cref="OriginPrice"/>（原价金额）与 <see cref="PaidPrice"/>（实付金额）仅出现在<b>响应</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolBuyInfo
{
    /// <summary>
    /// 获取或设置套件 ID（官方必填，不多于 64 字节）。
    /// </summary>
    [JsonPropertyName("suiteid")]
    public string SuiteId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置应用 ID（官方可选，<b>仅旧套件应用需要填</b>）。
    /// </summary>
    [JsonPropertyName("appid")]
    public int? AppId { get; set; }

    /// <summary>
    /// 获取或设置版本号 ID（普通第三方应用官方必填，不多于 64 字节）。
    /// </summary>
    [JsonPropertyName("edition_id")]
    public string? EditionId { get; set; }

    /// <summary>
    /// 获取或设置应用的购买人数（单位人，取值范围 1 ~ 1000000）。
    /// <para>官方必填口径：购买类型是新购或扩容、且购买的版本非固定总价类型时需要填写；
    /// 对扩容类型表示<b>增加</b>的人数。</para>
    /// </summary>
    [JsonPropertyName("user_count")]
    public int? UserCount { get; set; }

    /// <summary>
    /// 获取或设置应用的购买时长（单位天，取值范围 1 ~ 1825）。
    /// <para>官方必填口径：新购或续期时必填。行业解决方案分支中该字段挂在
    /// <c>promotion_case</c> 而非 <c>buy_info_list</c> 元素上。</para>
    /// </summary>
    [JsonPropertyName("duration_days")]
    public int? DurationDays { get; set; }

    /// <summary>
    /// 获取或设置生效日期（格式如 20221212，不多于 8 字节）。
    /// <para>官方限制：最迟不能超过一年；从生效日期当天开始计算购买时长。
    /// 官方特例：应用有剩余时长的「新购」不支持设置生效日期，
    /// 默认生效日期为该应用生效版本的结束日期。</para>
    /// </summary>
    [JsonPropertyName("take_effect_date")]
    public string? TakeEffectDate { get; set; }

    /// <summary>
    /// 获取或设置应用总价（单位分，<b>仅代开发应用请求</b>）。
    /// <para>官方约束：需大于 0 且不能超过 500 万（单位分）。</para>
    /// </summary>
    [JsonPropertyName("total_price")]
    public long? TotalPrice { get; set; }

    /// <summary>
    /// 获取或设置优惠信息（<b>仅普通第三方应用请求</b>，见 <see cref="PayToolDiscountInfo"/>）。
    /// </summary>
    [JsonPropertyName("discount_info")]
    public PayToolDiscountInfo? DiscountInfo { get; set; }

    /// <summary>
    /// 获取或设置原价金额（<b>仅响应</b>，单位分）。
    /// </summary>
    [JsonPropertyName("origin_price")]
    public long? OriginPrice { get; set; }

    /// <summary>
    /// 获取或设置实付金额（<b>仅响应</b>，单位分；免支付订单返回 0）。
    /// </summary>
    [JsonPropertyName("paid_price")]
    public long? PaidPrice { get; set; }
}

/// <summary>
/// 优惠信息（<c>buy_info_list[].discount_info</c>，仅普通第三方应用请求）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolDiscountInfo
{
    /// <summary>
    /// 获取或设置优惠类型（官方必填）：<c>1</c>-固定优惠 / <c>2</c>-价格折扣。
    /// <para>官方限制：如果不是新购，<b>必须指定了客户企业 <c>custom_corpid</c></b> 才能选择固定优惠。</para>
    /// </summary>
    [JsonPropertyName("discount_type")]
    public int DiscountType { get; set; }

    /// <summary>
    /// 获取或设置优惠金额（单位分，取值范围 1 ~ 10000000；<b>优惠类型为「固定优惠」时必填</b>）。
    /// <para>官方限制：非推荐第三方应用固定优惠金额不能超过原价的 90%；
    /// 推荐第三方应用不能超过原价的 50%（超过需前往服务商管理端操作）。</para>
    /// </summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>
    /// 获取或设置优惠折扣（单位 %，如填 75 表示 75% 优惠价即 7.5 折；
    /// <b>优惠类型为「价格折扣」时必填</b>，取值范围 10 ~ 99）。
    /// <para>官方限制：非推荐第三方应用优惠折扣最低 1 折；推荐第三方应用最低 5 折
    /// （低于 5 折需前往服务商管理端操作）。</para>
    /// </summary>
    [JsonPropertyName("discount_ratio")]
    public int? DiscountRatio { get; set; }

    /// <summary>
    /// 获取或设置优惠原因（官方必填，不多于 256 字节；客户侧可见）。
    /// </summary>
    [JsonPropertyName("discount_remarks")]
    public string DiscountRemarks { get; set; } = string.Empty;
}
