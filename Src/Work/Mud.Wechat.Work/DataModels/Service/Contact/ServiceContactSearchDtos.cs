// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Service;

/// <summary>
/// 通讯录搜索请求体（<c>/cgi-bin/service/contact/search</c>，官方 91844）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class SearchServiceContactsRequest
{
    /// <summary>
    /// 获取或设置查询的企业 corpid（<c>auth_corpid</c>，必填）。
    /// </summary>
    [JsonPropertyName("auth_corpid")]
    public string? AuthCorpId { get; set; }

    /// <summary>
    /// 获取或设置搜索关键词（<c>query_word</c>，必填，最长 64 个 UTF-8 字符）。
    /// <para>查询用户时应为用户名称、名称拼音或者英文名；查询部门时应为部门名称或者部门名称拼音。</para>
    /// </summary>
    [JsonPropertyName("query_word")]
    public string? QueryWord { get; set; }

    /// <summary>
    /// 获取或设置查询类型（<c>query_type</c>）：1 查询用户、2 查询部门；
    /// 不填或填 0 代表同时查询部门跟用户。
    /// </summary>
    [JsonPropertyName("query_type")]
    public int? QueryType { get; set; }

    /// <summary>
    /// 获取或设置查询范围（<c>query_range</c>，仅查询类型包含用户时有效）：
    /// 0 只查询在职用户、1 同时查询在职和离职用户（离职用户仅当离职前有激活企业微信才可被搜到）。
    /// </summary>
    [JsonPropertyName("query_range")]
    public int? QueryRange { get; set; }

    /// <summary>
    /// 获取或设置应用 id（<c>agentid</c>）：若非 0 则只返回应用可见范围内的用户或者部门信息。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }

    /// <summary>
    /// 获取或设置查询返回的最大数量（<c>limit</c>，默认 50、最多 200）。
    /// <para>返回数量可能小于该值，且 <c>limit</c> 会分别控制在职数据与离职数据的数量。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// 获取或设置精确匹配的字段（<c>full_match_field</c>）：1 匹配用户名称或部门名称、
    /// 2 匹配用户英文名；不填为模糊匹配。
    /// </summary>
    [JsonPropertyName("full_match_field")]
    public int? FullMatchField { get; set; }

    /// <summary>
    /// 获取或设置分页游标（<c>cursor</c>，由上一次调用返回，首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}

/// <summary>
/// 通讯录搜索响应体（官方 91844）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class SearchServiceContactsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置是否为最后一页（<c>is_last</c>）：为 <c>false</c> 时须用 limit + cursor 继续调用。
    /// </summary>
    [JsonPropertyName("is_last")]
    public bool? IsLast { get; set; }

    /// <summary>
    /// 获取或设置查询结果（<c>query_result</c>）。
    /// </summary>
    [JsonPropertyName("query_result")]
    public ServiceContactQueryResult? QueryResult { get; set; }

    /// <summary>
    /// 获取或设置下一页游标（<c>next_cursor</c>；无更多数据时返回空）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// 通讯录查询结果（<c>query_result</c>，官方 91844）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class ServiceContactQueryResult
{
    /// <summary>
    /// 获取或设置在职用户列表（<c>user</c>，按用户名称/拼音匹配）。
    /// </summary>
    [JsonPropertyName("user")]
    public IList<ServiceContactUser>? User { get; set; }

    /// <summary>
    /// 获取或设置部门列表（<c>party</c>，按部门名称/拼音匹配）。
    /// </summary>
    [JsonPropertyName("party")]
    public IList<ServiceContactParty>? Party { get; set; }

    /// <summary>
    /// 获取或设置离职用户列表（<c>dismiss_user</c>；仅 <c>query_range = 1</c> 时出现，形状同 <see cref="User"/>）。
    /// </summary>
    [JsonPropertyName("dismiss_user")]
    public IList<ServiceContactUser>? DismissUser { get; set; }
}

/// <summary>
/// 通讯录搜索命中的用户（官方 91844 的 <c>user</c> / <c>dismiss_user</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class ServiceContactUser
{
    /// <summary>
    /// 获取或设置查询到的用户 userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置查询到的用户 open_userid。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }
}

/// <summary>
/// 通讯录搜索命中的部门（官方 91844 的 <c>party</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class ServiceContactParty
{
    /// <summary>
    /// 获取或设置返回的部门 id。
    /// </summary>
    [JsonPropertyName("department_id")]
    public long? DepartmentId { get; set; }
}

/// <summary>
/// 批量通讯录搜索请求体（<c>/cgi-bin/service/contact/batchsearch</c>，官方 91844）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class BatchSearchServiceContactsRequest
{
    /// <summary>
    /// 获取或设置查询的企业 corpid（<c>auth_corpid</c>，必填）。
    /// </summary>
    [JsonPropertyName("auth_corpid")]
    public string? AuthCorpId { get; set; }

    /// <summary>
    /// 获取或设置应用 id（<c>agentid</c>）：若非 0 则只返回应用可见范围内的用户或者部门信息。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }

    /// <summary>
    /// 获取或设置搜索请求列表（<c>query_request_list</c>，必填，每次不超过 50 条）。
    /// </summary>
    [JsonPropertyName("query_request_list")]
    public IList<ServiceContactQueryRequest>? QueryRequestList { get; set; }
}

/// <summary>
/// 批量搜索的单个请求（<c>query_request_list</c> 元素，官方 91844）。
/// </summary>
/// <remarks>
/// 与单条搜索请求体相比官方<b>不接收</b> <c>auth_corpid</c> / <c>agentid</c>（二者提到批量请求外层）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class ServiceContactQueryRequest
{
    /// <summary>
    /// 获取或设置搜索关键词（<c>query_word</c>，必填，最长 64 个 UTF-8 字符）。
    /// </summary>
    [JsonPropertyName("query_word")]
    public string? QueryWord { get; set; }

    /// <summary>
    /// 获取或设置查询类型（<c>query_type</c>）：1 用户、2 部门；不填或 0 代表同时查询。
    /// </summary>
    [JsonPropertyName("query_type")]
    public int? QueryType { get; set; }

    /// <summary>
    /// 获取或设置查询范围（<c>query_range</c>）：0 只在职、1 在职 + 离职。
    /// </summary>
    [JsonPropertyName("query_range")]
    public int? QueryRange { get; set; }

    /// <summary>
    /// 获取或设置该条请求返回的最大数量（<c>limit</c>，默认 50、最多 200）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// 获取或设置精确匹配字段（<c>full_match_field</c>）：1 名称、2 英文名；不填为模糊匹配。
    /// </summary>
    [JsonPropertyName("full_match_field")]
    public int? FullMatchField { get; set; }

    /// <summary>
    /// 获取或设置该条请求的分页游标（<c>cursor</c>，由上一次调用返回，首次可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}

/// <summary>
/// 批量通讯录搜索响应体（官方 91844）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class BatchSearchServiceContactsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置搜索结果列表（<c>query_result_list</c>，与请求列表逐项对应）。
    /// </summary>
    [JsonPropertyName("query_result_list")]
    public IList<ServiceContactBatchQueryItem>? QueryResultList { get; set; }
}

/// <summary>
/// 批量搜索的单个结果（<c>query_result_list</c> 元素，官方 91844）。
/// </summary>
/// <remarks>
/// <b>官方逐请求错误形态</b>：某条请求失败时错误落在<b>本元素</b>的 <c>errcode</c> / <c>errmsg</c>，
/// 外层响应 <c>errcode</c> 仍为 0 ⇒ 调用方须逐项判定，不得只看外层返回码。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class ServiceContactBatchQueryItem
{
    /// <summary>
    /// 获取或设置原搜索请求报文（<c>query_request</c>）。
    /// </summary>
    [JsonPropertyName("query_request")]
    public ServiceContactQueryRequest? QueryRequest { get; set; }

    /// <summary>
    /// 获取或设置该请求对应的查询结果（<c>query_result</c>）。
    /// </summary>
    [JsonPropertyName("query_result")]
    public ServiceContactQueryResult? QueryResult { get; set; }

    /// <summary>
    /// 获取或设置该条请求的下一页游标（<c>next_cursor</c>）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置该条请求是否已到最后一页（<c>is_last</c>）。
    /// </summary>
    [JsonPropertyName("is_last")]
    public bool? IsLast { get; set; }

    /// <summary>
    /// 获取或设置该条请求的返回码（<c>errcode</c>；请求失败时非 0）。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int? ErrCode { get; set; }

    /// <summary>
    /// 获取或设置该条请求的返回码描述（<c>errmsg</c>）。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}
