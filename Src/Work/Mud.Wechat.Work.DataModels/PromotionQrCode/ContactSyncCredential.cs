// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PromotionQrCode;

/// <summary>
/// 通讯录迁移凭证信息（<c>/cgi-bin/service/get_register_info</c> 响应的 <c>contact_sync</c> 字段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>SDK 刻意不缓存该凭证</b>：官方契约下 <c>access_token</c> 有 30 分钟有效期、且被「设置通讯录同步完成」
/// 立即作废，同时具备<b>全部通讯录读写权限</b>；且它既不是应用自身 <c>access_token</c>，
/// 也不是服务商 <c>provider_access_token</c>（官方两处均特别标注「请注意与 provider_access_token 的区别」）。
/// 故 SDK 令牌基座不管理它，调用方须自行保存并经
/// <c>IWechatWorkPromotionQrCodeContactSyncService</c> 的显式 <c>access_token</c> 参数传入。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PromotionQrCode")]
public class ContactSyncCredential
{
    /// <summary>
    /// 获取或设置通讯录 API 接口调用凭证（有全部通讯录读写权限）。
    /// <para>官方提示：请注意与 <c>provider_access_token</c> 的区别。</para>
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 <c>access_token</c> 凭证的有效时间（秒，官方示例值为 1800）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
