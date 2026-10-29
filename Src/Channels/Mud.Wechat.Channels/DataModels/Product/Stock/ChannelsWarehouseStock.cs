// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「库存子域」（4 端点：stock/get、stock/update、stock/batchget、stock/getflow）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>区域库存（官方 <c>warehouse_stocks</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsWarehouseStock
{
    /// <summary>获取或设置区域库存外部 id（官方 <c>out_warehouse_id</c>）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string? OutWarehouseId { get; set; }

    /// <summary>获取或设置区域库存数量（官方 <c>num</c>）。</summary>
    [JsonPropertyName("num")]
    public long? Num { get; set; }

    /// <summary>获取或设置区域库存的锁定库存（已下单未支付的库存，官方 <c>lock_stock</c>）数量。</summary>
    [JsonPropertyName("lock_stock")]
    public long? LockStock { get; set; }
}
