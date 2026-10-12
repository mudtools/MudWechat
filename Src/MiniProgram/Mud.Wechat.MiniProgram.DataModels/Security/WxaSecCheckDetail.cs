// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Security;

/// <summary>命中明细（<c>detail[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaSecCheckDetail
{
    /// <summary>策略（<c>strategy</c>）：命中策略的名称（errcode 为 87014 时给出具体命中策略与子标签）。</summary>
    [JsonPropertyName("strategy")]
    public string? Strategy { get; set; }

    /// <summary>策略下错误码（<c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public long? ErrorCode { get; set; }

    /// <summary>建议（<c>suggest</c>）。</summary>
    [JsonPropertyName("suggest")]
    public string? Suggest { get; set; }

    /// <summary>命中标签（<c>label</c>）。</summary>
    [JsonPropertyName("label")]
    public long? Label { get; set; }

    /// <summary>命中关键词（<c>keyword</c>）。</summary>
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }

    /// <summary>置信度（<c>prob</c>）。</summary>
    [JsonPropertyName("prob")]
    public double? Probability { get; set; }
}
