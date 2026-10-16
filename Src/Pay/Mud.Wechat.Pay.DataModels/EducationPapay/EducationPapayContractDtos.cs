// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.EducationPapay;

/// <summary>按微信协议号查询教育代扣签约响应·【签约信息】 签约信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012522951"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465367"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayContractInfoResponseContractInformation
{
    /// <summary>【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方字段路径 <c>contract_information.contract_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【签约状态】 ADD-签约; DELETE-解约（官方字段路径 <c>contract_information.contract_status</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_status")]
    public string? ContractStatus { get; set; }

    /// <summary>【签约时间】 签约完成时间，按照使用 rfc3339 所定义的格式，格式为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>contract_information.create_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

}

/// <summary>按微信协议号查询教育代扣签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012522951"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465367"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayContractInfoResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【服务商APPID下的用户标识】 用户在服务商appid下的唯一标识（官方字段路径 <c>openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【签约模版号】 签约模版号，签约模版唯一标识（官方字段路径 <c>plan_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【签约信息】 签约信息（官方字段路径 <c>contract_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("contract_information")]
    public EducationPapayContractInfoResponseContractInformation? ContractInformation { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【子商户APPID下的用户标识】 用户在子商户appid下的唯一标识，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>sub_openid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>按用户查询教育代扣签约列表响应·【结果集】 符合条件的签约信息·【签约信息】 签约信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523265"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012489548"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayUserContractListResponseDataItemContractInformation
{
    /// <summary>【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方字段路径 <c>data.contract_information.contract_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【签约状态】 ADD-签约; DELETE-解约（官方字段路径 <c>data.contract_information.contract_status</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_status")]
    public string? ContractStatus { get; set; }

    /// <summary>【签约时间】 签约完成时间，按照使用 rfc3339 所定义的格式，格式为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>data.contract_information.create_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

}

/// <summary>按用户查询教育代扣签约列表响应·【结果集】 符合条件的签约信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523265"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012489548"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayUserContractListResponseDataItem
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>data.sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方字段路径 <c>data.appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【服务商APPID下的用户标识】 用户在服务商appid下的唯一标识（官方字段路径 <c>data.openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【签约模版号】 签约模版号，签约模版唯一标识（官方字段路径 <c>data.plan_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【签约信息】 签约信息（官方字段路径 <c>data.contract_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("contract_information")]
    public EducationPapayUserContractListResponseDataItemContractInformation? ContractInformation { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>data.sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>data.sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【子商户APPID下的用户标识】 用户在子商户appid下的唯一标识，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>data.sub_openid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>按用户查询教育代扣签约列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523265"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012489548"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayUserContractListResponse : WechatPayResponse
{
    /// <summary>【结果集】 符合条件的签约信息（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<EducationPapayUserContractListResponseDataItem>? Data { get; set; }

    /// <summary>【总数量】 资源总条数（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页页码】 该次请求资源的起始位置，从0开始计数。调用方选填，默认为0（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 该次请求可返回的最大资源条数。调用方选填，默认值为20（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}
