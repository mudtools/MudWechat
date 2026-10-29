// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PromotionQrCode;

/// <summary>
/// 设置授权应用可见范围请求体（<c>/cgi-bin/agent/set_scope</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>危险操作约束（官方明文，勿弱化）</b>：官方对三个可见范围参数均标注
/// 「若未填该字段，则清空可见范围中成员 / 部门 / 标签列表」——
/// 即<b>不传等价于清空</b>，而非「保持不变」。若希望保留某一类可见范围，必须显式传入该字段。
/// </para>
/// <para>
/// 本 SDK 的 AOT JSON 上下文采用 <c>JsonIgnoreCondition.WhenWritingNull</c>，
/// 故「不赋值（保持 <c>null</c>）」在序列化时<b>不出现该字段</b>，正好对上官方「未填 ⇒ 清空」的语义。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PromotionQrCode")]
public class SetAuthorizedAppScopeRequest
{
    /// <summary>
    /// 获取或设置授权方应用 id（官方必填）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int AgentId { get; set; }

    /// <summary>
    /// 获取或设置应用可见范围（成员）。
    /// <para><b>官方限制：若未填该字段，则清空可见范围中成员列表。</b></para>
    /// </summary>
    [JsonPropertyName("allow_user")]
    public List<string>? AllowUser { get; set; }

    /// <summary>
    /// 获取或设置应用可见范围（部门）。
    /// <para><b>官方限制：若未填该字段，则清空可见范围中部门列表。</b></para>
    /// </summary>
    [JsonPropertyName("allow_party")]
    public List<int>? AllowParty { get; set; }

    /// <summary>
    /// 获取或设置应用可见范围（标签）。
    /// <para><b>官方限制：若未填该字段，则清空可见范围中标签列表。</b></para>
    /// </summary>
    [JsonPropertyName("allow_tag")]
    public List<int>? AllowTag { get; set; }
}
