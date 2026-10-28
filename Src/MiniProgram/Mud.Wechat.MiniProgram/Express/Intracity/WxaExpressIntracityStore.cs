// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送门店（<c>querystore</c> 的 <c>store_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityStore
{
    /// <summary>省/自治区/直辖市（<c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>地级市（<c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区县（<c>area</c>）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>街道（<c>street</c>）。</summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>门牌号（<c>house</c>）。</summary>
    [JsonPropertyName("house")]
    public string? Address { get; set; }

    /// <summary>经度（<c>lng</c>）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>联系电话（<c>phone</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }

    /// <summary>自定义门店编号（<c>out_store_id</c>）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>门店所在城市 ID（<c>city_id</c>）。</summary>
    [JsonPropertyName("city_id")]
    public int CityId { get; set; }

    /// <summary>运力偏好（<c>order_pattern</c>）。</summary>
    [JsonPropertyName("order_pattern")]
    public int OrderPattern { get; set; }

    /// <summary>优先使用的运力 ID（<c>service_trans_prefer</c>）。</summary>
    [JsonPropertyName("service_trans_prefer")]
    public string? ServiceTransIdPrefer { get; set; }

    /// <summary>门店地址信息（<c>address_info</c>），见 <see cref="WxaExpressIntracityAddress"/>。</summary>
    [JsonPropertyName("address_info")]
    public WxaExpressIntracityAddress? AddressInfo { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
