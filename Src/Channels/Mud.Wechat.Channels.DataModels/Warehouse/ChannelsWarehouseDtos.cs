// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>
/// 创建区域仓库（<c>createwarehouse</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>out_warehouse_id</c> 命名规则为字母数字 <c>[0-9, a-z, A-Z]</c> 加特殊字符 <c>[-_.]</c>，
/// 最大 128 字符，一个店铺下同一个外部 ID 只能创建一个仓库。
/// </para>
/// <para>
/// 覆盖规则（A/B/C/D 均非 0）：仅填 <c>address_id1</c>=覆盖 A 省份，填 1、2=覆盖 A 省 B 市，
/// 填 1~3=覆盖 A 省 B 市 C 区，填 1~4=覆盖 A 省 B 市 C 区 D 街道。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsCreateWarehouseRequest
{
    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填；字母数字加 <c>[-_.]</c>，最大 128 字符，店铺内唯一）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;

    /// <summary>获取或设置仓库名称（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置仓库介绍（官方 <c>intro</c>，必填）。</summary>
    [JsonPropertyName("intro")]
    public string Intro { get; set; } = string.Empty;

    /// <summary>获取或设置覆盖区域（官方 <c>cover_locations</c>，选填，可在创建后经「批量增加覆盖区域」添加）。</summary>
    [JsonPropertyName("cover_locations")]
    public List<ChannelsWarehouseLocation>? CoverLocations { get; set; }
}

/// <summary>查询区域仓库列表（<c>list/get</c>）请求体。</summary>
/// <remarks>官方契约：<c>page_size</c> 最大不超过 10；查询结果按创建时间升序排序。</remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetWarehouseListRequest
{
    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，最大不超过 10）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// 获取或设置翻页上下文（官方 <c>next_key</c>，选填）——由上次请求返回，传入时会从上次返回的
    /// 结果往后翻一页，不传默认获取第一页数据。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>获取区域仓库（<c>get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetWarehouseRequest
{
    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填；可从查询区域仓库列表接口获取）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;
}

/// <summary>修改区域仓库详情（<c>detail/update</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsUpdateWarehouseDetailRequest
{
    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;

    /// <summary>获取或设置仓库名称（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置仓库介绍（官方 <c>intro</c>，必填）。</summary>
    [JsonPropertyName("intro")]
    public string Intro { get; set; } = string.Empty;
}

/// <summary>
/// 批量增加 / 批量删除覆盖区域（<c>coverlocations/add</c> 与 <c>coverlocations/del</c>）共用请求体。
/// </summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：两端点请求体字段集完全一致（<c>out_warehouse_id</c> + <c>cover_locations</c>，
/// 元素结构同为 <see cref="ChannelsWarehouseLocation"/>），共用同一请求 DTO；语义演化如官方分裂再按端点分建。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehouseCoverLocationsRequest
{
    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;

    /// <summary>获取或设置覆盖区域（官方 <c>cover_locations</c>，必填；可从获取区域仓库接口获得）。</summary>
    [JsonPropertyName("cover_locations")]
    public List<ChannelsWarehouseLocation> CoverLocations { get; set; } = new();
}

/// <summary>获取指定地址下区域仓库的优先级（<c>address/prioritysort/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetAddressPrioritySortRequest
{
    /// <summary>获取或设置省份地址编码（官方 <c>address_id1</c>，必填）。</summary>
    [JsonPropertyName("address_id1")]
    public long AddressId1 { get; set; }

    /// <summary>获取或设置市地址编码（官方 <c>address_id2</c>，选填）。</summary>
    [JsonPropertyName("address_id2")]
    public long? AddressId2 { get; set; }

    /// <summary>获取或设置区地址编码（官方 <c>address_id3</c>，选填）。</summary>
    [JsonPropertyName("address_id3")]
    public long? AddressId3 { get; set; }

    /// <summary>获取或设置街道地址编码（官方 <c>address_id4</c>，选填）。</summary>
    [JsonPropertyName("address_id4")]
    public long? AddressId4 { get; set; }
}

/// <summary>设置指定地址下区域仓库的优先级（<c>address/prioritysort/set</c>）请求体。</summary>
/// <remarks>
/// 官方契约：未设置优先级时默认按区域仓库创建时间作为优先级（越早创建优先级越高）；
/// <c>priority_sort</c> 列表内 <c>out_warehouse_id</c> 不得重复（官方错误码 10020206）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsSetAddressPrioritySortRequest
{
    /// <summary>获取或设置省份地址编码（官方 <c>address_id1</c>，必填）。</summary>
    [JsonPropertyName("address_id1")]
    public long AddressId1 { get; set; }

    /// <summary>获取或设置市地址编码（官方 <c>address_id2</c>，选填）。</summary>
    [JsonPropertyName("address_id2")]
    public long? AddressId2 { get; set; }

    /// <summary>获取或设置区地址编码（官方 <c>address_id3</c>，选填）。</summary>
    [JsonPropertyName("address_id3")]
    public long? AddressId3 { get; set; }

    /// <summary>获取或设置街道地址编码（官方 <c>address_id4</c>，选填）。</summary>
    [JsonPropertyName("address_id4")]
    public long? AddressId4 { get; set; }

    /// <summary>获取或设置按优先级从高到低排列的外部仓库 ID 列表（官方 <c>priority_sort</c>，选填）。</summary>
    [JsonPropertyName("priority_sort")]
    public List<string>? PrioritySort { get; set; }
}

/// <summary>更新区域仓库存数量（<c>stock/update</c>）请求体。</summary>
/// <remarks>
/// 官方契约：修改类型见 <see cref="ChannelsWarehouseStockDiffTypes"/>（1 增加 / 2 减少 / 3 设置）；
/// 官方下单匹配原则：按照地址从右往左进行匹配（先筛选库存 ≥ 下单数量的区域仓，通配符 0
/// 可匹配任意地址、精准匹配优先于通配，最后取优先级最高的区域仓）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsUpdateWarehouseStockRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填；官方参数表类型为 string，示例 <c>"10000000666721"</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置 skuID（官方 <c>sku_id</c>，必填；官方参数表类型为 string）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;

    /// <summary>获取或设置修改类型（官方 <c>diff_type</c>，必填，见 <see cref="ChannelsWarehouseStockDiffTypes"/>）。</summary>
    [JsonPropertyName("diff_type")]
    public int DiffType { get; set; }

    /// <summary>获取或设置增加、减少或者设置的库存值（官方 <c>num</c>，必填）。</summary>
    [JsonPropertyName("num")]
    public int Num { get; set; }
}

