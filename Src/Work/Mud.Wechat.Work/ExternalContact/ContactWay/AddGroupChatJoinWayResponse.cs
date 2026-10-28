// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

/// <summary>
/// 配置客户群进群方式响应体（<c>/cgi-bin/externalcontact/groupchat/add_join_way</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class AddGroupChatJoinWayResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置进群方式的配置 id（用于后续获取 / 更新 / 删除该进群方式配置）。
    /// </summary>
    [JsonPropertyName("config_id")]
    public string? ConfigId { get; set; }
}
