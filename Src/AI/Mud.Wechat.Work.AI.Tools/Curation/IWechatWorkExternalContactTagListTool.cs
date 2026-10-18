// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.tag.list</c>：读取企业客户标签库（标签组 + 标签）。
/// </summary>
/// <remarks>
/// <para>
/// <b>零参数是刻意的</b>：官方 <c>get_corp_tag_list</c> 的 <c>tag_id</c>/<c>group_id</c> 均为可选过滤条件；
/// 模型先要「企业有哪些标签」才谈得上按标签筛选客户或给客户打标，故工具只暴露「全量读取」这一种用法，
/// 避免让模型猜过滤参数（传错 ID 只会得到空列表，白白消耗一轮）。
/// </para>
/// <para>
/// <b>已删除项不投影</b>：官方用 <c>deleted</c> 标记软删除，执行器直接滤掉（含整组删除）。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.tag.list",
    Description = "读取企业微信客户标签库（零参数）。返回 tag_group 数组，每项含 group_id、group_name 与 tags（每项 id、name）。已删除的标签组与标签不返回。用它了解企业有哪些客户标签，再配合 client 侧筛选或 externalcontact.contact.get 返回的跟进标签名做对照。最多返回企业标签库的全量内容（官方上限约数千条），标签库很大时建议让用户先说明关注哪个标签组。",
    Source = "IWechatWorkExternalContactTagService.GetCorpTagListAsync",
    RequiredScopes = ["externalcontact.tag.read"])]
public interface IWechatWorkExternalContactTagListTool
{
    /// <summary>读取企业客户标签库。</summary>
    /// <returns>白名单投影后的 JSON 文本（tag_group、group_id、group_name、tags）。</returns>
    Task<string> GetCorpTagListAsync(CancellationToken cancellationToken = default);
}
