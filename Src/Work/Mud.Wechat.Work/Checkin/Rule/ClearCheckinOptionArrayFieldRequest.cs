// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 清空打卡规则数组元素请求体（<c>/cgi-bin/checkin/clear_checkin_option_array_field</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：打卡规则仅可由该规则的创建应用修改；wifimac_infos 与 loc_infos 不可同时为空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ClearCheckinOptionArrayFieldRequest
{
    /// <summary>
    /// 获取或设置打卡规则 id（官方必填）。
    /// </summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>
    /// 获取或设置清空的字段标识（官方必填）：1 - 清空 spe_workdays 字段；2 - 清空 spe_offdays 字段；
    /// 3 - 清空 wifimac_infos 字段；4 - 清空 loc_infos 字段（wifimac_infos 和 loc_infos 不可同时为空）。
    /// </summary>
    [JsonPropertyName("clear_field")]
    public List<int>? ClearField { get; set; }

    /// <summary>
    /// 获取或设置是否立即生效（默认为 false）。
    /// </summary>
    [JsonPropertyName("effective_now")]
    public bool? EffectiveNow { get; set; }
}
