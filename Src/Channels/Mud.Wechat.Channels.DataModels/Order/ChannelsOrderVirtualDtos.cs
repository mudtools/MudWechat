// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「虚拟号 / 真实号 / 敏感信息解密 / 商家私密号」DTO
// （order/virtualnumber/applyagain、order/virtualnumber/delay、order/realnumber/apply、
//  order/realnumberviewaudit/get、order/sensitiveinfo/decode、order/merchant/privatenumber/*）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>虚拟号信息（官方 <c>sensitiveinfo/decode</c> 与 <c>virtualnumber/applyagain</c> 响应的 <c>virtual_number_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderVirtualNumberInfo
{
    /// <summary>获取或设置虚拟号（官方 <c>virtual_number</c>，实名认证后可拨打）。</summary>
    [JsonPropertyName("virtual_number")]
    public string? VirtualNumber { get; set; }

    /// <summary>获取或设置分机号（官方 <c>extension</c>，需实名认证后可拨打）。</summary>
    [JsonPropertyName("extension")]
    public string? Extension { get; set; }

    /// <summary>获取或设置过期时间戳（官方 <c>expiration</c>）。</summary>
    [JsonPropertyName("expiration")]
    public long? Expiration { get; set; }
}

/// <summary>解密订单中的详细收货信息（<c>order/sensitiveinfo/decode</c>）请求体。</summary>
/// <remarks>
/// 为保护用户隐私（收货人昵称、电话号码、详细收货地址），平台订单的收货信息进行了部分隐藏；
/// 该接口用于解密（官方文档 ID：api_decodesensitiveinfo）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsDecodeSensitiveInfoRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>解密订单中的详细收货信息（<c>order/sensitiveinfo/decode</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsDecodeSensitiveInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置收货信息（官方 <c>address_info</c>，字段集与 <c>ChannelsOrderAddress</c> 一致）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsOrderAddress? AddressInfo { get; set; }

    /// <summary>获取或设置虚拟号信息（官方 <c>virtual_number_info</c>）。</summary>
    [JsonPropertyName("virtual_number_info")]
    public ChannelsOrderVirtualNumberInfo? VirtualNumberInfo { get; set; }
}

/// <summary>订单再次申请虚拟号（<c>order/virtualnumber/applyagain</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsApplyVirtualNumberAgainRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>订单再次申请虚拟号（<c>order/virtualnumber/applyagain</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsApplyVirtualNumberAgainResponse : ChannelsResponse
{
    /// <summary>获取或设置虚拟号（官方 <c>virtual_number</c>）。</summary>
    [JsonPropertyName("virtual_number")]
    public string? VirtualNumber { get; set; }

    /// <summary>获取或设置分机号（官方 <c>extension</c>）。</summary>
    [JsonPropertyName("extension")]
    public string? Extension { get; set; }

    /// <summary>获取或设置剩余申请次数（官方 <c>apply_quota</c>）。</summary>
    [JsonPropertyName("apply_quota")]
    public long? ApplyQuota { get; set; }
}

/// <summary>订单虚拟号延期（<c>order/virtualnumber/delay</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsDelayVirtualNumberRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填，19 位数字字符串）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置当前已延期次数（官方 <c>has_delay_times</c>，必填，可通过 获取订单详情 接口拿到，不填默认为 0）。
    /// </summary>
    [JsonPropertyName("has_delay_times")]
    public int HasDelayTimes { get; set; }
}

/// <summary>订单虚拟号延期（<c>order/virtualnumber/delay</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsDelayVirtualNumberResponse : ChannelsResponse
{
    /// <summary>获取或设置过期时间（官方 <c>expiration</c>，秒级时间戳）。</summary>
    [JsonPropertyName("expiration")]
    public long? Expiration { get; set; }

    /// <summary>获取或设置可延期次数（官方 <c>available_extend_num</c>）。</summary>
    [JsonPropertyName("available_extend_num")]
    public int? AvailableExtendNum { get; set; }
}

/// <summary>真实号查看审核状态（官方 <c>order/realnumberviewaudit/get</c> 的 <c>audit_state</c> 枚举）。</summary>
public static class ChannelsRealNumberAuditStates
{
    /// <summary>审核中。</summary>
    public const int Auditing = 1;

