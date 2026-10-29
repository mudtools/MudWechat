// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 批量导入上下游联系人的联系人条目（<see cref="ChainImportCorpItem"/> 中 <c>contact_info_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ChainContacts")]
public class ChainImportContactItem
{
    /// <summary>
    /// 获取或设置上下游联系人姓名（长度 1～32 个 utf8 字符）。
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置联系人身份类型：1 - 成员，2 - 负责人（单企业负责人最多 5 人，也可不填）。
    /// </summary>
    [JsonPropertyName("identity_type")]
    public int IdentityType { get; set; }

    /// <summary>
    /// 获取或设置手机号（支持国内、国际手机号；国际手机号必须包含加号以及国家地区码，如「+85259****45」）。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置上下游用户自定义 id（字符串，暂时只支持 64 比特无符号整型：取值 1 到 2^64-2、必须全数字、
    /// 不得传入前置 0、且不能为 11 位或 13 位数字。建议填写以便识别下级企业成员。不填时不出网）。
    /// </summary>
    [JsonPropertyName("user_custom_id")]
    public string? UserCustomId { get; set; }
}
