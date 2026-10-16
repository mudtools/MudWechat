// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.WebizpayUser;

/// <summary>企业商户发起员工开通企业支付授权请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507153"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayUser")]
public class WebizpayUserAuthorizationUrlRequest
{
    /// <summary>【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【发起授权应用类型】 用户从企业应用跳转至微信客户端完成授权，需传入发起授权企业应用类型。可选取值：`H5` 表示从商户的应用是外部H5，跳转微信拉起企业支付相关能力 / `MINIPROGRAM` 表示从商户的应用是微信小程序，跳转微信拉起企业支付相关能力（官方字段路径 <c>application_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("application_type")]
    public string? ApplicationType { get; set; }

}

/// <summary>企业商户发起员工开通企业支付授权响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507153"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayUser")]
public class WebizpayUserAuthorizationUrlResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号，具有企业支付服务商资质的微信支付商户号，由商户请求时传入（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资子商户号，企业支付的出资方商户号，由商户请求时传入（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户企业员工ID】 商户企业员工ID，由企业系统提供的唯一标识员工的ID，由商户请求时传入（官方字段路径 <c>user_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>【跳转链接】 跳转链接，用于员工授权企业支付功能的链接，通过schema跳转打开微信即可拉起企业支付开通页面，仅当可以开通时返回（官方字段路径 <c>authorization_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("authorization_url")]
    public string? AuthorizationUrl { get; set; }

    /// <summary>【链接失效时间 】 定义：链接失效时间，超过这个时间后，用户再通过此链接尝试拉起微信注册企业支付会拦截请求，需要重新调用此接口获取新的跳转链接。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T…（官方字段路径 <c>expire_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

    /// <summary>【发起授权应用类型】 由商户请求时传入。可选取值：`H5` 表示从商户的应用是外部H5，跳转微信拉起企业支付相关能力 / `MINIPROGRAM` 表示从商户的应用是微信小程序，跳转微信拉起企业支付相关能力（官方字段路径 <c>application_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("application_type")]
    public string? ApplicationType { get; set; }

    /// <summary>【小程序查询参数】 在类型为MINIPROGRAM 时，若允许拉起员工开通页面则会返回该参数（有效期35分钟），拉起企业支付页面可参考 从小程序调起企业支付相关能力-wx.openBusinessView 此参数对应 从小程序调起企业支付相关能力-wx.openBusinessView 中的queryString（官方字段路径 <c>mp_query</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("mp_query")]
    public string? MpQuery { get; set; }

    /// <summary>【小程序业务类型】 在类型为MINIPROGRAM 时，若允许拉起员工开通页面则会返回该参数（有效期35分钟），拉起企业支付页面可参考 从小程序调起企业支付相关能力-wx.openBusinessView 此参数对应 从小程序调起企业支付相关能力-wx.openBusinessView 中的businessType（官方字段路径 <c>mp_business_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mp_business_type")]
    public string? MpBusinessType { get; set; }

}

/// <summary>企业商户查询企业员工开通授权状态响应·【微信授权关系列表】 微信授权关系列表，包含该员工所有的微信企业支付授权关系，若。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507163"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayUser")]
public class WebizpayUserAuthorizationStateResponseEmployeeListItem
{
    /// <summary>【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用于后续额度卡发放、注销等操作 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方字段路径 <c>employee_list.employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("employee_id")]
    public string? EmployeeId { get; set; }

    /// <summary>【授权状态】 授权状态，表示员工授权关系是否有效，包括已授权(AUTHORIZED)和已解除授权(REVOKED)两种状态。可选取值：`AUTHORIZED` 已授权 / `REVOKED` 已解除授权（官方字段路径 <c>employee_list.authorization_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authorization_state")]
    public string? AuthorizationState { get; set; }

    /// <summary>【授权时间】 授权时间，员工完成授权操作的时间。 格式要求： 需遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>employee_list.authorization_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authorization_time")]
    public string? AuthorizationTime { get; set; }

    /// <summary>【解除授权时间】 解除授权时间，员工或企业解除授权的时间。 格式要求： 需遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29…（官方字段路径 <c>employee_list.authorization_revoked_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authorization_revoked_time")]
    public string? AuthorizationRevokedTime { get; set; }

}

/// <summary>企业商户查询企业员工开通授权状态响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507163"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayUser")]
public class WebizpayUserAuthorizationStateResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号，具有企业支付服务商资质的微信支付商户号，由商户请求时传入（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资子商户号，企业支付的出资方商户号，由商户请求时传入（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户企业员工ID】 商户企业员工ID，由企业系统提供的唯一标识员工的ID，由商户请求时传入（官方字段路径 <c>user_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>【微信授权关系列表】 微信授权关系列表，包含该员工所有的微信企业支付授权关系，若此商户下，相同企业员工ID没有开通过企业支付，则数组长度为0（官方字段路径 <c>employee_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("employee_list")]
    public List<WebizpayUserAuthorizationStateResponseEmployeeListItem>? EmployeeList { get; set; }

}
