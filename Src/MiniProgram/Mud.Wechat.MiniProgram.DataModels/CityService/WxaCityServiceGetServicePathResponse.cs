// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 获取城市服务限定页面链接回包（<c>POST /cityservice/getservicepath</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方示例字段拼写不一致（照录）</b>：字段表为 <c>business_type</c>，专题页示例写成
/// <c>bussiness_type</c> ⇒ 本类按字段表拼写建模。</para>
/// <para><c>app_id</c> / <c>username</c> 仅 page_type 为 5 时返回；<c>query_string</c> 仅 page_type 为 0 时返回。</para>
/// <para>错误码：83200 服务不存在（no exist service）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceGetServicePathResponse : WxaResponse
{
    /// <summary>结果路径。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>类型。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>小程序 id（page_type 为 5 时返回）。</summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    /// <summary>原始 id（page_type 为 5 时返回）。</summary>
    [JsonPropertyName("username")]
    public string? UserName { get; set; }

    /// <summary>path 的参数（page_type 为 0 时返回）。</summary>
    [JsonPropertyName("query_string")]
    public string? QueryString { get; set; }
}
