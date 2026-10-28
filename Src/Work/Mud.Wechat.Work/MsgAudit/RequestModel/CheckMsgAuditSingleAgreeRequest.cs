// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MsgAudit;

/// <summary>
/// 获取单聊会话同意情况请求体（<c>/cgi-bin/msgaudit/check_single_agree</c>）。
/// <para>
/// 官方业务限制：access_token 必须由「会话内容存档」应用 secret 获取；
/// 一次请求最多支持 100 个查询条目，超出会被拦截；调用频率不可超过 2500 次/分钟。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class CheckMsgAuditSingleAgreeRequest
{
    /// <summary>
    /// 获取或设置待查询的会话信息列表（官方必填，最多 100 条，
    /// 见 <see cref="MsgAuditAgreeQueryItem"/>）。
    /// </summary>
    [JsonPropertyName("info")]
    public List<MsgAuditAgreeQueryItem>? Info { get; set; }
}

/// <summary>
/// 单聊同意查询条目（<see cref="CheckMsgAuditSingleAgreeRequest.Info"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class MsgAuditAgreeQueryItem
{
    /// <summary>
    /// 获取或设置内部成员的 userid（官方必填）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置外部成员的 exteranalopenid（官方必填）。
    /// <para>官方文档原文即此拼写（<c>exteranalopenid</c>），本 SDK 照抄官方原文，勿改 external。</para>
    /// </summary>
    [JsonPropertyName("exteranalopenid")]
    public string? ExteranalOpenid { get; set; }
}
