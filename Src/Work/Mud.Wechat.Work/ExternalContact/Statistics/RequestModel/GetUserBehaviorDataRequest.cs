// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

/// <summary>
/// 获取「联系客户统计」数据请求体（<c>/cgi-bin/externalcontact/get_user_behavior_data</c>）。
/// <para>
/// <see cref="Userid"/> 与 <see cref="Partyid"/> 不可同时为空（传入多个 userid 时返回总体数据）；
/// 数据以天为维度，查询区间为闭区间，最大跨度 30 天，最多可取最近 180 天数据；
/// 非零点时间戳自动向下取整到当日 0 点。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GetUserBehaviorDataRequest
{
    /// <summary>
    /// 获取或设置成员 userid 列表（最多 100 个；与部门列表不可同时为空）。
    /// </summary>
    [JsonPropertyName("userid")]
    public List<string>? Userid { get; set; }

    /// <summary>
    /// 获取或设置部门 id 列表（最多 100 个；与成员列表不可同时为空）。
    /// </summary>
    [JsonPropertyName("partyid")]
    public List<long>? Partyid { get; set; }

    /// <summary>
    /// 获取或设置数据起始时间（官方必填；Unix 时间戳，非零点自动取整到当日 0 点）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置数据结束时间（官方必填；Unix 时间戳，与起始时间跨度最大 30 天，最多可查最近 180 天）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
