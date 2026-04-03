namespace Mud.Wechat.Work.DataModels.Authentication;

/// <summary>
/// 获取企业永久授权码响应体
/// </summary>
public class GetPermanentCodeResponse : WechatWorkResponse
{
    /// <summary>
    /// 授权方（企业）access_token，最长为512字节。代开发自建应用安装时不返回
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// 授权方（企业）access_token超时时间（秒）。代开发自建应用安装时不返回
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 企业微信永久授权码，最长为512字节
    /// </summary>
    [JsonPropertyName("permanent_code")]
    public string? PermanentCode { get; set; }

    /// <summary>
    /// 代理服务商企业信息。应用被代理后才有该信息
    /// </summary>
    [JsonPropertyName("dealer_corp_info")]
    public DealerCorpInfo DealerCorpInfo { get; set; } = new DealerCorpInfo();

    /// <summary>
    /// 授权方企业信息
    /// </summary>
    [JsonPropertyName("auth_corp_info")]
    public AuthCorpDetailInfo AuthCorpInfo { get; set; } = new AuthCorpDetailInfo();

    /// <summary>
    /// 授权信息。如果是通讯录应用，且没开启实体应用，是没有该项的。通讯录应用拥有企业通讯录的全部信息读写权限。「第三方会话存档接口」不返回该字段
    /// </summary>
    [JsonPropertyName("auth_info")]
    public AuthInfo AuthInfo { get; set; } = new AuthInfo();

    /// <summary>
    /// 授权管理员的信息，可能不返回
    /// </summary>
    [JsonPropertyName("auth_user_info")]
    public AuthUserInfo AuthUserInfo { get; set; } = new AuthUserInfo();

    /// <summary>
    /// 推广二维码安装相关信息，扫推广二维码安装时返回。成员授权时暂不支持。（注：无论企业是否新注册，只要通过扫推广二维码安装，都会返回该字段）
    /// </summary>
    [JsonPropertyName("register_code_info")]
    public RegisterCodeInfo RegisterCodeInfo { get; set; } = new RegisterCodeInfo();

    /// <summary>
    /// 安装应用时，扫码或者授权链接中带的state值
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}


/// <summary>
/// 授权管理员信息
/// </summary>
public class AuthUserInfo
{
    /// <summary>
    /// 授权管理员的userid，可能为空
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 授权管理员的open_userid，可能为空
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 授权管理员的name，可能为空
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 授权管理员的头像url，可能为空
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
}

/// <summary>
/// 推广二维码安装信息
/// </summary>
public class RegisterCodeInfo
{
    /// <summary>
    /// 注册码
    /// </summary>
    [JsonPropertyName("register_code")]
    public string? RegisterCode { get; set; }

    /// <summary>
    /// 推广包ID
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>
    /// 仅当获取注册码指定该字段时才返回
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}