    /// <summary>审核拒绝。</summary>
    public const int Rejected = 2;

    /// <summary>审核通过。</summary>
    public const int Approved = 3;
}

/// <summary>申请查看订单真实号码（<c>order/realnumber/apply</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsApplyRealNumberRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置申请原因枚举值（官方 <c>apply_type</c>，必填，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("apply_type")]
    public int ApplyType { get; set; }

    /// <summary>获取或设置申请原因（官方 <c>apply_reason</c>，必填）。</summary>
    [JsonPropertyName("apply_reason")]
    public string ApplyReason { get; set; } = string.Empty;

    /// <summary>获取或设置申请原因图片证明（官方 <c>pic_media_ids</c>，必填，1-5 张）。</summary>
    [JsonPropertyName("pic_media_ids")]
    public List<string> PicMediaIds { get; set; } = new();
}

/// <summary>查看订单真实号审核状态（<c>order/realnumberviewaudit/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetRealNumberViewAuditRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>查看订单真实号审核状态（<c>order/realnumberviewaudit/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetRealNumberViewAuditResponse : ChannelsResponse
{
    /// <summary>获取或设置申请时间（官方 <c>apply_time</c>）。</summary>
    [JsonPropertyName("apply_time")]
    public long? ApplyTime { get; set; }

    /// <summary>获取或设置审核状态（官方 <c>audit_state</c>，1 审核中 2 审核拒绝 3 审核通过，见 <see cref="ChannelsRealNumberAuditStates"/>）。</summary>
    [JsonPropertyName("audit_state")]
    public int? AuditState { get; set; }

    /// <summary>获取或设置申请原因（官方 <c>apply_reason</c>）。</summary>
    [JsonPropertyName("apply_reason")]
    public string? ApplyReason { get; set; }

    /// <summary>获取或设置申请 id（官方 <c>task_id</c>）。</summary>
    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }
}

/// <summary>获取短信验证码（<c>order/merchant/privatenumber/sendverifycode</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsPrivatenumberSendVerifyCodeRequest
{
    /// <summary>获取或设置需要实名认证的手机号（官方 <c>mobile</c>，必填）。</summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = string.Empty;
}

/// <summary>添加待认证的手机号（<c>order/merchant/privatenumber/addphone</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsPrivatenumberAddPhoneRequest
{
    /// <summary>获取或设置待认证的手机号（官方 <c>mobile</c>，必填）。</summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = string.Empty;

    /// <summary>获取或设置通过 获取短信验证码 接口获取的短信验证码（官方 <c>verify_code</c>，必填）。</summary>
    [JsonPropertyName("verify_code")]
    public string VerifyCode { get; set; } = string.Empty;

    /// <summary>获取或设置小店成员的微信号（官方 <c>wxusername</c>，必填）。</summary>
    [JsonPropertyName("wxusername")]
    public string WxUsername { get; set; } = string.Empty;
}

/// <summary>添加待认证的手机号（<c>order/merchant/privatenumber/addphone</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsPrivatenumberAddPhoneResponse : ChannelsResponse
{
    /// <summary>获取或设置认证链接（官方 <c>qrcode_url</c>，在手机上打开进入运营商的页面进行实名认证）。</summary>
    [JsonPropertyName("qrcode_url")]
    public string? QrcodeUrl { get; set; }

    /// <summary>获取或设置认证状态（官方 <c>status</c>，1=认证成功，2=认证失败，3=运营商审核中）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>获取当前手机号的认证状态（<c>order/merchant/privatenumber/getphone</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsPrivatenumberGetPhoneRequest
{
    /// <summary>获取或设置手机号（官方 <c>mobile</c>，必填）。</summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = string.Empty;
}

/// <summary>获取当前手机号的认证状态（<c>order/merchant/privatenumber/getphone</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsPrivatenumberGetPhoneResponse : ChannelsResponse
{
    /// <summary>获取或设置认证状态（官方 <c>status</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置审核失败原因（官方 <c>fail_reason</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }
}