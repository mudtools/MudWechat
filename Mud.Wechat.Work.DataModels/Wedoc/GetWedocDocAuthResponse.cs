// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 获取文档权限信息响应体（<c>/cgi-bin/wedoc/doc_get_auth</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetWedocDocAuthResponse : WechatWorkResponse
{
    /// <summary>获取或设置文档的查看规则（官方 <c>access_rule</c>）。</summary>
    [JsonPropertyName("access_rule")]
    public WedocDocAccessRule? AccessRule { get; set; }

    /// <summary>获取或设置文档的安全设置（官方 <c>secure_setting</c>）。</summary>
    [JsonPropertyName("secure_setting")]
    public WedocDocSecureSetting? SecureSetting { get; set; }

    /// <summary>获取或设置文档通知范围及权限列表（官方 <c>doc_member_list</c>）。</summary>
    [JsonPropertyName("doc_member_list")]
    public List<WedocDocMember>? DocMemberList { get; set; }

    /// <summary>获取或设置文档查看权限特定部门列表（官方 <c>co_auth_list</c>），列表中的部门可直接浏览文档。</summary>
    [JsonPropertyName("co_auth_list")]
    public List<WedocDocCoAuth>? CoAuthList { get; set; }
}
