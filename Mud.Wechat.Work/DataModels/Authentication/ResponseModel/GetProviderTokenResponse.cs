namespace Mud.Wechat.Work.DataModels.Authentication;

/// <summary>
/// 获取服务商凭证响应体
/// </summary>
public class GetProviderTokenResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置订单号。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = default!;
}
