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

/// <summary>设置活动门店 representative请求·【业务代理信息列表】 业务代理信息列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384386"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeSetRequestRepresentativeInfoListItem
{
    /// <summary>【业务代理的OpenID】 获取openid请查看文档（官方字段路径 <c>representative_info_list.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>设置活动门店 representative请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384386"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeSetRequest
{
    /// <summary>【业务代理信息列表】 业务代理信息列表（官方字段路径 <c>representative_info_list</c>，<b>必填</b>，类型 <c>array[RepresentativeInfo]</c>）。</summary>
    [JsonPropertyName("representative_info_list")]
    public List<RetailStoreActRepresentativeSetRequestRepresentativeInfoListItem>? RepresentativeInfoList { get; set; }

    /// <summary>【请求业务单据号】 商户新增业务代理请求的唯一标识，商户需保证唯一。可包含英文字母，数字，&amp;#124;，_，\*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【添加时间】 添加时间（官方字段路径 <c>add_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("add_time")]
    public string? AddTime { get; set; }

}

/// <summary>设置活动门店 representative响应·【添加失败业务代理信息列表】 添加失败业务代理信息列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384386"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeSetResponseFailedRepresentativeInfoListItem
{
    /// <summary>【业务代理的OpenID】 获取openid请查看文档（官方字段路径 <c>failed_representative_info_list.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>设置活动门店 representative响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384386"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeSetResponse : WechatPayResponse
{
    /// <summary>【零售小店活动ID】 零售小店活动ID（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【添加失败业务代理信息列表】 添加失败业务代理信息列表（官方字段路径 <c>failed_representative_info_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("failed_representative_info_list")]
    public List<RetailStoreActRepresentativeSetResponseFailedRepresentativeInfoListItem>? FailedRepresentativeInfoList { get; set; }

    /// <summary>【添加时间】 添加时间（官方字段路径 <c>add_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("add_time")]
    public string? AddTime { get; set; }

}

/// <summary>删除活动门店 representative请求·【业务代理信息列表】 业务代理信息列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384399"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeDeleteRequestRepresentativeInfoListItem
{
    /// <summary>【业务代理的OpenID】 获取openid请查看文档（官方字段路径 <c>representative_info_list.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>删除活动门店 representative请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384399"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeDeleteRequest
{
    /// <summary>【业务代理信息列表】 业务代理信息列表（官方字段路径 <c>representative_info_list</c>，<b>必填</b>，类型 <c>array[RepresentativeInfo]</c>）。</summary>
    [JsonPropertyName("representative_info_list")]
    public List<RetailStoreActRepresentativeDeleteRequestRepresentativeInfoListItem>? RepresentativeInfoList { get; set; }

    /// <summary>【请求业务单据号】 商户删除业务代理请求的唯一标识，商户需保证唯一。可包含英文字母，数字，&amp;#124;，_，\*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【删除时间】 删除时间（官方字段路径 <c>delete_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delete_time")]
    public string? DeleteTime { get; set; }

}

/// <summary>删除活动门店 representative响应·【删除失败业务代理信息列表】 删除失败业务代理信息列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384399"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeDeleteResponseFailedRepresentativeInfoListItem
{
    /// <summary>【业务代理的OpenID】 获取openid请查看文档（官方字段路径 <c>failed_representative_info_list.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>删除活动门店 representative响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384399"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeDeleteResponse : WechatPayResponse
{
    /// <summary>【零售小店活动ID】 零售小店活动ID（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【删除失败业务代理信息列表】 删除失败业务代理信息列表（官方字段路径 <c>failed_representative_info_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("failed_representative_info_list")]
    public List<RetailStoreActRepresentativeDeleteResponseFailedRepresentativeInfoListItem>? FailedRepresentativeInfoList { get; set; }

    /// <summary>【删除时间】 删除时间（官方字段路径 <c>delete_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delete_time")]
    public string? DeleteTime { get; set; }

}

/// <summary>查询活动门店 representative 列表响应·【小店活动业代集合】 小店活动业代集合。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384405"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeListResponseDataItem
{
    /// <summary>【业务代理的OpenID】 获取openid请查看文档（官方字段路径 <c>data.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>查询活动门店 representative 列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384405"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActRepresentativeListResponse : WechatPayResponse
{
    /// <summary>【小店活动业代总数】 小店活动业代总数（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【小店活动业代集合】 小店活动业代集合（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<RetailStoreActRepresentativeListResponseDataItem>? Data { get; set; }

    /// <summary>【分页页码】 页码从0开始，默认第0页（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}
