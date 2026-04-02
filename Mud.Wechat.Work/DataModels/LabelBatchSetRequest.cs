namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// <para>表示 [POST] /label/batchset/{TOKEN} 接口的请求。</para>
/// </summary>
public class LabelBatchSetRequest
{
    /// <summary>
    /// 获取或设置微信 AppId。
    /// </summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置标签名称。
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置二级标签名称。
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置标签分类规则。
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置要设置标签的 OpenId 列表。
    /// </summary>
    [JsonPropertyName("list")]
    public OpenIdList OpenIdList { get; set; } = new OpenIdList();
}

public class OpenIdList
{
    [JsonPropertyName("openid")]
    public IList<string> Items { get; set; } = new List<string>();
}