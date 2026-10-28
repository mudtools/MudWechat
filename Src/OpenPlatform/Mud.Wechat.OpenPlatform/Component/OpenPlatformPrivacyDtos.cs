// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 「查询用户隐私保护指引配置」请求（官方 <c>getprivacysetting</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetPrivacySettingRequest
{
    /// <summary>获取或设置指引版本（官方 <c>privacy_ver</c>，可选；1 表示新版用户隐私保护指引，不传默认旧版）。</summary>
    [JsonPropertyName("privacy_ver")]
    public int? PrivacyVer { get; set; }
}

/// <summary>
/// 「查询用户隐私保护指引配置」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetPrivacySettingResponse : OpenPlatformResponse
{
    /// <summary>获取或设置代码是否存在（官方 <c>code_exist</c>；官方以数字 0/1 承载布尔语义）。</summary>
    [JsonPropertyName("code_exist")]
    public int CodeExist { get; set; }

    /// <summary>获取或设置隐私负责人设置（官方 <c>owner_setting</c>；未设置时缺省）。</summary>
    [JsonPropertyName("owner_setting")]
    public OpenPlatformPrivacyOwnerSetting? OwnerSetting { get; set; }

    /// <summary>获取或设置官方预定义的隐私条目键列表（官方 <c>privacy_list</c>，可选）。</summary>
    [JsonPropertyName("privacy_list")]
    public string[]? PrivacyKeyList { get; set; }

    /// <summary>获取或设置开发者自定义的隐私条目配置列表（官方 <c>setting_list</c>，可选）。</summary>
    [JsonPropertyName("setting_list")]
    public OpenPlatformPrivacySetting[]? SettingList { get; set; }

    /// <summary>获取或设置隐私条目与用途描述的映射（官方 <c>privacy_desc</c>，可选）。</summary>
    [JsonPropertyName("privacy_desc")]
    public OpenPlatformPrivacyDescription? PrivacyDesc { get; set; }

    /// <summary>获取或设置最近更新时间戳（官方 <c>update_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>
/// 隐私负责人设置（官方 <c>owner_setting</c>；get / set 两端点共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformPrivacyOwnerSetting
{
    /// <summary>获取或设置联系邮箱（官方 <c>contact_email</c>，可选）。</summary>
    [JsonPropertyName("contact_email")]
    public string? ContactEmail { get; set; }

    /// <summary>获取或设置联系手机号（官方 <c>contact_phone</c>，可选）。</summary>
    [JsonPropertyName("contact_phone")]
    public string? ContactPhone { get; set; }

    /// <summary>获取或设置联系 QQ（官方 <c>contact_qq</c>，可选）。</summary>
    [JsonPropertyName("contact_qq")]
    public string? ContactQq { get; set; }

    /// <summary>获取或设置联系微信号（官方 <c>contact_weixin</c>，可选）。</summary>
    [JsonPropertyName("contact_weixin")]
    public string? ContactWeixin { get; set; }

    /// <summary>获取或设置隐私声明补充文件的临时素材 <c>media_id</c>（官方 <c>ext_file_media_id</c>；经 uploadprivacyextfile 上传获得）。</summary>
    [JsonPropertyName("ext_file_media_id")]
    public string? ExtFileMediaId { get; set; }

    /// <summary>获取或设置通知方式（官方 <c>notice_method</c>，可选）。</summary>
    [JsonPropertyName("notice_method")]
    public string? NoticeMethod { get; set; }

    /// <summary>获取或设置数据存储期限（官方 <c>store_expire_timestamp</c>，可选；秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("store_expire_timestamp")]
    public string? StoreExpireTimestamp { get; set; }
}

/// <summary>
/// 用户隐私保护指引条目（官方 <c>setting_list</c> 项；请求侧用 <see cref="PrivacyKey"/>/<see cref="PrivacyText"/>，响应侧额外含 <see cref="PrivacyLabel"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformPrivacySetting
{
    /// <summary>获取或设置隐私条目键（官方 <c>privacy_key</c>；官方预定义键或自定义键）。</summary>
    [JsonPropertyName("privacy_key")]
    public string PrivacyKey { get; set; } = string.Empty;

    /// <summary>获取或设置隐私条目用途描述（官方 <c>privacy_text</c>；开发者自定义，不超过 40 字）。</summary>
    [JsonPropertyName("privacy_text")]
    public string PrivacyText { get; set; } = string.Empty;

    /// <summary>获取或设置隐私条目含义说明（官方 <c>privacy_label</c>；仅查询响应携带，可选）。</summary>
    [JsonPropertyName("privacy_label")]
    public string? PrivacyLabel { get; set; }
}

/// <summary>
/// 隐私条目描述映射容器（官方 <c>privacy_desc</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformPrivacyDescription
{
    /// <summary>获取或设置条目描述列表（官方 <c>privacy_desc_list</c>）。</summary>
    [JsonPropertyName("privacy_desc_list")]
    public OpenPlatformPrivacyDescriptionItem[]? PrivacyDescList { get; set; }
}

/// <summary>
/// 隐私条目描述项（官方 <c>privacy_desc_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformPrivacyDescriptionItem
{
    /// <summary>获取或设置隐私条目键（官方 <c>privacy_key</c>）。</summary>
    [JsonPropertyName("privacy_key")]
    public string? PrivacyKey { get; set; }

    /// <summary>获取或设置隐私条目用途描述（官方 <c>privacy_desc</c>）。</summary>
    [JsonPropertyName("privacy_desc")]
    public string? PrivacyDesc { get; set; }
}

/// <summary>
/// 「设置用户隐私保护指引配置」请求（官方 <c>setprivacysetting</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformSetPrivacySettingRequest
{
    /// <summary>获取或设置指引版本（官方 <c>privacy_ver</c>，可选；1 表示新版）。</summary>
    [JsonPropertyName("privacy_ver")]
    public int? PrivacyVer { get; set; }

    /// <summary>获取或设置隐私负责人设置（官方 <c>owner_setting</c>；官方要求必填）。</summary>
    [JsonPropertyName("owner_setting")]
    public OpenPlatformPrivacyOwnerSetting? OwnerSetting { get; set; }

    /// <summary>获取或设置开发者自定义的隐私条目配置列表（官方 <c>setting_list</c>，可选）。</summary>
    [JsonPropertyName("setting_list")]
    public OpenPlatformPrivacySetting[]? SettingList { get; set; }
}

/// <summary>
/// 「上传隐私保护指引补充文件」响应（官方 <c>uploadprivacyextfile</c>；请求为 multipart 表单，文件字段名固定 <c>file</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformUploadPrivacyExtFileResponse : OpenPlatformResponse
{
    /// <summary>获取或设置补充文件的临时素材 <c>media_id</c>（官方 <c>ext_file_media_id</c>；填入 <see cref="OpenPlatformPrivacyOwnerSetting.ExtFileMediaId"/>）。</summary>
    [JsonPropertyName("ext_file_media_id")]
    public string? ExtFileMediaId { get; set; }
}
