// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>群发 / 预览提交结果（官方 sendall 与 mass/send 两页响应字段表一致 ⇒ 共用）。</summary>
/// <remarks>
/// 官方原文：「在返回成功时，意味着群发任务提交成功，并不意味着此时群发已经结束」——
/// 最终结果经回调事件 <c>masssendjobfinish</c>（MASSSENDJOBFINISH）异步推送。
/// 官方响应表另有 <c>type</c> 行（说明原文「次数为news」疑为笔误，照录）——示例响应未携带该字段，不建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassSendResponse : MpResponse
{
    /// <summary>获取或设置消息发送任务的 ID（官方 <c>msg_id</c>）。</summary>
    [JsonPropertyName("msg_id")]
    public long MsgId { get; set; }

    /// <summary>
    /// 获取或设置消息的数据 ID（官方 <c>msg_data_id</c>；<b>仅群发图文消息时出现</b>，
    /// 用于图文分析数据接口——官方说明原文含叠字「，，」，照录）。
    /// </summary>
    [JsonPropertyName("msg_data_id")]
    public long? MsgDataId { get; set; }
}
