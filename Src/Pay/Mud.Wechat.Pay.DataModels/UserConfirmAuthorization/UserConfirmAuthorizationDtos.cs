// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.UserConfirmAuthorization;

/// <summary>发起免确认收款授权请求·【场景信息】 用户端场景信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015901167"/></summary>
[HttpJsonSerializable(SerializerClassName = "UserConfirmAuthorization")]
public class UserConfirmAuthorizationCreateRequestSceneInfo
{
    /// <summary>【用户终端IP】 用户端实际IP，支持IPv4和IPv6两种格式的IP地址（官方字段路径 <c>scene_info.client_ip</c>，选填，类型 <c>string(45)</c>）。</summary>
    [JsonPropertyName("client_ip")]
    public string? ClientIp { get; set; }

    /// <summary>【用户设备ID】 用户设备ID（官方字段路径 <c>scene_info.device_id</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>【用户设备类型】 用户设备类型。可选取值：`IOS` 苹果设备 / `ANDROID` 安卓设备 / `HARMONY` 鸿蒙设备 / `OTHER` 其他设备（官方字段路径 <c>scene_info.device_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("device_type")]
    public string? DeviceType { get; set; }

}

/// <summary>发起免确认收款授权请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015901167"/></summary>
[HttpJsonSerializable(SerializerClassName = "UserConfirmAuthorization")]
public class UserConfirmAuthorizationCreateRequest
{
    /// <summary>【商户侧授权单号】 商户系统内部的免确认收款授权单号，商户需保证在系统内唯一且一个用户账号对应唯一一个生效中的授权（官方字段路径 <c>out_authorization_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_authorization_no")]
    public string? OutAuthorizationNo { get; set; }

    /// <summary>【商户AppID】 商户应用唯一标识（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【收款用户OpenID】 用户在商户appid下的唯一标识。参考 接口参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【转账场景ID】 用户授权后免确认收款的转账场景，可前往“商户平台-产品中心-商家转账”中申请。如：1000（现金营销），1006（企业报销）等（官方字段路径 <c>transfer_scene_id</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【用户展示名称】 免确认收款授权详情中展示的“开通账号”，用于区分用户在商户侧的不同账号。不支持非utf8编码的字符和表情等特殊字符（官方字段路径 <c>user_display_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_display_name")]
    public string? UserDisplayName { get; set; }

    /// <summary>【用户收款感知】 用户收款时感知到的收款原因，可查看产品文档了解，各场景展示的默认内容和支持传入的内容（官方字段路径 <c>user_recv_perception</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("user_recv_perception")]
    public string? UserRecvPerception { get; set; }

    /// <summary>【授权结果通知地址】 异步接收免确认收款授权结果通知的回调地址，通知url必须为公网可访问的URL，必须为HTTPS，不能携带参数（官方字段路径 <c>authorization_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("authorization_notify_url")]
    public string? AuthorizationNotifyUrl { get; set; }

    /// <summary>【场景信息】 用户端场景信息（官方字段路径 <c>scene_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("scene_info")]
    public UserConfirmAuthorizationCreateRequestSceneInfo? SceneInfo { get; set; }

}

/// <summary>发起免确认收款授权响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015901167"/></summary>
[HttpJsonSerializable(SerializerClassName = "UserConfirmAuthorization")]
public class UserConfirmAuthorizationCreateResponse : WechatPayResponse
{
    /// <summary>【商户侧授权单号】 商户系统内部的免确认收款授权单号，商户需保证在系统内唯一，并且一个用户账号对应一个生效中的授权（官方字段路径 <c>out_authorization_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_authorization_no")]
    public string? OutAuthorizationNo { get; set; }

    /// <summary>【授权状态】 授权状态。可选取值：`WAIT_USER_CONFIRM` 已受理商户的授权申请，待用户确认（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【单据创建时间】 单据受理成功时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【跳转授权页面的package信息】 JSAPI调起用户确认收款 时需要使用的参数。单据创建后，用户需在规定时间内完成授权。建议拉起用户授权页面前，先查单据状态。如果单据状态为待收款用户确认，可用之前获取到的package信息。单据到终态或已过期，需更换单号重新申请（官方字段路径 <c>package_info</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("package_info")]
    public string? PackageInfo { get; set; }

}

/// <summary>免确认收款授权结果查询响应·【免确认收款授权的关闭信息】 当状态为已关闭时返回，如关闭原因、关闭时间等。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399423"/></summary>
[HttpJsonSerializable(SerializerClassName = "UserConfirmAuthorization")]
public class UserConfirmAuthorizationQueryResponseCloseInfo
{
    /// <summary>【关闭时间】 当状态为已关闭时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>close_info.close_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【免确认收款授权的原因】 当状态为已关闭时返回。可选取值：`CLOSE_VIA_MCH_API` 商户通过API主动关闭授权 / `USER_CLOSE` 用户确认后，主动关闭授权 / `USER_OVERDUE_UNCONFIRMED` 用户超时未确认授权（官方字段路径 <c>close_info.close_reason</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

}

