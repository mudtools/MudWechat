// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Hr;

/// <summary>
/// 人事助手电话号码类型字段值结构（官方 value_mobile，value_type = 5 时出现）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Hr")]
public class HrMobileValue
{
    /// <summary>获取或设置电话号码的区号（官方 value_country_code，字符串）。</summary>
    [JsonPropertyName("value_country_code")]
    public string? ValueCountryCode { get; set; }

    /// <summary>获取或设置电话号码（官方 value_mobile，字符串；更新场景下不填/空串视为整个电话号码字段清空）。</summary>
    [JsonPropertyName("value_mobile")]
    public string? ValueMobile { get; set; }
}
