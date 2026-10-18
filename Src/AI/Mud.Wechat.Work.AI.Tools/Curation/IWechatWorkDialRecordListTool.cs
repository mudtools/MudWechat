// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>dial.record.list</c>：按时间范围读取公费电话拨打记录（主被叫与通话时长）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只自建口径</b>：官方本域仅向自建应用开放（代开发 / 第三方暂不支持），与打卡域同形——
/// 部署在代开发 / 第三方时该域软缺席（工具不入表）。
/// </para>
/// <para>
/// <b>被叫号码不外发</b>：<c>callee.phone</c> 是外部电话号码（个人数据），与成员域「手机号/邮箱不外发」
/// 同一条边界；只给被叫 <c>userid</c>（企业内标识）与通话时长。
/// </para>
/// </remarks>
[AgentTool(
    name: "dial.record.list",
    Description = "按时间范围读取企业微信公费电话拨打记录。参数 start_time / end_time 为时间范围（Unix 秒，如 \"1700000000\"；也接受带时区的 ISO 8601），双闭区间、跨度不得超过 30 天，两者需同时给出或同时省略（都省略时官方默认查最近 30 天）；offset 为分页偏移（默认 0）；limit 为单页条数（1~1000，默认 100）。返回 items（每项 call_time 拨打时间戳、total_duration 总时长（秒）、call_type 通话类型、caller 主叫 userid 与时长、callee 被叫 userid 与时长）、count 与 has_more（是否还有下一页）。不含被叫电话号码（外部号码不外发）；应用可见范围外的记录官方已过滤。仅自建应用可用（代开发与第三方官方暂不支持）。",
    Source = "IWechatWorkInternalDialService.GetDialRecordAsync",
    RequiredScopes = ["dial.record.read"])]
public interface IWechatWorkDialRecordListTool
{
    /// <summary>读取公费电话拨打记录。</summary>
    /// <returns>白名单投影后的 JSON 文本（record、count、has_more）。</returns>
    Task<string> GetDialRecordAsync(
        [ToolParameter("start_time", "起始时间（Unix 秒或带时区的 ISO 8601；与 end_time 同时给出或同时省略）。")] string? start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒或带时区的 ISO 8601；与 start_time 间隔 ≤ 30 天）。")] string? end_time,
        [ToolParameter("offset", "分页偏移（可空 ⇒ 0）。")] int? offset,
        [ToolParameter("limit", "单页条数（1~1000，可空 ⇒ 100）。")] int? limit,
        CancellationToken cancellationToken = default);
}
