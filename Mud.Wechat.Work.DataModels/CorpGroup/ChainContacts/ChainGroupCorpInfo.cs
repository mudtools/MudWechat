// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 上下游通讯录分组下的企业信息（获取企业上下游通讯录分组下的企业详情列表响应中 <c>group_corps[]</c> 的元素）。
/// </summary>
/// <remarks>官方列表示例中 <c>is_joined</c> 以 1/0 整型传输（详情接口则为布尔），本模型按各端点示例分别承载。</remarks>
public class ChainGroupCorpInfo
{
    /// <summary>
    /// 获取或设置企业所属上下游的分组 id。
    /// </summary>
    [JsonPropertyName("groupid")]
    public int? GroupId { get; set; }

    /// <summary>
    /// 获取或设置企业 id（最多 64 个字节；已加入的企业返回）。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置企业名称。
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }

    /// <summary>
    /// 获取或设置上下游企业自定义 id（返回批量导入上下游联系人时指定的企业自定义 id；未指定则为空）。
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    /// <summary>
    /// 获取或设置该上下游的邀请人 userid（仅「上下游-可调用接口的应用」调用时返回，且成员需在应用可见范围内）。
    /// </summary>
    [JsonPropertyName("invite_userid")]
    public string? InviteUserid { get; set; }

    /// <summary>
    /// 获取或设置未加入企业 id（未加入的企业返回）。
    /// </summary>
    [JsonPropertyName("pending_corpid")]
    public string? PendingCorpId { get; set; }

    /// <summary>
    /// 获取或设置企业是否已加入（官方列表示例以 1/0 整型传输）。
    /// </summary>
    [JsonPropertyName("is_joined")]
    public int? IsJoined { get; set; }
}
