// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 成员对外属性项（<c>external_profile.external_attr[]</c>，支持文本 / 网页 / 小程序三型）。
/// </summary>
public class UserExternalAttrItem
{
    /// <summary>
    /// 获取或设置对外属性类型（0 文本；1 网页；2 小程序）。
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 获取或设置属性名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置文本类型属性值（type = 0 时使用）。
    /// </summary>
    [JsonPropertyName("text")]
    public UserExtAttrText? Text { get; set; }

    /// <summary>
    /// 获取或设置网页类型属性（type = 1 时使用）。
    /// </summary>
    [JsonPropertyName("web")]
    public UserExtAttrWeb? Web { get; set; }

    /// <summary>
    /// 获取或设置小程序类型属性（type = 2 时使用）。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public UserMiniProgram? MiniProgram { get; set; }
}
