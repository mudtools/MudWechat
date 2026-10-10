// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 批量获取客户详情响应体（<c>/cgi-bin/externalcontact/batch/get_by_user</c>；
/// cursor + limit 分页，limit 最大 100、默认 50）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class BatchGetCustomerDetailsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置客户信息列表（external_contact_list，每项含 external_contact 与 follow_info）。
    /// </summary>
    [JsonPropertyName("external_contact_list")]
    public List<BatchCustomerContactItem>? ExternalContactList { get; set; }

    /// <summary>
    /// 获取或设置分页游标（下次请求经 cursor 传入；没有更多数据时返回空）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置失败信息（部分请求成员无有效互通许可时列出，可空）。
    /// </summary>
    [JsonPropertyName("fail_info")]
    public BatchCustomerFailInfo? FailInfo { get; set; }
}
