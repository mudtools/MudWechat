// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// unionid 转换为第三方 external_userid 请求体
/// （<c>/cgi-bin/idconvert/unionid_to_external_userid</c>）。
/// </summary>
/// <remarks>
/// unionid 与 openid 必须是在同一个小程序（或公众号）获取到的，且账号主体名称需与当前授权企业主体一致
/// （或与服务商主体一致）；授权企业必须已认证或已验证。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class ConvertUnionIdToExternalUserIdRequest
{
    /// <summary>
    /// 获取或设置微信客户的 unionid。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string? Unionid { get; set; }

    /// <summary>
    /// 获取或设置微信客户的 openid（与 unionid 须取自同一个小程序）。
    /// </summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>
    /// 获取或设置小程序或公众号的主体类型：0-主体名称是企业的（默认），1-主体名称是服务商的。
    /// </summary>
    [JsonPropertyName("subject_type")]
    public int? SubjectType { get; set; }
}
