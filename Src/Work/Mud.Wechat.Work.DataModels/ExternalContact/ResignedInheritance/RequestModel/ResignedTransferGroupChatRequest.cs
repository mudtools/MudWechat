// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;

/// <summary>
/// 分配离职成员的客户群请求体（<c>/cgi-bin/externalcontact/groupchat/transfer</c>）。
/// <para>将已离职成员为群主的客户群分配给另一个企业成员（新群主）；
/// 新群主须配置了客户联系功能、已实名且已激活企业微信，并在最近一年内至少登录过一次企业微信。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ResignedInheritance")]
public class ResignedTransferGroupChatRequest
{
    /// <summary>
    /// 获取或设置需要转群主的客户群 ID 列表（官方必填；取值范围 1 ~ 100）。
    /// </summary>
    [JsonPropertyName("chat_id_list")]
    public List<string>? ChatIdList { get; set; }

    /// <summary>
    /// 获取或设置新群主 ID（官方必填；须在应用可见范围内）。
    /// </summary>
    [JsonPropertyName("new_owner")]
    public string? NewOwner { get; set; }
}
