// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandCardConfig;

/// <summary>商家名片配置申请状态响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016475174"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigQueryResponse : WechatPayResponse
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【微信支付申请单号】 申请单的唯一标识，由微信支付生成（官方字段路径 <c>applyment_id</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【申请单状态】 商家名片配置的申请状态。可选取值：`STATE_UNKNOWN` 「状态未知，非终态」发布配置流程的中间状态，请稍后重试 / `DRAFTING` 「配置中，非终态」调用提交申请接口后，返回了预览二维码，尚未调用发布接口，即流转到该状态 / `AUDITING` 「审核中，非终态」调用发布接口后，平台将进行审核，即流转到该状态 / `AUDIT_REJECTED` 「审核驳回，非终态」审核阶段被驳回，会附带具体的驳回原因，请根据驳回原因修改调整，如仍需添加请使用原业务申请编号重新调用 [提交商家名片配置申请](https://pay.weixin.qq.com/doc/v3/partner/4016468440.md) 接口 / `PENDING_PUBLISH` 「待发布，非终态」审核通过后，未到达发布时间，即流转到该状态 / `PUBLISHED` 「已发布，终态」商家名片配置已顺利更新到线上，即流转到该状态 / `CANCELED` 「已撤销，终态」商家名片配置请求被撤销，如仍需配置请更换业务申请编号重新调用 [提交商家名片配置申请](https://pay.weixin.qq.com/doc/v3/partner/4016468440.md) 接口（官方字段路径 <c>applyment_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("applyment_state")]
    public string? ApplymentState { get; set; }

    /// <summary>【定时发布时间】 1、定义：提交商家名片配置审核通过后，平台发布配置上线的计划时间。 2、返回条件：当且仅当发布配置时设置了计划发布时间，本字段才会返回值。 3、格式：遵循 rfc3339 标准格式： `yyyy-MM-DDTHH:mm:ss+TIMEZONE`。 `yyyy-MM-DD` 表示年月日； `T` 字符用于分隔日期和时间部分； `HH:mm:ss` 表示具体的时分秒； `TIMEZONE` 表示时区（例如， `+08:00` 对应东八区时间，…（官方字段路径 <c>scheduled_publish_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("scheduled_publish_time")]
    public string? ScheduledPublishTime { get; set; }

    /// <summary>【驳回原因】 1、商家名片配置申请审核驳回的具体原因； 2、当申请单状态为「审核驳回」时有值（官方字段路径 <c>reject_reason</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>【实际发布时间】 1、定义：提交商家名片配置审核通过后，平台发布配置上线的实际时间。 2、返回条件：当且仅当申请单状态为「已发布」时，该字段才返回值。 3、格式：遵循 rfc3339 标准格式： `yyyy-MM-DDTHH:mm:ss+TIMEZONE`。 `yyyy-MM-DD` 表示年月日； `T` 字符用于分隔日期和时间部分； `HH:mm:ss` 表示具体的时分秒； `TIMEZONE` 表示时区（例如， `+08:00` 对应东八区时间，即北京…（官方字段路径 <c>actual_publish_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("actual_publish_time")]
    public string? ActualPublishTime { get; set; }

}

/// <summary>商家名片配置申请提交请求·【品牌小程序信息】 用于配置商家名片头部跳转的小程序，建议配置用户在品牌发起交易。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016468440"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigApplyRequestBrandMiniProgramInfo
{
    /// <summary>【公众账号ID】 1、公众账号ID也称 AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）；开发者需要在 微信开放平台 或 微信公众平台 中申请ID。 2、请填写配置商家名片头部跳转小程序的 AppID（官方字段路径 <c>brand_mini_program_info.appid</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序默认跳转路径】 商家名片头部跳转小程序的默认路径，不填写则默认跳转小程序首页（官方字段路径 <c>brand_mini_program_info.default_jump_path</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("default_jump_path")]
    public string? DefaultJumpPath { get; set; }

    /// <summary>【小程序跳转按钮文案】 商家可自定义小程序跳转按钮的显示文案，如未设置则默认显示"前往小程序"（官方字段路径 <c>brand_mini_program_info.button_text</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("button_text")]
    public string? ButtonText { get; set; }

}

