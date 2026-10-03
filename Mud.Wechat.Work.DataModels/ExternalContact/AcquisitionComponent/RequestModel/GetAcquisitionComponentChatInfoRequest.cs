// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件获取获客链接使用成员接受消息数据请求体
/// （<c>/cgi-bin/externalcontact/customer_acquisition/get_chat_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionComponentChatInfoRequest
{
    /// <summary>
    /// 获取或设置会话信息凭据 ChatKey（官方必填；来自成员多次收消息事件的回调内容，回调后 30 分钟内有效）。
    /// </summary>
    [JsonPropertyName("chat_key")]
    public string? ChatKey { get; set; }
}
