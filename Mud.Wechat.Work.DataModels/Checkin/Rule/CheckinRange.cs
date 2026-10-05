// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡人员范围信息（打卡规则 <c>range</c> 字段，请求/响应共用形态；官方要求至少有一种人员来源）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<c>range.userid</c> 在获取企业所有打卡规则的参数表中类型标注为 <c>string</c>（单个 userid），
/// 但官方返回示例为字符串数组（如 <c>["icef","LiJingZhong"]</c>），本模型以字符串数组承载以兼容示例形态。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRange
{
    /// <summary>获取或设置打卡人员中，部门节点的 id 列表（字符串数组形态）。</summary>
    [JsonPropertyName("party_id")]
    public List<string>? PartyId { get; set; }

    /// <summary>获取或设置打卡人员中，单个打卡人员节点的 userid 列表（官方参数表标注 string，返回示例为字符串数组，以示例的数组形态承载）。</summary>
    [JsonPropertyName("userid")]
    public List<string>? Userid { get; set; }

    /// <summary>获取或设置打卡人员中，标签节点的标签 id 列表。</summary>
    [JsonPropertyName("tagid")]
    public List<int>? Tagid { get; set; }
}
