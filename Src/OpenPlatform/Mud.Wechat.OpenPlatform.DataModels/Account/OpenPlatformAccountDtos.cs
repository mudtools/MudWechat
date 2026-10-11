// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Account;

/// <summary>
/// 「获取授权账号的基本信息」响应（官方 <c>account/getaccountbasicinfo</c>；GET、无请求体）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformAccountBasicInfoResponse : OpenPlatformResponse
{
    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置帐号类型（官方 <c>account_type</c>）。</summary>
    [JsonPropertyName("account_type")]
    public int AccountType { get; set; }

    /// <summary>获取或设置主体类型（官方 <c>principal_type</c>；1=企业等）。</summary>
    [JsonPropertyName("principal_type")]
    public int PrincipalType { get; set; }

    /// <summary>获取或设置主体名称（官方 <c>principal_name</c>）。</summary>
    [JsonPropertyName("principal_name")]
    public string? PrincipalName { get; set; }

    /// <summary>获取或设置主体证件号（官方 <c>credential</c>；如统一社会信用代码）。</summary>
    [JsonPropertyName("credential")]
    public string? Credential { get; set; }

    /// <summary>获取或设置实名验证状态（官方 <c>realname_status</c>；-1=未验证，0=验证中，1=已验证等）。</summary>
    [JsonPropertyName("realname_status")]
    public int RealnameStatus { get; set; }

    /// <summary>获取或设置注册国家（官方 <c>registered_country</c>，可选）。</summary>
    [JsonPropertyName("registered_country")]
    public int? RegisteredCountry { get; set; }

    /// <summary>获取或设置昵称信息与修改额度（官方 <c>nickname_info</c>）。</summary>
    [JsonPropertyName("nickname_info")]
    public OpenPlatformAccountQuotaItem? NicknameInfo { get; set; }

    /// <summary>获取或设置头像信息与修改额度（官方 <c>head_image_info</c>）。</summary>
    [JsonPropertyName("head_image_info")]
    public OpenPlatformAccountHeadImageInfo? HeadImageInfo { get; set; }

    /// <summary>获取或设置功能简介与修改额度（官方 <c>signature_info</c>）。</summary>
    [JsonPropertyName("signature_info")]
    public OpenPlatformAccountSignatureInfo? SignatureInfo { get; set; }

    /// <summary>获取或设置微信认证信息（官方 <c>wx_verify_info</c>）。</summary>
    [JsonPropertyName("wx_verify_info")]
    public OpenPlatformAccountWxVerifyInfo? WxVerifyInfo { get; set; }
}

/// <summary>
/// 昵称信息与修改额度（官方 <c>nickname_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformAccountQuotaItem
{
    /// <summary>获取或设置昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置本年度已使用修改次数（官方 <c>modify_used_count</c>）。</summary>
    [JsonPropertyName("modify_used_count")]
    public int ModifyUsedCount { get; set; }

    /// <summary>获取或设置本年度可修改次数（官方 <c>modify_quota</c>）。</summary>
    [JsonPropertyName("modify_quota")]
    public int ModifyQuota { get; set; }
}

/// <summary>
/// 头像信息与修改额度（官方 <c>head_image_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformAccountHeadImageInfo
{
    /// <summary>获取或设置头像 URL（官方 <c>head_image_url</c>）。</summary>
    [JsonPropertyName("head_image_url")]
    public string? HeadImageUrl { get; set; }

    /// <summary>获取或设置本年度已使用修改次数（官方 <c>modify_used_count</c>）。</summary>
    [JsonPropertyName("modify_used_count")]
    public int ModifyUsedCount { get; set; }

    /// <summary>获取或设置本年度可修改次数（官方 <c>modify_quota</c>）。</summary>
    [JsonPropertyName("modify_quota")]
    public int ModifyQuota { get; set; }
}

/// <summary>
/// 功能简介与修改额度（官方 <c>signature_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformAccountSignatureInfo
{
    /// <summary>获取或设置功能简介（官方 <c>signature</c>）。</summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    /// <summary>获取或设置本年度已使用修改次数（官方 <c>modify_used_count</c>）。</summary>
    [JsonPropertyName("modify_used_count")]
    public int ModifyUsedCount { get; set; }

    /// <summary>获取或设置本年度可修改次数（官方 <c>modify_quota</c>）。</summary>
    [JsonPropertyName("modify_quota")]
    public int ModifyQuota { get; set; }
}

