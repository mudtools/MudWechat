// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingPartnerProductCoupon;

/// <summary>创建商品券图片生成任务请求·【拼图数据】 当 `image_generation_type` 为 `COMB·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327735"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskCreateRequestCombineImageCombineImagenormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>combine_image.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>combine_image.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券图片生成任务请求·【拼图数据】 当 `image_generation_type` 为 `COMB·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327735"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskCreateRequestCombineImageCombineImagediscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>combine_image.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>combine_image.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券图片生成任务请求·【拼图数据】 当 `image_generation_type` 为 `COMB·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327735"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskCreateRequestCombineImageCombineImageexchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>combine_image.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>combine_image.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>创建商品券图片生成任务请求·【拼图数据】 当 `image_generation_type` 为 `COMB。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327735"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskCreateRequestCombineImage
{
    /// <summary>【优惠范围】 商品券的优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>combine_image.scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>combine_image.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式，当前仅支持 `SINGLE` 模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>combine_image.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>combine_image.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponImageTaskCreateRequestCombineImageCombineImagenormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写（官方字段路径 <c>combine_image.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponImageTaskCreateRequestCombineImageCombineImagediscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>combine_image.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponImageTaskCreateRequestCombineImageCombineImageexchangeCoupon? ExchangeCoupon { get; set; }

    /// <summary>【背景颜色色值】 指定背景色（要求6字节Hex描述以#开头），用于指定拼图品牌色（官方字段路径 <c>combine_image.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

}

/// <summary>创建商品券图片生成任务请求·【抠图数据】 当 `image_generation_type` 为 `CUT_。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327735"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskCreateRequestCutOut
{
    /// <summary>【商品图URL】 填写待处理图片的URL，仅支持储存在https://wxpaylogo.qpic.cn域名的图片（官方字段路径 <c>cut_out.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

}

/// <summary>创建商品券图片生成任务请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327735"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskCreateRequest
{
    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【图片生成的任务ID】 服务商创建图片生成任务的请求流水号，服务商侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>task_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>【图片生成类型】 图片生成类型。可选取值：`COMBINE_IMAGE` 适合全场类优惠头图，将品牌元素与商品券优惠等信息拼接生成头图 / `CUT_OUT` 适合单品类优惠头图，用于去除图片背景，保留商品主体（官方字段路径 <c>image_generation_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_generation_type")]
    public string? ImageGenerationType { get; set; }

    /// <summary>【拼图数据】 当 `image_generation_type` 为 `COMBINE_IMAGE` 时，必填（官方字段路径 <c>combine_image</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("combine_image")]
    public ProductCouponImageTaskCreateRequestCombineImage? CombineImage { get; set; }

    /// <summary>【抠图数据】 当 `image_generation_type` 为 `CUT_OUT` 时，必填（官方字段路径 <c>cut_out</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("cut_out")]
    public ProductCouponImageTaskCreateRequestCutOut? CutOut { get; set; }

}

/// <summary>创建商品券图片生成任务响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327735"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskCreateResponse : WechatPayResponse
{
    /// <summary>【图片生成的任务ID】 服务商创建图片生成任务的请求流水号，服务商侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>task_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>查询商品券图片生成任务响应·【拼图结果】 当 `image_generation_type` 为 `COMB。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327739"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskQueryResponseCombineImageResult
{
    /// <summary>【生成的图片URL】 当任务状态为 `SUCCESS` 时，返回该值，其他情况下不会返回（官方字段路径 <c>combine_image_result.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

}

/// <summary>查询商品券图片生成任务响应·【生成商品图结果】 当 `image_generation_type` 为 `C。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327739"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskQueryResponseCutOutResult
{
    /// <summary>【生成的图片URL】 当任务状态为 `SUCCESS` 时，返回该值，其他情况下不会返回（官方字段路径 <c>cut_out_result.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

}

/// <summary>查询商品券图片生成任务响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4017327739"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponImageTaskQueryResponse : WechatPayResponse
{
    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【图片生成的任务ID】 服务商创建图片生成任务的请求流水号，服务商侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>task_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>【图片生成类型】 图片生成类型。可选取值：`COMBINE_IMAGE` 适合全场类优惠头图，将品牌元素与商品券优惠等信息拼接生成头图 / `CUT_OUT` 适合单品类优惠头图，用于去除图片背景，保留商品主体（官方字段路径 <c>image_generation_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_generation_type")]
    public string? ImageGenerationType { get; set; }

    /// <summary>【任务状态】 任务状态。可选取值：`PROCESSING` 任务执行中 / `FAIL` 任务失败 / `SUCCESS` 任务成功（官方字段路径 <c>task_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("task_state")]
    public string? TaskState { get; set; }

    /// <summary>【拼图结果】 当 `image_generation_type` 为 `COMBINE_IMAGE` ，并且任务状态为 `SUCCESS` 时，该数据结构值有效（官方字段路径 <c>combine_image_result</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("combine_image_result")]
    public ProductCouponImageTaskQueryResponseCombineImageResult? CombineImageResult { get; set; }

    /// <summary>【生成商品图结果】 当 `image_generation_type` 为 `CUT_OUT` ，并且任务状态为 `SUCCESS` 时，该数据结构值有效（官方字段路径 <c>cut_out_result</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("cut_out_result")]
    public ProductCouponImageTaskQueryResponseCutOutResult? CutOutResult { get; set; }

}
