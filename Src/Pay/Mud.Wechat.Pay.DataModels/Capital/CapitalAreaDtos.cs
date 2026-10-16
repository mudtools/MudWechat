// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Capital;

/// <summary>查询省份列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698641"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalProvinceListResponse : WechatPayResponse
{
    /// <summary>【省份列表】 查询到的省份列表数组（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<CapitalProvinceListResponseDataItem>? Data { get; set; }

    /// <summary>【查询总条数】 查询到的省份数据总条数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>查询省份列表响应·【省份列表】 查询到的省份列表数组。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698641"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalProvinceListResponseDataItem
{
    /// <summary>【省份名称】 省份名称（官方字段路径 <c>data.province_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>【省份编码】 省份编码，唯一标识一个省份，用于根据省份编码查询省份下的城市列表数据（官方字段路径 <c>data.province_code</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("province_code")]
    public long? ProvinceCode { get; set; }
}

/// <summary>查询城市列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697470"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalCityListResponse : WechatPayResponse
{
    /// <summary>【城市列表】 查询到的城市列表数组（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<CapitalCityListResponseDataItem>? Data { get; set; }

    /// <summary>【查询总条数】 查询到的城市数据总条数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>查询城市列表响应·【城市列表】 查询到的城市列表数组。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697470"/></summary>
[HttpJsonSerializable(SerializerClassName = "Capital")]
public class CapitalCityListResponseDataItem
{
    /// <summary>【城市名称】 城市名称（官方字段路径 <c>data.city_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>【城市编码】 城市编码，唯一标识一个城市，用于根据城市编码查询支行列表数据（官方字段路径 <c>data.city_code</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("city_code")]
    public long? CityCode { get; set; }
}
