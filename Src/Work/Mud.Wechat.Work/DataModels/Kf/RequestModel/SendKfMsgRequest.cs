// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 发送消息请求基类（<c>/cgi-bin/kf/send_msg</c>，官方文档 94677 自建 / 94700 第三方 / 96427 代开发，三类应用契约完全一致）。
/// <para>
/// 本类仅承载信封公共字段，由各消息类型请求子类继承。
/// 仅当客户处于「新接入待处理」或「由智能助手接待」状态时可发送；
/// 客户主动发消息后 48 小时内企业最多可下发 5 条消息，客户再次发消息可重新计时。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class SendKfMsgRequest
{
    /// <summary>
    /// 获取或设置接收消息的客户 UserID（官方必填）。
    /// </summary>
    [JsonPropertyName("touser")]
    public string? ToUser { get; set; }

    /// <summary>
    /// 获取或设置发送消息的客服账号 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置消息 ID（不多于 32 个字节，取值需匹配正则 <c>[0-9a-zA-Z_-]*</c>）。
    /// <para>
    /// 须在客服账号内唯一，否则官方报错；响应中原样返回该字段，未指定时由系统自动生成。
    /// </para>
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>
    /// 获取或设置消息类型（官方必填），由各子类对应固定取值
    /// （text、image、voice、video、file、link、miniprogram、msgmenu、location、ca_link）。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }
}
