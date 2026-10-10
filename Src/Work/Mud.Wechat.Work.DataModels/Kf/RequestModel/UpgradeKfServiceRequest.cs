// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 为客户升级为专员或客户群服务请求体（<c>/cgi-bin/kf/customer/upgrade_service</c>）。
/// <para>
/// <see cref="Type"/> 为 1（专员服务）时填充 <see cref="Member"/>，为 2（客户群服务）时填充
/// <see cref="Groupchat"/>；指定的 userid / chat_id 必须已配置在微信客服「升级服务」中，否则官方返回 95021 错误码。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class UpgradeKfServiceRequest
{
    /// <summary>
    /// 获取或设置客服账号 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置微信客户的 external_userid（官方必填）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置升级服务类型（官方必填）：1 - 专员服务，2 - 客户群服务。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置推荐的服务专员（<see cref="Type"/> 为 1 时有效）。
    /// </summary>
    [JsonPropertyName("member")]
    public KfUpgradeMember? Member { get; set; }

    /// <summary>
    /// 获取或设置推荐的客户群（<see cref="Type"/> 为 2 时有效）。
    /// </summary>
    [JsonPropertyName("groupchat")]
    public KfUpgradeGroupchat? Groupchat { get; set; }
}

/// <summary>
/// 升级服务的推荐专员（<see cref="UpgradeKfServiceRequest.Member"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfUpgradeMember
{
    /// <summary>
    /// 获取或设置服务专员的 userid（type = 1 时官方必填）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置推荐语。
    /// </summary>
    [JsonPropertyName("wording")]
    public string? Wording { get; set; }
}

/// <summary>
/// 升级服务的推荐客户群（<see cref="UpgradeKfServiceRequest.Groupchat"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfUpgradeGroupchat
{
    /// <summary>
    /// 获取或设置客户群 id（type = 2 时官方必填）。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置推荐语。
    /// </summary>
    [JsonPropertyName("wording")]
    public string? Wording { get; set; }
}
