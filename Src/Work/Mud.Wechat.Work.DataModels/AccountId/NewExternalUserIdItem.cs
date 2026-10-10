// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// external_userid 转换结果元素（转换 external_userid 与转换客户群成员 external_userid 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class NewExternalUserIdItem
{
    /// <summary>
    /// 获取或设置传入的企业主体 external_userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置新外部联系人 id（服务商主体下的 external_userid；传入新的 external_userid 时原样返回）。
    /// </summary>
    [JsonPropertyName("new_external_userid")]
    public string? NewExternalUserId { get; set; }
}
