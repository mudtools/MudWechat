// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// unionid 转换为第三方 external_userid 请求体
/// （<c>/cgi-bin/idconvert/unionid_to_external_userid</c>）：根据微信开放平台的 unionid 与 openid
/// 查询对应的微信用户 external_userid。
/// </summary>
/// <remarks>
/// 官方限制：subject_type 为 0（主体名称是企业的）时按企业限制（所有服务商共用企业额度）、
/// 为 1 时按服务商限制，均为 10 万次/小时、48 万次/天、750 万次/月；当前授权企业必须已认证或已验证；
/// unionid 与 openid 对应主体需认证且主体名称一致；openid 与 unionid 必须来自同一个小程序；
/// 跟进人或群主不在应用可见范围时返回 pending_id 而非 external_userid；
/// 传入有效 <see cref="MassCallTicket"/>（获取接口大批量调用凭据）可不受业务频率限制（仍受基础频率限制）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class ConvertUnionidToExternalUserIdRequest
{
    /// <summary>
    /// 获取或设置微信客户的 unionid（官方必填）。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string? Unionid { get; set; }

    /// <summary>
    /// 获取或设置微信客户的 openid（官方必填）。
    /// </summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 获取或设置小程序或公众号的主体类型：0 - 主体名称是企业的（默认），1 - 主体名称是服务商的。
    /// </summary>
    [JsonPropertyName("subject_type")]
    public int? SubjectType { get; set; }

    /// <summary>
    /// 获取或设置大批量调用凭据（选填；经获取接口大批量调用凭据接口申请，可不受业务频率限制）。
    /// </summary>
    [JsonPropertyName("mass_call_ticket")]
    public string? MassCallTicket { get; set; }
}
