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
