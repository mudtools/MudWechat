// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.FreePublish;

/// <summary>发布草稿（<c>freepublish/submit</c>）响应。</summary>
/// <remarks>
/// <para>
/// 官方原文：「正常情况下调用成功时，errcode 将为 0，此时<b>只意味着发布任务提交成功</b>，
/// 并不意味着此时发布已经完成」——发布结果经 <c>PUBLISHJOBFINISH</c> 事件推送（SDK 未建模该事件：
/// 官方 API 页仅文字提及，XML 报文无独立页面，与 kf 会话事件同为待实测项）。
/// </para>
/// <para><b>官方文档矛盾（照录）</b>：返回体表列 <c>msg_data_id</c> 但返回示例未含该字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishSubmitResponse : MpResponse
{
    /// <summary>获取或设置发布任务的 id（官方 <c>publish_id</c>）。</summary>
    [JsonPropertyName("publish_id")]
    public string? PublishId { get; set; }

    /// <summary>获取或设置消息的数据 ID（官方 <c>msg_data_id</c>；官方返回表示例缺失，矛盾照录）。</summary>
    [JsonPropertyName("msg_data_id")]
    public string? MsgDataId { get; set; }
}
