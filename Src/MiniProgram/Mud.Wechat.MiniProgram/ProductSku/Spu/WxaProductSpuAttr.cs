// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>SPU 属性（商品级属性键值对，与 SKU 销售属性 <see cref="WxaProductSkuAttr"/> 不同）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSpuAttr
{
    /// <summary>属性键 key（属性自定义用）。</summary>
    [JsonPropertyName("attr_key")]
    public string? AttrKey { get; set; }

    /// <summary>属性值（属性自定义用）。</summary>
    [JsonPropertyName("attr_value")]
    public string? AttrValue { get; set; }
}
