// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>living.list</c>：拉取某成员创建的直播 ID 列表。
/// </summary>
/// <remarks>
/// <b>它是直播域的入口</b>：直播详情与观看统计都要求 <c>livingid</c>，模型先按成员翻页拿 ID。
/// </remarks>
[AgentTool(
    name: "living.list",
    Description = "拉取指定成员创建的微信直播 ID 列表。参数 userid 为成员 UserID（先用 contacts.user.simple_list 获取）；cursor 为分页游标（首页省略）；limit 为单页条数（1~100，可空 ⇒ 100）。返回 livingid_list、count 与 next_cursor（为空表示已取完）。拿到 livingid 后用 living.get 取详情、living.watch_stat 取观看统计。只能拉取本应用创建的直播。",
    Source = "IWechatWorkLivingService.GetUserAllLivingIdAsync",
    RequiredScopes = ["living.read"])]
public interface IWechatWorkLivingListTool
{
    /// <summary>拉取成员直播 ID 列表。</summary>
    /// <param name="userid">成员 UserID。</param>
    /// <param name="cursor">分页游标（可空 ⇒ 首页）。</param>
    /// <param name="limit">单页条数（1~100，可空 ⇒ 100）。</param>
    void List(string userid, string? cursor, int? limit);
}
