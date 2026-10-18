// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.tag.create</c>：创建一个通讯录标签（<b>L2 受控写</b>的首个样本）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么首个写工具选「建标签」</b>：它是<b>可逆、无外部副作用</b>的写（不通知任何人、可删除、影响面限于通讯录标签），
/// 适合先把「白名单默认封闭 → 授权 → 审批 → 执行 → 结果回填」整条写路径跑通；消息发送、成员增删等
/// 有外部感知或不可逆的写，应在该路径稳定后再逐个开放。
/// </para>
/// <para>
/// <b>IsWrite = true 的后果</b>：该工具进入生成物的 <c>WechatToolNames.WriteAll</c>，默认<b>不在白名单内</b>
/// （<c>WechatToolPolicy</c> 只装只读面）；未显式启用时模型甚至看不到它（投影按白名单）。
/// </para>
/// </remarks>
[AgentTool(
    name: "contacts.tag.create",
    Description = "创建一个企业微信通讯录标签。参数 tag_name 为标签名（≤32 个字，汉字或英文字母，不可与其他标签重名，必填）；tag_id 可选（非负整数，指定则按该 ID 创建，省略则按当前最大 ID 自增）。返回新建标签的 tag_id 与名称。标签总数上限 3000。该操作为受控写：需宿主启用 + 授权通过 + 审批放行。",
    Source = "IWechatWorkTagsService.CreateTagAsync",
    RequiredScopes = ["contacts.tag.write"],
    IsWrite = true)]
public interface IWechatWorkContactsTagCreateTool
{
    /// <summary>创建标签。</summary>
    /// <param name="tag_name">标签名（≤32 字，不可重名）。</param>
    /// <param name="tag_id">指定标签 ID（可空，非负整数）。</param>
    void Create([ToolParameter("tag_name", "标签名（≤32 字，不可重名）。", Required = true)] string tag_name, [ToolParameter("tag_id", "指定标签 ID（可空，非负整数）。")] int? tag_id);
}
