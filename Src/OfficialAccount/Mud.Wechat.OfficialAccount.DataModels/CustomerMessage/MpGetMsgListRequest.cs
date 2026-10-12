// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

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
