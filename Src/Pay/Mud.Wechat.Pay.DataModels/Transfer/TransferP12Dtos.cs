// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Transfer;

/// <summary>商户单号申请电子回单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716452"/></summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferElecsignApplyRequest
{
    /// <summary>【商户转账单号】 商户创建转账单据使用的单号（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

}

/// <summary>商户单号申请电子回单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716452"/></summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferElecsignApplyResponse : WechatPayResponse
{
    /// <summary>【申请单状态】 GENERATING：生成中，FINISHED: 已完成，FAILED: 已失败。可选取值：`GENERATING` 表示当前电子回单已受理成功并在处理中 / `FINISHED` 表示当前电子回单已处理完成 / `FAILED` 表示当前电子回单生成失败，失败原因字段会返回具体的失败原因（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【申请单创建时间】 申请单的创建时间，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

}

/// <summary>微信单号申请电子回单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716456"/></summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferElecsignApplyByWxNoRequest
{
    /// <summary>【微信转账单号】 转账单据对应的微信支付单号（官方字段路径 <c>transfer_bill_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

}

/// <summary>发起转账并完成免确认收款授权请求·【免确认收款授权信息】 如希望在用户确认收款后获取免确认收款授权，需要传入免确认。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399293"/></summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferPreTransferWithAuthorizationRequestAuthorizationInfo
{
    /// <summary>【用户展示名称】 用户免确认收款授权详情中展示的昵称，区分用户在商户侧的不同账号。不支持非utf8编码的字符和表情等特殊字符（官方字段路径 <c>authorization_info.user_display_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_display_name")]
    public string? UserDisplayName { get; set; }

    /// <summary>【商户侧授权单号】 商户系统内部的免确认收款授权单号，商户需保证在系统内唯一且一个用户账号对应唯一一个生效中的授权（官方字段路径 <c>authorization_info.out_authorization_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_authorization_no")]
    public string? OutAuthorizationNo { get; set; }

    /// <summary>【免确认收款授权结果通知地址】 异步接收免确认收款授权结果通知的回调地址，通知url必须为公网可访问的URL，必须为HTTPS，不能携带参数（官方字段路径 <c>authorization_info.authorization_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("authorization_notify_url")]
    public string? AuthorizationNotifyUrl { get; set; }

}

/// <summary>发起转账并完成免确认收款授权请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399293"/></summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferPreTransferWithAuthorizationRequest
{
    /// <summary>【商户AppID】 是微信开放平台和微信公众平台为开发者的应用程序(APP、小程序、公众号、企业号corpid即为此AppID)提供的一个唯一标识。此处，可以填写这四种类型中的任意一种APPID，但请确保该appid与商户号有绑定关。参考 微信支付普通商户与AppID账号关联管理（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【转账场景ID】 该笔转账使用的转账场景，可前往“商户平台-产品中心-商家转账”中申请。如：1000（现金营销），1006（企业报销）等（官方字段路径 <c>transfer_scene_id</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【收款用户OpenID】 用户在商户appid下的唯一标识。发起转账前需获取到用户的OpenID，参考 接口参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【收款用户姓名】 收款方真实姓名。若传入收款用户姓名，微信支付会校验收款用户与输入姓名是否一致。转账金额&gt;=2,000元时，必须传入该值。 该字段需要加密传入，参考 微信支付公钥加密敏感信息指引(推荐)、 平台证书加密敏感信息指引（官方字段路径 <c>user_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【转账金额】 转账金额，单位为“分”。参考 转账额度说明（官方字段路径 <c>transfer_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>【转账备注】 转账备注，用户收款时可见该备注信息，UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【通知地址】 异步接收商家转账结果的回调地址，通知url必须为公网可访问的URL，必须为HTTPS，不能携带参数（官方字段路径 <c>notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【用户收款感知】 用户收款时感知的收款原因。不填或填空，将展示转账场景的默认内容。如有其他展示需求，参考 商家转账产品介绍（官方字段路径 <c>user_recv_perception</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_recv_perception")]
    public string? UserRecvPerception { get; set; }

    /// <summary>【转账场景报备信息】 需按转账场景准确填写报备信息，参考 转账场景报备信息字段说明（官方字段路径 <c>transfer_scene_report_infos</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("transfer_scene_report_infos")]
    public List<TransferSceneReportInfo>? TransferSceneReportInfos { get; set; }

    /// <summary>【免确认收款授权信息】 如希望在用户确认收款后获取免确认收款授权，需要传入免确认收款授权信息（官方字段路径 <c>authorization_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("authorization_info")]
    public TransferPreTransferWithAuthorizationRequestAuthorizationInfo? AuthorizationInfo { get; set; }

}

