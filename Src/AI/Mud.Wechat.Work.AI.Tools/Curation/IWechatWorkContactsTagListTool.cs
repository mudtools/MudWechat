// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.tag.list</c>：列出通讯录标签（无参数）。
/// </summary>
/// <remarks>
/// 这是<b>零参工具</b>的样本：Schema 的 <c>parameters</c> 为空对象。零参不等于「不需要授权」——
/// 白名单与授权门禁对它同样生效（见 <c>WechatToolDispatcher</c>）。
/// </remarks>
[AgentTool(
    name: "contacts.tag.list",
    Description = "列出企业微信通讯录标签。无参数。返回 taglist（tag_id/name）。自建应用与通讯录同步助手可见全部标签；第三方应用与代开发自建应用仅可见自己创建的标签。",
    Source = "IWechatWorkTagsService.GetTagListAsync",
    RequiredScopes = ["contacts.tag.read"],
    IsWrite = false)]
public interface IWechatWorkContactsTagListTool
{
    /// <summary>列出标签（无参数）。</summary>
    void Tags();
}
