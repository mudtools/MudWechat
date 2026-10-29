// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 文件操作的企业外部用户信息（<see cref="FileOperRecordItem.ExternalUser"/>；企业内部用户时返回 userid 而非本对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class FileOperRecordExternalUser
{
    /// <summary>
    /// 获取或设置外部用户类型：1-微信用户 2-企业微信用户。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置外部用户名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置外部用户所属企业名称（企业微信用户时返回）。
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }
}
