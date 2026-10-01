// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 客户（外部联系人）基本信息（获取客户详情 <c>external_contact</c> 与批量获取客户详情共用）。
/// </summary>
/// <remarks>
/// 敏感字段官方口径：<see cref="Avatar"/>、<see cref="Gender"/>、<see cref="Unionid"/>
/// 第三方应用与代开发应用均不可获取（gender 统一返回 0）；<see cref="CorpFullName"/> 仅企业自建应用可获取，
/// 其余应用返回内容为企业名称（即 <see cref="CorpName"/>）。
/// </remarks>
public class ExternalContactInfo
{
    /// <summary>
    /// 获取或设置外部联系人的 userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置外部联系人的名称（微信用户返回微信昵称；企业微信联系人返回对外展示的别名或实名）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置外部联系人头像（第三方应用和代开发应用不可获取）。
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 获取或设置外部联系人的类型：1-微信用户，2-企业微信用户。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置外部联系人性别：0-未知，1-男性，2-女性（第三方应用和代开发应用不可获取，统一返回 0）。
    /// </summary>
    [JsonPropertyName("gender")]
    public int? Gender { get; set; }

    /// <summary>
    /// 获取或设置外部联系人在微信开放平台的唯一身份标识（unionid；仅微信用户且企业绑定微信开发者 ID 时返回，
    /// 第三方应用和代开发应用不可获取）。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string? Unionid { get; set; }

    /// <summary>
    /// 获取或设置外部联系人的职位（仅企业微信用户且未隐藏职位时返回）。
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }

    /// <summary>
    /// 获取或设置外部联系人所在企业的简称（仅企业微信用户时返回）。
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }

    /// <summary>
    /// 获取或设置外部联系人所在企业的主体名称（仅企业微信用户时返回；仅企业自建应用可获取）。
    /// </summary>
    [JsonPropertyName("corp_full_name")]
    public string? CorpFullName { get; set; }

    /// <summary>
    /// 获取或设置外部联系人的自定义展示信息（对外属性，仅企业微信用户时返回）。
    /// </summary>
    [JsonPropertyName("external_profile")]
    public ExternalProfile? ExternalProfile { get; set; }
}
