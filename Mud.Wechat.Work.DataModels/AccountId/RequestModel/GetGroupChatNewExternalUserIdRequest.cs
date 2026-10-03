// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// 转换客户群成员 external_userid 请求体（<c>/cgi-bin/externalcontact/groupchat/get_new_external_userid</c>）：
/// 转换客户群中无好友关系的群成员 external_userid（转换接口不支持客户群场景时使用）。
/// </summary>
/// <remarks>
/// 官方限制：external_userid_list 最多不超过 1000 个；传入新的 external_userid 则原样返回；
/// 客户群的群主需要在应用可见范围内。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class GetGroupChatNewExternalUserIdRequest
{
    /// <summary>
    /// 获取或设置客户群 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置旧外部联系人 id 列表（官方必填，最多不超过 1000 个）。
    /// </summary>
    [JsonPropertyName("external_userid_list")]
    public List<string>? ExternalUserIdList { get; set; }
}
