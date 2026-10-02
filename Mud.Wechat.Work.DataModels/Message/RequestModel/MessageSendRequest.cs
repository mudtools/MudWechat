// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 发送应用消息请求基类（<c>/cgi-bin/message/send</c>，官方文档 90236 自建 / 90372 第三方 / 96458 代开发，三类应用契约完全一致）。
/// <para>本类仅承载信封公共字段，由各消息类型请求子类继承；<c>touser</c>、<c>toparty</c>、<c>totag</c> 不能同时为空。</para>
/// <para>限频：每应用不可超过「账号上限数 × 200」人次/天；对同一成员不可超过 30 次/分钟、1000 次/小时，超过部分被丢弃不下发。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageSendRequest
{
    /// <summary>
    /// 获取或设置成员 ID 列表（多个接收者用 '|' 分隔，最多支持 1000 个）；
    /// 特殊取值 "@all" 表示向该企业应用的全部成员发送。
    /// </summary>
    [JsonPropertyName("touser")]
    public string? ToUser { get; set; }

    /// <summary>
    /// 获取或设置部门 ID 列表，多个接收者用 '|' 分隔，最多支持 100 个；
    /// 当 <c>touser</c> 为 "@all" 时忽略本参数。
    /// </summary>
    [JsonPropertyName("toparty")]
    public string? ToParty { get; set; }

    /// <summary>
    /// 获取或设置标签 ID 列表，多个接收者用 '|' 分隔，最多支持 100 个；
    /// 当 <c>touser</c> 为 "@all" 时忽略本参数。
    /// </summary>
    [JsonPropertyName("totag")]
    public string? ToTag { get; set; }

    /// <summary>
    /// 获取或设置企业应用的 id，整型（官方必填；小程序通知消息参数表未单列本字段，实际调用仍需填写）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }

    /// <summary>
    /// 获取或设置消息类型（官方必填），由各子类对应固定取值
    /// （text、image、voice、video、file、textcard、news、mpnews、markdown、miniprogram_notice、template_card、template_msg）。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>
    /// 获取或设置是否开启 id 转译：0 - 否（默认），1 - 是（图片、语音、视频、文件、markdown 消息不支持）。
    /// </summary>
    [JsonPropertyName("enable_id_trans")]
    public int? EnableIdTrans { get; set; }

    /// <summary>
    /// 获取或设置是否开启重复消息检查：0 - 否（默认），1 - 是。
    /// </summary>
    [JsonPropertyName("enable_duplicate_check")]
    public int? EnableDuplicateCheck { get; set; }

    /// <summary>
    /// 获取或设置重复消息检查的时间间隔，默认 1800s，最大不超过 4 小时。
    /// </summary>
    [JsonPropertyName("duplicate_check_interval")]
    public int? DuplicateCheckInterval { get; set; }
}
