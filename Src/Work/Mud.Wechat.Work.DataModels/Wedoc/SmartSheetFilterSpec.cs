// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 过滤设置（官方 FilterSpec）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetFilterSpec
{
    /// <summary>
    /// 获取或设置多个 <c>conditions</c> 的组合方式（官方 <c>conjunction</c>，必填）。
    /// 官方取值：<c>CONJUNCTION_AND</c> 使用 AND 组合、<c>CONJUNCTION_OR</c> 使用 OR 组合。
    /// </summary>
    [JsonPropertyName("conjunction")]
    public string? Conjunction { get; set; }

    /// <summary>获取或设置判断条件列表（官方 <c>conditions</c>，必填）。</summary>
    [JsonPropertyName("conditions")]
    public List<SmartSheetCondition>? Conditions { get; set; }
}
