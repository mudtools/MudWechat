// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

/// <summary>
/// 获取由获客链接添加的客户列表响应体（<c>/cgi-bin/externalcontact/customer_acquisition/customer</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class GetAcquisitionCustomerListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置获客客户列表。
    /// </summary>
    [JsonPropertyName("customer_list")]
    public List<AcquisitionCustomerItem>? CustomerList { get; set; }

    /// <summary>
    /// 获取或设置分页游标（下次请求时填入以获取后续分页数据，无更多数据时返回空）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// 获客客户项（获取由获客链接添加的客户列表响应中 <c>customer_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class AcquisitionCustomerItem
{
    /// <summary>
    /// 获取或设置客户的 external_userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置通过获客链接添加此客户的跟进人 userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置会话状态：0 - 客户未发消息，1 - 客户已发送消息，2 - 客户发送消息状态未知。
    /// </summary>
    [JsonPropertyName("chat_status")]
    public int? ChatStatus { get; set; }

    /// <summary>
    /// 获取或设置用于区分客户通过哪个获客链接添加的自定义参数
    /// （在获客链接后拼接 customer_channel=自定义字符串，不超过 64 字节，超出会被截断）。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}
