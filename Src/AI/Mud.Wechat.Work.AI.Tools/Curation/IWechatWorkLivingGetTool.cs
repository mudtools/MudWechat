// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>living.get</c>：按直播 ID 读取直播详情。
/// </summary>
/// <remarks>
/// <b>推流地址不外发</b>：官方 <c>LivingDetail.push_stream_url</c> 是主播推流凭证，拿到即可向该直播推流；
/// 与工具面「结果进提示词即不可撤回」的前提冲突，执行器不投影。
/// </remarks>
[AgentTool(
    name: "living.get",
    Description = "按直播 ID 获取企业微信直播详情。参数 livingid 为直播 ID（先用 living.list 获取）。返回 theme（直播主题）、living_start（开始时间戳）、living_duration（时长秒）、status（0 预约中 / 1 直播中 / 2 已结束 / 3 已过期 / 4 已取消）、anchor_userid（主播）、description、viewer_num（观看人数）、comment_num（评论数）、mic_num（连麦数）、online_count（当前在线）、subscribe_count（预约人数）、replay_status（回放状态）与 type。不含推流地址（推流凭证不外发）。",
    Source = "IWechatWorkLivingService.GetLivingInfoAsync",
    RequiredScopes = ["living.read"])]
public interface IWechatWorkLivingGetTool
{
    /// <summary>读取直播详情。</summary>
    /// <returns>白名单投影后的 JSON 文本（theme、living_start、living_duration、status、anchor_userid、description、viewer_num、comment_num、mic_num、online_count、subscribe_count、replay_status、type）。</returns>
    Task<string> GetLivingInfoAsync(
        [ToolParameter("livingid", "直播 ID。", Required = true)] string livingid,
        CancellationToken cancellationToken = default);
}
