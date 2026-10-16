// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceContract;

/// <summary>开通保险理赔功能请求·【签约协议列表】 签约协议列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012781476"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceContract")]
public class InsuranceContractOpenRequestAgreementListItem
{
    /// <summary>【协议编号】 协议编号（官方字段路径 <c>agreement_list.agreement_code</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("agreement_code")]
    public string? AgreementCode { get; set; }

    /// <summary>【协议版本号】 版本号为协议发布时间，格式如20240815（官方字段路径 <c>agreement_list.agreement_version</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("agreement_version")]
    public string? AgreementVersion { get; set; }

    /// <summary>【签约主体】 保险公司的商户主体名称（官方字段路径 <c>agreement_list.sign_organization</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sign_organization")]
    public string? SignOrganization { get; set; }

    /// <summary>【签约时间】 签约成功的时间，遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>agreement_list.sign_time</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sign_time")]
    public string? SignTime { get; set; }

}

/// <summary>开通保险理赔功能请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012781476"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceContract")]
public class InsuranceContractOpenRequest
{
    /// <summary>【商户申请单号】 需保证在业务方系统内唯一，只能是数字、大小写字母（官方字段路径 <c>out_apply_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_apply_no")]
    public string? OutApplyNo { get; set; }

    /// <summary>【二级商户号】 需开通保险理赔功能的保险公司商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【签约协议列表】 签约协议列表（官方字段路径 <c>agreement_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("agreement_list")]
    public List<InsuranceContractOpenRequestAgreementListItem>? AgreementList { get; set; }

}

/// <summary>开通保险理赔功能响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012781476"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceContract")]
public class InsuranceContractOpenResponse : WechatPayResponse
{
    /// <summary>【二级商户号】 保险公司的商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【是否开通成功】 保险理赔功能的开通结果 开通成功：true 开通失败：false（官方字段路径 <c>opened</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("opened")]
    public bool? Opened { get; set; }

    /// <summary>【成功时间】 开通成功时返回，时间格式遵循rfc3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>success_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

}
