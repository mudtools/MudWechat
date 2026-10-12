// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.OfficialAccount.DataModels.Semantic;

/// <summary>
/// 语义理解结果（官方 <c>semantic</c> 对象）。
/// </summary>
/// <remarks>
/// <para>
/// <b><see cref="Details"/> 以原始 JSON 承载（有意裁决）</b>：官方 details 的结构随
/// <see cref="Category"/> 有二十余种形态（机票/天气/音乐/……），且随官方演进不受控；
/// SDK 若逐形态建模将引入一大族开放多态 DTO（与本仓「不做运行时多态」红线冲突）——
/// 故以 <see cref="JsonElement"/> 原样透出，由调用方按服务类型解析。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Semantic")]
public class MpSemanticResult
{
    /// <summary>获取或设置服务类型（官方 <c>type</c>；与请求 <c>type</c> 一致）。</summary>
    [JsonPropertyName("type")]
    public string? Category { get; set; }

    /// <summary>获取或设置语义意图（官方 <c>intent</c>）。</summary>
    [JsonPropertyName("intent")]
    public string? Intent { get; set; }

    /// <summary>获取或设置本次查询文本（官方 <c>query</c>）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>获取或设置语义细节（官方 <c>details</c>；结构随服务类型不同——原样 JSON，见类型级 remarks）。</summary>
    [JsonPropertyName("details")]
    public JsonElement? Details { get; set; }
}
