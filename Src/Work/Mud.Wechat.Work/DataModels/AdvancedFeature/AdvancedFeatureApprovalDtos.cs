// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AdvancedFeature;

/// <summary>
/// 设置高级功能申请单审批信息请求体（<c>/cgi-bin/advanced_feature/set_approval_detail</c>，官方 99880）。
/// </summary>
/// <remarks>
/// <b>覆盖式写操作</b>：官方明确「如果需要变更审批节点信息，需要<b>全量</b>节点都传入」⇒ 本请求是
/// 全量覆盖而非增量修补；且审批 id 与申请 id <b>一一对应、不可改变</b>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedFeature")]
public class SetAdvancedFeatureApprovalDetailRequest
{
    /// <summary>
    /// 获取或设置申请 id（<c>apply_id</c>，必填）。
    /// </summary>
    [JsonPropertyName("apply_id")]
    public string? ApplyId { get; set; }

    /// <summary>
    /// 获取或设置审批 id（<c>approval_id</c>，必填）。
    /// <para>官方约束：应用生成审批 id 后，审批 id 和申请 id 是一一对应的，<b>不可改变</b>。</para>
    /// </summary>
    [JsonPropertyName("approval_id")]
    public string? ApprovalId { get; set; }

    /// <summary>
    /// 获取或设置审批单状态（<c>approval_status</c>，必填）：1 审批中、2 已驳回、3 已同意、101 已撤销。
    /// </summary>
    [JsonPropertyName("approval_status")]
    public int? ApprovalStatus { get; set; }

    /// <summary>
    /// 获取或设置审批单跳转链接（<c>approval_url</c>，必填）。
    /// <para>官方约束：须以 <c>http://</c> 或 <c>https://</c> 开头。</para>
    /// </summary>
    [JsonPropertyName("approval_url")]
    public string? ApprovalUrl { get; set; }

    /// <summary>
    /// 获取或设置审批节点集合（<c>process_list</c>，必填）。
    /// <para>官方约束：变更节点信息时须传入<b>全量</b>节点。</para>
    /// </summary>
    [JsonPropertyName("process_list")]
    public AdvancedFeatureApprovalProcessList? ProcessList { get; set; }
}

/// <summary>
/// 审批节点容器（<c>process_list</c>，官方 99880）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedFeature")]
public class AdvancedFeatureApprovalProcessList
{
    /// <summary>
    /// 获取或设置审批节点列表（<c>node_list</c>，必填）。
    /// </summary>
    [JsonPropertyName("node_list")]
    public IList<AdvancedFeatureApprovalNode>? NodeList { get; set; }
}

/// <summary>
/// 审批节点（<c>process_list.node_list</c> 元素，官方 99880）。
/// </summary>
/// <remarks>
/// <b>官方互斥约束</b>：待处理人列表与已处理人列表<b>不可同时为空</b>（各自最多 100 个）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedFeature")]
public class AdvancedFeatureApprovalNode
{
    /// <summary>
    /// 获取或设置审批节点状态（<c>node_apv_status</c>，必填）：1 审批中、2 已驳回、3 已同意、
    /// 101 已撤销、102 未到流程。
    /// </summary>
    [JsonPropertyName("node_apv_status")]
    public int? NodeApprovalStatus { get; set; }

    /// <summary>
    /// 获取或设置多人审批方式（<c>node_apv_rel</c>，必填）：1 会签、2 或签、3 依次审批。
    /// </summary>
    [JsonPropertyName("node_apv_rel")]
    public int? NodeApprovalRelation { get; set; }

    /// <summary>
    /// 获取或设置当前审批节点待处理人列表（<c>current_approvers</c>，最多 100 个）。
    /// </summary>
    [JsonPropertyName("current_approvers")]
    public IList<string>? CurrentApprovers { get; set; }

    /// <summary>
    /// 获取或设置当前审批节点已处理人列表（<c>completed_approvers</c>，最多 100 个）。
    /// </summary>
    [JsonPropertyName("completed_approvers")]
    public IList<string>? CompletedApprovers { get; set; }

    /// <summary>
    /// 获取或设置审批节点更新时间（<c>apv_update_time</c>）。
    /// </summary>
    [JsonPropertyName("apv_update_time")]
    public long? ApprovalUpdateTime { get; set; }
}

/// <summary>
/// 批量获取高级功能申请单 id 列表请求体（<c>/cgi-bin/advanced_feature/get_apply_id_list</c>，官方 99883）。
/// </summary>
/// <remarks>
/// 官方即 POST 的查询接口；企业自建应用专用（代开发 / 第三方官方标注「暂不支持」）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedFeature")]
public class GetAdvancedFeatureApplyIdListRequest
{
    /// <summary>
    /// 获取或设置申请的高级账号类型（<c>business_type</c>，必填）：1 邮件、2 文档、3 微盘、4 会议。
    /// </summary>
    [JsonPropertyName("business_type")]
    public int? BusinessType { get; set; }