/// <summary>发起转账并完成免确认收款授权响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399293"/></summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferPreTransferWithAuthorizationResponse : WechatPayResponse
{
    /// <summary>【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【微信转账单号】 微信转账单号，微信商家转账系统返回的唯一标识（官方字段路径 <c>transfer_bill_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>【单据创建时间】 单据受理成功时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【单据状态】 商家转账订单状态。可选取值：`ACCEPTED` 转账已受理，可原单重试（非终态）。 / `PROCESSING` 转账锁定资金中。如果一直停留在该状态，建议检查账户余额是否足够，如余额不足，可充值后再原单重试（非终态）。 / `WAIT_USER_CONFIRM` 待收款用户确认，当前转账单据资金已锁定，可拉起微信收款确认页面进行收款确认（非终态）。 / `TRANSFERING` 转账中，可拉起微信收款确认页面再次重试确认收款（非终态）。 / `SUCCESS` 转账成功，表示转账单据已成功（终态）。 / `CANCELING` 转账撤销中，商户撤销请求受理成功，该笔转账正在撤销中，需查单确认撤销的转账单据状态（非终态）。（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【跳转领取页面的package信息】 跳转微信支付收款页的package信息， APP调起用户确认收款 或者 JSAPI调起用户确认收款 时需要使用的参数。仅当转账单据状态为WAIT_USER_CONFIRM时返回。 单据创建后，用户24小时内不领取将过期关闭，建议拉起用户确认收款页面前，先查单据状态：如单据状态为WAIT_USER_CONFIRM，可用之前的package信息拉起；单据到终态时需更换单号重新发起转账（官方字段路径 <c>package_info</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("package_info")]
    public string? PackageInfo { get; set; }

    /// <summary>【用户展示昵称】 用户免确认收款授权详情中展示的昵称（官方字段路径 <c>user_display_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_display_name")]
    public string? UserDisplayName { get; set; }

    /// <summary>【商户侧授权单号】 商户系统内部的免确认收款授权单号（官方字段路径 <c>out_authorization_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_authorization_no")]
    public string? OutAuthorizationNo { get; set; }

}

/// <summary>用户授权后转账请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399371"/></summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferAuthorizedTransferRequest
{
    /// <summary>【商户AppID】 是微信开放平台和微信公众平台为开发者的应用程序(APP、小程序、公众号、企业号corpid即为此AppID)提供的一个唯一标识。此处，可以填写这四种类型中的任意一种APPID，但请确保该appid与商户号有绑定关。参考 微信支付普通商户与AppID账号关联管理（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【转账场景ID】 该笔转账使用的转账场景，可前往“商户平台-产品中心-商家转账”中申请。如：1000（现金营销），1006（企业报销）等（官方字段路径 <c>transfer_scene_id</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【收款用户姓名】 收款方真实姓名。若传入收款用户姓名，微信支付会校验收款用户与输入姓名是否一致。转账金额&gt;=2,000元时，必须传入该值。 该字段需要加密传入，参考 微信支付公钥加密敏感信息指引(推荐)、 平台证书加密敏感信息指引（官方字段路径 <c>user_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【转账金额】 转账金额，单位为“分”。参考 转账额度说明（官方字段路径 <c>transfer_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>【转账备注】 转账备注，用户收款时可见该备注信息，UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【用户收款感知】 用户收款时感知的收款原因。不填或填空，将展示转账场景的默认内容。如有其他展示需求，参考 商家转账产品介绍（官方字段路径 <c>user_recv_perception</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_recv_perception")]
    public string? UserRecvPerception { get; set; }

    /// <summary>【转账场景报备信息】 需按转账场景准确填写报备信息，参考 转账场景报备信息字段说明（官方字段路径 <c>transfer_scene_report_infos</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("transfer_scene_report_infos")]
    public List<TransferSceneReportInfo>? TransferSceneReportInfos { get; set; }

    /// <summary>【微信免确认收款授权单号】 用户确认授权后微信支付返回的授权单号，与out_authorization_no二选一必填。系统将转账到该授权对应的用户账户（官方字段路径 <c>authorization_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authorization_id")]
    public string? AuthorizationId { get; set; }

    /// <summary>【商户侧授权单号】 商户系统内部的免确认收款授权单号，与authorization_id二选一必填。系统将转账到该授权对应的用户账户（官方字段路径 <c>out_authorization_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_authorization_no")]
    public string? OutAuthorizationNo { get; set; }

}

/// <summary>用户授权后转账响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399371"/></summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferAuthorizedTransferResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号（官方字段路径 <c>mch_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【微信转账单号】 微信商家转账系统返回的唯一标识（官方字段路径 <c>transfer_bill_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>【商户AppID】 是微信开放平台和微信公众平台为开发者的应用程序(APP、小程序、公众号、企业号corpid即为此AppID)提供的一个唯一标识。此处，可以填写这四种类型中的任意一种APPID，但请确保该appid与商户号有绑定关系。详见： 普通商户模式开发必要参数说明（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【单据状态】。可选取值：`ACCEPTED` 转账已受理，可原单重试（非终态）。 / `PROCESSING` 转账锁定资金中。如果一直停留在该状态，建议检查账户余额是否足够，如余额不足，可充值后再原单重试（非终态）。 / `TRANSFERING` 转账中，可拉起微信收款确认页面再次重试确认收款（非终态）。 / `SUCCESS` 转账成功，表示转账单据已成功（终态）。（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【转账金额】 转账金额单位为“分”（官方字段路径 <c>transfer_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>【转账备注】 单条转账备注（微信用户会收到该备注），UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【失败原因】 订单已失败或者已退资金时，会返回 订单失败原因（官方字段路径 <c>fail_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【收款用户OpenID】 用户在商户appid下的唯一标识。发起转账前需获取到用户的OpenID，获取方式详见 参数说明（官方字段路径 <c>openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【收款用户姓名】 若商户在发起转账时传入了收款用户姓名，则查询接口中会返回，转账成功后提供电子回单。字段解密参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【单据创建时间】 单据受理成功时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【最后一次状态变更时间】 单据最后更新时间，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}
