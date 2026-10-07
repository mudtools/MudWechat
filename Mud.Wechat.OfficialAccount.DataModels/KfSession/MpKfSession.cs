// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfSession;

/// <summary>
/// 客服会话操作请求体（<c>createkfsession</c> 与 <c>closeSession</c> <b>共用</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>：官方两页请求体字段表逐字段一致（<c>kf_account</c> + <c>openid</c>，均必填），
/// 差异仅在语义（创建 / 关闭）与响应错误码 ⇒ 拆成两个同构类属纯冗余
/// （与标签域 <c>MpTagMembersRequest</c>、用户域 <c>MpBlacklistRequest</c> 同一处置）。
/// </para>
/// <para>
/// <b>创建会话的前置条件（官方「注意事项」原文）</b>：指定的客服账号<b>必须已经绑定微信号且在线</b>
/// （否则 <c>65415 the worker is not online</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpKfSessionRequest
{
    /// <summary>完整客服账号，格式为「账号前缀@公众号微信号」。</summary>
    [JsonPropertyName("kf_account")]
    public string KfAccount { get; set; } = string.Empty;

    /// <summary>粉丝的 openid。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;
}

/// <summary>
/// 会话条目（官方 <c>getsessionlist</c> 响应 <c>sessionlist</c> 元素）。
/// </summary>
/// <remarks>官方字段：<c>createtime</c>（会话接入时间，timestamp）/ <c>openid</c>（用户 openid）。</remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpKfSession
{
    /// <summary>会话接入时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("createtime")]
    public long CreateTime { get; set; }

    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>
/// 获取客户会话状态响应（<c>getkfsession</c>，<c>GET /customservice/kfsession/getsession</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>GET</b>，业务 Query 参数 <c>openid</c>（<b>必填</b>）；<b>无请求体</b>
/// ⇒ 故只以参数形态暴露 <c>openid</c>，不设请求 DTO。
/// </para>
/// <para>官方响应仅 <c>createtime</c>（会话接入时间，timestamp）与 <c>kf_account</c>（接待客服账号）。</para>
/// <para>官方错误码：<c>40003</c>（invalid openid）/ <c>65400</c>（未开通或未升级新版客服功能）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpGetKfSessionResponse : MpResponse
{
    /// <summary>会话接入时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("createtime")]
    public long CreateTime { get; set; }

    /// <summary>接待客服账号。</summary>
    [JsonPropertyName("kf_account")]
    public string? KfAccount { get; set; }
}

/// <summary>
/// 获取客服会话列表响应（<c>getkfsessionlist</c>，<c>GET /customservice/kfsession/getsessionlist</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>GET</b>，业务 Query 参数 <c>kf_account</c>（<b>必填</b>，完整客服账号）；
/// <b>无请求体</b> ⇒ 只以参数形态暴露。
/// </para>
/// <para>官方错误码：<c>0</c> / <c>65401</c>（无效客服账号）/ <c>65402</c>（客服账号尚未绑定微信号）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpGetKfSessionListResponse : MpResponse
{
    /// <summary>会话列表。</summary>
    [JsonPropertyName("sessionlist")]
    public List<MpKfSession>? SessionList { get; set; }
}

/// <summary>
/// 未接入会话条目（官方 <c>getwaitcase</c> 响应 <c>waitcaselist</c> 元素）。
/// </summary>
/// <remarks>
/// 官方该页元素字段<b>仅</b> <c>latest_time</c>（用户的最后一条消息的时间）与 <c>openid</c>
/// ——页面<b>未出现</b> <c>kf_account</c>/<c>createtime</c> ⇒ SDK 不据其他页「补全」字段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpWaitCase
{
    /// <summary>用户的最后一条消息的时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("latest_time")]
    public long LatestTime { get; set; }

    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>
/// 获取未接入会话列表响应（<c>getwaitcase</c>，<c>GET /customservice/kfsession/getwaitcase</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>GET 且无请求体</b>（无业务 Query 参数）。
/// </para>
/// <para>
/// <b>返回上限（官方原文）</b>：<c>waitcaselist</c> <b>最多返回 100 条数据，按照来访顺序</b>
/// ⇒ 调用方不得假定能一次取全（官方未提供该接口的分页游标，超出部分需靠反复轮询与人工接入消化）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpGetWaitCaseResponse : MpResponse
{
    /// <summary>未接入会话数量。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>未接入会话列表（最多 100 条，按来访顺序）。</summary>
    [JsonPropertyName("waitcaselist")]
    public List<MpWaitCase>? WaitCaseList { get; set; }
}
