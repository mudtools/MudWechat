// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「赠品子域」（6 端点：gift/add、gift/get、gift/list/get、gift/onsale/set、
// gift/stock/update、gift/update）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。
// gift/list/get 请求/响应与商品列表字段集一致 ⇒ 分别复用 ChannelsProductListRequest / ChannelsProductIdListResponse；
// gift/stock/update 与商品库存更新字段集一致 ⇒ 复用 ChannelsUpdateStockRequest。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>库存修改对象（官方 <c>stock_diff</c>，<c>product/gift/update</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftStockDiff
{
    /// <summary>获取或设置库存修改类型（官方 <c>diff_type</c>：1 增加，2 减少）。</summary>
    [JsonPropertyName("diff_type")]
    public int? DiffType { get; set; }

    /// <summary>获取或设置增加、减少或者设置的库存值（官方 <c>num</c>）。</summary>
    [JsonPropertyName("num")]
    public long? Num { get; set; }
}
