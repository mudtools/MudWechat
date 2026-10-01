// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 获取客户详情响应体（<c>/cgi-bin/externalcontact/get</c>；跟进人超过 500 人时以 cursor 分页）。
/// </summary>
public class GetCustomerDetailResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置客户基本信息（external_contact）。
    /// </summary>
    [JsonPropertyName("external_contact")]
    public ExternalContactInfo? ExternalContact { get; set; }

    /// <summary>
    /// 获取或设置添加了该客户的企微成员跟进信息列表（follow_user）。
    /// </summary>
    [JsonPropertyName("follow_user")]
    public List<CustomerFollowUser>? FollowUser { get; set; }

    /// <summary>
    /// 获取或设置分页游标（跟进人多于 500 人时返回；下次请求经 cursor 传入以获取后续分页）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}
