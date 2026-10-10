// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.HardwareDevice;

/// <summary>发送设备消息请求体（<c>POST /cgi-bin/message/device/subscribe/send</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>hardware-device/api_sendhardwaredevicemessage.html</c>。</para>
/// <para>
/// 与订阅消息发送（<c>subscribe/send</c>）同构，但接收者为<b>设备订阅用户列表</b>（<c>tousers</c> 数组，官方原文：
/// 接收者（用户）的 openid 列表）；<c>data</c> 键须与设备消息模板关键词一致。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaDeviceMessageSendRequest
{
    /// <summary>接收者（用户）的 openid 列表（<c>tousers</c>，必填）。</summary>
    [JsonPropertyName("tousers")]
    public List<string>? Tousers { get; set; }

    /// <summary>所需下发的设备消息模板 ID（<c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>点击模板卡片后的跳转页面（<c>page</c>，选填；仅限本小程序内页面）。</summary>
    [JsonPropertyName("page")]
    public string? Page { get; set; }

    /// <summary>跳转小程序类型（<c>miniprogram_state</c>，选填）：<c>developer</c> 开发版 / <c>trial</c> 体验版 / <c>formal</c> 正式版（默认）。</summary>
    [JsonPropertyName("miniprogram_state")]
    public string? MiniprogramState { get; set; }

    /// <summary>进入小程序查看的语言（<c>lang</c>，选填）：<c>zh_CN</c> / <c>zh_TW</c> / <c>en-US</c>，默认 <c>zh_CN</c>。</summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }

    /// <summary>模板内容（<c>data</c>，必填；键为模板关键词 ID），见 <see cref="WxaDeviceMessageDataValue"/>。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, WxaDeviceMessageDataValue>? Data { get; set; }
}

