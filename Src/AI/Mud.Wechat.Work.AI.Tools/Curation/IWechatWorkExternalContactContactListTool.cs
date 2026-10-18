// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.contact.list</c>：列出某成员添加的客户（external_userid）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只给 ID，不给资料</b>：官方此端点仅返回 <c>external_userid</c> 列表；客户昵称、标签、跟进关系等
/// 需再按单个 ID 调 <c>externalcontact.contact.get</c>（官方无「批量详情」的公共读端点用于本形态）。
/// 因此本工具在描述里显式要求模型「按需逐个取详情」，避免它凭空编造客户信息。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.contact.list",
    Description = "列出指定成员添加的企业微信客户。参数 userid 为成员 UserID（先用 externalcontact.follow_user.list 获取）。返回 external_userid_list（客户 ID 数组）与 count；此端点只返回客户 ID，客户的昵称、标签与跟进关系需再用 externalcontact.contact.get 按单个 ID 获取。仅覆盖应用可见范围内该成员添加的客户，超出可见范围时官方返回空列表。",
    Source = "IWechatWorkExternalContactCustomerService.GetCustomerListAsync",
    RequiredScopes = ["externalcontact.contact.read"])]
public interface IWechatWorkExternalContactContactListTool
{
    /// <summary>列出某成员添加的客户 ID。</summary>
    /// <returns>白名单投影后的 JSON 文本（external_userid_list、count）。</returns>
    Task<string> GetCustomerListAsync(
        [ToolParameter("userid", "成员 UserID。", Required = true)] string userid,
        CancellationToken cancellationToken = default);
}
