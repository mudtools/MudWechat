// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingGoodsSubsidyActivity;

/// <summary>添加小店活动门店请求·【待新增的小店活动门店列表】 待新增的小店活动门店列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384421"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreAddRequestStoresItem
{
    /// <summary>【门店编码】 门店编码，可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>stores.store_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("store_code")]
    public string? StoreCode { get; set; }

    /// <summary>【门店名称】 门店名称（官方字段路径 <c>stores.store_name</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

}

/// <summary>添加小店活动门店请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384421"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreAddRequest
{
    /// <summary>【请求业务单据】 商户新增小店活动门店凭据号，商户侧需保持唯一性。可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【添加时间】 添加时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>add_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("add_time")]
    public string? AddTime { get; set; }

    /// <summary>【待新增的小店活动门店列表】 待新增的小店活动门店列表（官方字段路径 <c>stores</c>，<b>必填</b>，类型 <c>array[RetailStoreInfo]</c>）。</summary>
    [JsonPropertyName("stores")]
    public List<RetailStoreActStoreAddRequestStoresItem>? Stores { get; set; }

}

/// <summary>添加小店活动门店响应·【添加失败的小店活动门店列表】 添加失败的小店活动门店列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384421"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreAddResponseFailedStoresItem
{
    /// <summary>【门店编码】 门店编码，可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>failed_stores.store_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("store_code")]
    public string? StoreCode { get; set; }

    /// <summary>【门店名称】 门店名称（官方字段路径 <c>failed_stores.store_name</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

}

/// <summary>添加小店活动门店响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384421"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreAddResponse : WechatPayResponse
{
    /// <summary>【添加时间】 添加时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>add_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("add_time")]
    public string? AddTime { get; set; }

    /// <summary>【添加失败的小店活动门店列表】 添加失败的小店活动门店列表（官方字段路径 <c>failed_stores</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("failed_stores")]
    public List<RetailStoreActStoreAddResponseFailedStoresItem>? FailedStores { get; set; }

}

/// <summary>删除小店活动门店请求·【待删除的小店活动门店列表】 待删除的小店活动门店列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384431"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreDeleteRequestStoresItem
{
    /// <summary>【门店编码】 门店编码，可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>stores.store_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("store_code")]
    public string? StoreCode { get; set; }

    /// <summary>【门店名称】 门店名称（官方字段路径 <c>stores.store_name</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

}

/// <summary>删除小店活动门店请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384431"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreDeleteRequest
{
    /// <summary>【请求业务单据】 商户删除小店活动门店凭据号，商户侧需保持唯一性。可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【删除时间】 删除时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>delete_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delete_time")]
    public string? DeleteTime { get; set; }

    /// <summary>【待删除的小店活动门店列表】 待删除的小店活动门店列表（官方字段路径 <c>stores</c>，<b>必填</b>，类型 <c>array[RetailStoreInfo]</c>）。</summary>
    [JsonPropertyName("stores")]
    public List<RetailStoreActStoreDeleteRequestStoresItem>? Stores { get; set; }

}

/// <summary>删除小店活动门店响应·【删除失败的小店活动门店列表】 删除失败的小店活动门店列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384431"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreDeleteResponseFailedStoresItem
{
    /// <summary>【门店编码】 门店编码，可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>failed_stores.store_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("store_code")]
    public string? StoreCode { get; set; }

    /// <summary>【门店名称】 门店名称（官方字段路径 <c>failed_stores.store_name</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

}

/// <summary>删除小店活动门店响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384431"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreDeleteResponse : WechatPayResponse
{
    /// <summary>【删除时间】 删除时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>delete_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delete_time")]
    public string? DeleteTime { get; set; }

    /// <summary>【删除失败的小店活动门店列表】 删除失败的小店活动门店列表（官方字段路径 <c>failed_stores</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("failed_stores")]
    public List<RetailStoreActStoreDeleteResponseFailedStoresItem>? FailedStores { get; set; }

}

/// <summary>查询小店活动门店列表响应·【小店活动门店集合】 小店活动门店集合。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384414"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreListResponseDataItem
{
    /// <summary>【门店编码】 门店编码，可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>data.store_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("store_code")]
    public string? StoreCode { get; set; }

    /// <summary>【门店名称】 门店名称（官方字段路径 <c>data.store_name</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

}

/// <summary>查询小店活动门店列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384414"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreListResponse : WechatPayResponse
{
    /// <summary>【小店活动门店总数】 小店活动门店总数（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【小店活动门店集合】 小店活动门店集合（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<RetailStoreActStoreListResponseDataItem>? Data { get; set; }

    /// <summary>【分页页码】 页码从0开始，默认第0页（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}

/// <summary>查询小店活动门店详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384437"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActStoreQueryResponse : WechatPayResponse
{
    /// <summary>【门店编码】 门店编码，可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>store_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("store_code")]
    public string? StoreCode { get; set; }

    /// <summary>【门店名称】 门店名称（官方字段路径 <c>store_name</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

}
