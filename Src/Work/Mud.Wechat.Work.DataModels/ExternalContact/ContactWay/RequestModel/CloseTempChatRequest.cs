// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

/// <summary>
/// 结束临时会话请求体（<c>/cgi-bin/externalcontact/close_temp_chat</c>）。
/// <para>成员与客户之间必须存在有效的临时会话才可结束；
/// 通过其他方式添加的外部联系人无法通过此接口关闭会话。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class CloseTempChatRequest
{
    /// <summary>
    /// 获取或设置企业成员的 userid（官方必填）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置外部联系人的 userid（官方必填）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }
}
