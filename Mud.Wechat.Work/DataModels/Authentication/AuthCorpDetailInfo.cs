namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// 授权方企业详细信息
/// </summary>
public class AuthCorpDetailInfo
{
    /// <summary>
    /// 授权方企业微信id
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 授权方企业名称，即企业简称
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }

    /// <summary>
    /// 授权方企业类型，认证号：verified，注册号：unverified
    /// </summary>
    [JsonPropertyName("corp_type")]
    public string? CorpType { get; set; }

    /// <summary>
    /// 授权方企业方形头像
    /// </summary>
    [JsonPropertyName("corp_square_logo_url")]
    public string? CorpSquareLogoUrl { get; set; }

    /// <summary>
    /// 授权方企业用户规模
    /// </summary>
    [JsonPropertyName("corp_user_max")]
    public int CorpUserMax { get; set; }

    /// <summary>
    /// 授权方企业的主体名称(仅认证或验证过的企业有)，即企业全称。企业微信将逐步回收该字段，后续实际返回内容为企业名称，即corp_name
    /// </summary>
    [JsonPropertyName("corp_full_name")]
    public string? CorpFullName { get; set; }

    /// <summary>
    /// 认证到期时间
    /// </summary>
    [JsonPropertyName("verified_end_time")]
    public long VerifiedEndTime { get; set; }

    /// <summary>
    /// 企业类型，1. 企业; 2. 政府以及事业单位; 3. 其他组织，4.团队号
    /// </summary>
    [JsonPropertyName("subject_type")]
    public int SubjectType { get; set; }

    /// <summary>
    /// 授权企业在微信插件（原企业号）的二维码，可用于关注微信插件
    /// </summary>
    [JsonPropertyName("corp_wxqrcode")]
    public string? CorpWxQrCode { get; set; }

    /// <summary>
    /// 企业规模。当企业未设置该属性时，值为空。成员授权下，即auth_info.agent.auth_mode为1时值为空
    /// </summary>
    [JsonPropertyName("corp_scale")]
    public string? CorpScale { get; set; }

    /// <summary>
    /// 企业所属行业。当企业未设置该属性时，值为空。成员授权下，即auth_info.agent.auth_mode为1时值为空
    /// </summary>
    [JsonPropertyName("corp_industry")]
    public string? CorpIndustry { get; set; }

    /// <summary>
    /// 企业所属子行业。当企业未设置该属性时，值为空。成员授权下，即auth_info.agent.auth_mode为1时值为空
    /// </summary>
    [JsonPropertyName("corp_sub_industry")]
    public string? CorpSubIndustry { get; set; }
}


/// <summary>
/// 授权方企业详细信息
/// </summary>
public class AuthCorpDetailInfoExt : AuthCorpDetailInfo
{
    /// <summary>
    /// 企业其他认证的名称，仅认证企业才有
    /// </summary>
    [JsonPropertyName("corp_ex_name")]
    public CorpExName CorpExName { get; set; } = new CorpExName();
}



/// <summary>
/// 企业其他认证名称信息
/// </summary>
public class CorpExName
{
    /// <summary>
    /// 企业其他认证的企业简称列表（不包括corp_name），仅认证企业才有
    /// </summary>
    [JsonPropertyName("name_list")]
    public List<string?> NameList { get; set; } = [];
}
