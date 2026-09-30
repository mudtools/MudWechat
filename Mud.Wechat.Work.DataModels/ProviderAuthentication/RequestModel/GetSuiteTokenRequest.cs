
namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;


/// <summary>
/// 获取第三方应用凭证请求体
/// </summary>
public class GetSuiteTokenRequest
{
    /// <summary>
    /// 获取或设置第三方应用 SuiteId。
    /// <para>第三方应用id或者代开发应用模板id。</para>
    /// <para>第三方应用以ww或wx开头应用id（对应于旧的以tj开头的套件id）；</para>
    /// <para>代开发应用以dk开头</para>
    /// 对应 <c>SuiteId</c>/> 参数。
    /// </summary>
    [JsonPropertyName("suite_id")]
    public string? SuiteId { get; set; }

    /// <summary>
    /// 获取或设置第三方应用 SuiteSecret。对应 <c>SuiteSecret</c>/> 参数。
    /// </summary>
    [JsonPropertyName("suite_secret")]
    public string? SuiteSecret { get; set; }

    /// <summary>
    /// 获取或设置企业微信后台推送的 Ticket。
    /// <para>suite_ticket实际有效期为30分钟，可以容错连续两次获取suite_ticket失败的情况，但是请永远使用最新接收到的suite_ticket。</para>
    /// </summary>
    [JsonPropertyName("suite_ticket")]
    public string SuiteTicket { get; set; } = string.Empty;
}
