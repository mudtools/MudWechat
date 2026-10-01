// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 批量导入上下游联系人请求体（<c>/cgi-bin/corpgroup/import_chain_contact</c>，提交导入任务）。
/// </summary>
/// <remarks>
/// 导入限制：单次导入的企业总数 ≤ 1000 个；单个企业导入人数 ≤ 200 人；单次最多导入 2000 人；
/// 每天最多导入 20000 人；<b>只允许串行调用</b>，且同时只能存在一个导入任务（含管理后台提交的任务）。
/// 仅已验证的企业可调用；自建应用须配置到「上下游-可调用接口的应用」中。
/// </remarks>
public class ImportChainContactsRequest
{
    /// <summary>
    /// 获取或设置上下游 id（文件中的联系人将会被导入此上下游中）。
    /// </summary>
    [JsonPropertyName("chain_id")]
    public string ChainId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置上下游联系人列表（这些联系人将会被导入此上下游中）。
    /// </summary>
    [JsonPropertyName("contact_list")]
    public List<ChainImportCorpItem>? ContactList { get; set; }
}
