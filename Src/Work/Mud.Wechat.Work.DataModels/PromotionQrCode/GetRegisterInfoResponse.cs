// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PromotionQrCode;

/// <summary>
/// 查询注册状态响应体（<c>/cgi-bin/service/get_register_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PromotionQrCode")]
public class GetRegisterInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置企业的 corpid（企业注册成功时返回）。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>
    /// 获取或设置通讯录迁移的凭证信息。
    /// <para>官方说明：仅当注册推广包开启通讯录迁移接口时返回该参数。</para>
    /// </summary>
    [JsonPropertyName("contact_sync")]
    public ContactSyncCredential? ContactSync { get; set; }

    /// <summary>
    /// 获取或设置授权管理员的信息。
    /// </summary>
    [JsonPropertyName("auth_user_info")]
    public RegisterAuthUserInfo? AuthUserInfo { get; set; }

    /// <summary>
    /// 获取或设置用户自定义的状态值（参数值由「获取注册码」接口指定）。
    /// <para>官方说明：若获取注册码时未指定，则返回结果中无该字段。</para>
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置推广包 ID。
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }
}
