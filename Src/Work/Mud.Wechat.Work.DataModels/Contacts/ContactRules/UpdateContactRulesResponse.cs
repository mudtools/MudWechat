// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.ContactRules;

/// <summary>
/// 修改通讯录隐藏规则响应体（<c>/cgi-bin/contactrule/update</c>）。
/// </summary>
/// <remarks>
/// 官方文档「返回参数说明」表仅列出 errcode / errmsg，但返回结果示例中含 <c>rules</c> 回显数组
/// （更新后的完整规则内容，含 rule_id / rule_type / range / whitelist）；
/// 本模型以可空字段承载该回显，官方未返回时为 <c>null</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ContactRules")]
public class UpdateContactRulesResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置修改后的规则回显列表（官方返回示例含此数组；官方未返回时为 <c>null</c>）。
    /// </summary>
    [JsonPropertyName("rules")]
    public List<ContactRule>? Rules { get; set; }
}
