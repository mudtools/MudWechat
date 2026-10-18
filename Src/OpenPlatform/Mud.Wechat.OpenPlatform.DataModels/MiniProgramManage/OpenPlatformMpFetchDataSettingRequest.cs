// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「数据预拉取 / 周期性运营数据配置」（<c>fetchdatasetting</c>）请求。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getFetchdataSetting.html"/>。
/// </summary>
/// <remarks>官方约束：以 <c>action</c> 区分操作，域名须已配置到业务域名白名单。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpFetchDataSettingRequest
{
    /// <summary>操作类型（官方必填 <c>action</c>，string）。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>是否开启数据预拉取（官方 <c>is_pre_fetch_open</c>，boolean）。</summary>
    [JsonPropertyName("is_pre_fetch_open")]
    public bool? IsPreFetchOpen { get; set; }

    /// <summary>数据预拉取类型（官方 <c>pre_fetch_type</c>，number）。</summary>
    [JsonPropertyName("pre_fetch_type")]
    public long? PreFetchType { get; set; }

    /// <summary>数据预拉取 URL（官方 <c>pre_fetch_url</c>，string）。</summary>
    [JsonPropertyName("pre_fetch_url")]
    public string? PreFetchUrl { get; set; }

    /// <summary>数据预拉取环境（官方 <c>pre_env</c>，string）。</summary>
    [JsonPropertyName("pre_env")]
    public string? PreEnv { get; set; }

    /// <summary>数据预拉取回调函数名（官方 <c>pre_function_name</c>，string）。</summary>
    [JsonPropertyName("pre_function_name")]
    public string? PreFunctionName { get; set; }

    /// <summary>是否开启周期性刷新（官方 <c>is_period_fetch_open</c>，boolean）。</summary>
    [JsonPropertyName("is_period_fetch_open")]
    public bool? IsPeriodFetchOpen { get; set; }

    /// <summary>周期性刷新类型（官方 <c>period_fetch_type</c>，number）。</summary>
    [JsonPropertyName("period_fetch_type")]
    public long? PeriodFetchType { get; set; }

    /// <summary>周期性刷新 URL（官方 <c>period_fetch_url</c>，string）。</summary>
    [JsonPropertyName("period_fetch_url")]
    public string? PeriodFetchUrl { get; set; }

    /// <summary>周期性刷新环境（官方 <c>period_env</c>，string）。</summary>
    [JsonPropertyName("period_env")]
    public string? PeriodEnv { get; set; }

    /// <summary>周期性刷新回调函数名（官方 <c>period_function_name</c>，string）。</summary>
    [JsonPropertyName("period_function_name")]
    public string? PeriodFunctionName { get; set; }
}
