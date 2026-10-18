// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 配送方式开关（官方 <c>supported_shipping_methods</c>）。
/// </summary>
/// <remarks>
/// <para>读写同形：三个开关的 number 值表示是否开启（1 开启）。设置侧三支全部必填，且需先配好同城发货地址 / 自提地址。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductSupportedShippingMethods
{
    /// <summary>是否开启快递配送。</summary>
    [JsonPropertyName("express")]
    public int? Express { get; set; }

    /// <summary>是否开启同城配送。</summary>
    [JsonPropertyName("same_city")]
    public int? SameCity { get; set; }

    /// <summary>是否开启线下自提。</summary>
    [JsonPropertyName("pickup")]
    public int? Pickup { get; set; }
}
