// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 批量导入上下游联系人的导入失败企业结果（获取导入任务结果响应中 <c>result.fail_list[]</c> 的元素）。
/// </summary>
/// <remarks>当企业中有联系人导入失败时，本次导入该企业所有联系人的导入都会被阻断。</remarks>
public class ChainImportFailedCorp
{
    /// <summary>
    /// 获取或设置自定义企业 id。
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    /// <summary>
    /// 获取或设置企业名称。
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }

    /// <summary>
    /// 获取或设置该企业导入操作的结果错误码。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int? ErrCode { get; set; }

    /// <summary>
    /// 获取或设置该企业导入操作的结果错误码描述。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }

    /// <summary>
    /// 获取或设置导入失败的联系人结果列表。
    /// </summary>
    [JsonPropertyName("contact_info_list")]
    public List<ChainImportFailedContact>? ContactInfoList { get; set; } = [];
}
