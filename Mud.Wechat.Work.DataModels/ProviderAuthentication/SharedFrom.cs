namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 共享应用的企业信息
/// </summary>
public class SharedFrom
{
    /// <summary>
    /// 共享了应用的企业信息，仅当企业互联或者上下游共享应用触发的安装时才返回
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 共享途径，0表示企业互联，1表示上下游
    /// </summary>
    [JsonPropertyName("share_type")]
    public int ShareType { get; set; }
}
