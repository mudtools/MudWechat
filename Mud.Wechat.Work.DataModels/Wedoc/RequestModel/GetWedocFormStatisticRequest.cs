// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表的统计信息查询请求体（<c>/cgi-bin/wedoc/get_form_statistic</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：<c>start_time</c> / <c>end_time</c> 仅在拉取已提交列表（req_type = 2）时必填，其余类型不可传，
/// 筛选以整天为界（开始当天 00:00:00、结束当天 23:59:59）；<c>limit</c> 批次大小最大 10000；
/// <c>cursor</c> 分页游标首次不传。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetWedocFormStatisticRequest
{
    /// <summary>获取或设置操作的收集表周期 id（官方 <c>repeated_id</c>，必填），来源于「获取收集表信息」接口的返回。</summary>
    [JsonPropertyName("repeated_id")]
    public string? RepeatedId { get; set; }

    /// <summary>
    /// 获取或设置请求类型（官方 <c>req_type</c>，必填）。
    /// 官方取值：<c>1</c> 只获取统计结果、<c>2</c> 获取已提交列表、<c>3</c> 获取未提交列表。
    /// </summary>
    [JsonPropertyName("req_type")]
    public uint? ReqType { get; set; }

    /// <summary>获取或设置筛选开始时间（官方 <c>start_time</c>，拉取已提交列表时必填），以当天的 00:00:00 开始筛选。</summary>
    [JsonPropertyName("start_time")]
    public ulong? StartTime { get; set; }

    /// <summary>获取或设置筛选结束时间（官方 <c>end_time</c>，拉取已提交列表时必填），以当天的 23:59:59 结束筛选。</summary>
    [JsonPropertyName("end_time")]
    public ulong? EndTime { get; set; }

    /// <summary>获取或设置分页拉取的批次大小（官方 <c>limit</c>），最大 10000。</summary>
    [JsonPropertyName("limit")]
    public ulong? Limit { get; set; }

    /// <summary>获取或设置分页拉取的游标（官方 <c>cursor</c>），首次不传。</summary>
    [JsonPropertyName("cursor")]
    public ulong? Cursor { get; set; }
}
