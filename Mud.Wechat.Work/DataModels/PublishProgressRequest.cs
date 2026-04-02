namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// <para>表示 [POST] /publish_progress/{TOKEN} 接口的请求。</para>
/// </summary>
public class PublishProgressRequest
{
    /// <summary>
    /// 获取或设置管理员 ID。
    /// </summary>
    [JsonPropertyName("managerid")]
    public string ManagerId { get; set; } = string.Empty;
}
