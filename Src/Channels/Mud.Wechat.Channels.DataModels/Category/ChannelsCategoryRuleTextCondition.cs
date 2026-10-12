// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>规则文本检查条件（官方 <c>text_conditions</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryRuleTextCondition
{
    /// <summary>获取或设置检查的字段（官方 <c>check_field</c>，见 <see cref="ChannelsCategoryCheckFields"/>）。</summary>
    [JsonPropertyName("check_field")]
    public int? CheckField { get; set; }

    /// <summary>获取或设置操作类型（官方 <c>oper_type</c>，见 <see cref="ChannelsCategoryOperTypes"/>）。</summary>
    [JsonPropertyName("oper_type")]
    public int? OperType { get; set; }

    /// <summary>获取或设置关键字列表（官方 <c>keywords</c>）。</summary>
    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }
}
