// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

/// <summary>
/// 更新企业已配置的「联系我」方式请求体（<c>/cgi-bin/externalcontact/update_contact_way</c>）。
/// <para>
/// <see cref="ConfigId"/> 为官方必填；传入字段将覆盖原配置；已失效的临时会话不允许编辑；
/// <see cref="User"/> / <see cref="Party"/> 须在应用可见范围内；
/// <see cref="MarkSource"/> 只能由创建该「联系我」的应用进行更新。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class UpdateContactWayRequest
{
    /// <summary>
    /// 获取或设置联系方式的配置 id（官方必填）。
    /// </summary>
    [JsonPropertyName("config_id")]
    public string? ConfigId { get; set; }

    /// <summary>
    /// 获取或设置联系方式的备注信息（最多 30 个字符；传入即覆盖原值）。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 获取或设置添加时是否无需验证。
    /// </summary>
    [JsonPropertyName("skip_verify")]
    public bool? SkipVerify { get; set; }

    /// <summary>
    /// 获取或设置小程序中联系的按钮样式（仅小程序中联系类型配置生效）。
    /// </summary>
    [JsonPropertyName("style")]
    public int? Style { get; set; }

    /// <summary>
    /// 获取或设置自定义渠道标识（传入即覆盖原值）。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置使用该联系方式的企业成员 userid 列表（传入即整体覆盖原列表，须在应用可见范围内）。
    /// </summary>
    [JsonPropertyName("user")]
    public List<string>? User { get; set; }

    /// <summary>
    /// 获取或设置使用该联系方式的部门 id 列表（传入即整体覆盖原列表，仅多人类型有效）。
    /// </summary>
    [JsonPropertyName("party")]
    public List<long>? Party { get; set; }

    /// <summary>
    /// 获取或设置临时二维码的有效期（秒；仅临时会话模式生效）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; set; }

    /// <summary>
    /// 获取或设置临时会话的有效期（秒；仅临时会话模式生效）。
    /// </summary>
    [JsonPropertyName("chat_expires_in")]
    public int? ChatExpiresIn { get; set; }

    /// <summary>
    /// 获取或设置可进行临时会话的客户 unionid（仅临时会话模式生效）。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string? Unionid { get; set; }

    /// <summary>
    /// 获取或设置是否标记客户添加来源为该应用创建的「联系我」
    /// （仅对「营销获客」应用生效，且只能由创建该「联系我」的应用更新）。
    /// </summary>
    [JsonPropertyName("mark_source")]
    public bool? MarkSource { get; set; }

    /// <summary>
    /// 获取或设置结束语（仅临时会话模式可设置；传入即整体覆盖原值）。
    /// </summary>
    [JsonPropertyName("conclusions")]
    public ContactWayConclusion? Conclusions { get; set; }
}