/// <summary>商家名片配置申请提交请求·【品牌客服信息】 1、品牌客服信息配置，用于在商家名片中展示客服信息； 2、根据。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016468440"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigApplyRequestBrandCustomerService
{
    /// <summary>【客服类型】 客服类型枚举，用于配置商家名片中的客服信息。可选取值：`MINI_PROGRAM` 小程序官方客服组件：小程序官方提供的客服组件，需传入小程序 AppID，详细见 [链接](https://developers.weixin.qq.com/miniprogram/introduction/custom.html) / `WECOM` 企业微信客服：企业微信官方提供的微信客服组件，详细见 [概述](https://developer.work.weixin.qq.com/document/path/99370) / `CUSTOMIZE_WEB` 自建客服（网页）：品牌自主或通过第三方搭建的客服系统，在网页内向用户提供咨询服务，传入网页链接即可 / `CUSTOMIZE_MP` 自建客服（小程序）：品牌自主或通过第三方搭建的客服系统，在小程序内向用户提供咨询服务，需传入小程序 AppID 及小程序路径 / `SERVICE_PHONE` 客服电话：品牌官方客服电话（官方字段路径 <c>brand_customer_service.customer_service_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("customer_service_type")]
    public string? CustomerServiceType { get; set; }

    /// <summary>【客服电话】 1、品牌官方客服电话，需确保电话号码真实有效； 2、当客服类型是"客服电话"时必填（官方字段路径 <c>brand_customer_service.customer_service_phone</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("customer_service_phone")]
    public string? CustomerServicePhone { get; set; }

    /// <summary>【客服路径】 1、客服页面的具体路径，需确保路径真实存在； 2、当客服类型是"企业微信客服"、"自建客服（网页）"、"自建客服（小程序）"时必填（官方字段路径 <c>brand_customer_service.customer_service_path</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("customer_service_path")]
    public string? CustomerServicePath { get; set; }

    /// <summary>【公众账号ID】 1、公众账号ID也称 AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）；开发者需要在 微信开放平台 或 微信公众平台 中申请ID。 2、当客服类型是"小程序官方客服组件"、"自建客服（小程序）"时必填； 3、请使用已认证的小程序 AppID（官方字段路径 <c>brand_customer_service.appid</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>商家名片配置申请提交请求·【服务列表】 1、用于在商家名片中展示品牌提供的各项服务； 2、条件：如需要配置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016468440"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigApplyRequestServiceListItem
{
    /// <summary>【服务分类名称】 商家名片服务列表中的服务分类名称，多个服务可以归属为一个服务分类，最多可创建三个服务分类（官方字段路径 <c>service_list.service_classify_name</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("service_classify_name")]
    public string? ServiceClassifyName { get; set; }

    /// <summary>【服务名称】 商家名片服务列表中的服务名称，一个服务分类下最多可配置5个服务名称，整个服务列表最多可配置15个服务名称（官方字段路径 <c>service_list.service_name</c>，<b>必填</b>，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("service_name")]
    public string? ServiceName { get; set; }

    /// <summary>【服务跳转类型】 决定了用户点击服务后的跳转方式。可选取值：`JUMP_MINI_PROGRAM` 小程序：小程序页面路径 / `JUMP_WEB_PAGE` 网页：完整的网页URL，必须是HTTPS协议（官方字段路径 <c>service_list.service_jump_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("service_jump_type")]
    public string? ServiceJumpType { get; set; }

    /// <summary>【服务跳转路径】 服务跳转路径，根据服务跳转类型不同，格式要求不同（官方字段路径 <c>service_list.service_jump_path</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("service_jump_path")]
    public string? ServiceJumpPath { get; set; }

    /// <summary>【公众账号ID】 1、公众账号ID也称 AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）；开发者需要在 微信开放平台 或 微信公众平台 中申请ID。 2、当服务跳转类型是小程序时，需传入对应小程序的 AppID。 3、请使用已认证的小程序 AppID（官方字段路径 <c>service_list.appid</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>商家名片配置申请提交请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016468440"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigApplyRequest
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单； 3、若申请被驳回，可填写相同的“业务申请编号”，即可覆盖修改原申请信息（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌小程序信息】 用于配置商家名片头部跳转的小程序，建议配置用户在品牌发起交易的小程序（官方字段路径 <c>brand_mini_program_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("brand_mini_program_info")]
    public BrandCardConfigApplyRequestBrandMiniProgramInfo? BrandMiniProgramInfo { get; set; }

    /// <summary>【品牌客服信息】 1、品牌客服信息配置，用于在商家名片中展示客服信息； 2、根据客服类型不同，需要填写对应的配置信息（官方字段路径 <c>brand_customer_service</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("brand_customer_service")]
    public BrandCardConfigApplyRequestBrandCustomerService? BrandCustomerService { get; set; }

    /// <summary>【服务列表】 1、用于在商家名片中展示品牌提供的各项服务； 2、条件：如需要配置服务，则至少填写2项，最多15项；如无需配置服务，则保持列表为空； 3、列表排序：列表顺序即为最终展示顺序，系统将按输入顺序对服务进行分类和排序，相同服务分类的项会自动归组，组内顺序保持数组中的原始排列（官方字段路径 <c>service_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("service_list")]
    public List<BrandCardConfigApplyRequestServiceListItem>? ServiceList { get; set; }

}

