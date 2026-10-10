// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

/// <summary>
/// 客服输入状态请求体（<c>typing</c>，<c>POST /cgi-bin/message/custom/typing</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>touser</c>（必填）与 <c>command</c>（必填，<c>Typing</c> / <c>CancelTyping</c>）。
/// </para>
/// <para>
/// <b>官方约束（硬性）</b>：下发输入状态需<b>之前 30 秒内与该用户有过消息交互</b>（否则 <c>45080</c>）；
/// 已处于输入状态时<b>不可重复下发</b>（<c>45081</c>）。
/// </para>
/// <para>
/// <b>路径差异（勿混用）</b>：公众号 / 服务号为 <c>/cgi-bin/message/custom/typing</c>；
/// 小程序 / 小游戏为 <c>/cgi-bin/message/custom/business/typing</c>（本 SDK 仅覆盖公众号形态）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpTypingRequest
{
    /// <summary>用户的 OpenID。</summary>
    [JsonPropertyName("touser")]
    public string ToUser { get; set; } = string.Empty;

    /// <summary>命令（<c>Typing</c> 下发「正在输入」/ <c>CancelTyping</c> 取消）。</summary>
    [JsonPropertyName("command")]
    public string Command { get; set; } = string.Empty;

    /// <summary>子商户 ID（<b>普通账号无需填写</b>）。</summary>
    [JsonPropertyName("businessid")]
    public string? BusinessId { get; set; }
}

/// <summary>
/// 获取聊天记录请求体（<c>getMsgList</c>，<c>POST /customservice/msgrecord/getmsglist</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档缺陷（勿据「请求体：无」而省字段）</b>：该页字段区标注「请求体无」，
/// 但请求示例明确携带 <c>starttime</c>/<c>endtime</c>/<c>msgid</c>/<c>number</c> 四个字段
/// ⇒ SDK 按其<b>示例</b>建模（否则接口无法分页拉取）。
/// </para>
/// <para>
/// <b>时间与条数上限（官方明文）</b>：查询时间段<b>不能超过 24 小时</b>；
/// 每次最多获取 <b>10000 条</b>记录。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpGetMsgListRequest
{
    /// <summary>起始时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("starttime")]
    public long StartTime { get; set; }

    /// <summary>结束时间（Unix 时间戳，秒；与起始时间跨度<b>不得超过 24 小时</b>）。</summary>
    [JsonPropertyName("endtime")]
    public long EndTime { get; set; }

    /// <summary>消息 id（分页游标，取上一页响应的 <c>msgid</c>）。</summary>
    [JsonPropertyName("msgid")]
    public long MsgId { get; set; }

    /// <summary>获取数量（每次最多 10000）。</summary>
    [JsonPropertyName("number")]
    public int Number { get; set; }
}

/// <summary>获取聊天记录响应（<c>getMsgList</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpGetMsgListResponse : MpResponse
{
    /// <summary>消息内容列表。</summary>
    [JsonPropertyName("recordlist")]
    public List<MpMsgRecord>? RecordList { get; set; }

    /// <summary>消息数量。</summary>
    [JsonPropertyName("number")]
    public int Number { get; set; }

    /// <summary>消息 id（下一页请求回填）。</summary>
    [JsonPropertyName("msgid")]
    public long MsgId { get; set; }
}

/// <summary>聊天记录条目（<c>recordlist</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMsgRecord
{
    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>操作码（<c>2002</c> 客服发送信息 / <c>2003</c> 客服接收消息）。</summary>
    [JsonPropertyName("opercode")]
    public int OperCode { get; set; }

    /// <summary>聊天记录文本。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>操作时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("time")]
    public long Time { get; set; }

    /// <summary>完整客服账号，格式为「账号前缀@公众号微信号」。</summary>
    [JsonPropertyName("worker")]
    public string? Worker { get; set; }
}
