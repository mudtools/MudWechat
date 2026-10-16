// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PayrollCard;

/// <summary>凭授权单号预下单取令牌请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461761"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardAuthenticationPreOrderRequest
{
    /// <summary>【用户OpenID】 用户OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【服务商AppID】 当输入服务商AppID时，会校验其与服务商商户号的绑定关系。服务商AppID和与特约商户AppID至少输入一个，且必须要有拉起微工卡小程序时使用的AppID（官方字段路径 <c>appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【特约商户号】 特约商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【特约商户AppID】 当输入特约商户AppID时，会校验其与特约商户号的绑定关系。服务商AppID和与特约商户AppID至少输入一个，且必须要有拉起微工卡小程序时使用的AppID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【商家核身单号】 商户系统内部的商家核身单号，要求此参数只能由数字、大小写字母组成，在服务商内部唯一（官方字段路径 <c>authenticate_number</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("authenticate_number")]
    public string? AuthenticateNumber { get; set; }

    /// <summary>【项目名称】 该劳务活动的项目名称（官方字段路径 <c>project_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("project_name")]
    public string? ProjectName { get; set; }

    /// <summary>【用工单位名称】 该工人所属的用工企业（官方字段路径 <c>employer_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("employer_name")]
    public string? EmployerName { get; set; }

    /// <summary>【用户实名】 用户实名信息，按照APIV3标准加密该字段（官方字段路径 <c>user_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【用户证件号】 用户证件号，按照APIV3标准加密该字段（官方字段路径 <c>id_card_number</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>【用工类型】 微工卡服务仅支持用于与商户有用工关系的用户，需明确用工类型；非必填；未授权必填，授权过，非必填，如填写，需要校验和初次填写的一致性；参考值：长期用工：LONG_TERM_EMPLOYMENT，短期用工：SHORT_TERM_EMPLOYMENT，合作关系：COOPERATION_EMPLOYMENT。可选取值：`LONG_TERM_EMPLOYMENT` 长期用工：有劳务合同关系的长期用工 / `SHORT_TERM_EMPLOYMENT` 短期用工：有劳务合同关系的短期用工 / `COOPERATION_EMPLOYMENT` 合作关系：有业务服务、合作关系的人员（官方字段路径 <c>employment_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("employment_type")]
    public string? EmploymentType { get; set; }

    /// <summary>【核身类型】 核身类型。可选取值：`NORMAL` 普通核身类型，可用于用户注册认证等 / `SIGN_IN` 签到打卡类型，需要传项目信息 / `INSURANCE` 投保类型 / `CONTRACT` 签约类型（官方字段路径 <c>authenticate_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authenticate_type")]
    public string? AuthenticateType { get; set; }

}

/// <summary>凭授权单号预下单取令牌响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461761"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardAuthenticationPreOrderResponse : WechatPayResponse
{
    /// <summary>【商家核身单号】 商户系统内部的商家核身单号，要求此参数只能由数字、大小写字母组成，在服务商内部唯一（官方字段路径 <c>authenticate_number</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("authenticate_number")]
    public string? AuthenticateNumber { get; set; }

    /// <summary>【微信用户OpenID】 微信用户OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户号】 商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【特约商户号】 特约商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【token值】 token值（官方字段路径 <c>token</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>【token有效时间】 token有效时间，单位秒（官方字段路径 <c>expires_in</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("expires_in")]
    public long? ExpiresIn { get; set; }

}

/// <summary>查询代发薪授权单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461785"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardAuthenticationInfoResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【特约商户商户号】 特约商户商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【用户OpenID】 用户OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【核身渠道】 核身渠道，发起核身时的来源渠道，如通过小程序，硬件设备等。可选取值：`FROM_MINI_APP` 核身场景来源于小程序 / `FROM_HARDWARE` 核身场景来源于硬件设备（官方字段路径 <c>authenticate_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authenticate_scene")]
    public string? AuthenticateScene { get; set; }

    /// <summary>【核身渠道标识】 核身渠道标识，用于定位渠道具体来源，如果是扫码打卡渠道标识就是具体的小程序AppID，若是硬件设备，则是设备的序列号等（官方字段路径 <c>authenticate_source</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("authenticate_source")]
    public string? AuthenticateSource { get; set; }

    /// <summary>【项目名称】 项目名称（官方字段路径 <c>project_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("project_name")]
    public string? ProjectName { get; set; }

    /// <summary>【用工企业】 该工人所属的用工企业，由商户核身下单时传入（官方字段路径 <c>employer_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("employer_name")]
    public string? EmployerName { get; set; }

    /// <summary>【核身状态】 核身状态。可选取值：`AUTHENTICATE_PROCESSING` 正在进行核身中 / `AUTHENTICATE_SUCCESS` 核身已经成功 / `AUTHENTICATE_FAILED` 核身已经失败（官方字段路径 <c>authenticate_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authenticate_state")]
    public string? AuthenticateState { get; set; }

    /// <summary>【核身时间】 核身时间，遵循RFC3339标准格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>authenticate_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authenticate_time")]
    public string? AuthenticateTime { get; set; }

    /// <summary>【商家核身单号】 商户系统内部的商家核身单号，要求此参数只能由数字、大小写字母组成，在服务商内部唯一（官方字段路径 <c>authenticate_number</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("authenticate_number")]
    public string? AuthenticateNumber { get; set; }

    /// <summary>【核身失败原因】 结果为核身失败时的原因描述，仅在失败记录返回（官方字段路径 <c>authenticate_failed_reason</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("authenticate_failed_reason")]
    public string? AuthenticateFailedReason { get; set; }

    /// <summary>【核身类型】 核身类型。可选取值：`NORMAL` 普通核身类型，可用于用户注册认证等 / `SIGN_IN` 签到打卡类型，需要传项目信息 / `INSURANCE` 投保类型 / `CONTRACT` 签约类型（官方字段路径 <c>authenticate_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authenticate_type")]
    public string? AuthenticateType { get; set; }

}

