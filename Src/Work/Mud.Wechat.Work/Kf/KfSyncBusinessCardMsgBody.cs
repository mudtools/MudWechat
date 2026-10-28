// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服同步消息的名片消息体（<c>/cgi-bin/kf/sync_msg</c> 的 business_card 消息）。
/// <para>为企业成员名片，属同步侧独有的消息类型（发送侧 <c>kf/send_msg</c> 不开放该 msgtype）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfSyncBusinessCardMsgBody
{
    /// <summary>
    /// 获取或设置名片对应成员的 userid（第三方 / 代开发应用获取到的是密文 userid 即 open_userid）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }
}
