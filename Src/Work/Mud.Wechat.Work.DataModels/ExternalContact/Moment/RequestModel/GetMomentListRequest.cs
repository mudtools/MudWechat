// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 获取企业全部的发表列表请求体（<c>/cgi-bin/externalcontact/get_moment_list</c>）。
/// <para>起止时间间隔不能超过 30 天；仅取 (start_time, end_time) 范围内的数据；
/// 已删除的朋友圈不会通过 API 返回。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class GetMomentListRequest
{
    /// <summary>
    /// 获取或设置朋友圈记录开始时间（Unix 时间戳，官方必填）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置朋友圈记录结束时间（Unix 时间戳，官方必填）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置朋友圈创建人的 userid（不填表示全部创建人）。
    /// </summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>
    /// 获取或设置朋友圈类型过滤：0 - 企业发表，1 - 个人发表，2 - 所有（默认为 2）。
    /// </summary>
    [JsonPropertyName("filter_type")]
    public int? FilterType { get; set; }

    /// <summary>
    /// 获取或设置用于分页查询的游标，由上一次调用返回，首次调用不填。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数（最大值为 20，默认为 20）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
