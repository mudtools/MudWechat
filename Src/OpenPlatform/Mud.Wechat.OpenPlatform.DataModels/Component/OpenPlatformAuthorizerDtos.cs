// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 「获取授权方的帐号基本信息」请求（官方 <c>api_get_authorizer_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetAuthorizerInfoRequest
{
    /// <summary>获取或设置第三方平台 <c>appid</c>（官方 <c>component_appid</c>，可选）。</summary>
    [JsonPropertyName("component_appid")]
    public string? ComponentAppId { get; set; }

    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>authorizer_appid</c>，必填）。</summary>
    [JsonPropertyName("authorizer_appid")]
    public string AuthorizerAppId { get; set; } = string.Empty;
}

/// <summary>
/// 「获取授权方的帐号基本信息」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetAuthorizerInfoResponse : OpenPlatformResponse
{
    /// <summary>获取或设置授权方帐号基本信息（官方 <c>authorizer_info</c>；未授权或注销帐号可能缺省）。</summary>
    [JsonPropertyName("authorizer_info")]
    public OpenPlatformAuthorizerInfo? AuthorizerInfo { get; set; }

    /// <summary>获取或设置授权信息（官方 <c>authorization_info</c>；含接口权限集）。</summary>
    [JsonPropertyName("authorization_info")]
    public OpenPlatformAuthorizationInfo? AuthorizationInfo { get; set; }
}

/// <summary>
/// 授权方帐号基本信息（官方 <c>authorizer_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformAuthorizerInfo
{
    /// <summary>获取或设置帐号原始 ID（官方 <c>user_name</c>，如 <c>gh_</c> 前缀）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置昵称（官方 <c>nick_name</c>）。</summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }

    /// <summary>获取或设置头像 URL（官方 <c>head_img</c>；末位数值即头像尺寸，0/46/64/96/132 数值可选）。</summary>
    [JsonPropertyName("head_img")]
    public string? HeadImg { get; set; }

    /// <summary>获取或设置帐号类型（官方 <c>service_type_info</c>）。</summary>
    [JsonPropertyName("service_type_info")]
    public OpenPlatformIdNamePair? ServiceTypeInfo { get; set; }

    /// <summary>获取或设置认证类型（官方 <c>verify_type_info</c>）。</summary>
    [JsonPropertyName("verify_type_info")]
    public OpenPlatformIdNamePair? VerifyTypeInfo { get; set; }

    /// <summary>获取或设置主体名称（官方 <c>principal_name</c>；未认证时为空）。</summary>
    [JsonPropertyName("principal_name")]
    public string? PrincipalName { get; set; }

    /// <summary>获取或设置微信号（官方 <c>alias</c>；未设置时为空）。</summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>获取或设置功能简介（官方 <c>signature</c>）。</summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    /// <summary>获取或设置带参数二维码地址（官方 <c>qrcode_url</c>）。</summary>
    [JsonPropertyName("qrcode_url")]
    public string? QrcodeUrl { get; set; }

    /// <summary>获取或设置微信认证的补充信息（官方 <c>business_info</c>；仅企业类型帐号，官方以数字 0/1 承载布尔语义）。</summary>
    [JsonPropertyName("business_info")]
    public OpenPlatformAuthorizerBusinessInfo? BusinessInfo { get; set; }

    /// <summary>获取或设置小程序配置信息（官方 <c>MiniProgramInfo</c>；仅授权方为小程序时返回）。</summary>
    [JsonPropertyName("MiniProgramInfo")]
    public OpenPlatformMiniProgramInfo? MiniProgramInfo { get; set; }
}

