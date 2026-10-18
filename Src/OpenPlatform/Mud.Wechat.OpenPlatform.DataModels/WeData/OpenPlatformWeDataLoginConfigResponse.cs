// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.WeData;

/// <summary>
/// 查询登录配置应答（<c>POST /wedata/wedata_get_login_config</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataGetLoginConfig.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WeData")]
public class OpenPlatformWeDataLoginConfigResponse : OpenPlatformResponse
{
    /// <summary>服务商 AppId（官方 <c>component_appid</c>）。</summary>
    [JsonPropertyName("component_appid")]
    public string? ComponentAppId { get; set; }

    /// <summary>服务商昵称（官方 <c>component_nickname</c>）。</summary>
    [JsonPropertyName("component_nickname")]
    public string? ComponentNickname { get; set; }

    /// <summary>反查地址（官方 <c>recheck_url</c>）。</summary>
    [JsonPropertyName("recheck_url")]
    public string? RecheckUrl { get; set; }

    /// <summary>关联小程序列表（官方 <c>appinfo</c>）。</summary>
    [JsonPropertyName("appinfo")]
    public List<OpenPlatformWeDataAppInfo>? AppInfo { get; set; }
}
