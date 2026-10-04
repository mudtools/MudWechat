// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 禁用/启用邮箱账号请求体（<c>/cgi-bin/exmail/account/act_email</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class SetEmailAccountStatusRequest
{
    /// <summary>
    /// 获取或设置成员 UserID（官方 userid）。
    /// </summary>
    /// <remarks>
    /// <para>官方业务限制：userid 与 publicemail_id 至少应该传一项，同时传则只操作 userid；
    /// 不可禁用超管与企业创建人。</para>
    /// </remarks>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置业务邮箱 ID（官方 publicemail_id）。</summary>
    /// <remarks><para>官方业务限制：userid 与 publicemail_id 至少应该传一项，同时传则只操作 userid。</para></remarks>
    [JsonPropertyName("publicemail_id")]
    public long? PublicemailId { get; set; }

    /// <summary>获取或设置操作类型（官方 type，必填）：1 - 启用，2 - 禁用。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }
}
