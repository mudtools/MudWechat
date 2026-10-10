// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表多选题的校验设置（官方 <c>checkbox_setting</c>；不填则不会校验）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：<c>number</c> 不能超过选项 option_item 个数；<c>type</c> 为 1/2/3 时必须指定且大于 0。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormCheckboxSetting
{
    /// <summary>获取或设置是否增加「其他」选项（官方 <c>add_other_option</c>）。</summary>
    [JsonPropertyName("add_other_option")]
    public bool? AddOtherOption { get; set; }

    /// <summary>
    /// 获取或设置多选类型（官方 <c>type</c>，默认 0），结合 <c>number</c> 使用。
    /// 官方取值：<c>0</c> 不限制可选数量、<c>1</c> 至少选择、<c>2</c> 最多选择、<c>3</c> 固定选择。
    /// </summary>
    [JsonPropertyName("type")]
    public uint? Type { get; set; }

    /// <summary>获取或设置可勾选数量限制（官方 <c>number</c>），<c>type</c> 为 1/2/3 时需指定且大于 0、不能超过选项个数。</summary>
    [JsonPropertyName("number")]
    public uint? Number { get; set; }
}
