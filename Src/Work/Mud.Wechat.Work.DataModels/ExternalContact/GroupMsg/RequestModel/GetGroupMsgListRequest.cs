// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 获取群发记录列表请求体（<c>/cgi-bin/externalcontact/get_groupmsg_list_v2</c>）。
/// <para><see cref="ChatType"/> / <see cref="StartTime"/> / <see cref="EndTime"/> 为官方必填；
/// 起止时间间隔不能超过 1 个月。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GetGroupMsgListRequest
{
    /// <summary>
    /// 获取或设置群发任务类型（官方必填）：single - 发给客户（默认），group - 发给客户群。
    /// </summary>
    [JsonPropertyName("chat_type")]
    public string? ChatType { get; set; }

    /// <summary>
    /// 获取或设置群发任务记录的开始时间（官方必填；与结束时间间隔不能超过 1 个月）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置群发任务记录的结束时间（官方必填；与开始时间间隔不能超过 1 个月）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置群发任务创建人的企业账号 id（不填表示全部创建人）。
    /// </summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>
    /// 获取或设置创建人类型过滤：0 - 企业创建，1 - 个人创建，2 - 所有（默认为 2）。
    /// </summary>
    [JsonPropertyName("filter_type")]
    public int? FilterType { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数（最大值为 100，默认为 50，超限取默认值）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// 获取或设置用于分页查询的游标，由上一次调用返回，首次调用可不填。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}
