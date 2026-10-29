// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Hr;

/// <summary>
/// 人事助手员工字段信息（<c>/cgi-bin/hr/get_staff_info</c> 响应内嵌结构）。
/// </summary>
/// <remarks>
/// <para>
/// 官方按 <c>value_type</c> 决定承载字段值的字段名：<c>value_string</c>（1 字符串）、
/// <c>value_uint64</c>（2 64位非负整数）、<c>value_uint32</c>（3 32位非负整数）、
/// <c>value_int64</c>（4 64位整数）、<c>value_mobile</c>（5 电话号码，结构见 <see cref="HrMobileValue"/>）、
/// <c>value_file</c>（6 文件，结构见 <see cref="HrFileValue"/>）——本模型以可空并列属性覆盖全部形态，
/// 读取时按 <see cref="ValueType"/> 取对应属性。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Hr")]
public class HrFieldInfo
{
    /// <summary>获取或设置字段 id（官方 fieldid）。</summary>
    [JsonPropertyName("fieldid")]
    public long? Fieldid { get; set; }

    /// <summary>获取或设置下标（官方 sub_idx，可重复字段组中的第几组，非可重复组为 0）。</summary>
    [JsonPropertyName("sub_idx")]
    public long? SubIdx { get; set; }

    /// <summary>获取或设置查询结果（官方 result）：1 - 成功；2 - 失败；3 - 字段未找到；5 - 不支持获取的字段类型。</summary>
    [JsonPropertyName("result")]
    public long? Result { get; set; }

    /// <summary>获取或设置字段值的类型（官方 value_type，1~6，含义见类注释）。</summary>
    [JsonPropertyName("value_type")]
    public long? ValueType { get; set; }

    /// <summary>获取或设置字符串字段值（官方 value_string，value_type = 1 时返回）。</summary>
    [JsonPropertyName("value_string")]
    public string? ValueString { get; set; }

    /// <summary>获取或设置 64 位非负整数字段值（官方 value_uint64，value_type = 2 时返回；时间类型字段为此形态的时间戳）。</summary>
    [JsonPropertyName("value_uint64")]
    public long? ValueUint64 { get; set; }

    /// <summary>获取或设置 32 位非负整数字段值（官方 value_uint32，value_type = 3 时返回；选项类型字段为此形态的枚举值）。</summary>
    [JsonPropertyName("value_uint32")]
    public long? ValueUint32 { get; set; }

    /// <summary>获取或设置 64 位整数字段值（官方 value_int64，value_type = 4 时返回）。</summary>
    [JsonPropertyName("value_int64")]
    public long? ValueInt64 { get; set; }

    /// <summary>获取或设置电话号码字段值（官方 value_mobile，value_type = 5 时返回）。</summary>
    [JsonPropertyName("value_mobile")]
    public HrMobileValue? ValueMobile { get; set; }

    /// <summary>获取或设置文件字段值（官方 value_file，value_type = 6 时返回）。</summary>
    [JsonPropertyName("value_file")]
    public HrFileValue? ValueFile { get; set; }
}
