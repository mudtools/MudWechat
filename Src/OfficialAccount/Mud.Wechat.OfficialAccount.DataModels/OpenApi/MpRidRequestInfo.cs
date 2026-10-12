// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OpenApi;

/// <summary>rid 对应的请求详情（官方 <c>request</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpRidRequestInfo
{
    /// <summary>获取或设置发起请求的时间戳（官方 <c>invoke_time</c>）。</summary>
    [JsonPropertyName("invoke_time")]
    public long? InvokeTime { get; set; }

    /// <summary>获取或设置请求毫秒级耗时（官方 <c>cost_in_ms</c>）。</summary>
    [JsonPropertyName("cost_in_ms")]
    public int? CostInMs { get; set; }

    /// <summary>获取或设置请求的 URL 参数（官方 <c>request_url</c>）。</summary>
    [JsonPropertyName("request_url")]
    public string? RequestUrl { get; set; }

    /// <summary>获取或设置 post 请求的请求参数（官方 <c>request_body</c>）。</summary>
    [JsonPropertyName("request_body")]
    public string? RequestBody { get; set; }

    /// <summary>获取或设置接口请求返回参数（官方 <c>response_body</c>）。</summary>
    [JsonPropertyName("response_body")]
    public string? ResponseBody { get; set; }

    /// <summary>获取或设置接口请求的客户端 ip（官方 <c>client_ip</c>）。</summary>
    [JsonPropertyName("client_ip")]
    public string? ClientIp { get; set; }
}
