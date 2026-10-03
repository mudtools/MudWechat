// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件获取获客链接使用成员接受消息数据响应体
/// （<c>/cgi-bin/externalcontact/customer_acquisition/get_chat_info</c>）。
/// </summary>
/// <remarks>
/// 组件版响应<b>不返回</b>顶层 <c>userid</c> / <c>external_userid</c>
/// （与自建/第三方直连版 <c>CustomerAcquisition</c> 域的收消息详情差异点），仅返回聚合会话信息。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionComponentChatInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置会话详情（收消息次数与来源获客链接信息）。
    /// </summary>
    [JsonPropertyName("chat_info")]
    public AcquisitionComponentChatInfo? ChatInfo { get; set; }
}

/// <summary>
/// 获客助手组件会话详情。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class AcquisitionComponentChatInfo
{
    /// <summary>
    /// 获取或设置成员收到的某个客户的消息次数。
    /// </summary>
    [JsonPropertyName("recv_msg_cnt")]
    public int? RecvMsgCnt { get; set; }

    /// <summary>
    /// 获取或设置成员添加客户的获客链接 id。
    /// </summary>
    [JsonPropertyName("link_id")]
    public string? LinkId { get; set; }

    /// <summary>
    /// 获取或设置成员添加客户的 state。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}