/// <summary>获取区域仓库存数量（<c>stock/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetWarehouseStockRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填；官方参数表类型为 string）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置 skuID（官方 <c>sku_id</c>，必填；官方参数表类型为 string）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;
}

/// <summary>获取地址行政编码（<c>addresscode/get</c>）请求体。</summary>
/// <remarks>官方契约：<c>addr_code</c> 不填或者填 0 时，获取全国的省级行政编码。</remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetAddressCodeRequest
{
    /// <summary>获取或设置地址行政编码（官方 <c>addr_code</c>，选填；不填或填 0 时获取全国的省级行政编码）。</summary>
    [JsonPropertyName("addr_code")]
    public long? AddrCode { get; set; }
}

/// <summary>查询区域仓库列表（<c>list/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetWarehouseListResponse : ChannelsResponse
{
    /// <summary>获取或设置列表数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsWarehouseListData? Data { get; set; }
}

/// <summary>查询区域仓库列表数据对象（官方 <c>data</c>，<c>list/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehouseListData
{
    /// <summary>获取或设置外部仓库 ID 列表（官方 <c>out_warehouse_ids</c>）。</summary>
    [JsonPropertyName("out_warehouse_ids")]
    public List<string>? OutWarehouseIds { get; set; }

    /// <summary>
    /// 获取或设置本次翻页的上下文（官方 <c>next_key</c>）——用于请求下一页；
    /// 如果是空，则当前是最后一页。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>获取区域仓库（<c>get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetWarehouseResponse : ChannelsResponse
{
    /// <summary>获取或设置仓库详情（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsWarehouseInfo? Data { get; set; }
}

/// <summary>区域仓库详情（官方 <c>data</c> 对象，<c>get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehouseInfo
{
    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string? OutWarehouseId { get; set; }

    /// <summary>获取或设置仓库名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置仓库介绍（官方 <c>intro</c>）。</summary>
    [JsonPropertyName("intro")]
    public string? Intro { get; set; }

    /// <summary>获取或设置覆盖区域（官方 <c>cover_locations</c>，可作为「批量增加/删除覆盖区域」入参来源）。</summary>
    [JsonPropertyName("cover_locations")]
    public List<ChannelsWarehouseLocation>? CoverLocations { get; set; }
}

/// <summary>获取指定地址下区域仓库的优先级（<c>address/prioritysort/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetAddressPrioritySortResponse : ChannelsResponse
{
    /// <summary>获取或设置优先级数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsWarehousePrioritySortData? Data { get; set; }
}

/// <summary>地址优先级数据对象（官方 <c>data</c>，<c>address/prioritysort/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehousePrioritySortData
{
    /// <summary>获取或设置按 <c>out_warehouse_id</c> 排序的优先级列表（官方 <c>priority_sort</c>，优先级从高到低）。</summary>
    [JsonPropertyName("priority_sort")]
    public List<string>? PrioritySort { get; set; }
}

/// <summary>获取区域仓库存数量（<c>stock/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetWarehouseStockResponse : ChannelsResponse
{
    /// <summary>获取或设置库存信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsWarehouseStockData? Data { get; set; }
}

/// <summary>区域仓库存信息（官方 <c>data</c> 对象，<c>stock/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehouseStockData
{
    /// <summary>获取或设置库存数量（官方 <c>num</c>）。</summary>
    [JsonPropertyName("num")]
    public int? Num { get; set; }
}

/// <summary>获取地址行政编码（<c>addresscode/get</c>）响应。</summary>
/// <remarks>
/// 官方契约：<c>addrs_msg</c> 与 <c>next_level_addrs</c> 位于响应<b>顶层</b>（无 <c>data</c> 包裹，照抄官方结构）；
/// 最多获取 4 级地址的行政编码。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetAddressCodeResponse : ChannelsResponse
{
    /// <summary>获取或设置本行政编码地址信息（官方 <c>addrs_msg</c>）。</summary>
    [JsonPropertyName("addrs_msg")]
    public ChannelsAddress? AddrsMsg { get; set; }

    /// <summary>获取或设置下一级所有地址信息（官方 <c>next_level_addrs</c>）。</summary>
    [JsonPropertyName("next_level_addrs")]
    public List<ChannelsAddress>? NextLevelAddrs { get; set; }
}

/// <summary>覆盖区域地址编码（官方 <c>cover_locations</c> 数组元素，<c>create</c> / <c>get</c> / <c>coverlocations/add</c> / <c>coverlocations/del</c> 共用）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：创建 / 获取 / 批量增 / 批量删四端点的覆盖区域元素字段集完全一致
/// （<c>address_id1</c> 必填 + 可选 2~4 级），共用同一元素 DTO。
/// 覆盖规则（A/B/C/D 均非 0）：A000=A 省份；AB00=A 省 B 市；ABC0=A 省 B 市 C 区；ABCD=A 省 B 市 C 区 D 街道。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehouseLocation
{
    /// <summary>获取或设置省份地址编码（官方 <c>address_id1</c>，必填）。</summary>
    [JsonPropertyName("address_id1")]
    public long AddressId1 { get; set; }

    /// <summary>获取或设置市地址编码（官方 <c>address_id2</c>，选填）。</summary>
    [JsonPropertyName("address_id2")]
    public long? AddressId2 { get; set; }

    /// <summary>获取或设置区地址编码（官方 <c>address_id3</c>，选填）。</summary>
    [JsonPropertyName("address_id3")]
    public long? AddressId3 { get; set; }

    /// <summary>获取或设置街道地址编码（官方 <c>address_id4</c>，选填）。</summary>
    [JsonPropertyName("address_id4")]
    public long? AddressId4 { get; set; }
}

/// <summary>地址行政编码信息（官方 <c>addrs_msg</c> 对象与 <c>next_level_addrs</c> 数组元素共用，<c>addresscode/get</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：<c>addrs_msg</c> 与 <c>next_level_addrs</c> 元素字段集完全一致
/// （<c>name</c>/<c>code</c>/<c>level</c>），共用同一元素 DTO。
/// 官方返回示例中根节点「中华人民共和国」<c>level</c>=<c>0</c>、<c>code</c>=<c>0</c>（照录，
/// 参数表仅声明 1~4 级，见 <see cref="ChannelsAddressLevels"/>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsAddress
{
    /// <summary>获取或设置地址名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置地址行政编码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public long? Code { get; set; }

    /// <summary>获取或设置地址级别（官方 <c>level</c>，见 <see cref="ChannelsAddressLevels"/>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }
}

/// <summary>区域仓库存修改类型常量（官方 <c>diff_type</c> 枚举值，<c>stock/update</c>）。</summary>
public static class ChannelsWarehouseStockDiffTypes
{
    /// <summary>增加。</summary>
    public const int Increase = 1;

    /// <summary>减少。</summary>
    public const int Decrease = 2;

    /// <summary>设置。</summary>
    public const int Set = 3;
}

/// <summary>地址级别常量（官方 <c>level</c> 枚举值，<c>addresscode/get</c>）。</summary>
public static class ChannelsAddressLevels
{
    /// <summary>省级。</summary>
    public const int Province = 1;

    /// <summary>市级。</summary>
    public const int City = 2;

    /// <summary>区县级。</summary>
    public const int County = 3;

    /// <summary>街道。</summary>
    public const int Street = 4;
}
