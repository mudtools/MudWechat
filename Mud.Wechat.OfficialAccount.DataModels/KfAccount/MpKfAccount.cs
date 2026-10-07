// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfAccount;

/// <summary>
/// 客服账号（官方 <c>getkflist</c> 响应 <c>kf_list</c> 元素）。
/// </summary>
/// <remarks>
/// <para>官方字段：<c>kf_account</c>（完整客服账号，格式「账号前缀@公众号微信号」）/ <c>kf_nick</c>（昵称）
/// / <c>kf_headimgurl</c>（头像）/ <c>kf_id</c>（客服编号，<b>字符串</b>）。</para>
/// <para>
/// <b>注意与「在线客服」列表的字段差异</b>：在线列表的 <c>kf_id</c> 官方标为 <b>number</b>，
/// 且额外含 <c>status</c>/<c>accepted_case</c>/<c>kf_openid</c> ⇒ 官方两页形态不同，故刻意保留两套 DTO
/// （合并会静默丢字段或改错类型）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpKfAccount
{
    /// <summary>完整客服账号，格式为「账号前缀@公众号微信号」。</summary>
    [JsonPropertyName("kf_account")]
    public string? KfAccount { get; set; }

    /// <summary>客服昵称。</summary>
    [JsonPropertyName("kf_nick")]
    public string? KfNick { get; set; }

    /// <summary>客服头像。</summary>
    [JsonPropertyName("kf_headimgurl")]
    public string? KfHeadImgUrl { get; set; }

    /// <summary>客服编号（官方本页为字符串）。</summary>
    [JsonPropertyName("kf_id")]
    public string? KfId { get; set; }
}

/// <summary>
/// 获取所有客服账号响应（<c>getkflist</c>，<c>GET /cgi-bin/customservice/getkflist</c>）。
/// </summary>
/// <remarks>
/// 官方契约：<b>GET 且无请求体</b>；业务 Query 仅可选 <c>business_id</c>（客服子商户的 business_id，
/// 普通账号不需要填）。故本端点只以参数形态暴露 <c>business_id</c>，**不设请求 DTO**。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpGetKfListResponse : MpResponse
{
    /// <summary>客服列表。</summary>
    [JsonPropertyName("kf_list")]
    public List<MpKfAccount>? KfList { get; set; }
}

/// <summary>
/// 在线客服（官方 <c>getonlinekflist</c> 响应 <c>kf_online_list</c> 元素）。
/// </summary>
/// <remarks>官方字段：<c>kf_account</c> / <c>status</c>（<c>0</c> 不在线、<c>1</c> web 在线）/
/// <c>kf_id</c>（编号，<b>number</b>）/ <c>accepted_case</c>（当前正在接待的会话数）/ <c>kf_openid</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpKfOnlineAccount
{
    /// <summary>完整客服账号，格式为「账号前缀@公众号微信号」。</summary>
    [JsonPropertyName("kf_account")]
    public string? KfAccount { get; set; }

    /// <summary>客服在线状态（目前为 <c>0</c> 不在线、<c>1</c> web 在线）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>客服编号（官方本页为数值）。</summary>
    [JsonPropertyName("kf_id")]
    public long KfId { get; set; }

    /// <summary>客服当前正在接待的会话数。</summary>
    [JsonPropertyName("accepted_case")]
    public int AcceptedCase { get; set; }

    /// <summary>客服 openid。</summary>
    [JsonPropertyName("kf_openid")]
    public string? KfOpenId { get; set; }
}

/// <summary>
/// 获取在线客服列表响应（<c>getonlinekflist</c>，<c>GET /cgi-bin/customservice/getonlinekflist</c>）。
/// </summary>
/// <remarks>官方契约：<b>GET 且无请求体</b>；业务 Query 仅可选 <c>business_id</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpGetOnlineKfListResponse : MpResponse
{
    /// <summary>在线客服列表。</summary>
    [JsonPropertyName("kf_online_list")]
    public List<MpKfOnlineAccount>? KfOnlineList { get; set; }
}


