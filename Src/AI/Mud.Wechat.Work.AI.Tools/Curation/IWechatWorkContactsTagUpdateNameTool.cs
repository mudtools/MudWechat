// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.tag.update_name</c>：重命名通讯录标签（受控写）。
/// </summary>
/// <remarks>
/// <para>
/// <b>它是 <c>contacts.tag.create</c> 的纠错配对</b>：标签名打错（错别字、口径变化）时，唯一选择是改名——
/// 没有改名能力时，模型只能建议「删了重建」，而删除会连带解除成员与标签的关联（影响面远大于改名）。
/// </para>
/// <para>
/// <b>域归属是通讯录标签，不是客户标签</b>：端点 <c>/cgi-bin/tag/update</c> 属 <c>IWechatWorkTagsService</c>
/// （通讯录域），与客户联系域的「企业客户标签」是两套体系。工具名沿用 <c>contacts.*</c> 前缀，与
/// <c>contacts.tag.list</c> / <c>contacts.tag.members</c> 一致。
/// </para>
/// </remarks>
[AgentTool(
    name: "contacts.tag.update_name",
    Description = "重命名通讯录标签（成员与标签的关联不变）。参数 tag_id 为标签 ID（整数，先用 contacts.tag.list 取得）；tag_name 为新标签名（非空）。返回 ok。用于纠正标签名错别字或口径变化——比「删除后重建」安全得多（重建会丢失成员关联）。",
    Source = "IWechatWorkTagsService.UpdateTagNameAsync",
    RequiredScopes = ["contacts.tag.write"],
    IsWrite = true,
    OutputSchema = "{\"type\":\"object\",\"properties\":{\"ok\":{\"type\":\"boolean\"},\"tag_id\":{\"type\":\"integer\"},\"tag_name\":{\"type\":\"string\"}},\"required\":[\"ok\"]}")]
public interface IWechatWorkContactsTagUpdateNameTool
{
    /// <summary>重命名标签。</summary>
    /// <returns>白名单投影后的 JSON 文本（ok、tag_id、tag_name）。</returns>
    Task<string> UpdateTagNameAsync(
        [ToolParameter("tag_id", "标签 ID（整数）。", Required = true)] int tag_id,
        [ToolParameter("tag_name", "新标签名（非空）。", Required = true)] string tag_name,
        CancellationToken cancellationToken = default);
}