/// <summary>商家名片配置申请提交响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016468440"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigApplyResponse : WechatPayResponse
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【微信支付申请单号】 申请单的唯一标识，由微信支付生成（官方字段路径 <c>applyment_id</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【商家名片预览二维码链接】 商家名片配置申请提交成功后，将返回可供商家预览的二维码链接，商家可查看各跳转路径是否正常，符合预期（官方字段路径 <c>card_preview_url</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("card_preview_url")]
    public string? CardPreviewUrl { get; set; }

    /// <summary>【预览二维码链接过期时间】 1、商家名片配置申请提交成功后，系统将返回预览二维码链接的有效截止时间。商家需在此时间点前完成预览确认，逾期链接将自动失效。 2、格式：遵循 rfc3339 标准格式： `yyyy-MM-DDTHH:mm:ss+TIMEZONE`。 `yyyy-MM-DD` 表示年月日； `T` 字符用于分隔日期和时间部分； `HH:mm:ss` 表示具体的时分秒； `TIMEZONE` 表示时区（例如， `+08:00` 对应东八区时间，即北…（官方字段路径 <c>url_expired_time</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("url_expired_time")]
    public string? UrlExpiredTime { get; set; }

}

/// <summary>商家名片配置申请撤销请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016475172"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigCancelRequest
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成，和微信支付申请单号二选一作为请求参数； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单； 3、填写调用 提交商家名片配置申请 时传递的业务申请编号（官方字段路径 <c>business_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【微信支付申请单号】 1、申请单的唯一标识，由微信支付生成，和业务申请编号二选一作为请求参数。 2、填写调用 提交商家名片配置申请 时返回的微信支付申请单号（官方字段路径 <c>applyment_id</c>，选填，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>商家名片配置申请撤销响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016475172"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigCancelResponse : WechatPayResponse
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【微信支付申请单号】 申请单的唯一标识，由微信支付生成（官方字段路径 <c>applyment_id</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【申请单状态】 商家名片配置的申请状态，如果撤销成功，该字段只会返回 `CANCELED`。可选取值：`STATE_UNKNOWN` 「状态未知，非终态」发布配置流程的中间状态，请稍后重试 / `DRAFTING` 「配置中，非终态」调用提交申请接口后，返回了预览二维码，尚未调用发布接口，即流转到该状态 / `AUDITING` 「审核中，非终态」调用发布接口后，平台将进行审核，即流转到该状态 / `AUDIT_REJECTED` 「审核驳回，非终态」审核阶段被驳回，会附带具体的驳回原因，请根据驳回原因修改调整，如仍需添加请使用原业务申请编号重新调用 [提交商家名片配置申请](https://pay.weixin.qq.com/doc/v3/partner/4016468440.md) 接口 / `PENDING_PUBLISH` 「待发布，非终态」审核通过后，未到达发布时间，即流转到该状态 / `PUBLISHED` 「已发布，终态」商家名片配置已顺利更新到线上，即流转到该状态 / `CANCELED` 「已撤销，终态」商家名片配置请求被撤销，如仍需配置请更换业务申请编号重新调用 [提交商家名片配置申请](https://pay.weixin.qq.com/doc/v3/partner/4016468440.md) 接口（官方字段路径 <c>applyment_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("applyment_state")]
    public string? ApplymentState { get; set; }

}

