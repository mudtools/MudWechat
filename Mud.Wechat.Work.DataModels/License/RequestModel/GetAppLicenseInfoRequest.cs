// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 获取应用的接口许可状态请求体（<c>/cgi-bin/license/get_app_license_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class GetAppLicenseInfoRequest
{
    /// <summary>获取或设置企业 id（官方必填）。</summary>
    [JsonPropertyName("corpid")]
    public string Corpid { get; set; } = string.Empty;

    /// <summary>获取或设置套件 id（官方必填）。</summary>
    [JsonPropertyName("suite_id")]
    public string SuiteId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置旧的多应用套件中的应用 id（官方可选，新开发者请忽略）。
    /// </summary>
    [JsonPropertyName("appid")]
    public long? Appid { get; set; }
}