    /// <summary>
    /// 获取或设置申请的成员 userid（<c>userid</c>，必填）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置分页数据上限（<c>limit</c>，默认 100、最大 200）。
    /// <para>官方约束：不保证每次返回的数据刚好为指定 limit，须用返回的 <c>has_more</c> 判断是否继续请求。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// 获取或设置分页游标（<c>cursor</c>，由上一次调用返回，首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置申请单来源过滤（<c>req_type</c>，默认 0）：0 所有、1 仅 api 单、2 非 api 申请单。
    /// </summary>
    [JsonPropertyName("req_type")]
    public int? RequestType { get; set; }
}

/// <summary>
/// 高级功能申请单 id 列表响应体（官方 99883）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedFeature")]
public class GetAdvancedFeatureApplyIdListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置下一页游标（<c>next_cursor</c>）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置申请 id 列表（<c>apply_id_list</c>）。
    /// </summary>
    [JsonPropertyName("apply_id_list")]
    public IList<string>? ApplyIdList { get; set; }

    /// <summary>
    /// 获取或设置是否还有下一页数据（<c>has_more</c>，官方样本为<b>布尔</b> true）。
    /// <para>注意与「高级接口订单列表」的同名字段形态不同（那里是整型 0/1），照抄各自原文、不做归一。</para>
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }
}

/// <summary>
/// 获取高级功能申请单详情请求体（<c>/cgi-bin/advanced_feature/get_approval_info</c>，官方 99885）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedFeature")]
public class GetAdvancedFeatureApprovalInfoRequest
{
    /// <summary>
    /// 获取或设置申请 id（<c>apply_id</c>，必填）。
    /// </summary>
    [JsonPropertyName("apply_id")]
    public string? ApplyId { get; set; }
}

/// <summary>
/// 高级功能申请单详情响应体（官方 99885）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedFeature")]
public class GetAdvancedFeatureApprovalInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置申请单详情（<c>approval_info</c>）。
    /// </summary>
    [JsonPropertyName("approval_info")]
    public AdvancedFeatureApprovalInfo? ApprovalInfo { get; set; }
}

/// <summary>
/// 高级功能申请单详情（<c>approval_info</c>，官方 99885）。
/// </summary>
/// <remarks>
/// <b>状态枚举与设置面不同</b>：查询面为 0 历史申请记录（无流程信息）、1 审批人审批中、2 审批人驳回、
/// 3 审批人已同意、4 管理员驳回、5 管理员已分配 —— 与设置面的 1/2/3/101 <b>不是一套值域</b>，
/// 故不复用同一枚举常量。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedFeature")]
public class AdvancedFeatureApprovalInfo
{
    /// <summary>
    /// 获取或设置申请人 userid（<c>applicant</c>）。
    /// </summary>
    [JsonPropertyName("applicant")]
    public string? Applicant { get; set; }

    /// <summary>
    /// 获取或设置创建时间戳（<c>create_time</c>）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置申请的高级账号类型（<c>business_type</c>）：1 邮件、2 文档、3 微盘、4 会议。
    /// </summary>
    [JsonPropertyName("business_type")]
    public int? BusinessType { get; set; }

    /// <summary>
    /// 获取或设置申请 id（<c>apply_id</c>）。
    /// </summary>
    [JsonPropertyName("apply_id")]
    public string? ApplyId { get; set; }

    /// <summary>
    /// 获取或设置自建应用中的审批 id（<c>approval_id</c>）。
    /// </summary>
    [JsonPropertyName("approval_id")]
    public string? ApprovalId { get; set; }

    /// <summary>
    /// 获取或设置自建应用中的审批单跳转链接（<c>approval_url</c>）。
    /// </summary>
    [JsonPropertyName("approval_url")]
    public string? ApprovalUrl { get; set; }

    /// <summary>
    /// 获取或设置审批单状态（<c>approval_status</c>）：0 历史申请记录（无流程信息）、1 审批人审批中、
    /// 2 审批人驳回、3 审批人已同意、4 管理员驳回、5 管理员已分配。
    /// </summary>
    [JsonPropertyName("approval_status")]
    public int? ApprovalStatus { get; set; }

    /// <summary>
    /// 获取或设置申请单类型（<c>approval_type</c>）：0 默认申请、1 自定义审批、2 企业 openapi。
    /// </summary>
    [JsonPropertyName("approval_type")]
    public int? ApprovalType { get; set; }

    /// <summary>
    /// 获取或设置申请原因（<c>request_reason</c>）。
    /// <para>属成员填写的自由文本（个人信息），不得写入日志、遥测或异常消息。</para>
    /// </summary>
    [JsonPropertyName("request_reason")]
    public string? RequestReason { get; set; }
}
