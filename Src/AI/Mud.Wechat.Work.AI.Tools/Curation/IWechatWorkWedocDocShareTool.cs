// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>wedoc.doc.share</c>：获取微文档的分享链接。
/// </summary>
/// <remarks>
/// <para>
/// <b>「只是取链接」为什么按写处理</b>：分享链接一旦离开宿主（进入模型回复、日志、群聊）就等于<b>扩大了可见范围</b>——
/// 对外部世界而言不可收回。故按受控写处理（需白名单 + 授权 + 审批），而不是当作普通读取。
/// </para>
/// <para>
/// <b>docid / formid 二选一</b>：同 rename/delete，由执行器前置校验。
/// </para>
/// </remarks>
[AgentTool(
    name: "wedoc.doc.share",
    Description = "获取企业微信微文档或收集表的分享链接。参数 docid 与 formid 二选一（只能填其中一个）。仅能访问本应用创建的文档。返回 share_url——该链接会扩大文档可见范围，请谨慎转发。该操作按受控写处理：需宿主启用 + 授权通过 + 审批放行。",
    Source = "IWechatWorkWedocService.ShareDocumentAsync",
    RequiredScopes = ["wedoc.doc.share"],
    IsWrite = true)]
public interface IWechatWorkWedocDocShareTool
{
    /// <summary>获取分享链接。</summary>
    /// <param name="docid">文档 ID（与 formid 二选一）。</param>
    /// <param name="formid">收集表 ID（与 docid 二选一）。</param>
    void Share(string? docid, string? formid);
}
