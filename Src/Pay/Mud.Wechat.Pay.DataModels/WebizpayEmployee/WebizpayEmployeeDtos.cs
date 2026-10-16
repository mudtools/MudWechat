// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.WebizpayEmployee;

/// <summary>企业商户为员工下发企业支付额度卡请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507148"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayEmployee")]
public class WebizpayQuotaCardApplyRequest
{
    /// <summary>【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【额度卡模板 ID】 额度卡模板ID，指定使用的额度卡模板，决定额度卡的使用规则、退款规则等，请联系微信支付侧申请卡模板ID（官方字段路径 <c>card_template_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("card_template_id")]
    public string? CardTemplateId { get; set; }

    /// <summary>【卡可用金额】 卡可用金额，单位为分，整型。 示例：1元应填写 100（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【卡片名称】 卡片名称，企业自定义的额度卡名称，用于标识额度卡用途，如'报销额度卡'、'差旅费用卡'等。 长度不超过20个字符（官方字段路径 <c>card_name</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("card_name")]
    public string? CardName { get; set; }

    /// <summary>【商户卡号】 商户卡号，由商户系统生成的唯一标识额度卡的编号，用于后续查询、作废等操作。 要求6-32个字符内，只能是数字、大小写字母_-&amp;#124;\* 且在同一个商户号下唯一（官方字段路径 <c>out_card_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_card_no")]
    public string? OutCardNo { get; set; }

    /// <summary>【卡片有效期截止时间】 卡片有效期截止时间 要求必须大于当前时间。在超过这个时间之后，企业支付额度卡不再可用，无法拉起付款码页面，已经入付款码页面的用户无法进行付款。 格式要求：需遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）…（官方字段路径 <c>effective_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("effective_end_time")]
    public string? EffectiveEndTime { get; set; }

    /// <summary>【卡片备注】 卡片备注，额度卡的补充说明信息，如'2023年1月差旅报销'、'项目经费'等 长度不超过50个字符（官方字段路径 <c>card_remark</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("card_remark")]
    public string? CardRemark { get; set; }

    /// <summary>【可用商户范围ID】 可用商户范围ID，用于限制该额度卡的可用商户范围。通过创建可用商户范围接口获取，将额度卡的使用限制在指定的商户集合内。 如果不填写，则该额度卡可在所有支持企业支付的商户使用。 长度不超过64个字符（官方字段路径 <c>avail_mch_range_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("avail_mch_range_id")]
    public string? AvailMchRangeId { get; set; }

}

/// <summary>企业商户为员工下发企业支付额度卡响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507148"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayEmployee")]
public class WebizpayQuotaCardApplyResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号，具有企业支付服务商资质的微信支付商户号，由商户请求时传入（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资子商户号，企业支付的出资方商户号，指定额度卡资金来源，由商户请求时传入（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户卡号】 商户卡号，由商户系统生成的唯一标识额度卡的编号，由商户下发额度卡时传入（官方字段路径 <c>out_card_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_card_no")]
    public string? OutCardNo { get; set; }

    /// <summary>【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方字段路径 <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("employee_id")]
    public string? EmployeeId { get; set; }

    /// <summary>【企业支付额度卡卡号】 企业支付额度卡卡号，微信支付平台生成的唯一标识额度卡的编号 示例：1068019671702503425（官方字段路径 <c>card_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("card_no")]
    public string? CardNo { get; set; }

    /// <summary>【卡片生效时间】 卡片生效时间，额度卡开始生效的时间。由商户下发额度卡时传入 格式要求：需遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月…（官方字段路径 <c>effective_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("effective_begin_time")]
    public string? EffectiveBeginTime { get; set; }

    /// <summary>【卡片有效期截止时间】 卡片有效期截止时间，额度卡失效的时间。由商户下发额度卡时传入。 格式要求：需遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间20…（官方字段路径 <c>effective_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("effective_end_time")]
    public string? EffectiveEndTime { get; set; }

}

