// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Tags;

/// <summary>
/// 获取标签成员响应体（<c>/cgi-bin/tag/get</c>；返回列表仅包含应用可见范围内的成员）。
/// </summary>
public class GetTagMembersResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置标签名称。
    /// </summary>
    [JsonPropertyName("tagname")]
    public string? TagName { get; set; }

    /// <summary>
    /// 获取或设置标签内的企业成员列表。
    /// </summary>
    [JsonPropertyName("userlist")]
    public List<TagMemberInfo>? UserList { get; set; } = [];

    /// <summary>
    /// 获取或设置标签内的企业部门 ID 列表。
    /// </summary>
    [JsonPropertyName("partylist")]
    public List<int>? PartyList { get; set; } = [];
}
