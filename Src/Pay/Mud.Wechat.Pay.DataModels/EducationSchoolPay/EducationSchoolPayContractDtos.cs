// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.EducationSchoolPay;

/// <summary>查询校园缴费签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468959"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayContractInfoResponse : WechatPayResponse
{
    /// <summary>【签约协议号】 微信用户在校园轻松付下的合约的唯一标识，用户在校园轻松付小程序完成签约后，由微信支付生成（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【商户号】 微信支付分配的商户号。 如果是服务商模式接入则为服务商商户号；如果是直连商户模式接入则为直连商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【商户AppID】 微信公众平台（mp.weixin.qq.com）为商户分配的公众账号ID，与商户号存在绑定关系。 如果是服务商模式接入则为服务商商户号绑定的AppID；如果是直连商户模式接入则为直连商户号绑定的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户AppID下的用户标识】 用户在商户AppID下的唯一标识。如果是服务商模式接入则为服务商商户AppID下的唯一标识；如果是直连商户模式接入则为直连商户AppID下的唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【签约模板号】 签约模板的唯一标识。商户在商户平台（pay.weixin.qq.com）接入校园轻松付，并为学校申请了签约模板后，由微信支付生成（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【签约状态】 ADD-已签约; DELETE-已解约（官方字段路径 <c>contract_status</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_status")]
    public string? ContractStatus { get; set; }

    /// <summary>【签约时间】 签约完成时间，使用 rfc3339 所定义的格式，即为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【外部签约号】 由商户自行生成的签约号，要求64个字符内，只能是数字、大小写字母和 `_-`，且在同一签约模板下唯一（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

}

/// <summary>按用户查询校园缴费签约列表响应·【查询结果列表】 符合查询条件的用户签约协议列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468993"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayUserContractListResponseDataItem
{
    /// <summary>【签约协议号】 微信用户在校园轻松付下的合约的唯一标识，用户在校园轻松付小程序完成签约后，由微信支付生成（官方字段路径 <c>data.contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【商户号】 微信支付分配的商户号。 如果是服务商模式接入则为服务商商户号；如果是直连商户模式接入则为直连商户号（官方字段路径 <c>data.mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【商户AppID】 微信公众平台（mp.weixin.qq.com）为商户分配的公众账号ID，与商户号存在绑定关系。 如果是服务商模式接入则为服务商商户号绑定的AppID；如果是直连商户模式接入则为直连商户号绑定的AppID（官方字段路径 <c>data.appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户AppID下的用户标识】 用户在商户AppID下的唯一标识。如果是服务商模式接入则为服务商商户AppID下的唯一标识；如果是直连商户模式接入则为直连商户AppID下的唯一标识（官方字段路径 <c>data.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【签约模板号】 签约模板的唯一标识。商户在商户平台（pay.weixin.qq.com）接入校园轻松付，并为学校申请了签约模板后，由微信支付生成（官方字段路径 <c>data.plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【签约状态】 ADD-已签约; DELETE-已解约（官方字段路径 <c>data.contract_status</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_status")]
    public string? ContractStatus { get; set; }

    /// <summary>【签约时间】 签约完成时间，使用 rfc3339 所定义的格式，即为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>data.create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【外部签约号】 由商户自行生成的签约号，要求64个字符内，只能是数字、大小写字母和 `_-`，且在同一签约模板下唯一（官方字段路径 <c>data.out_contract_code</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

}

/// <summary>按用户查询校园缴费签约列表响应·【相关链接】 请求翻页相关链接。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468993"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayUserContractListResponseLinks
{
    /// <summary>【下一页链接】 使用同样的limit进行下一页查询时的请求链接。如果已经到最后时，为空（官方字段路径 <c>links.next</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }

    /// <summary>【上一页链接】 使用同样的limit进行上一页查询时的请求链接。如果是第一页，为空（官方字段路径 <c>links.prev</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("prev")]
    public string? Prev { get; set; }

    /// <summary>【当前链接】 当前的请求链接（官方字段路径 <c>links.self</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("self")]
    public string? Self { get; set; }

}

/// <summary>按用户查询校园缴费签约列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468993"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayUserContractListResponse : WechatPayResponse
{
    /// <summary>【查询结果列表】 符合查询条件的用户签约协议列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<EducationSchoolPayUserContractListResponseDataItem>? Data { get; set; }

    /// <summary>【请求资源起始位置】 该次请求资源的起始位置，请求参数原样返回（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【最大资源条数】 该次请求可返回的最大资源条数，请求参数原样返回（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【资源总条数】 符合查询条件的资源总条数（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【相关链接】 请求翻页相关链接（官方字段路径 <c>links</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("links")]
    public EducationSchoolPayUserContractListResponseLinks? Links { get; set; }

}
