// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>
/// 团购优惠售卖单元请求形态（官方 <c>skus[]</c> 元素；add/update/auditfree 三端点共用——字段集一致：
/// <c>sku_id</c> 更新与免审更新时必填（add 由平台生成、官方示例仅传 sale_price），<c>sale_price</c> 必填）。
/// </summary>
/// <remarks>官方契约：本组接口均不支持直接设置库存数量，须调用修改库存接口。</remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeSkuRequest
{
    /// <summary>获取或设置平台生成的 SKU ID（官方 <c>sku_id</c>；更新/免审更新时必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置实际售卖价格（官方 <c>sale_price</c>，必填，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long SalePrice { get; set; }
}
