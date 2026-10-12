// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

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