/// <summary>免确认收款授权结果查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399423"/></summary>
[HttpJsonSerializable(SerializerClassName = "UserConfirmAuthorization")]
public class UserConfirmAuthorizationQueryResponse : WechatPayResponse
{
    /// <summary>【商户侧授权单号】 商户系统内部的免确认收款授权单号（官方字段路径 <c>out_authorization_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_authorization_no")]
    public string? OutAuthorizationNo { get; set; }

    /// <summary>【商户AppID】 商户应用唯一标识（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【收款用户OpenID】 用户在商户appid下的唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【用户展示昵称】 用户免确认收款授权详情中展示的昵称（官方字段路径 <c>user_display_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_display_name")]
    public string? UserDisplayName { get; set; }

    /// <summary>【微信免确认收款授权单号】 用户确认授权后微信支付返回的授权单号（官方字段路径 <c>authorization_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authorization_id")]
    public string? AuthorizationId { get; set; }

    /// <summary>【授权状态】 授权状态。可选取值：`TAKING_EFFECT` 用户已确认授权，授权生效中，可发起转账到授权对应的用户 / `CLOSED` 用户或商户系统关闭，无法转账至该授权对应的用户 / `WAIT_USER_CONFIRM` 已受理商户的授权申请，待用户确认（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【用户确认授权的时间】 用户确认授权后返回。按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>authorize_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authorize_time")]
    public string? AuthorizeTime { get; set; }

    /// <summary>【免确认收款授权的关闭信息】 当状态为已关闭时返回，如关闭原因、关闭时间等（官方字段路径 <c>close_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("close_info")]
    public UserConfirmAuthorizationQueryResponseCloseInfo? CloseInfo { get; set; }

    /// <summary>【转账场景ID】 用户授权后免确认收款的转账场景，可前往“商户平台-产品中心-商家转账”中申请。如：1000（现金营销），1006（企业报销）等（官方字段路径 <c>transfer_scene_id</c>，选填，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【用户收款感知】 商户申请免确认收款授权时传入的内容（官方字段路径 <c>user_recv_perception</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("user_recv_perception")]
    public string? UserRecvPerception { get; set; }

    /// <summary>【单据创建时间】 商户申请授权的时间，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【跳转授权信息页的package信息】 跳转用户授权信息页的package信息， JSAPI调起用户授权信息页 或 APP调起用户授权信息页 时需要使用的参数。授权单状态为生效中时返回，package信息的有效期为10分钟，过期后须重新获取（官方字段路径 <c>package_info</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("package_info")]
    public string? PackageInfo { get; set; }

}

/// <summary>解除免确认收款授权响应·【免确认收款授权的关闭信息】 当状态为已关闭时返回，如关闭原因、关闭时间等。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015653811"/></summary>
[HttpJsonSerializable(SerializerClassName = "UserConfirmAuthorization")]
public class UserConfirmAuthorizationCloseResponseCloseInfo
{
    /// <summary>【关闭时间】 当状态为已关闭时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>close_info.close_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【免确认收款授权的原因】 当状态为已关闭时返回。可选取值：`CLOSE_VIA_MCH_API` 商户通过API主动关闭授权（官方字段路径 <c>close_info.close_reason</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

}

/// <summary>解除免确认收款授权响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015653811"/></summary>
[HttpJsonSerializable(SerializerClassName = "UserConfirmAuthorization")]
public class UserConfirmAuthorizationCloseResponse : WechatPayResponse
{
    /// <summary>【商户侧授权单号】 商户系统内部的免确认收款授权单号（官方字段路径 <c>out_authorization_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_authorization_no")]
    public string? OutAuthorizationNo { get; set; }

    /// <summary>【商户AppID】 商户应用唯一标识（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【收款用户OpenID】 用户在商户appid下的唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【用户展示昵称】 用户免确认收款授权详情中展示的昵称（官方字段路径 <c>user_display_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_display_name")]
    public string? UserDisplayName { get; set; }

    /// <summary>【微信免确认收款授权单号】 用户确认授权后微信支付返回的授权单号（官方字段路径 <c>authorization_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authorization_id")]
    public string? AuthorizationId { get; set; }

    /// <summary>【授权状态】 授权状态。可选取值：`CLOSED` 用户或商户系统关闭，无法转账至该授权对应的用户（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【用户确认授权的时间】 用户确认授权后返回。按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>authorize_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authorize_time")]
    public string? AuthorizeTime { get; set; }

    /// <summary>【免确认收款授权的关闭信息】 当状态为已关闭时返回，如关闭原因、关闭时间等（官方字段路径 <c>close_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("close_info")]
    public UserConfirmAuthorizationCloseResponseCloseInfo? CloseInfo { get; set; }

    /// <summary>【转账场景ID】 用户授权后免确认收款的转账场景，可前往“商户平台-产品中心-商家转账”中申请。如：1000（现金营销），1006（企业报销）等（官方字段路径 <c>transfer_scene_id</c>，选填，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【用户收款感知】 商户申请免确认收款授权时传入的内容（官方字段路径 <c>user_recv_perception</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("user_recv_perception")]
    public string? UserRecvPerception { get; set; }

    /// <summary>【单据创建时间】 商户申请授权的时间，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

}
