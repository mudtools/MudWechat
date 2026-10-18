namespace Mud.Wechat.Work.DataModels.OpenHardware;

/// <summary>
/// 获取设备型号调用凭证请求体（openhw/get_model_token，官方 95992）。
/// </summary>
/// <remarks>
/// <para><b>凭据保密</b>：<see cref="ModelSecret"/> 与 <see cref="ModelTicket"/> 均为长期/滚动凭据，
/// 不得写入日志、遥测或异常消息；本类型不参与任何令牌管理链路，由宿主自行保管与传入。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetModelTokenRequest
{
    /// <summary>
    /// 获取或设置设备型号 ID（官方：在服务商管理端创建设备型号后可在该页面获取）。
    /// </summary>
    [JsonPropertyName("model_id")]
    public string? ModelId { get; set; }

    /// <summary>
    /// 获取或设置设备型号 secret（每个型号唯一）。
    /// </summary>
    [JsonPropertyName("model_secret")]
    public string? ModelSecret { get; set; }

    /// <summary>
    /// 获取或设置企业微信后台推送的 <c>model_ticket</c>（随「设备型号 model_ticket 推送事件」滚动更新）。
    /// </summary>
    [JsonPropertyName("model_ticket")]
    public string? ModelTicket { get; set; }
}

/// <summary>
/// 获取设备型号调用凭证响应体（含 <c>model_access_token</c>，最长 512 字节）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetModelTokenResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置设备型号调用凭证（后续型号面端点以 Query 参数 <c>model_access_token</c> 传入）。
    /// </summary>
    [JsonPropertyName("model_access_token")]
    public string? ModelAccessToken { get; set; }

    /// <summary>
    /// 获取或设置有效期（单位：秒）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

/// <summary>
/// 获取设备授权密钥请求体（openhw/get_device_secret，官方 95993）。
/// </summary>
/// <remarks>
/// <para><b>一次性凭据</b>：官方明确 <see cref="AuthCode"/>「仅支持消费一次」，长度 64~512 字节，
/// 取自企业绑定设备时企业微信回调厂商的授权 code。重复使用必失败，故本 SDK 不做重试包装。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetDeviceSecretRequest
{
    /// <summary>
    /// 获取或设置企业绑定设备时企业微信回调的授权 code（仅可消费一次）。
    /// </summary>
    [JsonPropertyName("auth_code")]
    public string? AuthCode { get; set; }
}

/// <summary>
/// 获取设备授权密钥响应体（同时返回设备长期密钥与设备调用凭证）。
/// </summary>
/// <remarks>
/// <para><b>凭据保密</b>：<see cref="DeviceSecret"/> 是设备长期授权密钥（后续换取 <c>device_access_token</c>
/// 的凭据），<see cref="DeviceAccessToken"/> 是短时效调用凭证；二者均不得写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetDeviceSecretResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置设备授权密钥（最长 512 字节）。
    /// </summary>
    [JsonPropertyName("device_secret")]
    public string? DeviceSecret { get; set; }

    /// <summary>
    /// 获取或设置设备调用凭证（最长 512 字节）。
    /// </summary>
    [JsonPropertyName("device_access_token")]
    public string? DeviceAccessToken { get; set; }

    /// <summary>
    /// 获取或设置 <see cref="DeviceAccessToken"/> 的有效期（单位：秒）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

/// <summary>
/// 获取设备调用凭证请求体（openhw/get_device_token，官方 96022）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetDeviceTokenRequest
{
    /// <summary>
    /// 获取或设置设备 SN。
    /// </summary>
    [JsonPropertyName("device_sn")]
    public string? DeviceSn { get; set; }

    /// <summary>
    /// 获取或设置设备授权密钥（经「获取设备授权密钥」接口取得）。
    /// </summary>
    [JsonPropertyName("device_secret")]
    public string? DeviceSecret { get; set; }
}

/// <summary>
/// 获取设备调用凭证响应体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetDeviceTokenResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置设备调用凭证（最长 512 字节）。
    /// </summary>
    [JsonPropertyName("device_access_token")]
    public string? DeviceAccessToken { get; set; }

    /// <summary>
    /// 获取或设置有效期（单位：秒）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
