// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 批量获取审批单号请求体（<c>/cgi-bin/oa/getapprovalinfo</c>）。
/// </summary>
/// <remarks>
/// <para>官方说明：一次拉取调用最多拉取 100 个审批记录；老的分页游标字段 cursor 和 next_cursor 待废弃，请使用新字段 new_cursor 和 new_next_cursor；推荐使用此接口获取审批数据，旧接口（获取审批数据（旧））后续将不再维护。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class BatchGetApprovalNumbersRequest
{
    /// <summary>
    /// 获取或设置审批单提交的时间范围开始时间（Unix时间戳，官方必填）。
    /// </summary>
    /// <remarks>
    /// <para>官方请求示例按字符串传输（如 "1569546000"），故本模型以字符串承载。</para>
    /// </remarks>
    [JsonPropertyName("starttime")]
    public string? Starttime { get; set; }

    /// <summary>
    /// 获取或设置审批单提交的时间范围结束时间（Unix时间戳，官方必填）。
    /// </summary>
    /// <remarks>
    /// <para>官方请求示例按字符串传输；需大于 starttime，起始时间跨度不能超过 31 天。</para>
    /// </remarks>
    [JsonPropertyName("endtime")]
    public string? Endtime { get; set; }

    /// <summary>
    /// 获取或设置分页查询游标（官方必填），默认为空串，后续使用返回的 new_next_cursor 进行分页拉取。
    /// </summary>
    [JsonPropertyName("new_cursor")]
    public string? NewCursor { get; set; }

    /// <summary>
    /// 获取或设置一次请求拉取审批单数量（官方必填），默认值为 100，上限值为 100；返回的 sp_no_list 个数可能和 size 不一致，开发者需用 next_cursor 判断表单记录是否拉取完。
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    /// <summary>
    /// 获取或设置筛选条件，可对批量拉取的审批申请设置约束条件，支持设置多个条件。
    /// </summary>
    [JsonPropertyName("filters")]
    public List<ApprovalInfoFilter>? Filters { get; set; }
}
