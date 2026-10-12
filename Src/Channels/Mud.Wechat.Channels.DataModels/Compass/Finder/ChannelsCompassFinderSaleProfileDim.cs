// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>人群维度指标（官方 <c>data_list</c> 数组元素，<c>getfindersaleprofiledata</c>）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：<c>dim_value</c> 参数表标注为 number、示例为字符串——按参数表（number）建模；与商家版（string）口径不同 ⇒ 不共用 DTO。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderSaleProfileDim
{
    /// <summary>获取或设置维度指标名（官方 <c>dim_key</c>）。</summary>
    [JsonPropertyName("dim_key")]
    public string? DimKey { get; set; }

    /// <summary>获取或设置维度指标值（官方 <c>dim_value</c>，参数表类型 number）。</summary>
    [JsonPropertyName("dim_value")]
    public long? DimValue { get; set; }
}
