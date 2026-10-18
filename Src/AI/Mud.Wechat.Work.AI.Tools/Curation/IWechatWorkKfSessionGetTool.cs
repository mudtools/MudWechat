// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>kf.session.get</c>：查询某客户与某客服账号的会话状态。
/// </summary>
/// <remarks>
/// <b>只读会话状态，不读聊天内容</b>：官方另有 <c>sync_msg</c> 拉取消息（含用户文本内容），属会话内容面，
/// 本工具不承接——「谁在接待 / 排在第几 / 是否结束」是可回答的业务事实，聊天正文不是。
/// </remarks>
[AgentTool(
    name: "kf.session.get",
    Description = "查询指定客户与指定微信客服账号之间的会话状态。参数 open_kfid 为客服账号 ID（先用 kf.account.list 获取）；external_userid 为客户 ID。返回 service_state（0 未处理 / 1 由智能助手接待 / 2 待接入池排队 / 3 由人工接待 / 4 已结束 / 5 待接入）与 service_state_desc（中文说明）；人工接待时另返回 servicer_userid。不返回聊天记录正文。",
    Source = "IWechatWorkKfSessionService.GetServiceStateAsync",
    RequiredScopes = ["kf.session.read"])]
public interface IWechatWorkKfSessionGetTool
{
    /// <summary>查询会话状态。</summary>
    /// <param name="open_kfid">客服账号 ID。</param>
    /// <param name="external_userid">客户 ID。</param>
    void Get(
        [ToolParameter("open_kfid", "客服账号 ID。", Required = true)] string open_kfid,
        [ToolParameter("external_userid", "客户 ID。", Required = true)] string external_userid);
}
