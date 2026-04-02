namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// <para>表示 [POST] /label/batchset/{TOKEN} 接口的响应。</para>
/// </summary>
public class LabelBatchSetResponse : WechatChatbotResponse
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonPropertyName("code")]
    public override int? ReturnCode { get; set; }

    /// <summary>
    /// 获取微信智能对话 API 返回的错误描述。
    /// </summary>
    [JsonPropertyName("msg")]
    public string? ReturnMessage { get; set; }
}