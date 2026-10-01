// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 批量导入上下游联系人的任务处理结果（获取导入任务结果响应中 <c>result</c> 字段；任务完成后有效）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ChainContacts")]
public class ChainImportResult
{
    /// <summary>
    /// 获取或设置上下游 id。
    /// </summary>
    [JsonPropertyName("chain_id")]
    public string? ChainId { get; set; }

    /// <summary>
    /// 获取或设置导入状态：1 - 全部企业导入成功，2 - 部分企业导入成功，3 - 全部企业导入失败。
    /// </summary>
    [JsonPropertyName("import_status")]
    public int? ImportStatus { get; set; }

    /// <summary>
    /// 获取或设置导入失败结果列表。
    /// </summary>
    [JsonPropertyName("fail_list")]
    public List<ChainImportFailedCorp>? FailList { get; set; } = [];
}
