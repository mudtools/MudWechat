namespace Mud.Wechat.Work.DataModels.OpenHardware;

/// <summary>
/// 录入设备请求体（openhw/model/register_sn，官方 95980）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class RegisterDeviceSnRequest
{
    /// <summary>
    /// 获取或设置设备 SN（官方：全局唯一）。
    /// </summary>
    [JsonPropertyName("device_sn")]
    public string? DeviceSn { get; set; }
}

/// <summary>
/// 录入设备响应体（返回设备静态二维码内容）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class RegisterDeviceSnResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置设备静态二维码（官方字段名为 <c>qr_code</c>，为 url 内容，由厂商自行渲染成图）。
    /// </summary>
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }
}

/// <summary>
/// 删除设备请求体（openhw/model/unregister_sn，官方 95981）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class UnregisterDeviceSnRequest
{
    /// <summary>
    /// 获取或设置设备 SN。
    /// </summary>
    [JsonPropertyName("device_sn")]
    public string? DeviceSn { get; set; }
}

/// <summary>
/// 获取设备详情请求体（openhw/device/get_device_detail，官方 95982）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetDeviceDetailRequest
{
    /// <summary>
    /// 获取或设置设备 SN。
    /// </summary>
    [JsonPropertyName("device_sn")]
    public string? DeviceSn { get; set; }
}

/// <summary>
/// 获取设备详情响应体（业务负载收敛于 <c>device_detail</c> 包装对象下）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetDeviceDetailResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置设备详情（官方在顶层包一层 <c>device_detail</c>，字段名勿与响应基类混淆）。
    /// </summary>
    [JsonPropertyName("device_detail")]
    public OpenHardwareDeviceDetail? DeviceDetail { get; set; }
}

/// <summary>
/// 设备详情（官方 95982 响应字段表）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareDeviceDetail
{
    /// <summary>
    /// 获取或设置设备型号名。
    /// </summary>
    [JsonPropertyName("model_name")]
    public string? ModelName { get; set; }

    /// <summary>
    /// 获取或设置同型号设备出厂名。
    /// </summary>
    [JsonPropertyName("default_name")]
    public string? DefaultName { get; set; }

    /// <summary>
    /// 获取或设置设备静态二维码。
    /// </summary>
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }

    /// <summary>
    /// 获取或设置设备绑定状态：0-未绑定；1-已绑定。
    /// </summary>
    /// <remarks><para>官方：仅在已绑定情况下才返回 <see cref="BindCorpInfo"/> 与 <see cref="RemarkName"/>。</para></remarks>
    [JsonPropertyName("bind_status")]
    public int BindStatus { get; set; }

    /// <summary>
    /// 获取或设置绑定企业信息（未绑定时官方不返回）。
    /// </summary>
    [JsonPropertyName("bind_corpinfo")]
    public OpenHardwareBindCorpInfo? BindCorpInfo { get; set; }

    /// <summary>
    /// 获取或设置设备备注名（官方：用户未设置时返回空字符串，非省略字段）。
    /// </summary>
    [JsonPropertyName("remark_name")]
    public string? RemarkName { get; set; }
}

/// <summary>
/// 设备绑定企业信息。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareBindCorpInfo
{
    /// <summary>
    /// 获取或设置绑定企业的 corpid。
    /// </summary>
    [JsonPropertyName("open_corpid")]
    public string? OpenCorpId { get; set; }

    /// <summary>
    /// 获取或设置绑定企业的 corp 名称。
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }
}
