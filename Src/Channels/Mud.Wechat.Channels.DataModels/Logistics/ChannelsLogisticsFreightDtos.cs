// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「运费模板」类 DTO（merchant/addfreighttemplate、getfreighttemplatedetail、
// getfreighttemplatelist、updatefreighttemplate）。命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>运费模板发货地址（官方 <c>freight_template.address_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsFreightAddressInfo
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

/// <summary>运费计费规则列表项（官方 <c>all_condition_free_detail.condition_free_detail_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsConditionFreeDetail
{
    /// <summary>获取或设置支持的地址列表（官方 <c>address_infos</c>）。</summary>
    [JsonPropertyName("address_infos")]
    public List<ChannelsLogisticsFreightAddressInfo>? AddressInfos { get; set; }

    /// <summary>获取或设置最低件数（官方 <c>min_piece</c>）。</summary>
    [JsonPropertyName("min_piece")]
    public int? MinPiece { get; set; }

    /// <summary>获取或设置最低重量（官方 <c>min_weight</c>，单位千克；订单商品总质量小于一千克算作一千克）。</summary>
    [JsonPropertyName("min_weight")]
    public int? MinWeight { get; set; }

    /// <summary>获取或设置最低金额（官方 <c>min_amount</c>，单位分）。</summary>
    [JsonPropertyName("min_amount")]
    public long? MinAmount { get; set; }

    /// <summary>获取或设置计费方式对应的选项是否已设置（官方 <c>valuation_flag</c>）。</summary>
    [JsonPropertyName("valuation_flag")]
    public int? ValuationFlag { get; set; }

    /// <summary>获取或设置金额是否设置（官方 <c>amount_flag</c>）。</summary>
    [JsonPropertyName("amount_flag")]
    public int? AmountFlag { get; set; }
}

/// <summary>条件包邮详情（官方 <c>all_condition_free_detail</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAllConditionFreeDetail
{
    /// <summary>获取或设置计费规则列表（官方 <c>condition_free_detail_list</c>）。</summary>
    [JsonPropertyName("condition_free_detail_list")]
    public List<ChannelsLogisticsConditionFreeDetail>? ConditionFreeDetailList { get; set; }
}

/// <summary>具体计费方法列表项（官方 <c>all_freight_calc_method.freight_calc_method_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsFreightCalcMethod
{
    /// <summary>获取或设置支持的地址列表（官方 <c>address_infos</c>）。</summary>
    [JsonPropertyName("address_infos")]
    public List<ChannelsLogisticsFreightAddressInfo>? AddressInfos { get; set; }

    /// <summary>获取或设置是否默认运费（官方 <c>is_default</c>）。</summary>
    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; set; }

    /// <summary>获取或设置快递公司（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置首段运费需要满足的数量（官方 <c>first_val_amount</c>）。</summary>
    [JsonPropertyName("first_val_amount")]
    public long? FirstValAmount { get; set; }

    /// <summary>获取或设置首段运费的金额（官方 <c>first_price</c>）。</summary>
    [JsonPropertyName("first_price")]
    public long? FirstPrice { get; set; }

    /// <summary>获取或设置续费的数量（官方 <c>second_val_amount</c>）。</summary>
    [JsonPropertyName("second_val_amount")]
    public long? SecondValAmount { get; set; }

    /// <summary>获取或设置续费的金额（官方 <c>second_price</c>）。</summary>
    [JsonPropertyName("second_price")]
    public long? SecondPrice { get; set; }
}

/// <summary>具体计费方法（官方 <c>all_freight_calc_method</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAllFreightCalcMethod
{
    /// <summary>获取或设置 freight_calc_method_list（官方 <c>freight_calc_method_list</c>）。</summary>
    [JsonPropertyName("freight_calc_method_list")]
    public List<ChannelsLogisticsFreightCalcMethod>? FreightCalcMethodList { get; set; }
}

/// <summary>不发货区域（官方 <c>not_send_area</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsNotSendArea
{
    /// <summary>获取或设置不支持的地址列表（官方 <c>address_infos</c>）。</summary>
    [JsonPropertyName("address_infos")]
    public List<ChannelsLogisticsFreightAddressInfo>? AddressInfos { get; set; }
}