/// <summary>按日期分页查询代发薪授权单列表响应·【查询结果记录列表】 查询结果记录列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461820"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardAuthenticationListResponseDataItem
{
    /// <summary>【服务商商户号】 服务商商户号（官方字段路径 <c>data.mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【特约商户商户号】 特约商户商户号（官方字段路径 <c>data.sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【用户OpenID】 用户OpenID（官方字段路径 <c>data.openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【核身渠道】 核身渠道，发起核身时的来源渠道，如通过小程序，硬件设备等。可选取值：`FROM_MINI_APP` 核身场景来源于小程序 / `FROM_HARDWARE` 核身场景来源于硬件设备（官方字段路径 <c>data.authenticate_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authenticate_scene")]
    public string? AuthenticateScene { get; set; }

    /// <summary>【核身渠道标识】 核身渠道标识，用于定位渠道具体来源，如果是扫码打卡渠道标识就是具体的小程序AppID，若是硬件设备，则是设备的序列号等（官方字段路径 <c>data.authenticate_source</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("authenticate_source")]
    public string? AuthenticateSource { get; set; }

    /// <summary>【项目名称】 项目名称（官方字段路径 <c>data.project_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("project_name")]
    public string? ProjectName { get; set; }

    /// <summary>【用工企业】 该工人所属的用工企业，由商户核身下单时传入（官方字段路径 <c>data.employer_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("employer_name")]
    public string? EmployerName { get; set; }

    /// <summary>【核身状态】 核身状态。可选取值：`AUTHENTICATE_PROCESSING` 正在进行核身中 / `AUTHENTICATE_SUCCESS` 核身已经成功 / `AUTHENTICATE_FAILED` 核身已经失败（官方字段路径 <c>data.authenticate_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authenticate_state")]
    public string? AuthenticateState { get; set; }

    /// <summary>【核身时间】 核身时间，遵循RFC3339标准格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>data.authenticate_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authenticate_time")]
    public string? AuthenticateTime { get; set; }

    /// <summary>【商家核身单号】 商户系统内部的商家核身单号，要求此参数只能由数字、大小写字母组成，在服务商内部唯一（官方字段路径 <c>data.authenticate_number</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("authenticate_number")]
    public string? AuthenticateNumber { get; set; }

    /// <summary>【核身失败原因】 结果为核身失败时的原因描述，仅在失败记录返回（官方字段路径 <c>data.authenticate_failed_reason</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("authenticate_failed_reason")]
    public string? AuthenticateFailedReason { get; set; }

    /// <summary>【核身类型】 核身类型。可选取值：`NORMAL` 普通核身类型，可用于用户注册认证等 / `SIGN_IN` 签到打卡类型，需要传项目信息 / `INSURANCE` 投保类型 / `CONTRACT` 签约类型（官方字段路径 <c>data.authenticate_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authenticate_type")]
    public string? AuthenticateType { get; set; }

}

/// <summary>按日期分页查询代发薪授权单列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461820"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardAuthenticationListResponse : WechatPayResponse
{
    /// <summary>【查询结果记录列表】 查询结果记录列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<PayrollCardAuthenticationListResponseDataItem>? Data { get; set; }

    /// <summary>【总记录条数】 经过条件筛选，查询到的记录总数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【记录起始位置】 该次请求资源的起始位置，请求中包含偏移量时应答消息返回相同偏移量，否则返回默认值0（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【本次返回条数】 经过条件筛选，本次查询到的记录条数（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}
