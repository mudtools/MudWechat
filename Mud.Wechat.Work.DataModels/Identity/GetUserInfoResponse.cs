// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Identity;

/// <summary>
/// 获取访问用户身份响应体（<c>/cgi-bin/auth/getuserinfo</c>，自建应用/代开发；
/// 企业微信 Web 登录复用本端点换取登录用户身份）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：企业成员与非企业成员两类返回互斥——成员返回 <see cref="Userid"/> 系字段，
/// 非成员返回 <see cref="Openid"/> / <see cref="ExternalUserid"/>，调用方须按可空性分流。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Identity")]
public class GetUserInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置成员 UserID；互联企业/企业互联/上下游场景格式为 <c>CorpId/userid</c>。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置成员票据（最大 512 字节，有效期 1800 秒）；
    /// 仅授权 scope 为 snsapi_privateinfo 且用户在应用可见范围内时返回，
    /// 可凭其调用「获取访问用户敏感信息」接口（暂不支持上下游/企业互联场景）。
    /// </summary>
    [JsonPropertyName("user_ticket")]
    public string? UserTicket { get; set; }

    /// <summary>
    /// 获取或设置文档检索票据（可在数据专区用于召回文档片段，有效期 30 天）；
    /// 仅拥有智能专区文档存档权限的应用返回该字段（灰度内测中）。
    /// </summary>
    [JsonPropertyName("user_doc_ticket")]
    public string? UserDocTicket { get; set; }

    /// <summary>
    /// 获取或设置非企业成员的标识（对当前企业唯一，不超过 64 字节）；仅用户非企业成员时返回。
    /// </summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>
    /// 获取或设置外部联系人 id；当且仅当用户是企业的客户且跟进人在应用可见范围内时返回。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }
}
