// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 获取应用的接口许可状态响应体（<c>/cgi-bin/license/get_app_license_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class GetAppLicenseInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置 license 检查开启状态：<c>0</c>-未开启 license 检查状态（未迁移的历史授权的第三方应用
    /// （接入版本付费）或者未达到拦截时间的历史授权的的第三方应用（未接入版本付费）以及代开发应用）/
    /// <c>1</c>-已开启 license 检查状态。若开启且已过试用期，则需要为企业购买 license 账号才可以使用。
    /// </summary>
    [JsonPropertyName("license_status")]
    public int? LicenseStatus { get; set; }

    /// <summary>获取或设置应用 license 试用期信息（仅当 license_status 为 1 且应用有试用期时返回该字段）。</summary>
    [JsonPropertyName("trail_info")]
    public LicenseTrailInfo? TrailInfo { get; set; }

    /// <summary>获取或设置接口开启拦截校验时间（unix 时间戳）。开始拦截校验后，无接口许可将会被拦截，有接口许可将不会被拦截。</summary>
    [JsonPropertyName("license_check_time")]
    public long? LicenseCheckTime { get; set; }
}
