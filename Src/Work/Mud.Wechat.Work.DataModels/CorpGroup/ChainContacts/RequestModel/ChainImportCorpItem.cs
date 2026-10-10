// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 批量导入上下游联系人的企业条目（<see cref="ImportChainContactsRequest"/> 中 <c>contact_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ChainContacts")]
public class ChainImportCorpItem
{
    /// <summary>
    /// 获取或设置上下游企业名称（长度 1-32 个 utf8 字符，只能由中文、字母、数字和“ -_()（）”六种字符组成）。
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string CorpName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置导入后企业所在分组（格式如「华北区/北京市/海淀区」；分组为空的企业放在根分组下；
    /// 仅针对新导入企业生效，不会修改已导入企业的分组。不填时不出网）。
    /// </summary>
    [JsonPropertyName("group_path")]
    public string? GroupPath { get; set; }

    /// <summary>
    /// 获取或设置上下游企业自定义 id（长度 0～64 个字节，只能由数字和字母组成；建议填写以便识别下级企业。不填时不出网）。
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    /// <summary>
    /// 获取或设置上下游联系人信息列表。
    /// </summary>
    [JsonPropertyName("contact_info_list")]
    public List<ChainImportContactItem>? ContactInfoList { get; set; }
}
