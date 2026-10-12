// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>查询群发消息发送状态（<c>message/mass/get</c>）响应。</summary>
/// <remarks>
/// <b>逐页核验重要偏差</b>：本接口响应<b>仅 msg_id / msg_status 两字段</b>——
/// TotalCount/FilterCount/SentCount/ErrorCount 只出现在 MASSSENDJOBFINISH 事件推送 XML 中，
/// 不在本查询接口的响应字段表里（方案文档预判已按核验修正）。
/// <b>官方文档矛盾（照录）</b>：响应表 msg_id 标 string、示例为数字 201053012 ⇒ 按示例数字形态建模
/// （请求体示例为字符串 "201053012"——两侧形态本就不一致）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassStatusResponse : MpResponse
{
    /// <summary>获取或设置群发消息后返回的消息 id（官方 <c>msg_id</c>；响应表标 string、示例为数字，按示例建模）。</summary>
    [JsonPropertyName("msg_id")]
    public long? MsgId { get; set; }

    /// <summary>获取或设置发送状态（官方 <c>msg_status</c>：SEND_SUCCESS / SENDING / SEND_FAIL / DELETE）。</summary>
    [JsonPropertyName("msg_status")]
    public string? MsgStatus { get; set; }
}
