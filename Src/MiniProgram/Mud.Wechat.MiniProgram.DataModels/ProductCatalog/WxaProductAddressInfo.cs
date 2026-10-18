// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 省市区三级地址（官方 <c>address_info</c>）。
/// </summary>
/// <remarks>
/// <para>读写同形，运费模板与同城/自提发货地址共用；官方只给省 / 市 / 区三级，不含详细门牌。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductAddressInfo
{
    /// <summary>省份。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>城市。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>区县。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }
}
