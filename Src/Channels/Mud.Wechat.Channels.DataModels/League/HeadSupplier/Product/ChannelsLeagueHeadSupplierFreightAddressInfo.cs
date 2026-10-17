// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 国标地址（官方 <c>freight_info</c> 内四处同形对象共用类型：
/// <c>address_info</c> / <c>condition_free_detail_list[].address_infos</c> /
/// <c>freight_calc_method_list[].address_infos</c> / <c>not_send_area[]</c>）。
/// </summary>
/// <remarks>四处字段集完全同形 ⇒ 一支类型复用，由守卫 CLG5 断言四处 <c>PropertyType</c> 同一，防止后续各自 fork。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierFreightAddressInfo
{
    /// <summary>获取或设置邮编（官方 <c>postal_code</c>）。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>获取或设置一级地址（官方 <c>province_name</c>；直辖市填区，如「浦东新区」；可经「获取地址编码」取得）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置二级地址（官方 <c>city_name</c>，可经「获取地址编码」取得）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置三级地址（官方 <c>county_name</c>，可经「获取地址编码」取得）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>获取或设置详细收货地址信息（官方 <c>detail_info</c>）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>获取或设置收货地址国家码（官方 <c>national_code</c>）。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }
}
