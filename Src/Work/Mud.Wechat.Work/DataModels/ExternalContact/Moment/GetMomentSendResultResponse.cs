// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 获取客户朋友圈发表后的可见客户列表响应体（<c>/cgi-bin/externalcontact/get_moment_send_result</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class GetMomentSendResultResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置发送成功的外部联系人列表。
    /// </summary>
    [JsonPropertyName("customer_list")]
    public List<MomentSendResultItem>? CustomerList { get; set; }

    /// <summary>
    /// 获取或设置分页游标（用于查询下一个分页，无更多数据时不返回该字段）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// 发送成功客户项（获取客户朋友圈发表后的可见客户列表响应中 <c>customer_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentSendResultItem
{
    /// <summary>
    /// 获取或设置发送成功的外部联系人 userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }
}
