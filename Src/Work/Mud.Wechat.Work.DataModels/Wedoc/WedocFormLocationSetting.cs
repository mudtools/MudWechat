// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表位置题的设置（官方 <c>location_setting</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormLocationSetting
{
    /// <summary>
    /// 获取或设置位置类型（官方 <c>location_type</c>）。
    /// 官方取值：<c>0</c> 省/市/区/街道+详细地址、<c>1</c> 省/市、<c>2</c> 省/市/区、<c>3</c> 省/市/区/街道、<c>4</c> 自动定位。
    /// </summary>
    [JsonPropertyName("location_type")]
    public uint? LocationType { get; set; }

    /// <summary>
    /// 获取或设置允许定位范围（官方 <c>distance_type</c>），仅 <c>location_type</c> 为 <c>4</c>（自动定位）时适用。
    /// 官方取值：<c>0</c> 当前位置、<c>1</c> 附近100米、<c>2</c> 附近200米、<c>3</c> 附近300米。
    /// </summary>
    [JsonPropertyName("distance_type")]
    public uint? DistanceType { get; set; }
}
