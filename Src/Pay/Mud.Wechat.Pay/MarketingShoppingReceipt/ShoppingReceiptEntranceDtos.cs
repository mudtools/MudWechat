// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingShoppingReceipt;

/// <summary>创建自定义入口请求·【跳转信息】 自定义入口跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697703"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceCreateRequestJumpLink
{
    /// <summary>【商家小程序AppID】 自定义入口的跳转小程序AppID。该AppID需要与品牌ID具有绑定关系（官方字段路径 <c>jump_link.mini_programs_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】 自定义入口跳转小程序的path（官方字段路径 <c>jump_link.mini_programs_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>创建自定义入口请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697703"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceCreateRequest
{
    /// <summary>【自定义入口种类】 自定义入口种类。种类为商家小程序，主标题展示的字为“商家小程序”，种类为商家活动，主标题展示的字为“商家活动”（官方字段路径 <c>custom_entrance_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_type")]
    public string? CustomEntranceType { get; set; }

    /// <summary>【副标题】 副标题，辅助描述商家活动或商家小程序信息（官方字段路径 <c>subtitle</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>【商品缩略图URL】 展示商品信息，当自定义入口种类为商家活动时，该字段必填，种类为商家小程序该字段不填。 仅支持通过《 图片上传API》接口获取的图片URL地址。大小需要满足42 _42px，格式仅支持png、jpg。_（官方字段路径 <c>goods_thumbnail_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("goods_thumbnail_url")]
    public string? GoodsThumbnailUrl { get; set; }

    /// <summary>【入口展示开始时间】 自定义入口的开始时间。需要大于当前时间，且小于结束时间。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29…（官方字段路径 <c>start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【入口展示结束时间】 自定义入口的结束时间。需要大于开始时间，最长可以配置180天内的活动。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月2…（官方字段路径 <c>end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【自定义入口状态】 设置自定义入口的状态，在线对用户展示，下线对用户不展示（官方字段路径 <c>custom_entrance_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_state")]
    public string? CustomEntranceState { get; set; }

    /// <summary>【品牌ID】 自定义入口的归属品牌ID。创建者需要是该品牌的品牌服务商或品牌主（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【请求业务单据号】 商户创建自定义入口的凭据号（格式：商户ID+日期+流水号），商家侧需要保持唯一且递增（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【跳转信息】 自定义入口跳转信息（官方字段路径 <c>jump_link</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_link")]
    public ShoppingReceiptEntranceCreateRequestJumpLink? JumpLink { get; set; }

}

/// <summary>创建自定义入口响应·【跳转信息】 自定义入口跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697703"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceCreateResponseJumpLink
{
    /// <summary>【商家小程序AppID】 自定义入口的跳转小程序AppID。该AppID需要与品牌ID具有绑定关系（官方字段路径 <c>jump_link.mini_programs_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】 自定义入口跳转小程序的path（官方字段路径 <c>jump_link.mini_programs_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>创建自定义入口响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697703"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceCreateResponse : WechatPayResponse
{
    /// <summary>【自定义入口种类】 自定义入口种类。种类为商家小程序，主标题展示的字为“商家小程序”，种类为商家活动，主标题展示的字为“商家活动”（官方字段路径 <c>custom_entrance_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_type")]
    public string? CustomEntranceType { get; set; }

    /// <summary>【副标题】 副标题，辅助描述商家活动或商家小程序信息（官方字段路径 <c>subtitle</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>【商品缩略图URL】 展示商品信息，当自定义入口种类为商家活动时，该字段必填，种类为商家小程序该字段不填。 仅支持通过《 图片上传API》接口获取的图片URL地址。大小需要满足42 _42px，格式仅支持png、jpg。_（官方字段路径 <c>goods_thumbnail_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("goods_thumbnail_url")]
    public string? GoodsThumbnailUrl { get; set; }

    /// <summary>【自定义入口状态】 设置自定义入口的状态，在线对用户展示，下线对用户不展示（官方字段路径 <c>custom_entrance_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_state")]
    public string? CustomEntranceState { get; set; }

    /// <summary>【入口展示开始时间】 自定义入口的开始时间。需要大于当前时间，且小于结束时间。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29…（官方字段路径 <c>start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【入口展示结束时间】 自定义入口的结束时间。需要大于开始时间，最长可以配置180天内的活动。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月2…（官方字段路径 <c>end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【品牌ID】 自定义入口的归属品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【创建时间】 自定义入口创建的时间，遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【修改时间】 自定义入口修改的时间，遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>modify_time</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

    /// <summary>【请求业务单据号】 商户创建自定义入口的凭据号（格式：商户ID+日期+流水号），商家侧需要保持唯一且递增（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【跳转信息】 自定义入口跳转信息（官方字段路径 <c>jump_link</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_link")]
    public ShoppingReceiptEntranceCreateResponseJumpLink? JumpLink { get; set; }

}

/// <summary>查询自定义入口响应·【跳转信息】 自定义入口跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698331"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceQueryResponseJumpLink
{
    /// <summary>【商家小程序AppID】 自定义入口的跳转小程序AppID。该AppID需要与品牌ID具有绑定关系（官方字段路径 <c>jump_link.mini_programs_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】 自定义入口跳转小程序的path（官方字段路径 <c>jump_link.mini_programs_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>查询自定义入口响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698331"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceQueryResponse : WechatPayResponse
{
    /// <summary>【自定义入口种类】 自定义入口种类。种类为商家小程序，主标题展示的字为“商家小程序”，种类为商家活动，主标题展示的字为“商家活动”（官方字段路径 <c>custom_entrance_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_type")]
    public string? CustomEntranceType { get; set; }

    /// <summary>【副标题】 副标题，辅助描述商家活动或商家小程序信息（官方字段路径 <c>subtitle</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>【商品缩略图URL】 展示商品信息，当自定义入口种类为商家活动时，该字段必填，种类为商家小程序该字段不填。 仅支持通过《 图片上传API》接口获取的图片URL地址。大小需要满足42 _42px，格式仅支持png、jpg。_（官方字段路径 <c>goods_thumbnail_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("goods_thumbnail_url")]
    public string? GoodsThumbnailUrl { get; set; }

    /// <summary>【自定义入口状态】 设置自定义入口的状态，在线对用户展示，下线对用户不展示（官方字段路径 <c>custom_entrance_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_state")]
    public string? CustomEntranceState { get; set; }

    /// <summary>【入口展示开始时间】 自定义入口的开始时间。需要大于当前时间，且小于结束时间。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29…（官方字段路径 <c>start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【入口展示结束时间】 自定义入口的结束时间。需要大于开始时间，最长可以配置180天内的活动。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月2…（官方字段路径 <c>end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【品牌ID】 自定义入口的归属品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【创建时间】 自定义入口创建的时间，遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【修改时间】 自定义入口修改的时间，遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>modify_time</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

    /// <summary>【请求业务单据号】 商户创建自定义入口的凭据号（格式：商户ID+日期+流水号），商家侧需要保持唯一且递增（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【跳转信息】 自定义入口跳转信息（官方字段路径 <c>jump_link</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_link")]
    public ShoppingReceiptEntranceQueryResponseJumpLink? JumpLink { get; set; }

}

/// <summary>修改自定义入口请求·【跳转信息】 自定义入口跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697769"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceUpdateRequestJumpLink
{
    /// <summary>【商家小程序AppID】 自定义入口的跳转小程序AppID。该AppID需要与品牌ID具有绑定关系（官方字段路径 <c>jump_link.mini_programs_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】 自定义入口跳转小程序的path（官方字段路径 <c>jump_link.mini_programs_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>修改自定义入口请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697769"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceUpdateRequest
{
    /// <summary>【自定义入口种类】 自定义入口种类。种类为商家小程序，主标题展示的字为“商家小程序”，种类为商家活动，主标题展示的字为“商家活动”（官方字段路径 <c>custom_entrance_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_type")]
    public string? CustomEntranceType { get; set; }

    /// <summary>【副标题】 副标题，辅助描述商家活动或商家小程序信息（官方字段路径 <c>subtitle</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>【商品缩略图URL】 展示商品信息，当自定义入口种类为商家活动时，该字段必填，种类为商家小程序该字段不填。 仅支持通过《 图片上传API》接口获取的图片URL地址。大小需要满足42 _42px，格式仅支持png、jpg。_（官方字段路径 <c>goods_thumbnail_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("goods_thumbnail_url")]
    public string? GoodsThumbnailUrl { get; set; }

    /// <summary>【自定义入口状态】 设置自定义入口的状态，在线对用户展示，下线对用户不展示（官方字段路径 <c>custom_entrance_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_state")]
    public string? CustomEntranceState { get; set; }

    /// <summary>【入口展示开始时间】 自定义入口的开始时间。需要大于当前时间，且小于结束时间。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29…（官方字段路径 <c>start_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【入口展示结束时间】 自定义入口的结束时间。需要大于开始时间，最长可以配置180天内的活动。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月2…（官方字段路径 <c>end_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【请求业务单据号】 商户创建自定义入口的凭据号（格式：商户ID+日期+流水号），商家侧需要保持唯一且递增（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【跳转信息】 自定义入口跳转信息（官方字段路径 <c>jump_link</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_link")]
    public ShoppingReceiptEntranceUpdateRequestJumpLink? JumpLink { get; set; }

}

/// <summary>修改自定义入口响应·【跳转信息】 自定义入口跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697769"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceUpdateResponseJumpLink
{
    /// <summary>【商家小程序AppID】 自定义入口的跳转小程序AppID。该AppID需要与品牌ID具有绑定关系（官方字段路径 <c>jump_link.mini_programs_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】 自定义入口跳转小程序的path（官方字段路径 <c>jump_link.mini_programs_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>修改自定义入口响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697769"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptEntranceUpdateResponse : WechatPayResponse
{
    /// <summary>【自定义入口种类】 自定义入口种类。种类为商家小程序，主标题展示的字为“商家小程序”，种类为商家活动，主标题展示的字为“商家活动”（官方字段路径 <c>custom_entrance_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_type")]
    public string? CustomEntranceType { get; set; }

    /// <summary>【副标题】 副标题，辅助描述商家活动或商家小程序信息（官方字段路径 <c>subtitle</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>【商品缩略图URL】 展示商品信息，当自定义入口种类为商家活动时，该字段必填，种类为商家小程序该字段不填。 仅支持通过《 图片上传API》接口获取的图片URL地址。大小需要满足42 _42px，格式仅支持png、jpg。_（官方字段路径 <c>goods_thumbnail_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("goods_thumbnail_url")]
    public string? GoodsThumbnailUrl { get; set; }

    /// <summary>【自定义入口状态】 设置自定义入口的状态，在线对用户展示，下线对用户不展示（官方字段路径 <c>custom_entrance_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("custom_entrance_state")]
    public string? CustomEntranceState { get; set; }

    /// <summary>【入口展示开始时间】 自定义入口的开始时间。需要大于当前时间，且小于结束时间。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29…（官方字段路径 <c>start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【入口展示结束时间】 自定义入口的结束时间。需要大于开始时间，最长可以配置180天内的活动。遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月2…（官方字段路径 <c>end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【品牌ID】 自定义入口的归属品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【创建时间】 自定义入口创建的时间，遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【修改时间】 自定义入口修改的时间，遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>modify_time</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

    /// <summary>【请求业务单据号】 商户创建自定义入口的凭据号（格式：商户ID+日期+流水号），商家侧需要保持唯一且递增（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【跳转信息】 自定义入口跳转信息（官方字段路径 <c>jump_link</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_link")]
    public ShoppingReceiptEntranceUpdateResponseJumpLink? JumpLink { get; set; }

}
