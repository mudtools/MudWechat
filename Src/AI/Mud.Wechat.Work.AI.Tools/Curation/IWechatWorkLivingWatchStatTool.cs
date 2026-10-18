// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任，我们不承担任何责任。
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>living.watch_stat</c>：读取直播的企业内观看明细。
/// </summary>
/// <remarks>
/// <para>
/// <b>只给企业内成员，不给外部联系人</b>：官方统计里 <c>external_users</c> 是外部联系人观看记录（含其
/// <c>external_userid</c> 与邀请关系），属企业外部自然人行为数据；执行器只回填成员明细与外部观看<b>人数</b>。
/// </para>
/// <para>
/// <b>分页靠 page_token</b>：官方以 <c>ending</c>（0 还有 / 1 已无）与 <c>next_key</c> 翻页，投影时归一为
/// <c>has_more</c>（false 还有 / true 已无）与 <c>page_token</c>，与其他域同形。
/// </para>
/// </remarks>
[AgentTool(
    name: "living.watch_stat",
    Description = "获取企业微信直播的企业内成员观看统计。参数 livingid 为直播 ID（先用 living.list 获取）；page_token 为分页键（首页省略，续页**原样回传上一页返回的 page_token**）。返回 items（成员 userid、watch_time 观看秒数、is_comment 是否评论、is_mic 是否连麦）、count、external_user_count（外部联系人观看人数，仅计数）以及 has_more（false 还有数据 / true 已无数据）与 page_token（有下一页时才出现）。不含外部联系人的 ID 与邀请关系（外部自然人数据不外发）。",
    Source = "IWechatWorkLivingService.GetWatchStatAsync",
    RequiredScopes = ["living.read"])]
public interface IWechatWorkLivingWatchStatTool
{
    /// <summary>读取直播观看统计。</summary>
    /// <returns>白名单投影后的 JSON 文本（items、userid、watch_time、is_comment、is_mic、count、external_user_count、has_more、page_token）。</returns>
    Task<string> GetWatchStatAsync(
        [ToolParameter("livingid", "直播 ID。", Required = true)] string livingid,
        [ToolParameter("page_token", "分页键（可空 ⇒ 首页；续页原样回传上一页返回的 page_token）。")] string? page_token,
        CancellationToken cancellationToken = default);
}
