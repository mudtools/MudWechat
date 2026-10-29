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
