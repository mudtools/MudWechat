// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// tmp_external_userid 转换结果元素（官方按 user_type 返回不同字段：可空超集覆盖全部用户类型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class TmpExternalUserIdResultItem
{
    /// <summary>
    /// 获取或设置输入的 tmp_external_userid。
    /// </summary>
    [JsonPropertyName("tmp_external_userid")]
    public string? TmpExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置转换后的 external_userid（user_type 为 1 - 客户时返回）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置 userid 对应的 corpid（user_type 为 2 - 企业互联、3 - 上下游、4 - 互联企业（圈子）时返回）。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置转换后的 userid（user_type 为 2 - 企业互联、3 - 上下游、4 - 互联企业（圈子）时返回）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }
}
