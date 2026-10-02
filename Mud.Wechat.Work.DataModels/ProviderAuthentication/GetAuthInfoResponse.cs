namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;


/// <summary>
/// 企业微信服务商授权响应实体
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProviderAuthentication")]
public class GetAuthInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 代理服务商企业信息。应用被代理后才有该信息
    /// </summary>
    [JsonPropertyName("dealer_corp_info")]
    public DealerCorpInfo DealerCorpInfo { get; set; } = new DealerCorpInfo();

    /// <summary>
    /// 授权方企业信息
    /// </summary>
    [JsonPropertyName("auth_corp_info")]
    public AuthCorpDetailInfoExt AuthCorpInfo { get; set; } = new AuthCorpDetailInfoExt();

    /// <summary>
    /// 授权信息。如果是通讯录应用，且没开启实体应用，是没有该项的。通讯录应用拥有企业通讯录的全部信息读写权限。「第三方会话存档接口」不返回该字段
    /// </summary>
    [JsonPropertyName("auth_info")]
    public AuthInfo AuthInfo { get; set; } = new AuthInfo();


}

