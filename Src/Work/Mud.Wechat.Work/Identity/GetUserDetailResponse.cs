// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Identity;

/// <summary>
/// 获取访问用户敏感信息响应体（<c>/cgi-bin/auth/getuserdetail</c>，自建应用/代开发）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<see cref="Gender"/> 为<b>字符串</b>枚举（官方示例即 <c>"1"</c>）；
/// 各敏感字段仅在成员同意 snsapi_privateinfo 授权（且管理员在应用详情中勾选）时返回真实值，
/// 否则性别返回 "0"、头像返回默认头像，其余字段为空。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Identity")]
public class GetUserDetailResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置成员 UserID。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置性别（字符串："0" 未定义 / "1" 男 / "2" 女）；
    /// 仅在用户同意 snsapi_privateinfo 授权时返回真实值，否则返回 "0"。
    /// </summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>
    /// 获取或设置头像 url；仅在用户同意 snsapi_privateinfo 授权时返回真实头像，否则返回默认头像。
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 获取或设置员工个人二维码（扫描可添加为外部联系人）；仅在用户同意 snsapi_privateinfo 授权时返回。
    /// </summary>
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }

    /// <summary>
    /// 获取或设置手机号；仅在用户同意 snsapi_privateinfo 授权时返回。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// 获取或设置邮箱；仅在用户同意 snsapi_privateinfo 授权时返回。
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// 获取或设置企业邮箱；仅在用户同意 snsapi_privateinfo 授权时返回。
    /// </summary>
    [JsonPropertyName("biz_mail")]
    public string? BizMail { get; set; }

    /// <summary>
    /// 获取或设置地址；仅在用户同意 snsapi_privateinfo 授权时返回。
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}
