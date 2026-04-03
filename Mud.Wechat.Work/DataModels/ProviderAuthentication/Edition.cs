namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 版本信息
/// </summary>
public class Edition
{
    /// <summary>
    /// 获取或设置授权的应用信息列表。
    /// </summary>
    [JsonPropertyName("agent")]
    public EditionAgent[] AgentList { get; set; } = default!;
}
