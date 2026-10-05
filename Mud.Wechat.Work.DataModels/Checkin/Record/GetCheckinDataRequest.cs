// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 获取打卡记录数据请求体（<c>/cgi-bin/checkin/getcheckindata</c>；三种应用类型请求形态一致）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：获取记录时间跨度不超过 30 天（自建/代开发口径）；用户列表不超过 100 个，若用户超过 100 个请分批获取；
/// 有打卡记录即可获取打卡数据，与当前「打卡应用」是否开启无关。
/// 第三方应用文档页口径：上下班打卡时间跨度不能超过一个月，外出打卡/全部打卡不能超过三天。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class GetCheckinDataRequest
{
    /// <summary>获取或设置打卡类型（自建/代开发官方必填）：1 - 上下班打卡；2 - 外出打卡；3 - 全部打卡（第三方文档页标注为非必填）。</summary>
    [JsonPropertyName("opencheckindatatype")]
    public int? Opencheckindatatype { get; set; }

    /// <summary>获取或设置获取打卡记录的开始时间（Unix 时间戳，官方必填）。</summary>
    [JsonPropertyName("starttime")]
    public long? Starttime { get; set; }

    /// <summary>获取或设置获取打卡记录的结束时间（Unix 时间戳，官方必填）。</summary>
    [JsonPropertyName("endtime")]
    public long? Endtime { get; set; }

    /// <summary>获取或设置需要获取打卡记录的用户列表（官方必填；不超过 100 个，若超过 100 个请分批获取）。</summary>
    [JsonPropertyName("useridlist")]
    public List<string>? UserIdList { get; set; }
}
