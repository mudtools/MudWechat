// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 库存信息（官方 <c>stock_info</c> 对象：<c>diff_type</c> + <c>num</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsStockInfo
{
    /// <summary>获取或设置修改类型（官方 <c>diff_type</c>：1 增加，2 减少，3 设置；建议使用 1 或 2）。</summary>
    [JsonPropertyName("diff_type")]
    public int? DiffType { get; set; }

    /// <summary>获取或设置增加、减少或者设置的库存值（官方 <c>num</c>）。</summary>
    [JsonPropertyName("num")]
    public long? Num { get; set; }
}
