// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 获取企业上下游通讯录下的企业信息响应体（<c>/cgi-bin/corpgroup/corp/get_chain_corpinfo</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ChainContacts")]
public class GetChainCorpInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置企业名称。
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }

    /// <summary>
    /// 获取或设置企业是否验证或认证：1 - 未验证，2 - 已验证，3 - 已认证（已加入的企业返回）。
    /// </summary>
    [JsonPropertyName("qualification_status")]
    public int? QualificationStatus { get; set; }

    /// <summary>
    /// 获取或设置上下游企业自定义 id（返回批量导入时指定的企业自定义 id；未指定则为空）。
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    /// <summary>
    /// 获取或设置企业所属上下游的分组 id。
    /// </summary>
    [JsonPropertyName("groupid")]
    public int? GroupId { get; set; }

    /// <summary>
    /// 获取或设置企业是否已加入（官方详情示例以布尔传输）。
    /// </summary>
    [JsonPropertyName("is_joined")]
    public bool? IsJoined { get; set; }
}
