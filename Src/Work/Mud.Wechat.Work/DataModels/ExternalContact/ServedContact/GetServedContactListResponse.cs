// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ServedContact;

/// <summary>
/// 获取已服务的外部联系人响应体（<c>/cgi-bin/externalcontact/contact_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ServedContact")]
public class GetServedContactListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置已服务的外部联系人记录列表。
    /// </summary>
    [JsonPropertyName("info_list")]
    public List<ServedContactInfo>? InfoList { get; set; }

    /// <summary>
    /// 获取或设置分页游标（无更多数据则返回空；有效期 4 小时）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// 已服务的外部联系人记录（<c>info_list[]</c> 元素）。
/// <para>客户返回临时 id + external_userid；其他外部联系人只返回临时 id 与脱敏昵称。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ServedContact")]
public class ServedContactInfo
{
    /// <summary>
    /// 获取或设置该外部联系人是否被成员标记为客户。
    /// </summary>
    [JsonPropertyName("is_customer")]
    public bool? IsCustomer { get; set; }

    /// <summary>
    /// 获取或设置外部联系人临时 id（可用于去重，有效期 4 小时；
    /// 仅在一轮完整遍历查询中唯一，每次首个分页返回的临时 id 都会变化）。
    /// </summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>
    /// 获取或设置外部联系人的 external_userid（仅客户返回）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置脱敏后的昵称（仅其他外部联系人返回）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置添加该联系人的成员或群主的 userid。
    /// </summary>
    [JsonPropertyName("follow_userid")]
    public string? FollowUserid { get; set; }

    /// <summary>
    /// 获取或设置群聊 id（群聊被标记为客户群时返回）。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置群名（群聊未被标记为客户群时返回）。
    /// </summary>
    [JsonPropertyName("chat_name")]
    public string? ChatName { get; set; }

    /// <summary>
    /// 获取或设置首次添加 / 进群时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("add_time")]
    public long? AddTime { get; set; }
}