/// <summary>企业商户查询企业支付额度卡响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507119"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayEmployee")]
public class WebizpayQuotaCardQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号，具有企业支付服务商资质的微信支付商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资子商户号，企业支付的出资方商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【企业支付额度卡卡号】 企业支付额度卡卡号，微信支付平台生成的唯一标识额度卡的编号 示例：1068019671702503425（官方字段路径 <c>card_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("card_no")]
    public string? CardNo { get; set; }

    /// <summary>【商户卡号】 商户卡号，由商户系统生成的唯一标识额度卡的编号（官方字段路径 <c>out_card_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_card_no")]
    public string? OutCardNo { get; set; }

    /// <summary>【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方字段路径 <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("employee_id")]
    public string? EmployeeId { get; set; }

    /// <summary>【企业支付卡模板ID】 企业支付卡模板ID，额度卡使用的模板ID，决定卡片的使用规则、退款规则等。可以咨询微信支付了解卡模板ID对应规则（官方字段路径 <c>card_template_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("card_template_id")]
    public string? CardTemplateId { get; set; }

    /// <summary>【使用规则】 单次使用/多次使用，卡模板中配置项。可选取值：`SINGLE` 单次使用 / `MULTIPLE` 多次使用（官方字段路径 <c>card_rule</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_rule")]
    public string? CardRule { get; set; }

    /// <summary>【退款规则】 退款规则，描述额度卡的退款策略 退回：'RETURN'不退回：'NOT_RETURN'。可选取值：`RETURN` 退款后返回卡 / `NOT_RETURN` 退款后不返回卡（官方字段路径 <c>refund_rule</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("refund_rule")]
    public string? RefundRule { get; set; }

    /// <summary>【卡片总金额】 卡片总金额，单位为分，整型。示例：1元应填写 100（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【卡片可用金额】 卡片可用金额，单位为分，整型。示例：1元应填写 100（官方字段路径 <c>remain_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("remain_amount")]
    public long? RemainAmount { get; set; }

    /// <summary>【卡片有效期】 卡片有效期，额度卡失效的时间，超过此日期的卡无法被使用，无法获得卡片的微信支付跳转链接。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时…（官方字段路径 <c>effective_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("effective_end_time")]
    public string? EffectiveEndTime { get; set; }

    /// <summary>【卡片生效时间】 卡片生效时间，额度卡开始生效的时间。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>effective_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("effective_begin_time")]
    public string? EffectiveBeginTime { get; set; }

    /// <summary>【卡片状态】 卡片状态，表示额度卡当前的状态，包括生效(ACTIVE)和失效(DISABLED)两种状态。可选取值：`ACTIVE` 生效 / `DISABLED` 失效（官方字段路径 <c>card_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_state")]
    public string? CardState { get; set; }

    /// <summary>【卡片名称】 卡片名称，企业自定义的额度卡名称，用于标识额度卡用途。由商户下发额度卡是传入（官方字段路径 <c>card_name</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("card_name")]
    public string? CardName { get; set; }

    /// <summary>【失效原因】 失效原因，说明额度卡失效的原因，如'有效期结束'、'管理员作废'等（官方字段路径 <c>reason</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>【卡片使用时间】 卡片使用时间，最近一次使用额度卡进行支付的时间。 对单次卡来说，为第一次支付的时间，此后卡片过期。 对多次卡来说，为最后一次支付的时间。 若卡片没被使用，则没有这个字段返回 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，…（官方字段路径 <c>card_usage_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_usage_time")]
    public string? CardUsageTime { get; set; }

    /// <summary>【卡片关联微信支付订单号】 卡片关联微信支付订单号，最近一次使用该额度卡支付产生的微信支付交易单号。 若卡片没被使用，则没有这个字段返回（官方字段路径 <c>transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【卡片失效时间】 卡片失效时间，额度卡变为失效状态的时间。 若卡片仍在生效，则没有这个字段返回。 例如：单次卡被使用、单次卡到期、多次卡被使用、多次卡到期、单次卡被作废、多次卡被作废。 卡片失效时间表示上述的某一个最先到达的状态的时间。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表…（官方字段路径 <c>card_disabled_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_disabled_time")]
    public string? CardDisabledTime { get; set; }

    /// <summary>【卡片备注】 卡片备注，额度卡的补充说明信息。由商户下发额度卡时传入（官方字段路径 <c>card_remark</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("card_remark")]
    public string? CardRemark { get; set; }

    /// <summary>【可用商户范围ID】 可用商户范围ID，该额度卡关联的可用商户范围。如果有值，则该额度卡仅能在指定的商户范围内使用；如果为空，则可在所有支持企业支付的商户使用（官方字段路径 <c>avail_mch_range_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("avail_mch_range_id")]
    public string? AvailMchRangeId { get; set; }

}

/// <summary>企业商户作废员工企业支付额度卡请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507127"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayEmployee")]
public class WebizpayQuotaCardCancelRequest
{
    /// <summary>【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。子商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【作废原因】 作废原因，长度不可超过50个字符（官方字段路径 <c>reason</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

}

