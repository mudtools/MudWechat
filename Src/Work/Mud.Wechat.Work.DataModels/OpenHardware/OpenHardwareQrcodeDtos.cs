namespace Mud.Wechat.Work.DataModels.OpenHardware;

/// <summary>
/// 获取设备管理二维码请求体（openhw/device/gen_login_qrcode，官方 98023）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GenDeviceLoginQrcodeRequest
{
    /// <summary>
    /// 获取或设置透传状态值（官方：扫码后由「扫码事件」下发并透传给设备）。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置二维码类型：0-静态二维码，1-动态二维码。
    /// </summary>
    [JsonPropertyName("type")]
    public uint Type { get; set; }
}

/// <summary>
/// 设备二维码响应体（官方 98023 / 98024 两页的应答字段集同形：
/// <c>qrcode_content</c> 恒有，<c>expires_in</c> 仅动态二维码返回 ⇒ 一份可空超集）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GenDeviceQrcodeResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置二维码 url 内容（官方：厂商需自行将该 url 渲染成二维码图片供扫码）。
    /// </summary>
    [JsonPropertyName("qrcode_content")]
    public string? QrcodeContent { get; set; }

    /// <summary>
    /// 获取或设置二维码有效期（单位：秒）。
    /// </summary>
    /// <remarks><para>官方：动态二维码固定 300 秒（5 分钟）后过期；静态二维码形态官方不返回本字段。</para></remarks>
    [JsonPropertyName("expires_in")]
    public uint ExpiresIn { get; set; }
}

/// <summary>
/// 获取设备身份识别二维码请求体（openhw/device/gen_id_dynamic_qrcode，官方 98024）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GenDeviceIdQrcodeRequest
{
    /// <summary>
    /// 获取或设置设备 SN 编码。
    /// </summary>
    [JsonPropertyName("device_sn")]
    public string? DeviceSn { get; set; }

    /// <summary>
    /// 获取或设置透传状态值（官方：扫码后由「扫码事件」下发并透传给设备）。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}