/// <summary>
/// 微信认证信息（官方 <c>wx_verify_info</c>；布尔字段官方以数字 0/1 承载）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformAccountWxVerifyInfo
{
    /// <summary>是否资质认证（官方 <c>qualification_verify</c>；0/1）。</summary>
    [JsonPropertyName("qualification_verify")]
    public int QualificationVerify { get; set; }

    /// <summary>是否名称认证（官方 <c>naming_verify</c>；0/1）。</summary>
    [JsonPropertyName("naming_verify")]
    public int NamingVerify { get; set; }

    /// <summary>获取或设置是否年审中（官方 <c>annual_review</c>；0/1，可选）。</summary>
    [JsonPropertyName("annual_review")]
    public int? AnnualReview { get; set; }

    /// <summary>获取或设置年审开始时间（官方 <c>annual_review_begin_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("annual_review_begin_time")]
    public long? AnnualReviewBeginTime { get; set; }

    /// <summary>获取或设置年审截止时间（官方 <c>annual_review_end_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("annual_review_end_time")]
    public long? AnnualReviewEndTime { get; set; }
}

/// <summary>
/// 「修改授权账号头像」请求（官方 <c>account/modifyheadimage</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformModifyAccountHeadImageRequest
{
    /// <summary>获取或设置头像图片的临时素材 <c>media_id</c>（官方 <c>head_img_media_id</c>，必填；经素材上传接口获得，有效期 3 天）。</summary>
    [JsonPropertyName("head_img_media_id")]
    public string HeadImgMediaId { get; set; } = string.Empty;

    /// <summary>获取或设置裁剪左上角 x（官方 <c>x1</c>，取值 [0,1)）。</summary>
    [JsonPropertyName("x1")]
    public decimal X1 { get; set; }

    /// <summary>获取或设置裁剪左上角 y（官方 <c>y1</c>，取值 [0,1)）。</summary>
    [JsonPropertyName("y1")]
    public decimal Y1 { get; set; }

    /// <summary>获取或设置裁剪右下角 x（官方 <c>x2</c>，取值 (0,1]）。</summary>
    [JsonPropertyName("x2")]
    public decimal X2 { get; set; }

    /// <summary>获取或设置裁剪右下角 y（官方 <c>y2</c>，取值 (0,1]）。</summary>
    [JsonPropertyName("y2")]
    public decimal Y2 { get; set; }
}

/// <summary>
/// 「修改授权账号功能简介」请求（官方 <c>account/modifysignature</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformModifyAccountSignatureRequest
{
    /// <summary>获取或设置功能简介（官方 <c>signature</c>，必填；4-120 字，一个月内可申请修改 5 次）。</summary>
    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
}

/// <summary>
/// 「使用公众号快速注册小程序」请求（官方 <c>account/fastregister</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformAccountFastRegisterRequest
{
    /// <summary>获取或设置快速注册任务票据（官方 <c>ticket</c>，必填；来自公众号侧快速注册任务）。</summary>
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = string.Empty;
}

/// <summary>
/// 「使用公众号快速注册小程序」响应（官方 <c>account/fastregister</c>；请求体仅 <c>ticket</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Account")]
public class OpenPlatformAccountFastRegisterResponse : OpenPlatformResponse
{
    /// <summary>获取或设置注册生成的小程序 <c>appid</c>（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置授权码（官方 <c>authorization_code</c>；<b>敏感凭据勿落日志</b>，可用于代开发授权链）。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>获取或设置微信认证结果（官方 <c>is_wx_verify_succ</c>；<b>官方以字符串 "true"/"false" 承载布尔语义</b>，原样取用）。</summary>
    [JsonPropertyName("is_wx_verify_succ")]
    public string? IsWxVerifySucc { get; set; }

    /// <summary>获取或设置公众号与管理员绑定结果（官方 <c>is_link_succ</c>；<b>官方以字符串 "true"/"false" 承载布尔语义</b>，原样取用）。</summary>
    [JsonPropertyName("is_link_succ")]
    public string? IsLinkSucc { get; set; }
}