/// <summary>企业商户作废员工企业支付额度卡响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507127"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayEmployee")]
public class WebizpayQuotaCardCancelResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资子商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信授权关系 ID】 微信授权关系ID，指定接收额度卡的员工授权关系，该员工必须已完成企业支付授权 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方字段路径 <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("employee_id")]
    public string? EmployeeId { get; set; }

    /// <summary>【企业支付额度卡卡号】 企业支付额度卡卡号，微信支付平台生成的唯一标识额度卡的编号。通过创建卡和查询卡返回。由商户作废卡时传入 示例：1068019671702503425（官方字段路径 <c>card_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("card_no")]
    public string? CardNo { get; set; }

    /// <summary>【卡片状态】 卡片状态。可选取值：`ACTIVE` 生效 / `DISABLED` 失效（官方字段路径 <c>card_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_state")]
    public string? CardState { get; set; }

    /// <summary>【卡片失效时间】 卡片失效时间，当卡片不可用时存在此字段 格式要求：支付结束时间需遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日1…（官方字段路径 <c>card_disabled_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_disabled_time")]
    public string? CardDisabledTime { get; set; }

}

/// <summary>企业商户获取指定额度卡支付链接响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507160"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayEmployee")]
public class WebizpayQuotaCardPaymentUrlResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资子商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方字段路径 <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("employee_id")]
    public string? EmployeeId { get; set; }

    /// <summary>【企业支付额度卡卡号】 企业支付额度卡卡号，微信支付平台生成的唯一标识额度卡的编号 示例：1068019671702503425（官方字段路径 <c>card_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("card_no")]
    public string? CardNo { get; set; }

    /// <summary>【链接失效时间】 定义：链接失效时间，超过这个时间后，用户再通过此链接尝试拉起微信付款码支付会拦截请求，需要重新调用此接口获取新的跳转链接。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13…（官方字段路径 <c>expire_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

    /// <summary>【跳转链接】 当跳转类型为H5，并且允许拉起微信时返回。 跳转链接,通过schema跳转到微信可以拉起付款码页面（官方字段路径 <c>payment_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("payment_url")]
    public string? PaymentUrl { get; set; }

    /// <summary>【拉起付款码页面的企业应用类型】 由商户请求时传入。可选取值：`H5` 表示从商户的应用是外部H5，跳转微信拉起企业支付相关能力 / `MINIPROGRAM` 表示从商户的应用是微信小程序，跳转微信拉起企业支付相关能力（官方字段路径 <c>application_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("application_type")]
    public string? ApplicationType { get; set; }

    /// <summary>【小程序查询参数】 在类型为MINIPROGRAM 时，若允许拉起付款码页面则会返回该参数（有效期35分钟），拉起企业支付页面可参考 从小程序调起企业支付相关能力-wx.openBusinessView 此参数对应 从小程序调起企业支付相关能力-wx.openBusinessView 中的queryString（官方字段路径 <c>mp_query</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("mp_query")]
    public string? MpQuery { get; set; }

    /// <summary>【小程序业务类型】 在类型为MINIPROGRAM 时，若允许拉起付款码页面则会返回该参数（有效期35分钟），拉起企业支付页面可参考 从小程序调起企业支付相关能力-wx.openBusinessView 此参数对应 从小程序调起企业支付相关能力-wx.openBusinessView 中的businessType（官方字段路径 <c>mp_business_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mp_business_type")]
    public string? MpBusinessType { get; set; }

}

/// <summary>企业商户发起解除员工企业支付授权请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507156"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayEmployee")]
public class WebizpayEmployeeRevokeRequest
{
    /// <summary>【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>企业商户发起解除员工企业支付授权响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507156"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayEmployee")]
public class WebizpayEmployeeRevokeResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号，具有企业支付服务商资质的微信支付商户号，由商户请求时传入（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资子商户号，企业支付的出资方商户号，由商户请求时传入（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户企业员工ID】 商户企业员工ID，由企业系统提供的唯一标识员工的ID，由商户请求时传入（官方字段路径 <c>user_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>【员工企业支付权限状态】 员工企业支付权限状态，表示解除授权后的状态，同步接口，若此接口返回状态为“REVOKED”，则认为此员工授权企业支付授权状态已经解除，无法在此服务商-子账户下继续使用企业支付额度卡，无法进行企业支付，此商户下的企业支付额度卡全部作废。 若要恢复状态，需要重新授权开通。可选取值：`AUTHORIZED` 已授权 / `REVOKED` 已解除授权（官方字段路径 <c>authorization_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authorization_state")]
    public string? AuthorizationState { get; set; }

    /// <summary>【员工企业支付失效时间】 员工企业支付失效时间，解除授权操作的生效时间。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29…（官方字段路径 <c>authorization_revoked_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authorization_revoked_time")]
    public string? AuthorizationRevokedTime { get; set; }

    /// <summary>【失效原因】 失效原因，说明授权关系被解除的原因，如'用户主动撤销'、'企业管理员撤销'等（官方字段路径 <c>reason</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

}
