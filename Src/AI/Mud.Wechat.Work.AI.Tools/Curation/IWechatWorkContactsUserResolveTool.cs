// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.user.resolve</c>：把「人」的日常标识（手机号 / 邮箱 / 姓名）解析为可用的 <c>userid</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>它补的是真实断点，不是「多一个查询」</b>：所有写入类工具都以 <c>userid</c> 为目标，而人只会说
/// 「给张三发消息」。没有本工具时模型只有两条路：猜 ID（必然失败）或要求用户提供 ID（把定位工作退回给人）。
/// 这是写面在真实组织里最常卡住的一环。
/// </para>
/// <para>
/// <b>只收精确标识，不做模糊人名全局搜索</b>：手机号与邮箱在企微侧是<b>唯一</b>索引（服务端解析，一次调用）；
/// 姓名不是唯一键，也没有「按姓名搜全企业」的官方端点。按姓名解析必须给 <c>department_id</c>（在该部门内匹配），
/// 这是<b>刻意</b>的：不设「全企业扫描」兜底，避免一次调用拉全量通讯录（既撞频控，也把整本通讯录送进模型上下文）。
/// </para>
/// <para>
/// <b>不出参个人信息</b>：入参的手机号/邮箱是用户自己给出的，结果只回 <c>userid</c> 与姓名，
/// <b>不回显</b>手机号/邮箱——模型上下文与日志的留存期远长于一次请求。
/// </para>
/// </remarks>
[AgentTool(
    name: "contacts.user.resolve",
    Description = "把成员标识解析为 userid，供 message.send / 审批 / 日程等写工具使用。三选一：mobile（手机号，服务端唯一索引，最快）、email（邮箱）、name（姓名，必须同时给 department_id，在该部门内匹配）。返回 matches（每项 userid 与 name）、count；未命中时 count=0。不要用姓名猜 userid，也不要一次给多个标识。",
    Source = "IWechatWorkUsersService.GetUserIdByMobileAsync",
    RequiredScopes = ["contacts.user.read"],
    IsWrite = false)]
public interface IWechatWorkContactsUserResolveTool
{
    /// <summary>按手机号 / 邮箱 / 姓名解析成员。</summary>
    /// <returns>白名单投影后的 JSON 文本（matches、count）。</returns>
    Task<string> ResolveUserAsync(
        [ToolParameter("mobile", "手机号（与 email / name 三选一）。")] string? mobile,
        [ToolParameter("email", "邮箱（与 mobile / name 三选一）。")] string? email,
        [ToolParameter("name", "姓名（三选一；给姓名时必须同时给 department_id）。")] string? name,
        [ToolParameter("department_id", "部门 ID（按姓名解析时必填；仅在该部门（含子部门）内匹配）。")] int? department_id,
        CancellationToken cancellationToken = default);
}
