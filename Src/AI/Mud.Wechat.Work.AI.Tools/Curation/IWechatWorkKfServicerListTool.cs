// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>kf.servicer.list</c>：列出某客服账号的接待人员。
/// </summary>
/// <remarks>
/// <b>状态字段是重点</b>：官方 <c>status</c> 表达「接待中 / 停止接待」、<c>stop_type</c> 表达停接原因
/// （如「接待人数达到上限」），二者组合才能回答「为什么没人接」。工具描述里直接给出取值语义，避免模型臆测。
/// </remarks>
[AgentTool(
    name: "kf.servicer.list",
    Description = "列出指定微信客服账号的接待人员。参数 open_kfid 为客服账号 ID（先用 kf.account.list 获取）。返回 servicer_list，每项含 userid、status（0 接待中 / 1 停止接待）、stop_type（停止接待类型：0 单独停止 / 1 接待人数达上限 / 2 无在线接待人员 / 3 全部客服人员离线）、department_id（所属部门）。",
    Source = "IWechatWorkKfServicerService.GetServicerListAsync",
    RequiredScopes = ["kf.servicer.read"])]
public interface IWechatWorkKfServicerListTool
{
    /// <summary>列出客服账号的接待人员。</summary>
    /// <param name="open_kfid">客服账号 ID。</param>
    void List(string open_kfid);
}
