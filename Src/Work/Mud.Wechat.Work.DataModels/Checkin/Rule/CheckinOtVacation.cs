// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则加班配置 V2 同步假期信息（<c>ot_info_v2.*.vacation</c>，记为调休）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtVacation
{
    /// <summary>获取或设置调休比例（百分比；默认值为 100 表示 1:1，最大为 1:30，即 trans_ratio 最大值为 3000）。</summary>
    [JsonPropertyName("trans_ratio")]
    public int? TransRatio { get; set; }

    /// <summary>获取或设置是否自动关联假勤（默认为 true）。</summary>
    [JsonPropertyName("sync_vacation")]
    public bool? SyncVacation { get; set; }
}