/// <summary>
/// 帐号类型 / 认证类型对（官方 <c>service_type_info</c> / <c>verify_type_info</c> 形态：<c>{"id":n}</c>，旧文档偶带 <c>name</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformIdNamePair
{
    /// <summary>获取或设置类型编号（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>获取或设置类型名称（官方 <c>name</c>；历史字段，现代响应常缺省）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>
/// 授权方经营能力开关（官方 <c>business_info</c>；官方以数字 0/1 承载布尔语义）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformAuthorizerBusinessInfo
{
    /// <summary>是否开通微信门店（官方 <c>open_store</c>；0/1）。</summary>
    [JsonPropertyName("open_store")]
    public int OpenStore { get; set; }

    /// <summary>是否开通微信扫商品条码功能（官方 <c>open_scan</c>；0/1）。</summary>
    [JsonPropertyName("open_scan")]
    public int OpenScan { get; set; }

    /// <summary>是否开通微信支付功能（官方 <c>open_pay</c>；0/1）。</summary>
    [JsonPropertyName("open_pay")]
    public int OpenPay { get; set; }

    /// <summary>是否开通微信卡券功能（官方 <c>open_card</c>；0/1）。</summary>
    [JsonPropertyName("open_card")]
    public int OpenCard { get; set; }

    /// <summary>是否开通微信摇一摇功能（官方 <c>open_shake</c>；0/1）。</summary>
    [JsonPropertyName("open_shake")]
    public int OpenShake { get; set; }
}

/// <summary>
/// 授权方小程序配置信息（官方 <c>MiniProgramInfo</c>；注意官方 JSON 键为驼峰 <c>MiniProgramInfo</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformMiniProgramInfo
{
    /// <summary>获取或设置网络配置（官方 <c>network</c>）。</summary>
    [JsonPropertyName("network")]
    public OpenPlatformMiniProgramNetwork? Network { get; set; }

    /// <summary>获取或设置类目配置（官方 <c>categories</c>）。</summary>
    [JsonPropertyName("categories")]
    public OpenPlatformMiniProgramCategory[]? Categories { get; set; }

    /// <summary>获取或设置是否被搜索（官方 <c>visit_status</c>；0=不可搜索，1=可搜索）。</summary>
    [JsonPropertyName("visit_status")]
    public int VisitStatus { get; set; }

    /// <summary>获取或设置帐号状态（官方 <c>account_status</c>）。</summary>
    [JsonPropertyName("account_status")]
    public int AccountStatus { get; set; }

    /// <summary>获取或设置注册方式（官方 <c>register_type</c>；仅注册方式为以下值时返回，可选）。</summary>
    [JsonPropertyName("register_type")]
    public int? RegisterType { get; set; }

    /// <summary>获取或设置基础配置（官方 <c>basic_config</c>；是否已配置手机号 / 邮箱，官方以数字 0/1 承载布尔语义）。</summary>
    [JsonPropertyName("basic_config")]
    public OpenPlatformMiniProgramBasicConfig? BasicConfig { get; set; }
}

/// <summary>
/// 小程序网络域名配置（官方 <c>network</c>；各域名为数组）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformMiniProgramNetwork
{
    /// <summary>request 合法域名（官方 <c>RequestDomain</c>）。</summary>
    [JsonPropertyName("RequestDomain")]
    public string[]? RequestDomain { get; set; }

    /// <summary>socket 合法域名（官方 <c>WsRequestDomain</c>）。</summary>
    [JsonPropertyName("WsRequestDomain")]
    public string[]? WsRequestDomain { get; set; }

    /// <summary>uploadFile 合法域名（官方 <c>UploadDomain</c>）。</summary>
    [JsonPropertyName("UploadDomain")]
    public string[]? UploadDomain { get; set; }

    /// <summary>downloadFile 合法域名（官方 <c>DownloadDomain</c>）。</summary>
    [JsonPropertyName("DownloadDomain")]
    public string[]? DownloadDomain { get; set; }

    /// <summary>UDP 合法域名（官方 <c>UDPDomain</c>）。</summary>
    [JsonPropertyName("UDPDomain")]
    public string[]? UdpDomain { get; set; }

    /// <summary>业务域名（官方 <c>BizDomain</c>）。</summary>
    [JsonPropertyName("BizDomain")]
    public string[]? BizDomain { get; set; }
}

/// <summary>
/// 小程序类目（官方 <c>categories</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformMiniProgramCategory
{
    /// <summary>一级类目（官方 <c>first</c>）。</summary>
    [JsonPropertyName("first")]
    public string? First { get; set; }

    /// <summary>二级类目（官方 <c>second</c>）。</summary>
    [JsonPropertyName("second")]
    public string? Second { get; set; }
}

/// <summary>
/// 小程序基础配置（官方 <c>basic_config</c>；官方以数字 0/1 承载布尔语义）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformMiniProgramBasicConfig
{
    /// <summary>是否已配置手机号（官方 <c>is_phone_configured</c>；0/1）。</summary>
    [JsonPropertyName("is_phone_configured")]
    public int IsPhoneConfigured { get; set; }

    /// <summary>是否已配置邮箱（官方 <c>is_email_configured</c>；0/1）。</summary>
    [JsonPropertyName("is_email_configured")]
    public int IsEmailConfigured { get; set; }
}

/// <summary>
/// 授权信息（官方 <c>authorization_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformAuthorizationInfo
{
    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>authorizer_appid</c>）。</summary>
    [JsonPropertyName("authorizer_appid")]
    public string? AuthorizerAppId { get; set; }

    /// <summary>获取或设置授权方接口调用令牌（官方 <c>authorizer_access_token</c>；仅查询令牌本身时返回，<b>敏感字段勿落日志</b>）。</summary>
    [JsonPropertyName("authorizer_access_token")]
    public string? AuthorizerAccessToken { get; set; }

    /// <summary>获取或设置刷新令牌（官方 <c>authorizer_refresh_token</c>；<b>长期凭据勿落日志</b>）。</summary>
    [JsonPropertyName("authorizer_refresh_token")]
    public string? AuthorizerRefreshToken { get; set; }

    /// <summary>获取或设置令牌有效期（官方 <c>expires_in</c>，秒；可选）。</summary>
    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; set; }

    /// <summary>获取或设置授权的接口权限集列表（官方 <c>func_info</c>）。</summary>
    [JsonPropertyName("func_info")]
    public OpenPlatformFuncInfoItem[]? FuncInfo { get; set; }
}

