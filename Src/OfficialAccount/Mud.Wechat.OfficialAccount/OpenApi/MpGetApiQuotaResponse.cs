// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OpenApi;

/// <summary>查询 API 调用额度（<c>openapi/quota/get</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<c>"/xxx/sns/xxx"</c> 类接口不支持本查询（76022）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpGetApiQuotaResponse : MpResponse
{
    /// <summary>获取或设置当日调用额度详情（官方 <c>quota</c>）。</summary>
    [JsonPropertyName("quota")]
    public MpApiQuota? Quota { get; set; }

    /// <summary>获取或设置普通调用频率限制（官方 <c>rate_limit</c>）。</summary>
    [JsonPropertyName("rate_limit")]
    public MpApiRateLimit? RateLimit { get; set; }

    /// <summary>获取或设置代调用频率限制（官方 <c>component_rate_limit</c>，字段同 rate_limit；非代调用形态可能缺省）。</summary>
    [JsonPropertyName("component_rate_limit")]
    public MpApiRateLimit? ComponentRateLimit { get; set; }
}
