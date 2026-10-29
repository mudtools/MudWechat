// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// userid 转换（自建应用与第三方/代开发应用的对接）请求体
/// （<c>/cgi-bin/batch/openuserid_to_userid</c>）：将代开发应用或第三方应用获取的密文
/// open_userid 转换为明文 userid。
/// </summary>
/// <remarks>
/// 官方限制：open_userid_list 最多不超过 1000 个，必须是 source_agentid 对应的应用所获取；
/// 需要使用自建应用的 access_token；成员需要同时在 access_token 和 source_agentid
/// 所对应应用的可见范围内。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class OpenUserIdToUserIdRequest
{
    /// <summary>
    /// 获取或设置企业主体下加密的 userid（open_userid）列表（官方必填，最多不超过 1000 个，
    /// 且必须是 <see cref="SourceAgentId"/> 对应的应用所获取）。
    /// </summary>
    [JsonPropertyName("open_userid_list")]
    public List<string>? OpenUserIdList { get; set; }

    /// <summary>
    /// 获取或设置企业授权的代开发自建应用或第三方应用的 agentid（官方必填）。
    /// </summary>
    [JsonPropertyName("source_agentid")]
    public string? SourceAgentId { get; set; }
}
