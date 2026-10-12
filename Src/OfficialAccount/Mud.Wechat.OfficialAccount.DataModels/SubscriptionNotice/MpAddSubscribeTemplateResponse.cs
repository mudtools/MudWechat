// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

/// <summary>选用模板（<c>wxaapi/newtmpl/addtemplate</c>）响应。</summary>
/// <remarks>官方响应字段为 <c>priTmplId</c>（非 pid/template_id——逐页核验确认）。</remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpAddSubscribeTemplateResponse : MpResponse
{
    /// <summary>获取或设置添加至帐号下的模板 id（官方 <c>priTmplId</c>，发送订阅通知时所需）。</summary>
    [JsonPropertyName("priTmplId")]
    public string? PriTmplId { get; set; }
}