/// <summary>设备消息模板内容取值（<c>data.{key}</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaDeviceMessageDataValue
{
    /// <summary>关键词值（<c>value</c>，必填）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>获取设备票据应答（<c>POST /wxa/getsnticket</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>hardware-device/api_getsnticket.html</c>。</para>
/// <para>票据（<see cref="SnTicket"/>）用于生成设备码，<b>5 分钟内有效</b>（官方原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaSnTicketResponse : WxaResponse
{
    /// <summary>设备票据（<c>sn_ticket</c>）。</summary>
    [JsonPropertyName("sn_ticket")]
    public string? SnTicket { get; set; }

    /// <summary>有效期（<c>expires_in</c>，单位秒）。</summary>
    [JsonPropertyName("expires_in")]
    public long? ExpiresIn { get; set; }
}

/// <summary>创建设备组应答（<c>POST /wxa/business/group/createid</c>）。</summary>
/// <remarks>官方文档：<c>hardware-device/api_createiotgroupid.html</c>；便于用户一次性订阅多个设备。</remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaIoTGroupCreateResponse : WxaResponse
{
    /// <summary>设备组 ID（<c>group_id</c>）。</summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>设备组名称（<c>group_name</c>）。</summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }
}

/// <summary>查询设备组信息请求体（<c>POST /wxa/business/group/getinfo</c>）。</summary>
/// <remarks>官方文档：<c>hardware-device/api_getiotgroupinfo.html</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaIoTGroupInfoRequest
{
    /// <summary>设备组 ID（<c>group_id</c>，必填）。</summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }
}

/// <summary>查询设备组信息应答（<c>group_id</c> + 设备列表）。</summary>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaIoTGroupInfoResponse : WxaResponse
{
    /// <summary>设备组 ID（<c>group_id</c>）。</summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>设备组名称（<c>group_name</c>）。</summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }

    /// <summary>组内设备列表（<c>device_list</c>），见 <see cref="WxaIoTDeviceInfo"/>。</summary>
    [JsonPropertyName("device_list")]
    public List<WxaIoTDeviceInfo>? DeviceList { get; set; }
}

/// <summary>设备组设备信息（<c>device_list[]</c>）。</summary>
/// <remarks>硬件设备以 <c>sn</c>（设备序列号）与 <c>mac</c>（设备 MAC）标识；字段以官方页面为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaIoTDeviceInfo
{
    /// <summary>设备序列号（<c>sn</c>）。</summary>
    [JsonPropertyName("sn")]
    public string? Sn { get; set; }

    /// <summary>设备 MAC 地址（<c>mac</c>）。</summary>
    [JsonPropertyName("mac")]
    public string? Mac { get; set; }
}

/// <summary>设备组添加 / 删除设备请求体（<c>POST /wxa/business/group/{adddevice,removedevice}</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>hardware-device/api_addiotgroupdevice.html</c>、<c>api_removeiotgroupdevice.html</c>。</para>
/// <para>同一请求体两端点复用；删除时仅以 <c>sn</c> 定位亦可（<c>mac</c> 选填）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaIoTGroupDevicesRequest
{
    /// <summary>设备组 ID（<c>group_id</c>，必填）。</summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>待操作的设备列表（<c>device_list</c>，必填），见 <see cref="WxaIoTDeviceInfo"/>。</summary>
    [JsonPropertyName("device_list")]
    public List<WxaIoTDeviceInfo>? DeviceList { get; set; }
}

/// <summary>查询 license 资源包列表应答（<c>POST /wxa/business/license/getpkglist</c>）。</summary>
/// <remarks>官方文档：<c>hardware-device/api_getlicensepkglist.html</c>；字段以官方页面为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaLicensePkgListResponse : WxaResponse
{
    /// <summary>资源包列表（<c>pkg_list</c>），见 <see cref="WxaLicensePkg"/>。</summary>
    [JsonPropertyName("pkg_list")]
    public List<WxaLicensePkg>? PkgList { get; set; }
}

/// <summary>license 资源包信息（<c>pkg_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaLicensePkg
{
    /// <summary>资源包 ID（<c>pkg_id</c>）。</summary>
    [JsonPropertyName("pkg_id")]
    public string? PkgId { get; set; }

    /// <summary>资源包名称（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>剩余可用激活码数量（<c>license</c>）。</summary>
    [JsonPropertyName("license")]
    public long? License { get; set; }
}

/// <summary>激活设备 license 请求体（<c>POST /wxa/business/license/activedevice</c>）。</summary>
/// <remarks>
/// 官方文档：<c>hardware-device/api_activelicensedevice.html</c>。
/// 批量绑定设备并消耗资源包中的激活码序号（<c>sn_list</c> 长度须与资源包剩余激活数匹配，否则报错）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaLicenseActivateRequest
{
    /// <summary>资源包 ID（<c>pkg_id</c>，必填）。</summary>
    [JsonPropertyName("pkg_id")]
    public string? PkgId { get; set; }

    /// <summary>待激活设备序列号列表（<c>sn_list</c>，必填）。</summary>
    [JsonPropertyName("sn_list")]
    public List<string>? SnList { get; set; }
}

/// <summary>查询设备激活详情请求体（<c>POST /wxa/business/license/getdeviceinfo</c>）。</summary>
/// <remarks>官方文档：<c>hardware-device/api_getlicensedeviceinfo.html</c>；批量查询设备剩余有效期。</remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaLicenseDeviceInfoRequest
{
    /// <summary>设备序列号列表（<c>sn_list</c>，必填）。</summary>
    [JsonPropertyName("sn_list")]
    public List<string>? SnList { get; set; }
}

/// <summary>查询设备激活详情应答（<c>device_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaLicenseDeviceInfoResponse : WxaResponse
{
    /// <summary>设备有效期列表（<c>device_list</c>），见 <see cref="WxaLicenseDevice"/>。</summary>
    [JsonPropertyName("device_list")]
    public List<WxaLicenseDevice>? DeviceList { get; set; }
}

/// <summary>设备剩余有效期信息（<c>device_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaLicenseDevice
{
    /// <summary>设备序列号（<c>sn</c>）。</summary>
    [JsonPropertyName("sn")]
    public string? Sn { get; set; }

    /// <summary>剩余有效期（<c>expire_time</c>；以官方页面字段与单位为准）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}