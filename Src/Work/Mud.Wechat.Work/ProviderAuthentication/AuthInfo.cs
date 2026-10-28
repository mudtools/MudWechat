namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;


/// <summary>
/// 授权信息
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProviderAuthentication")]
public class AuthInfo
{
    /// <summary>
    /// 授权的应用信息，注意是一个数组，但仅旧的多应用套件授权时会返回多个agent，对新的单应用授权，永远只返回一个agent
    /// </summary>
    [JsonPropertyName("agent")]
    public List<Agent> Agents { get; set; } = [];


    /// <summary>
    /// 获取或设置当前生效的版本信息。
    /// </summary>
    [JsonPropertyName("edition_info")]
    public Edition? Edition { get; set; }
}
