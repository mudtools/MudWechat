// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 创建打卡规则请求体（<c>/cgi-bin/checkin/add_checkin_option</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：创建打卡规则时 groupid 无需传入，该字段会被忽略；wifimac_infos 与 loc_infos 不能同时为空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class AddCheckinOptionRequest
{
    /// <summary>
    /// 获取或设置打卡规则详细定义（官方视情况必填；创建时 groupid 无需传入，该字段会被忽略）。
    /// </summary>
    [JsonPropertyName("group")]
    public CheckinRuleGroup? Group { get; set; }

    /// <summary>
    /// 获取或设置是否立即生效（默认为 false）。
    /// </summary>
    [JsonPropertyName("effective_now")]
    public bool? EffectiveNow { get; set; }
}
