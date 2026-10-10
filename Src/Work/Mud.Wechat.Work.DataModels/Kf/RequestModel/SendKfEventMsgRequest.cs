// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 发送欢迎语等事件响应消息请求基类（<c>/cgi-bin/kf/send_msg_on_event</c>，
/// 官方文档 95122 自建 / 94910 第三方 / 96428 代开发，三类应用契约完全一致）。
/// <para>
/// 本类仅承载信封公共字段，由各消息类型请求子类继承；
/// <see cref="Code"/> 由事件回调下发、仅可使用一次，且有效期有限（欢迎语 / 结束会话场景仅 20 秒，须及时调用）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class SendKfEventMsgRequest
{
    /// <summary>
    /// 获取或设置事件响应消息对应的 code（官方必填；事件回调下发，仅可使用一次）。
    /// <para>
    /// 除「用户进入会话」外，响应消息仅支持会话处于获取该 code 时的会话状态。
    /// </para>
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// 获取或设置消息 ID（不多于 32 个字节，取值需匹配正则 <c>[0-9a-zA-Z_-]*</c>；响应中原样返回）。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>
    /// 获取或设置消息类型（官方必填），由各子类对应固定取值（text、msgmenu）。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }
}
