namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// <para>表示 [POST] /publish_progress/{TOKEN} 接口的响应。</para>
/// </summary>
public class PublishProgressResponse : WechatChatbotResponse<PublishProgressData>
{

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonPropertyName("code")]
    public int? ReturnCode { get; set; }

    /// <summary>
    /// 获取微信智能对话 API 返回的错误描述。
    /// </summary>
    [JsonPropertyName("msg")]
    public string? ReturnMessage { get; set; }
}

/// <summary>
/// 发布进度数据
/// </summary>
public class PublishProgressData
{
    /// <summary>
    /// 获取或设置进度（范围：0～100）。
    /// </summary>
    [JsonPropertyName("progress")]
    public int Progress { get; set; }

    /// <summary>
    /// 获取或设置状态。
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}