/// <summary>运费模板详细信息（官方 <c>freight_template</c>，增改查共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsFreightTemplate
{
    /// <summary>获取或设置模板 id（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置模板名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置计费类型（官方 <c>valuation_type</c>）：PIECE 按件数；WEIGHT 按重量。</summary>
    [JsonPropertyName("valuation_type")]
    public string? ValuationType { get; set; }

    /// <summary>获取或设置发货时间期限详情（官方 <c>send_time</c>）：SendTime_TWENTYFOUR_HOUR / SendTime_FOUTYEIGHT_HOUR / SendTime_SHIP_TODAY。</summary>
    [JsonPropertyName("send_time")]
    public string? SendTime { get; set; }

    /// <summary>获取或设置发货地址（官方 <c>address_info</c>）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsLogisticsFreightAddressInfo? AddressInfo { get; set; }

    /// <summary>获取或设置运输方式（官方 <c>delivery_type</c>）：EXPRESS 快递。</summary>
    [JsonPropertyName("delivery_type")]
    public string? DeliveryType { get; set; }

    /// <summary>获取或设置计费方式（官方 <c>shipping_method</c>）：FREE 包邮 / CONDITION_FREE 条件包邮 / NO_FREE 不包邮。</summary>
    [JsonPropertyName("shipping_method")]
    public string? ShippingMethod { get; set; }

    /// <summary>获取或设置条件包邮详情（官方 <c>all_condition_free_detail</c>）。</summary>
    [JsonPropertyName("all_condition_free_detail")]
    public ChannelsLogisticsAllConditionFreeDetail? AllConditionFreeDetail { get; set; }

    /// <summary>获取或设置具体计费方法（官方 <c>all_freight_calc_method</c>）。</summary>
    [JsonPropertyName("all_freight_calc_method")]
    public ChannelsLogisticsAllFreightCalcMethod? AllFreightCalcMethod { get; set; }

    /// <summary>获取或设置创建时间戳（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间戳（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置是否默认模板（官方 <c>is_default</c>）。</summary>
    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; set; }

    /// <summary>获取或设置不发货区域（官方 <c>not_send_area</c>）。</summary>
    [JsonPropertyName("not_send_area")]
    public ChannelsLogisticsNotSendArea? NotSendArea { get; set; }
}

/// <summary>增加运费模板（<c>merchant/addfreighttemplate</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddFreightTemplateRequest
{
    /// <summary>获取或设置运费模板详细信息（官方 <c>freight_template</c>，必填）。</summary>
    [JsonPropertyName("freight_template")]
    public ChannelsLogisticsFreightTemplate FreightTemplate { get; set; } = new();
}

/// <summary>增加运费模板（<c>merchant/addfreighttemplate</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddFreightTemplateResponse : ChannelsResponse
{
    /// <summary>获取或设置运费模板 id（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }
}

/// <summary>模板 id 查询请求（<c>merchant/getfreighttemplatedetail</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetFreightTemplateDetailRequest
{
    /// <summary>获取或设置运费模板 id（官方 <c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;
}

/// <summary>查询运费模板（<c>merchant/getfreighttemplatedetail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetFreightTemplateDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置运费模板详细信息（官方 <c>freight_template</c>）。</summary>
    [JsonPropertyName("freight_template")]
    public ChannelsLogisticsFreightTemplate? FreightTemplate { get; set; }
}

/// <summary>获取运费模板列表（<c>merchant/getfreighttemplatelist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetFreightTemplateListRequest
{
    /// <summary>获取或设置起始位置（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置获取个数（官方 <c>limit</c>，必填）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>获取运费模板列表（<c>merchant/getfreighttemplatelist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetFreightTemplateListResponse : ChannelsResponse
{
    /// <summary>获取或设置运费模板 id 列表（官方 <c>template_id_list</c>）。</summary>
    [JsonPropertyName("template_id_list")]
    public List<string>? TemplateIdList { get; set; }
}

/// <summary>更新运费模板（<c>merchant/updatefreighttemplate</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsUpdateFreightTemplateRequest
{
    /// <summary>获取或设置运费模板详细信息（官方 <c>freight_template</c>，必填，原地覆盖写入须带全字段）。</summary>
    [JsonPropertyName("freight_template")]
    public ChannelsLogisticsFreightTemplate FreightTemplate { get; set; } = new();
}
