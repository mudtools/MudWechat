namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取企业永久授权码请求体
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProviderAuthentication")]
public class GetPermanentCodeRequest
{
    /// <summary>
    /// 获取或设置临时授权码。
    /// </summary>
    [JsonPropertyName("auth_code")]
    public string TempAuthCode { get; set; } = string.Empty;
}
