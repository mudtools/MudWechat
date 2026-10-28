// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

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
