// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 区域坐标（官方 <c>location</c>）。
/// </summary>
/// <remarks>
/// <b>类型冲突口径（标量取官方返回示例）</b>：官方字段表标 <c>lat</c>/<c>lng</c> 为 <c>string</c>，
/// 但返回示例为<b>数字</b>；本域同类字段在 <c>search_map_poi</c> 页字段表即标 <c>number</c>
/// ⇒ 字段表类型列在本域系统性失真。按「标量类型冲突取官方返回示例」建模为 <c>double</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpDistrictLocation
{
    /// <summary>获取或设置纬度（官方 <c>lat</c>；字段表标 string、示例为数字，取示例口径）。</summary>
    [JsonPropertyName("lat")]
    public double? Latitude { get; set; }

    /// <summary>获取或设置经度（官方 <c>lng</c>；字段表标 string、示例为数字，取示例口径）。</summary>
    [JsonPropertyName("lng")]
    public double? Longitude { get; set; }
}