/// <summary>
/// 接口权限集项（官方 <c>func_info</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFuncInfoItem
{
    /// <summary>获取或设置权限集详情（官方 <c>funcscope_category</c>）。</summary>
    [JsonPropertyName("funcscope_category")]
    public OpenPlatformFuncScopeCategory? FuncscopeCategory { get; set; }
}

/// <summary>
/// 权限集详情（官方 <c>funcscope_category</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFuncScopeCategory
{
    /// <summary>获取或设置权限集编号（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>获取或设置权限集类型（官方 <c>type</c>；历史字段，可选）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置权限集名称（官方 <c>name</c>；历史字段，可选）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置权限集说明（官方 <c>desc</c>；历史字段，可选）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}

/// <summary>
/// 「获取授权方列表」请求（官方 <c>api_get_authorizer_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetAuthorizerListRequest
{
    /// <summary>获取或设置第三方平台 <c>appid</c>（官方 <c>component_appid</c>，可选）。</summary>
    [JsonPropertyName("component_appid")]
    public string? ComponentAppId { get; set; }

    /// <summary>获取或设置偏移位置（官方 <c>offset</c>，必填；起始为 0）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置拉取数量（官方 <c>count</c>，必填；最大 500）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; } = 10;
}

/// <summary>
/// 「获取授权方列表」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetAuthorizerListResponse : OpenPlatformResponse
{
    /// <summary>获取或设置授权的账号总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    /// <summary>获取或设置当前页授权账号列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public OpenPlatformAuthorizerListItem[]? List { get; set; }
}

/// <summary>
/// 授权账号基本信息项（官方 <c>list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformAuthorizerListItem
{
    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>authorizer_appid</c>）。</summary>
    [JsonPropertyName("authorizer_appid")]
    public string? AuthorizerAppId { get; set; }

    /// <summary>获取或设置刷新令牌（官方 <c>refresh_token</c>；<b>长期凭据勿落日志</b>）。</summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>获取或设置授权时间（官方 <c>auth_time</c>；秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("auth_time")]
    public long AuthTime { get; set; }
}

/// <summary>
/// 「获取授权方选项设置信息」请求（官方 <c>api_get_authorizer_option</c>）。
/// </summary>
/// <remarks>
/// <para><b>与 SKIT 的差异（有意）</b>：SKIT 未建模 <c>authorizer_appid</c> 请求体字段，官方文档要求携带——本 DTO 按官方补齐。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetAuthorizerOptionRequest
{
    /// <summary>获取或设置第三方平台 <c>appid</c>（官方 <c>component_appid</c>，可选）。</summary>
    [JsonPropertyName("component_appid")]
    public string? ComponentAppId { get; set; }

    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>authorizer_appid</c>，必填）。</summary>
    [JsonPropertyName("authorizer_appid")]
    public string AuthorizerAppId { get; set; } = string.Empty;

    /// <summary>获取或设置选项名称（官方 <c>option_name</c>，必填；如 <c>voice_recognize</c> / <c>customer_service</c>）。</summary>
    [JsonPropertyName("option_name")]
    public string OptionName { get; set; } = string.Empty;
}

/// <summary>
/// 「获取授权方选项设置信息」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetAuthorizerOptionResponse : OpenPlatformResponse
{
    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>authorizer_appid</c>）。</summary>
    [JsonPropertyName("authorizer_appid")]
    public string? AuthorizerAppId { get; set; }

    /// <summary>获取或设置选项名称（官方 <c>option_name</c>）。</summary>
    [JsonPropertyName("option_name")]
    public string? OptionName { get; set; }

    /// <summary>获取或设置选项值（官方 <c>option_value</c>；官方可能以数字承载，原样取用）。</summary>
    [JsonPropertyName("option_value")]
    public string? OptionValue { get; set; }
}

/// <summary>
/// 「设置授权方选项信息」请求（官方 <c>api_set_authorizer_option</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformSetAuthorizerOptionRequest
{
    /// <summary>获取或设置第三方平台 <c>appid</c>（官方 <c>component_appid</c>，可选）。</summary>
    [JsonPropertyName("component_appid")]
    public string? ComponentAppId { get; set; }

    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>authorizer_appid</c>，必填）。</summary>
    [JsonPropertyName("authorizer_appid")]
    public string AuthorizerAppId { get; set; } = string.Empty;

    /// <summary>获取或设置选项名称（官方 <c>option_name</c>，必填）。</summary>
    [JsonPropertyName("option_name")]
    public string OptionName { get; set; } = string.Empty;

    /// <summary>获取或设置选项值（官方 <c>option_value</c>，必填）。</summary>
    [JsonPropertyName("option_value")]
    public string OptionValue { get; set; } = string.Empty;
}
