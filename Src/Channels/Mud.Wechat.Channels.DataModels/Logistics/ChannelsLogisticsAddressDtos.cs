// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「地址」类 DTO（merchant/address/add、get、list、delete、update）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>线下配送地址类型（官方 <c>address_detail.address_type</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddressType
{
    /// <summary>获取或设置是否同城配送（官方 <c>same_city</c>，1 表示同城配送）。</summary>
    [JsonPropertyName("same_city")]
    public int? SameCity { get; set; }

    /// <summary>获取或设置是否用户自提（官方 <c>pickup</c>，1 表示用户自提）。</summary>
    [JsonPropertyName("pickup")]
    public int? Pickup { get; set; }
}

/// <summary>地区信息（官方 <c>address_detail.address_info</c>，地址增改查共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddressInfo
{
    /// <summary>获取或设置收货人姓名（官方 <c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置邮编（官方 <c>postal_code</c>）。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>获取或设置国标收货地址第一级地址（官方 <c>province_name</c>，可调用获取地址编码获取）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置国标收货地址第二级地址（官方 <c>city_name</c>，直辖市填区，如「浦东新区」）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置国标收货地址第三级地址（官方 <c>county_name</c>）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>获取或设置详细收货地址信息（官方 <c>detail_info</c>）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>获取或设置收货地址国家码（官方 <c>national_code</c>）。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }

    /// <summary>获取或设置收货人手机号码（官方 <c>tel_number</c>）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>获取或设置纬度（官方 <c>lat</c>）。</summary>
    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    /// <summary>获取或设置经度（官方 <c>lng</c>）。</summary>
    [JsonPropertyName("lng")]
    public double? Lng { get; set; }

    /// <summary>获取或设置门牌号（官方 <c>house_number</c>）。</summary>
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }
}

/// <summary>地址信息（官方 <c>address_detail</c>，添加 / 更新共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddressDetail
{
    /// <summary>获取或设置地址 id（官方 <c>address_id</c>）。</summary>
    [JsonPropertyName("address_id")]
    public string AddressId { get; set; } = string.Empty;

    /// <summary>获取或设置联系人姓名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置地区信息（官方 <c>address_info</c>）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsLogisticsAddressInfo? AddressInfo { get; set; }

    /// <summary>获取或设置座机（官方 <c>landline</c>）。</summary>
    [JsonPropertyName("landline")]
    public string? Landline { get; set; }

    /// <summary>获取或设置是否为发货地址（官方 <c>send_addr</c>）。</summary>
    [JsonPropertyName("send_addr")]
    public bool? SendAddr { get; set; }

    /// <summary>获取或设置是否为默认发货地址（官方 <c>default_send</c>）。</summary>
    [JsonPropertyName("default_send")]
    public bool? DefaultSend { get; set; }

    /// <summary>获取或设置是否为收货地址（官方 <c>recv_addr</c>）。</summary>
    [JsonPropertyName("recv_addr")]
    public bool? RecvAddr { get; set; }

    /// <summary>获取或设置是否为默认收货地址（官方 <c>default_recv</c>）。</summary>
    [JsonPropertyName("default_recv")]
    public bool? DefaultRecv { get; set; }

    /// <summary>获取或设置创建时间戳（秒，官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间戳（秒，官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置线下配送地址类型（官方 <c>address_type</c>）。</summary>
    [JsonPropertyName("address_type")]
    public ChannelsLogisticsAddressType? AddressType { get; set; }
}

/// <summary>添加地址（<c>merchant/address/add</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddAddressRequest
{
    /// <summary>获取或设置地址信息（官方 <c>address_detail</c>，必填）。</summary>
    [JsonPropertyName("address_detail")]
    public ChannelsLogisticsAddressDetail AddressDetail { get; set; } = new();
}

/// <summary>添加地址（<c>merchant/address/add</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddAddressResponse : ChannelsResponse
{
    /// <summary>获取或设置新的地址 id（官方 <c>address_id</c>）。</summary>
    [JsonPropertyName("address_id")]
    public string? AddressId { get; set; }
}

/// <summary>地址 id 请求（<c>merchant/address/get</c> / <c>delete</c> 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddressIdRequest
{
    /// <summary>获取或设置地址 id（官方 <c>address_id</c>，必填）。</summary>
    [JsonPropertyName("address_id")]
    public string AddressId { get; set; } = string.Empty;
}

/// <summary>获取地址详情（<c>merchant/address/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetAddressResponse : ChannelsResponse
{
    /// <summary>获取或设置地址详情（官方 <c>address_detail</c>）。</summary>
    [JsonPropertyName("address_detail")]
    public ChannelsLogisticsAddressDetail? AddressDetail { get; set; }
}

/// <summary>获取地址列表（<c>merchant/address/list</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetAddressListRequest
{
    /// <summary>获取或设置获取的偏移量（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置获取的个数（官方 <c>limit</c>，必填）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>获取地址列表（<c>merchant/address/list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetAddressListResponse : ChannelsResponse
{
    /// <summary>获取或设置地址 id 列表（官方 <c>address_id_list</c>）。</summary>
    [JsonPropertyName("address_id_list")]
    public List<string>? AddressIdList { get; set; }
}

/// <summary>更新地址（<c>merchant/address/update</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsUpdateAddressRequest
{
    /// <summary>获取或设置地址信息（官方 <c>address_detail</c>，必填）。</summary>
    [JsonPropertyName("address_detail")]
    public ChannelsLogisticsAddressDetail AddressDetail { get; set; } = new();
}
