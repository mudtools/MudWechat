// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>openid 分页数据容器（<c>user/get</c> 与 <c>tags/members/getblacklist</c> 共用同一形态）。</summary>
/// <remarks>两个端点的官方 <c>data</c> 对象字段表逐字段一致（只有 <c>openid</c> 数组），故共用容器类型。</remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpOpenIdPageData
{
    /// <summary>openid 列表。</summary>
    [JsonPropertyName("openid")]
    public List<string>? OpenId { get; set; }
}

/// <summary>
/// 获取关注用户列表响应（<c>getFans</c>，<c>GET /cgi-bin/user/get</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>GET</b>，Query 参数 <c>next_openid</c>（上一批列表的最后一个 OPENID，不填默认从头拉取）；
/// 响应 <c>total</c>（关注总用户数）/ <c>count</c>（本批 OPENID 个数，<b>最大 10000</b>）/
/// <c>data.openid</c> / <c>next_openid</c>。
/// </para>
/// <para>
/// 官方「注意事项」原文：①「一次拉取调用最多拉取 <b>10000</b> 个关注者的 OpenID，可以通过多次拉取的方式来满足需求」；
/// ②「<b>最后一次返回时 <c>next_openid</c> 可能为空表示列表结束</b>」⇒ 翻页终止条件为
/// <c>next_openid</c> 为空。
/// </para>
/// <para>
/// 与 <c>getBlacklist</c> 的差异：本端点单批 10000（黑名单单批 1000）⇒ 故各自声明响应 DTO，
/// 仅共用 <c>data</c> 容器。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpGetFansResponse : MpResponse
{
    /// <summary>关注该公众号的总用户数。</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>本批拉取的 OPENID 个数（最大 10000）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>本批 openid 数据（为空通常表示该账号无关注者）。</summary>
    [JsonPropertyName("data")]
    public MpOpenIdPageData? Data { get; set; }

    /// <summary>本批列表后一个用户的 OPENID（<b>为空表示列表结束</b>）。</summary>
    [JsonPropertyName("next_openid")]
    public string? NextOpenId { get; set; }
}

/// <summary>
/// 获取公众号黑名单列表请求体（<c>getBlacklist</c>，<c>POST /cgi-bin/tags/members/getblacklist</c>）。
/// </summary>
/// <remarks>官方字段表：<c>begin_openid</c>（可选，起始 OpenID；为空时从开头拉取）。</remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpGetBlacklistRequest
{
    /// <summary>起始 OpenID（为空或 <c>null</c> 时从开头拉取）。</summary>
    [JsonPropertyName("begin_openid")]
    public string? BeginOpenId { get; set; }
}

/// <summary>
/// 获取公众号黑名单列表响应（<c>getBlacklist</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>total</c> / <c>count</c> / <c>data.openid</c> / <c>next_openid</c>。
/// </para>
/// <para>
/// 官方「注意事项」原文：①「每次最多拉取 <b>1000</b> 个 OpenID」；②「通过多次拉取满足需求」；
/// ③「<c>begin_openid</c> 为空时默认从头开始」。
/// </para>
/// <para>
/// 分页游标语义注意：本端点起始游标字段名为 <c>begin_openid</c>，而响应游标为 <c>next_openid</c>
/// （字段名不同，勿互相「对齐」）；<c>getFans</c> 的请求游标则与响应同名 <c>next_openid</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpGetBlacklistResponse : MpResponse
{
    /// <summary>黑名单用户总数。</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>本次返回的用户数（最多 1000）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>本批 openid 数据。</summary>
    [JsonPropertyName("data")]
    public MpOpenIdPageData? Data { get; set; }

    /// <summary>本次列表后一位 openid（下一页起始游标，回传为请求的 <c>begin_openid</c>）。</summary>
    [JsonPropertyName("next_openid")]
    public string? NextOpenId { get; set; }
}

/// <summary>
/// 拉黑 / 取消拉黑用户请求体（<c>batchBlacklist</c> 与 <c>batchUnblacklist</c> <b>共用</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>：官方两页请求体字段表一致（仅 <c>openid_list</c>），差异仅在语义（拉黑 / 取消）与
/// 数量上限表述（两页「注意事项」分别为「一次最多拉黑 20 个用户」「一次最多取消 20 个用户」）。
/// 拆成两个同构类属纯冗余（与标签域 <c>MpTagMembersRequest</c> 同一处置）。
/// </para>
/// <para>响应仅 <c>errcode</c>/<c>errmsg</c>（由 <see cref="MpResponse"/> 承载，故不设专用响应 DTO）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpBlacklistRequest
{
    /// <summary>需要拉黑 / 取消拉黑的 openid 列表（<b>单次最多 20 个</b>）。</summary>
    [JsonPropertyName("openid_list")]
    public List<string> OpenIdList { get; set; } = new();
}
