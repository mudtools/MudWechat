// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Identity;

/// <summary>
/// 获取访问用户身份响应体（第三方，<c>/cgi-bin/service/auth/getuserinfo3rd</c>；
/// 企业微信 Web 登录复用本端点换取登录用户身份）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：用户属于某企业时返回 <see cref="Corpid"/> / <see cref="Userid"/> 系字段，
/// 不属于任何企业时仅返回 <see cref="Openid"/>，调用方须按可空性分流。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Identity")]
public class GetUserInfo3rdResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置用户所属企业的 corpid。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>
    /// 获取或设置用户在企业内的 UserID；
    /// 该企业与第三方应用无授权关系时返回密文 UserId，有授权关系时按升级后的 ID 策略返回明文或密文。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置成员票据（最大 512 字节）；
    /// 仅授权 scope 为 snsapi_privateinfo 且用户在应用可见范围内时返回，
    /// 可凭其调用「获取访问用户敏感信息（第三方）」接口。
    /// </summary>
    [JsonPropertyName("user_ticket")]
    public string? UserTicket { get; set; }

    /// <summary>
    /// 获取或设置 user_ticket 的有效时间（秒），随 user_ticket 一起返回。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; set; }

    /// <summary>
    /// 获取或设置全局唯一的 open_userid：对同一服务商不同应用获取同一成员结果相同（最多 64 字节），仅第三方应用可获取。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserid { get; set; }

    /// <summary>
    /// 获取或设置搜索票据（可在数据专区用于召回文档片段）；
    /// 仅拥有智能专区文档存档权限且所在企业已安装归属当前服务商应用的应用返回该字段（灰度内测中）。
    /// </summary>
    [JsonPropertyName("user_doc_ticket")]
    public string? UserDocTicket { get; set; }

    /// <summary>
    /// 获取或设置非企业成员的标识（对当前服务商唯一）；仅用户不属于任何企业时返回。
    /// </summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }
}
