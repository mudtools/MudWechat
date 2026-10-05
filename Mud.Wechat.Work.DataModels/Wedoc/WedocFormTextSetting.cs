// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表文本题的题目设置（官方 <c>text_setting</c>；不填时使用默认设置）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：<c>validation_type</c> 为 0（字符个数）时适用 <c>validation_detail</c> 取值 1/2/3，
/// 此时 <c>char_len</c> 必须有值且大于 0、最大 4000；
/// 为 1（数字）时适用取值 4~11，<c>number_min</c> 适用于取值 5/6/9/10、<c>number_max</c> 适用于取值 7/8/9/10。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormTextSetting
{
    /// <summary>
    /// 获取或设置校验类型（官方 <c>validation_type</c>，默认 0）。
    /// 官方取值：<c>0</c> 字符个数、<c>1</c> 数字、<c>2</c> 电子邮箱、<c>3</c> 网址、<c>4</c> 身份证、
    /// <c>5</c> 手机号（大陆地区）、<c>6</c> 固定电话。
    /// </summary>
    [JsonPropertyName("validation_type")]
    public uint? ValidationType { get; set; }

    /// <summary>
    /// 获取或设置校验详情（官方 <c>validation_detail</c>）。
    /// 官方取值：<c>1</c> 字符数不超过、<c>2</c> 字符数不小于、<c>3</c> 字符数等于、<c>4</c> 数字没有限制、
    /// <c>5</c> 数字大于、<c>6</c> 数字大于等于、<c>7</c> 数字小于、<c>8</c> 数字小于等于、
    /// <c>9</c> 数字在范围之间、<c>10</c> 数字不在范围之间、<c>11</c> 数字为整数。
    /// </summary>
    [JsonPropertyName("validation_detail")]
    public uint? ValidationDetail { get; set; }

    /// <summary>获取或设置字符长度（官方 <c>char_len</c>），<c>validation_type</c> 为 0 时必须有值且大于 0、最大 4000。</summary>
    [JsonPropertyName("char_len")]
    public uint? CharLen { get; set; }

    /// <summary>获取或设置数字的区间左端（官方 <c>number_min</c>），<c>validation_detail</c> 取 5/6/9/10 时适用。</summary>
    [JsonPropertyName("number_min")]
    public double? NumberMin { get; set; }

    /// <summary>获取或设置数字的区间右端（官方 <c>number_max</c>），<c>validation_detail</c> 取 7/8/9/10 时适用。</summary>
    [JsonPropertyName("number_max")]
    public double? NumberMax { get; set; }
}
