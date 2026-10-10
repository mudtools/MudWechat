// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

/// <summary>
/// 获取成员多次收消息详情响应体（<c>/cgi-bin/externalcontact/customer_acquisition/get_chat_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class GetAcquisitionChatInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置成员的 userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置客户的 external_userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置会话详情（收消息次数与来源获客链接信息）。
    /// </summary>
    [JsonPropertyName("chat_info")]
    public AcquisitionChatInfo? ChatInfo { get; set; }
}

/// <summary>
/// 成员多次收消息的会话详情（<c>chat_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class AcquisitionChatInfo
{
    /// <summary>
    /// 获取或设置成员收到的该客户的消息次数。
    /// </summary>
    [JsonPropertyName("recv_msg_cnt")]
    public int? RecvMsgCnt { get; set; }

    /// <summary>
    /// 获取或设置成员添加客户的获客链接 id。
    /// </summary>
    [JsonPropertyName("link_id")]
    public string? LinkId { get; set; }

    /// <summary>
    /// 获取或设置成员添加客户时获客链接的自定义参数 state。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}
