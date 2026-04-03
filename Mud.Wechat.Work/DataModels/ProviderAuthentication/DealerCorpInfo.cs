namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 代理服务商企业信息
/// </summary>
public class DealerCorpInfo
{
    /// <summary>
    /// 代理服务商企业微信id
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 代理服务商企业微信名称
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }
}