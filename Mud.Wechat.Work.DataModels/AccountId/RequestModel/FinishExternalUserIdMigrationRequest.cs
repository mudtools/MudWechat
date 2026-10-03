// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// 设置迁移完成（external_userid）请求体
/// （<c>/cgi-bin/service/externalcontact/finish_external_userid_migration</c>）：
/// 服务商完成企业下所有第三方应用 external_userid 新旧 id 迁移后，主动设置为「迁移完成」。
/// </summary>
/// <remarks>
/// 官方限制：需以 <c>provider_access_token</c> 调用；该企业须已授权该服务商第三方应用；
/// 当该企业同时是服务商并对自己授权的情况，无需调用本接口。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class FinishExternalUserIdMigrationRequest
{
    /// <summary>
    /// 获取或设置企业 corpid（官方必填）。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }
}
