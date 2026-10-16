// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingPartnerships;

/// <summary>建立品牌委托营销合作关系请求·【合作方信息】合作方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380498"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipBuildRequestPartner
{
    /// <summary>【合作方类别】合作方类别，枚举值见文档。可选取值：`APPID` 合作方为Appid / `MERCHANT` 合作方为商户（官方字段路径 <c>partner.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【合作方Appid】合作方Appid,合作方类别为APPID时必填（官方字段路径 <c>partner.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【合作方商户ID】合作方商户ID，合作方类别为MERCHANT时必填（官方字段路径 <c>partner.merchant_id</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }

}

/// <summary>建立品牌委托营销合作关系请求·【被授权数据】被授权数据。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380498"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipBuildRequestAuthorizedData
{
    /// <summary>【授权业务类别】授权业务类别，枚举值见文档。可选取值：`FAVOR_STOCK` 授权业务类型-代金券批次 / `BUSIFAVOR_STOCK` 授权业务类型-商家券批次（官方字段路径 <c>authorized_data.business_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>【授权批次ID】授权批次ID，授权业务类别为券批次时必填（官方字段路径 <c>authorized_data.stock_id</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>建立品牌委托营销合作关系请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380498"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipBuildRequest
{
    /// <summary>【合作方信息】合作方信息（官方字段路径 <c>partner</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("partner")]
    public PartnershipPartnershipBuildRequestPartner? Partner { get; set; }

    /// <summary>【被授权数据】被授权数据（官方字段路径 <c>authorized_data</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("authorized_data")]
    public PartnershipPartnershipBuildRequestAuthorizedData? AuthorizedData { get; set; }

}

/// <summary>建立品牌委托营销合作关系响应·【合作方信息】合作方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380498"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipBuildResponsePartner
{
    /// <summary>【合作方类别】合作方类别，枚举值见文档。可选取值：`APPID` 合作方为Appid / `MERCHANT` 合作方为商户（官方字段路径 <c>partner.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【合作方Appid】合作方Appid,合作方类别为APPID时必填（官方字段路径 <c>partner.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【合作方商户ID】合作方商户ID，合作方类别为MERCHANT时必填（官方字段路径 <c>partner.merchant_id</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }

}

/// <summary>建立品牌委托营销合作关系响应·【被授权数据】被授权数据。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380498"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipBuildResponseAuthorizedData
{
    /// <summary>【授权业务类别】授权业务类别，枚举值见文档。可选取值：`FAVOR_STOCK` 授权业务类型-代金券批次 / `BUSIFAVOR_STOCK` 授权业务类型-商家券批次（官方字段路径 <c>authorized_data.business_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>【授权批次ID】授权批次ID，授权业务类别为券批次时必填（官方字段路径 <c>authorized_data.stock_id</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>建立品牌委托营销合作关系响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380498"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipBuildResponse : WechatPayResponse
{
    /// <summary>【合作方信息】合作方信息（官方字段路径 <c>partner</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("partner")]
    public PartnershipPartnershipBuildResponsePartner? Partner { get; set; }

    /// <summary>【被授权数据】被授权数据（官方字段路径 <c>authorized_data</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("authorized_data")]
    public PartnershipPartnershipBuildResponseAuthorizedData? AuthorizedData { get; set; }

    /// <summary>【合作状态】合作状态，枚举值见文档。可选取值：`ESTABLISHED` 合作状态-已建立 / `TERMINATED` 合作状态-已终止（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【建立合作关系时间】建立合作关系时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35…（官方字段路径 <c>build_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("build_time")]
    public string? BuildTime { get; set; }

    /// <summary>【创建时间】创建时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】更新时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>查询品牌委托营销合作关系列表响应·【合作关系结果集】 合作关系查询结果集。如查询结果为0，将在控制流返回错误·【合作方信息】 合作方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380536"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipListResponseDataItemDatapartner
{
    /// <summary>【合作方类别】 合作方类别，枚举值见文档。可选取值：`APPID` 合作方为Appid / `MERCHANT` 合作方为商户（官方字段路径 <c>data.partner.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【合作方Appid】 合作方Appid,合作方类别为APPID时必填（官方字段路径 <c>data.partner.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【合作方商户ID】 合作方商户ID，合作方类别为MERCHANT时必填（官方字段路径 <c>data.partner.merchant_id</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }

}

/// <summary>查询品牌委托营销合作关系列表响应·【合作关系结果集】 合作关系查询结果集。如查询结果为0，将在控制流返回错误·【被授权数据】 被授权数据。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380536"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipListResponseDataItemDataauthorizedData
{
    /// <summary>【授权业务类别】 授权业务类别，枚举值见文档。可选取值：`FAVOR_STOCK` 授权业务类型-代金券批次 / `BUSIFAVOR_STOCK` 授权业务类型-商家券批次（官方字段路径 <c>data.authorized_data.business_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>【授权批次ID】 授权批次ID，授权业务类别为券批次时必填（官方字段路径 <c>data.authorized_data.stock_id</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>查询品牌委托营销合作关系列表响应·【合作关系结果集】 合作关系查询结果集。如查询结果为0，将在控制流返回错误。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380536"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipListResponseDataItem
{
    /// <summary>【合作方信息】 合作方信息（官方字段路径 <c>data.partner</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("partner")]
    public PartnershipPartnershipListResponseDataItemDatapartner? Partner { get; set; }

    /// <summary>【被授权数据】 被授权数据（官方字段路径 <c>data.authorized_data</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("authorized_data")]
    public PartnershipPartnershipListResponseDataItemDataauthorizedData? AuthorizedData { get; set; }

    /// <summary>【建立合作关系时间】 建立合作关系时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分3…（官方字段路径 <c>data.build_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("build_time")]
    public string? BuildTime { get; set; }

    /// <summary>【终止合作关系时间】 终止合作关系时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分3…（官方字段路径 <c>data.terminate_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("terminate_time")]
    public string? TerminateTime { get; set; }

    /// <summary>【创建时间】 创建时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.update_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>查询品牌委托营销合作关系列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380536"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerships")]
public class PartnershipPartnershipListResponse : WechatPayResponse
{
    /// <summary>【合作关系结果集】 合作关系查询结果集。如查询结果为0，将在控制流返回错误（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<PartnershipPartnershipListResponseDataItem>? Data { get; set; }

    /// <summary>【分页页码】 分页页码（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【总数量】 总数量，当offset=0或者当前查询为空时返回（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

}
