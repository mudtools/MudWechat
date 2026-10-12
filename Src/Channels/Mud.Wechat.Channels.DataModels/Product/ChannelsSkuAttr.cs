// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// SKU 规格（官方 <c>sku_attrs</c> 数组元素：<c>attr_key</c> + <c>attr_value</c>）。
/// </summary>
/// <remarks>字段名与商品参数 <see cref="ChannelsProductAttr"/> 一致（官方同构），但语义为「销售规格」。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSkuAttr
{
    /// <summary>获取或设置规格 key（官方 <c>attr_key</c>，最多 40 字符）。</summary>
    [JsonPropertyName("attr_key")]
    public string AttrKey { get; set; } = string.Empty;

    /// <summary>获取或设置规格 value（官方 <c>attr_value</c>，最多 40 字符）。</summary>
    [JsonPropertyName("attr_value")]
    public string AttrValue { get; set; } = string.Empty;
}
