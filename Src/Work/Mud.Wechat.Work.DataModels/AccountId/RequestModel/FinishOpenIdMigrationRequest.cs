// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// ID 迁移完成状态的设置请求体（<c>/cgi-bin/service/finish_openid_migration</c>）：
/// 服务商完成新旧 id 迁移后，主动将企业设置为「迁移完成」，此后获取到的将是升级后的 id。
/// </summary>
/// <remarks>
/// 官方限制：需以 <c>provider_access_token</c> 调用；userid 与 corpid 只能同时设置为迁移完成，
/// external_userid 可以单独设置；设置迁移完成后，接口不再返回该企业相关的 unionid；
/// 当该企业同时是服务商并对自己授权时，无需调用本接口；
/// 第三方应用场景（99375）仅传 corpid + openid_type；代开发场景（99378）另传 agentid，
/// 且仅传入正确 corpid、未传入 agentid 时，视为更新该企业下同服务商所有第三方应用的对应升级状态；
/// 迁移完成不可回退。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class FinishOpenIdMigrationRequest
{
    /// <summary>
    /// 获取或设置企业 corpid（官方必填）。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置企业代开发应用 id（代开发场景官方必填；仅传 corpid 不传 agentid 时，
    /// 视为更新该企业下同服务商所有第三方应用的对应升级状态）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }

    /// <summary>
    /// 获取或设置 id 类型数组（官方必填）：1 - userid 与 corpid；3 - external_userid 及 external_tagid。
    /// </summary>
    [JsonPropertyName("openid_type")]
    public List<int>? OpenIdType { get; set; }
}
