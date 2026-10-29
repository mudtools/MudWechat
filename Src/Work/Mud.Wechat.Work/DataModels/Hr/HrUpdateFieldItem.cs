// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Hr;

/// <summary>
/// 人事助手字段更新项（<c>/cgi-bin/hr/update_staff_info</c> 的 update_items / insert_items.item 内嵌结构）。
/// </summary>
/// <remarks>
/// <para>
/// 官方按字段值类型填写对应的内容字段：<c>value_string</c>（字符串）、<c>value_uint64</c>（64位非负整数）、
/// <c>value_uint32</c>（32位非负整数）、<c>value_int64</c>（64位整数）、<c>value_mobile</c>（电话号码，
/// 结构见 <see cref="HrMobileValue"/>，不填/空串视为整个电话号码字段清空）——
/// 除对应字段外，在其他字段填写的内容将被忽略。
/// 官方标注不支持更新的字段：11006（年龄）、11012（社会工龄）、12004（员工状态）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Hr")]
public class HrUpdateFieldItem
{
    /// <summary>获取或设置字段 id（官方 fieldid，必填）。</summary>
    [JsonPropertyName("fieldid")]
    public long? Fieldid { get; set; }

    /// <summary>获取或设置下标（官方 sub_idx，可重复组中的字段下标；非可重复组中的字段时需填 0）。</summary>
    [JsonPropertyName("sub_idx")]
    public long? SubIdx { get; set; }

    /// <summary>获取或设置字符串字段值（官方 value_string）。</summary>
    [JsonPropertyName("value_string")]
    public string? ValueString { get; set; }

    /// <summary>获取或设置 64 位非负整数字段值（官方 value_uint64；时间类型字段为此形态的时间戳）。</summary>
    [JsonPropertyName("value_uint64")]
    public long? ValueUint64 { get; set; }

    /// <summary>获取或设置 32 位非负整数字段值（官方 value_uint32；选项类型字段为此形态的枚举值）。</summary>
    [JsonPropertyName("value_uint32")]
    public long? ValueUint32 { get; set; }

    /// <summary>获取或设置 64 位整数字段值（官方 value_int64）。</summary>
    [JsonPropertyName("value_int64")]
    public long? ValueInt64 { get; set; }

    /// <summary>获取或设置电话号码字段值（官方 value_mobile）。</summary>
    [JsonPropertyName("value_mobile")]
    public HrMobileValue? ValueMobile { get; set; }
}
