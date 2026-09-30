namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// 表示微信智能对话 API 响应的基类。
/// </summary>
public abstract class WechatWorkResponse
{
    /// <summary>
    /// 获取微信智能对话 API 返回的错误码。
    /// </summary>
    [JsonPropertyName("errcode")]
    public virtual int? ErrorCode { get; set; }

    /// <summary>
    /// 获取微信智能对话 API 返回的错误信息。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public virtual string? ErrorMessage { get; set; }
}

/// <summary>
/// 表示微信智能对话 API 响应的泛型基类。
/// </summary>
public abstract class WechatChatbotResponse<TData> : WechatWorkResponse
    where TData : class
{
    /// <summary>
    /// 获取微信智能对话 API 返回的数据。
    /// </summary>
    [JsonPropertyName("data")]
    public virtual TData? Data { get; set; }
}