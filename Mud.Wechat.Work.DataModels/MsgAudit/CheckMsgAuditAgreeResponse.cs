// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MsgAudit;

/// <summary>
/// 获取会话同意情况响应体（<c>/cgi-bin/msgaudit/check_single_agree</c> 单聊 /
/// <c>/cgi-bin/msgaudit/check_room_agree</c> 群聊，官方两接口响应结构一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class CheckMsgAuditAgreeResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置同意情况列表（见 <see cref="MsgAuditAgreeInfo"/>）。
    /// </summary>
    [JsonPropertyName("agreeinfo")]
    public List<MsgAuditAgreeInfo>? AgreeInfo { get; set; }
}

/// <summary>
/// 会话同意情况条目（<see cref="CheckMsgAuditAgreeResponse.AgreeInfo"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class MsgAuditAgreeInfo
{
    /// <summary>
    /// 获取或设置内部成员的 userid（单聊响应返回；群聊响应不返回该字段）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置外部成员的 exteranalopenid。
    /// <para>官方文档原文即此拼写（<c>exteranalopenid</c>），本 SDK 照抄官方原文，勿改 external。</para>
    /// </summary>
    [JsonPropertyName("exteranalopenid")]
    public string? ExteranalOpenid { get; set; }

    /// <summary>
    /// 获取或设置同意状态：Agree - 同意、Disagree - 不同意。
    /// </summary>
    [JsonPropertyName("agree_status")]
    public string? AgreeStatus { get; set; }

    /// <summary>
    /// 获取或设置同意状态改变的具体时间（Unix 时间戳，秒；UTC）。
    /// </summary>
    [JsonPropertyName("status_change_time")]
    public long? StatusChangeTime { get; set; }
}
