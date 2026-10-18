// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.groupmsg.send</c>：创建企业群发任务（发给客户或客户群，受控写）。
/// </summary>
/// <remarks>
/// <para>
/// <b>外部可见、不可撤回</b>：群发直达客户的企业微信/微信。标为 <c>IsWrite = true</c>：默认白名单关闭，
/// 宿主须显式 <c>Enable</c> + 授权器 + 审批门才可执行；与 <c>message.send</c> 同一路径。
/// </para>
/// <para>
/// <b>目标集必须显式</b>：官方允许只给 <c>sender</c>（= 该成员的全部客户/全部群），本工具要求
/// 按模式给出 <c>external_userid</c>（客户）或 <c>chat_id_list</c>（群）——全量发送让模型先用只读面
/// （<c>contact.list</c> / <c>groupchat.list</c>）枚举，可核对、可分批。
/// </para>
/// <para>
/// <b>只发文本</b>：附件要 <c>media_id</c>（宿主素材资产，模型无法产出），故不开放附件入参；
/// 官方「文本与附件不可同时为空」在本工具里等价于 <c>content</c> 必填。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.groupmsg.send",
    Description = "创建企业微信客户群发任务（对外可见、不可撤回，成员确认后才真正送达）。参数 chat_type 必填（single 发给客户 / group 发给客户群）；sender 为发送成员 userid（必填，客户群发时官方强制）；content 为文本内容（必填，官方上限 4000 字节；本工具只发文本，附件需宿主素材 media_id 故不开放）；external_userid 为客户 ID 数组（chat_type=single 时必填，单次最多 1 万个，先用 externalcontact.contact.list 获取）；chat_id_list 为客户群 ID 数组（chat_type=group 时必填，单次最多 2000 个，先用 externalcontact.groupchat.list 获取）；allow_select 可选（true 允许成员发送前重新勾选客户，仅客户群发生效）。返回 msgid（群发消息 ID，可用于 externalcontact.groupmsg.task_list 查执行情况）与 fail_list（无效/无法发送的目标，官方即使部分失败也可能返回 errcode 0，必须逐一检查）。该操作为受控写：需宿主启用 + 授权通过 + 审批放行。",
    Source = "IWechatWorkExternalContactGroupMsgService.AddGroupMsgTemplateAsync",
    RequiredScopes = ["externalcontact.groupmsg.write"],
    IsWrite = true)]
public interface IWechatWorkExternalContactGroupMsgSendTool
{
    /// <summary>创建企业群发任务。</summary>
    /// <param name="chat_type">群发任务类型（single 发给客户 / group 发给客户群）。</param>
    /// <param name="sender">发送成员 userid（客户群发时官方强制）。</param>
    /// <param name="content">文本内容（≤ 4000 字节；本工具只发文本）。</param>
    /// <param name="external_userid">客户 ID 列表（chat_type = single 时必填；单次最多 1 万个）。</param>
    /// <param name="chat_id_list">客户群 ID 列表（chat_type = group 时必填；单次最多 2000 个）。</param>
    /// <param name="allow_select">是否允许成员发送前重新勾选客户（仅客户群发生效）。</param>
    void Send(
        [ToolParameter("chat_type", "群发任务类型（single 发给客户 / group 发给客户群）。", Required = true)] string chat_type,
        [ToolParameter("sender", "发送成员 userid（客户群发时官方强制）。", Required = true)] string sender,
        [ToolParameter("content", "文本内容（≤ 4000 字节；本工具只发文本）。", Required = true)] string content,
        string[]? external_userid,
        string[]? chat_id_list,
        bool? allow_select);
}
