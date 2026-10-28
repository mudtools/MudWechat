// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服同步消息的文本消息体（<c>/cgi-bin/kf/sync_msg</c> 的 text 消息）。
/// <para>
/// 与发送侧 <see cref="KfTextMsgBody"/> 的差异：客户点击菜单消息中的 click 项时，
/// 同步回来的文本消息额外携带被点击的菜单项 ID（<c>menu_id</c>）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfSyncTextMsgBody
{
    /// <summary>
    /// 获取或设置消息内容。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 获取或设置客户点击菜单消息时的菜单项 ID。
    /// </summary>
    [JsonPropertyName("menu_id")]
    public string? MenuId { get; set; }
}
