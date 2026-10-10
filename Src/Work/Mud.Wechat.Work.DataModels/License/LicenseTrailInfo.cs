// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 应用接口许可试用期信息（<c>trail_info</c>，<c>/cgi-bin/license/get_app_license_info</c> 响应嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档对 JSON 字段名的原文拼写为 <c>trail_info</c>（非 <c>trial_info</c>），本 SDK 照抄原文。
/// </para>
/// <para>官方口径：仅当 license_status 为 1 且应用有试用期时返回该字段；服务商测试企业、历史迁移应用无试用期。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseTrailInfo
{
    /// <summary>获取或设置接口许可试用开始时间（unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置接口许可试用到期时间（unix 时间戳）。
    /// <para>官方口径：若企业多次安装卸载同一个第三方应用，以第一次安装的时间为试用期开始时间，
    /// 第一次安装完 90 天后为结束试用时间。</para>
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
