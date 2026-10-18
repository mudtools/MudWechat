// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.moment.comments</c>：读取某条朋友圈的评论与点赞名单。
/// </summary>
/// <remarks>
/// <para>
/// <b>userid 是必填的第二段钥匙</b>：官方按「朋友圈 + 发表成员」定位互动数据；企业发表的朋友圈其发表成员须先从
/// <c>externalcontact.moment.publish_list</c> 取得，个人发表的朋友圈即创建人。工具在描述里写明这条自举路径，
/// 不让模型猜「为什么单给 moment_id 会报错」。
/// </para>
/// <para>
/// <b>只给互动者身份与时间</b>：官方此端点不返回评论正文，工具也不臆造；客户互动给 <c>external_userid</c>，
/// 企业成员互动给 <c>userid</c>，两者不会同时出现（用 <c>actor</c> 区分来源）。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.moment.comments",
    Description = "获取某条企业微信客户朋友圈的评论与点赞名单。参数 moment_id 为朋友圈 ID（先用 externalcontact.moment.list 获取）；userid 为发表该朋友圈的成员 userid（必填：个人朋友圈即创建人 userid；企业发表的朋友圈须先用 externalcontact.moment.publish_list 取到执行成员 userid）。返回 comment_list / like_list（每项 actor 为 member 或 customer、对应 userid 或 external_userid、create_time）与 comment_count、like_count。官方此端点不返回评论正文，只给互动者身份与时间。",
    Source = "IWechatWorkExternalContactMomentService.GetMomentCommentsAsync",
    RequiredScopes = ["externalcontact.moment.read"])]
public interface IWechatWorkExternalContactMomentCommentsTool
{
    /// <summary>读取朋友圈互动名单。</summary>
    /// <returns>白名单投影后的 JSON 文本（comment_list、like_list、comment_count、like_count）。</returns>
    Task<string> GetMomentCommentsAsync(
        [ToolParameter("moment_id", "朋友圈 ID（先用 externalcontact.moment.list 获取）。", Required = true)] string moment_id,
        [ToolParameter("userid", "发表该朋友圈的成员 userid（企业发表的先经 externalcontact.moment.publish_list 取得）。", Required = true)] string userid,
        CancellationToken cancellationToken = default);
}
