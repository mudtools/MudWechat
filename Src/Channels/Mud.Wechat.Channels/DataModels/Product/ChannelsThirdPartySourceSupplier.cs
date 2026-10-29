// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>货主（官方 <c>supplier</c> 对象，<c>addproductthirdpartysource</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsThirdPartySourceSupplier
{
    /// <summary>获取或设置货主主体名称（官方 <c>supplier_name</c>，必填）。</summary>
    [JsonPropertyName("supplier_name")]
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置货源平台类型（官方 <c>source_platform_type</c>）——
    /// <c>scene_value==1</c>（分销铺货）或 <c>scene_value==2</c>（商品搬家）时必填。
    /// </summary>
    [JsonPropertyName("source_platform_type")]
    public int? SourcePlatformType { get; set; }

    /// <summary>获取或设置货源平台名称（官方 <c>source_platform_name</c>；<c>scene_value==3</c> 其他场景时需入参平台名称）。</summary>
    [JsonPropertyName("source_platform_name")]
    public string? SourcePlatformName { get; set; }
}
