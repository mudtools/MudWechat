// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.JobInheritance;

/// <summary>
/// 分配在职成员的客户群请求体（<c>/cgi-bin/externalcontact/groupchat/onjob_transfer</c>）。
/// <para>新群主必须是配置了客户联系功能的成员、已设置实名且已激活企业微信；
/// 新旧群主需在最近一年内登录过至少一次企业微信；
/// 同一个人的群每天最多分配 300 个给新群主；
/// 为保障客户服务体验，90 个自然日内在职成员的每个客户群仅可被转接 2 次；
/// 群主必须在应用的可见范围内。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "JobInheritance")]
public class TransferGroupChatRequest
{
    /// <summary>
    /// 获取或设置需要转群主的客户群 ID 列表（官方必填；取值范围 1~100 个）。
    /// </summary>
    [JsonPropertyName("chat_id_list")]
    public List<string>? ChatIdList { get; set; }

    /// <summary>
    /// 获取或设置新群主 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("new_owner")]
    public string? NewOwner { get; set; }
}
