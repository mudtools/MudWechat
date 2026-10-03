// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// external_userid 转换（自建应用与第三方/代开发应用的对接）请求体
/// （<c>/cgi-bin/externalcontact/from_service_external_userid</c>）：将代开发应用或第三方应用获取的
/// external_userid 转换成自建应用（企业主体）的 external_userid。
/// </summary>
/// <remarks>
/// 官方限制：需要使用自建应用的 access_token；客户的跟进人，或者用户所在客户群的群主，
/// 需要同时在 access_token 和 source_agentid 所对应应用的可见范围内。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class FromServiceExternalUserIdRequest
{
    /// <summary>
    /// 获取或设置服务商主体的 external_userid（官方必填，必须是 source_agentid 对应的应用所获取）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置企业授权的代开发自建应用或第三方应用的 agentid（官方必填）。
    /// </summary>
    [JsonPropertyName("source_agentid")]
    public string? SourceAgentId { get; set; }
}
