// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服同步消息的事件消息体（<c>/cgi-bin/kf/sync_msg</c> 的 event 消息）。
/// <para>
/// 官方以一份扁平 event 结构承载全部事件类型，按 <see cref="EventType"/> 判别哪些字段出现
/// （如欢迎语 code、会话变更前后接待人员、发送失败消息等），不做运行时多态。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfSyncEventMsgBody
{
    /// <summary>
    /// 获取或设置事件类型（取值以官方参数表为准）。
    /// </summary>
    [JsonPropertyName("event_type")]
    public string? EventType { get; set; }

    /// <summary>
    /// 获取或设置客服账号 ID。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置客户的 external_userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置接待人员的 userid（第三方 / 代开发应用获取到的是密文 userid 即 open_userid）。
    /// </summary>
    [JsonPropertyName("servicer_userid")]
    public string? ServicerUserId { get; set; }

    /// <summary>
    /// 获取或设置场景值（官方事件侧为字符串形态）。
    /// </summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>
    /// 获取或设置场景参数。
    /// </summary>
    [JsonPropertyName("scene_param")]
    public string? SceneParam { get; set; }

    /// <summary>
    /// 获取或设置发送欢迎语等事件响应消息所用的 code（有效期有限，须及时使用且仅可使用一次）。
    /// </summary>
    [JsonPropertyName("welcome_code")]
    public string? WelcomeCode { get; set; }

    /// <summary>
    /// 获取或设置状态值（会话状态变更类事件携带）。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置发送失败的消息 ID。
    /// </summary>
    [JsonPropertyName("fail_msgid")]
    public string? FailMessageId { get; set; }

    /// <summary>
    /// 获取或设置发送失败的类型（取值以官方参数表为准）。
    /// </summary>
    [JsonPropertyName("fail_type")]
    public int? FailType { get; set; }

    /// <summary>
    /// 获取或设置会话状态变更类型。
    /// </summary>
    [JsonPropertyName("change_type")]
    public int? ChangeType { get; set; }

    /// <summary>
    /// 获取或设置变更前的接待人员 userid。
    /// </summary>
    [JsonPropertyName("old_servicer_userid")]
    public string? OldServicerUserId { get; set; }

    /// <summary>
    /// 获取或设置变更后的接待人员 userid。
    /// </summary>
    [JsonPropertyName("new_servicer_userid")]
    public string? NewServicerUserId { get; set; }

    /// <summary>
    /// 获取或设置用于发送事件响应消息的 code。
    /// </summary>
    [JsonPropertyName("msg_code")]
    public string? MsgCode { get; set; }

    /// <summary>
    /// 获取或设置视频号信息（视频号相关事件携带）。
    /// </summary>
    [JsonPropertyName("wechat_channels")]
    public KfSyncEventWechatChannels? WechatChannels { get; set; }

    /// <summary>
    /// 获取或设置被撤回的消息 ID。
    /// </summary>
    [JsonPropertyName("recall_msgid")]
    public string? RecallMessageId { get; set; }
}
