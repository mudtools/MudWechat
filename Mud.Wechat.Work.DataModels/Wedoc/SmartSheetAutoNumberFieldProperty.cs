// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 自动编号类型字段属性（官方 AutoNumberFieldProperty）。
/// </summary>
/// <remarks>
/// <para>
/// 官方对同一字段在「添加字段」文档中拼写为 <c>typ</c>，在「查询字段」文档中拼写为 <c>type</c>；本模型同时提供两个互斥的可空属性以精确承载两种形态。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetAutoNumberFieldProperty
{
    /// <summary>
    /// 获取或设置编号类型（官方 <c>typ</c>，新增 / 更新字段侧官方拼写）。
    /// 官方取值：<c>NUMBER_TYPE_INCR</c> 自增数字类型、<c>NUMBER_TYPE_CUSTOM</c> 自定义类型。
    /// </summary>
    [JsonPropertyName("typ")]
    public string? Typ { get; set; }

    /// <summary>
    /// 获取或设置编号类型（官方 <c>type</c>，查询字段侧官方拼写）。
    /// 官方取值：<c>NUMBER_TYPE_INCR</c> 自增数字类型、<c>NUMBER_TYPE_CUSTOM</c> 自定义类型。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置自定义规则（官方 <c>rules</c>）。</summary>
    [JsonPropertyName("rules")]
    public List<SmartSheetNumberRule>? Rules { get; set; }

    /// <summary>获取或设置自定义规则是否应用于已有编号（官方 <c>reformat_existing_record</c>）。</summary>
    [JsonPropertyName("reformat_existing_record")]
    public bool? ReformatExistingRecord { get; set; }
}
