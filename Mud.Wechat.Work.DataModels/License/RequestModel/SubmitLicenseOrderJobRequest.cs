// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 提交续期订单请求体（<c>/cgi-bin/license/submit_order_job</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class SubmitLicenseOrderJobRequest
{
    /// <summary>获取或设置任务 id（官方必填）。</summary>
    [JsonPropertyName("jobid")]
    public string Jobid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置下单人（官方必填），服务商企业内成员的明文 userid。
    /// <para>官方约束：该 userid 必须登录过企业微信，并且企业微信已绑定微信，
    /// 且必须为服务商企业内具有「购买接口许可」权限的管理员。</para>
    /// </summary>
    [JsonPropertyName("buyer_userid")]
    public string BuyerUserid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置账号购买时长（官方必填）。
    /// <para>官方约束：<see cref="LicenseAccountDuration.Months"/> 与
    /// <see cref="LicenseAccountDuration.NewExpireTime"/> 二者填其一。</para>
    /// </summary>
    [JsonPropertyName("account_duration")]
    public LicenseAccountDuration AccountDuration { get; set; } = new LicenseAccountDuration();
}
