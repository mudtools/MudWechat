// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>免审更新团购优惠（<c>updatelocalauditfree</c>）请求体。</summary>
/// <remarks>
/// 官方契约：仅针对<b>已上线</b>的本地团购优惠；更新时 <c>sku_id</c> 必填；
/// 本接口不支持直接设置库存数量，须调用修改库存接口。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsAuditFreeUpdateLocallifeProductRequest
{
    /// <summary>获取或设置平台生成的商品 ID（官方 <c>product_id</c>，string(uint64)，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置售卖单元列表（官方 <c>skus</c>，必填；更新时 sku_id 必填）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsLocallifeSkuRequest> Skus { get; set; } = new();
}
