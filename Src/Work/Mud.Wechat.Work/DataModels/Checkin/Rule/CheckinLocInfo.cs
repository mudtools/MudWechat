// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡地点位置打卡信息（打卡规则 <c>loc_infos</c> 元素，请求/响应共用形态）。
/// </summary>
/// <remarks>
/// <para>官方限制：经度/纬度不可为空（经度 -180 度到 180 度，纬度 -90 度到 90 度）；loc_title/loc_detail 不可为空且字符个数不可超过 40；距离枚举 100、200、300、400、500、600、700、800、900、1000、1500、2000、2500、3000 米；wifi/地点个数不可超过 500。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinLocInfo
{
    /// <summary>获取或设置位置打卡地点纬度（实际纬度的 1000000 倍，与腾讯地图一致采用 GCJ-02 坐标系统标准）。</summary>
    [JsonPropertyName("lat")]
    public long? Lat { get; set; }

    /// <summary>获取或设置位置打卡地点经度（实际经度的 1000000 倍，与腾讯地图一致采用 GCJ-02 坐标系统标准）。</summary>
    [JsonPropertyName("lng")]
    public long? Lng { get; set; }

    /// <summary>获取或设置位置打卡地点名称（不可为空，字符个数不可超过 40 个）。</summary>
    [JsonPropertyName("loc_title")]
    public string? LocTitle { get; set; }

    /// <summary>获取或设置位置打卡地点详情（不可为空，字符个数不可超过 40 个）。</summary>
    [JsonPropertyName("loc_detail")]
    public string? LocDetail { get; set; }

    /// <summary>获取或设置允许打卡范围（米；取值枚举 100~3000）。</summary>
    [JsonPropertyName("distance")]
    public int? Distance { get; set; }
}
