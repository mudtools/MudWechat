// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>获取性能 / 访问来源 / 客户端版本通用请求体（<c>date</c> + <c>module</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>operation/api_getperformance.html</c>、<c>api_getscenelist.html</c>、<c>api_getversionlist.html</c>。</para>
/// <para><c>module</c> 取值：<c>all</c> / <c>js</c> / <c>network</c> / <c>run</c>（以官方页面为准）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaLogDateModuleRequest
{
    /// <summary>日期（<c>date</c>，必填；格式 <c>yyyyMMdd</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>模块（<c>module</c>，必填）：<c>all</c> / <c>js</c> / <c>network</c> / <c>run</c>。</summary>
    [JsonPropertyName("module")]
    public string? Module { get; set; }
}
