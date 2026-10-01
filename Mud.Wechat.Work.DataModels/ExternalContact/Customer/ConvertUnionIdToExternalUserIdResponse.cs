// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// unionid 转换为第三方 external_userid 响应体
/// （<c>/cgi-bin/idconvert/unionid_to_external_userid</c>）。
/// </summary>
/// <remarks>
/// 微信用户尚未成为企业客户、或跟进人 / 群主不在应用可见范围时，不返回 external_userid 而返回 pending_id
/// （90 天内有效，仅用于关联 unionid 与 external_userid，不能当成 external_userid 调用接口）。
/// </remarks>
public class ConvertUnionIdToExternalUserIdResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置该授权企业的外部联系人 ID。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置临时外部联系人 ID（该微信账号尚未成为企业客户时返回，有效期 90 天）。
    /// </summary>
    [JsonPropertyName("pending_id")]
    public string? PendingId { get; set; }
}
