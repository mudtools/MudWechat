// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 假期额度计算规则区间（quota_rules.list 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class VacationQuotaRuleItem
{
    /// <summary>
    /// 获取或设置区间发放时长，单位为秒。
    /// </summary>
    [JsonPropertyName("quota")]
    public long? Quota { get; set; }

    /// <summary>
    /// 获取或设置区间开始点，单位为年。
    /// </summary>
    [JsonPropertyName("begin")]
    public int? Begin { get; set; }

    /// <summary>
    /// 获取或设置区间结束点，无穷大则为 0，单位为年。
    /// </summary>
    [JsonPropertyName("end")]
    public int? End { get; set; }
}
