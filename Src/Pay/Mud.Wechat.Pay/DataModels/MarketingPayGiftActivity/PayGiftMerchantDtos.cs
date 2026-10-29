// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingPayGiftActivity;

/// <summary>查询活动商户列表响应·【结果集】 商户信息列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466149"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityMerchantsResponseDataItem
{
    /// <summary>【商户Id】 商户Id（官方字段路径 <c>data.mchid</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【商户名称】 商户名称（官方字段路径 <c>data.merchant_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>【创建时间】 创建时间（官方字段路径 <c>data.create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间（官方字段路径 <c>data.update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>查询活动商户列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466149"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityMerchantsResponse : WechatPayResponse
{
    /// <summary>【结果集】 商户信息列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<PayGiftActivityMerchantsResponseDataItem>? Data { get; set; }

    /// <summary>【总数】 商户数量（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页页码】 分页页码（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【活动Id】 活动Id（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

}

/// <summary>添加活动商户请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466671"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftMerchantAddRequest
{
    /// <summary>【添加的发券商户号】 新增到活动中的发券商户号列表（官方字段路径 <c>merchant_id_list</c>，<b>必填</b>，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("merchant_id_list")]
    public List<string>? MerchantIdList { get; set; }

    /// <summary>【请求业务单据号】 商户添加发券商户号的凭据号，商户侧需保持唯一性（官方字段路径 <c>add_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("add_request_no")]
    public string? AddRequestNo { get; set; }

}

/// <summary>添加活动商户响应·【校验失败的发券商户号】 未通过规则校验的发券商户号列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466671"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftMerchantAddResponseInvalidMerchantIdListItem
{
    /// <summary>【商户Id】 商户Id（官方字段路径 <c>invalid_merchant_id_list.mchid</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【无效原因】 活动参与商户校验失败的原因（官方字段路径 <c>invalid_merchant_id_list.invalid_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("invalid_reason")]
    public string? InvalidReason { get; set; }

}

/// <summary>添加活动商户响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466671"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftMerchantAddResponse : WechatPayResponse
{
    /// <summary>【活动Id】 活动Id（官方字段路径 <c>activity_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【校验失败的发券商户号】 未通过规则校验的发券商户号列表（官方字段路径 <c>invalid_merchant_id_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("invalid_merchant_id_list")]
    public List<PayGiftMerchantAddResponseInvalidMerchantIdListItem>? InvalidMerchantIdList { get; set; }

    /// <summary>【添加时间】 成功添加发券商户号的时间（官方字段路径 <c>add_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("add_time")]
    public string? AddTime { get; set; }

}

/// <summary>删除活动商户请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466787"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftMerchantDeleteRequest
{
    /// <summary>【删除的发券商户号】 从活动已有的发券商户号中移除的商户号列表（官方字段路径 <c>merchant_id_list</c>，<b>必填</b>，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("merchant_id_list")]
    public List<string>? MerchantIdList { get; set; }

    /// <summary>【请求业务单据号】 商户添加发券商户号的凭据号，商户侧需保持唯一性（官方字段路径 <c>delete_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("delete_request_no")]
    public string? DeleteRequestNo { get; set; }

}

/// <summary>删除活动商户响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466787"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftMerchantDeleteResponse : WechatPayResponse
{
    /// <summary>【活动Id】 活动Id（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【删除时间】 成功删除发券商户号的时间（官方字段路径 <c>delete_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delete_time")]
    public string? DeleteTime { get; set; }

}
