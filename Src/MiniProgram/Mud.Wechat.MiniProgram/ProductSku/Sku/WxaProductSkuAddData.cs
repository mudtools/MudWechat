// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>添加 SKU 应答的 <c>data</c> 载荷（单对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSkuAddData
{
    /// <summary>小商店内部 SKU ID。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>创建时间（形如 <c>2020-03-25 12:05:25</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}
