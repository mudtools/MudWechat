// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.moment.list</c>：按时间范围拉取客户朋友圈发表记录（企业发表 + 个人发表）。
/// </summary>
/// <remarks>
/// <para>
/// <b>时间范围是入口而非可选项</b>：官方要求必填起止时间且跨度不超过 30 天；工具在执行器里先做跨度校验并回填可读错误，
/// 免得模型把官方错误码当成「没数据」。
/// </para>
/// <para>
/// <b>只投影可读内容</b>：文本、图片张数、有无视频、图文标题与链接、地点名；<c>media_id</c>（宿主素材资产）与
/// 经纬度（行踪）不投影。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.moment.list",
    Description = "按时间范围拉取企业微信客户朋友圈发表记录。参数 start_time / end_time 为时间范围（Unix 秒，如 \"1700000000\"；也接受带时区的 ISO 8601，如 \"2026-10-10T09:00:00+08:00\"），跨度不得超过 30 天且 end_time 必须大于 start_time；creator 可选（限定发表人 userid，不填为全部）；filter_type 可选（CorpPublish 企业发表 / PersonalPublish 个人发表 / All 全部，默认 All）；cursor 为分页游标（首页省略，续页传上一页的 page_token）；limit 为单页条数（1~20，默认 20）。返回 items（每项 moment_id、creator、create_time、create_type 0 企业 / 1 个人、visible_type 0 部分可见 / 1 公开、text 文本内容、image_count 图片张数、has_video 是否含视频、link 图文标题与链接、location 地点名）、count 与 page_token。已删除的朋友圈官方不返回；不含图片/视频的 media_id（宿主素材资产）与经纬度（行踪不外发）。",
    Source = "IWechatWorkExternalContactMomentService.GetMomentListAsync",
    RequiredScopes = ["externalcontact.moment.read"])]
public interface IWechatWorkExternalContactMomentListTool
{
    /// <summary>拉取朋友圈发表记录。</summary>
    /// <returns>白名单投影后的 JSON 文本（items、count、page_token）。</returns>
    Task<string> GetMomentListAsync(
        [ToolParameter("start_time", "起始时间（Unix 秒或带时区的 ISO 8601）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒或带时区的 ISO 8601；与起始间隔 ≤ 30 天）。", Required = true)] string end_time,
        [ToolParameter("creator", "发表人 userid（可空 ⇒ 全部）。")] string? creator,
        [ToolParameter("filter_type", "类型过滤（CorpPublish 企业发表 / PersonalPublish 个人发表 / All 全部；可空 ⇒ All）。", EnumType = typeof(WechatMomentFilterTypes))] int? filter_type,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        [ToolParameter("limit", "单页条数（1~20，可空 ⇒ 20）。")] int? limit,
        CancellationToken cancellationToken = default);
}