/// <summary>商家名片预览二维码响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016641998"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigPreviewResponse : WechatPayResponse
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【微信支付申请单号】 申请单的唯一标识，由微信支付生成（官方字段路径 <c>applyment_id</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【商家名片预览二维码链接】 商家名片配置申请提交后、正式发布提交前，将返回可供商家预览的二维码链接，商家可查看各跳转路径是否正常，符合预期（官方字段路径 <c>card_preview_url</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("card_preview_url")]
    public string? CardPreviewUrl { get; set; }

    /// <summary>【预览二维码链接过期时间】 1、商家名片配置申请提交成功后、正式发布提交前，将返回可供商家预览的二维码链接的有效过期时间，超时后链接将自动失效，需重新申请获取。 2、格式：遵循 rfc3339 标准格式： `yyyy-MM-DDTHH:mm:ss+TIMEZONE`。 `yyyy-MM-DD` 表示年月日； `T` 字符用于分隔日期和时间部分； `HH:mm:ss` 表示具体的时分秒； `TIMEZONE` 表示时区（例如， `+08:00` 对应东八区时…（官方字段路径 <c>url_expired_time</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("url_expired_time")]
    public string? UrlExpiredTime { get; set; }

}

/// <summary>商家名片配置发布请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016475176"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigPublishRequest
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成，和微信支付申请单号二选一作为请求参数； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单； 3、填写调用 提交商家名片配置申请 时传递的业务申请编号（官方字段路径 <c>business_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【微信支付申请单号】 1、申请单的唯一标识，由微信支付生成，和业务申请编号二选一作为请求参数。 2、填写调用 提交商家名片配置申请 时返回的微信支付申请单号（官方字段路径 <c>applyment_id</c>，选填，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【发布方式】 配置商家名片的发布方式。可选取值：`IMMEDIATE_PUBLISH` 即时发布：审核通过后立即发布 / `SCHEDULED_PUBLISH` 定时发布：审核通过后，若定时时间未到，则等待至设定时间发布；若定时时间已过，则立即发布（官方字段路径 <c>publish_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("publish_type")]
    public string? PublishType { get; set; }

    /// <summary>【定时发布时间】 1、定义：提交商家名片配置审核通过后，平台发布配置上线的计划时间。 2、填写条件：发布方式为 `SCHEDULED_PUBLISH` 时必填，填写时间需在当前时间1小时后且90天内。 3、格式：遵循 rfc3339 标准格式： `yyyy-MM-DDTHH:mm:ss+TIMEZONE`。 `yyyy-MM-DD` 表示年月日； `T` 字符用于分隔日期和时间部分； `HH:mm:ss` 表示具体的时分秒； `TIMEZONE` 表示时区…（官方字段路径 <c>scheduled_publish_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("scheduled_publish_time")]
    public string? ScheduledPublishTime { get; set; }

}

/// <summary>商家名片配置发布响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016475176"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardConfig")]
public class BrandCardConfigPublishResponse : WechatPayResponse
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【微信支付申请单号】 申请单的唯一标识，由微信支付生成（官方字段路径 <c>applyment_id</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【发布方式】 配置商家名片的发布方式。可选取值：`IMMEDIATE_PUBLISH` 即时发布：审核通过后立即发布 / `SCHEDULED_PUBLISH` 定时发布：审核通过后，若定时时间未到，则等待至设定时间发布；若定时时间已过，则立即发布（官方字段路径 <c>publish_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("publish_type")]
    public string? PublishType { get; set; }

    /// <summary>【定时发布时间】 1、定义：提交商家名片配置审核通过后，平台发布配置上线的计划时间。 2、填写条件：发布方式为 `SCHEDULED_PUBLISH` 时返回。 3、格式：遵循 rfc3339 标准格式： `yyyy-MM-DDTHH:mm:ss+TIMEZONE`。 `yyyy-MM-DD` 表示年月日； `T` 字符用于分隔日期和时间部分； `HH:mm:ss` 表示具体的时分秒； `TIMEZONE` 表示时区（例如， `+08:00` 对应东八区时…（官方字段路径 <c>scheduled_publish_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("scheduled_publish_time")]
    public string? ScheduledPublishTime { get; set; }

}
