// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Poi;

/// <summary>
/// 「查询门店」请求（官方 <c>poi/getpoi</c>）。
/// </summary>
/// <remarks>
/// <para><b>与店铺域（<c>Store</c>）的关系</b>：本域是<b>旧版微信门店接口（POI）</b>
/// （<c>/cgi-bin/poi/*</c>），<c>Store</c> 域是新版小程序店铺 API（<c>/wxa/*</c>）——两代接口并存勿合并。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Poi")]
public class MpPoiGetRequest
{
    /// <summary>获取或设置门店 ID（官方 <c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;
}

/// <summary>
/// 门店基础信息（官方 <c>base_info</c>；<b>列表端点的同一对象额外携带</b>
/// <see cref="PoiId"/> / <see cref="MapPoiId"/> / <see cref="UpgradeStatus"/> / <see cref="UpgradeRejectReason"/>
/// 四个字段——单一超集类承载两形态，未出现的字段缺省）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Poi")]
public class MpPoiBaseInfo
{
    /// <summary>获取或设置商户自己的门店 ID（官方 <c>sid</c>，可选）。</summary>
    [JsonPropertyName("sid")]
    public string? Sid { get; set; }

    /// <summary>获取或设置门店名称（官方 <c>business_name</c>；仅列表端点返回）。</summary>
    [JsonPropertyName("business_name")]
    public string? BusinessName { get; set; }

    /// <summary>获取或设置分店名（官方 <c>branch_name</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

    /// <summary>获取或设置省份（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>获取或设置城市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置区县（官方 <c>district</c>）。</summary>
    [JsonPropertyName("district")]
    public string? District { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置电话（官方 <c>telephone</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>获取或设置门店类目列表（官方 <c>categories</c>；三级类目字符串数组）。</summary>
    [JsonPropertyName("categories")]
    public string[]? CategoryList { get; set; }

    /// <summary>获取或设置坐标类型（官方 <c>offset_type</c>；1=火星坐标/GCJ-02）。</summary>
    [JsonPropertyName("offset_type")]
    public int CoordinateType { get; set; }

    /// <summary>获取或设置经度（官方 <c>longitude</c>）。</summary>
    [JsonPropertyName("longitude")]
    public decimal Longitude { get; set; }

    /// <summary>获取或设置纬度（官方 <c>latitude</c>）。</summary>
    [JsonPropertyName("latitude")]
    public decimal Latitude { get; set; }

    /// <summary>获取或设置图片列表（官方 <c>photo_list</c>，可选）。</summary>
    [JsonPropertyName("photo_list")]
    public MpPoiPhoto[]? PhotoList { get; set; }

    /// <summary>获取或设置推荐菜品/商品（官方 <c>recommend</c>，可选）。</summary>
    [JsonPropertyName("recommend")]
    public string? Recommend { get; set; }

    /// <summary>获取或设置特色服务（官方 <c>special</c>，可选）。</summary>
    [JsonPropertyName("special")]
    public string? Special { get; set; }

    /// <summary>获取或设置商户简介（官方 <c>introduction</c>，可选）。</summary>
    [JsonPropertyName("introduction")]
    public string? Introduction { get; set; }

    /// <summary>获取或设置营业时间（官方 <c>open_time</c>，可选）。</summary>
    [JsonPropertyName("open_time")]
    public string? OpenTime { get; set; }

    /// <summary>获取或设置人均价格（官方 <c>avg_price</c>，可选；单位元）。</summary>
    [JsonPropertyName("avg_price")]
    public decimal? AveragePrice { get; set; }

    /// <summary>获取或设置门店可用状态（官方 <c>available_state</c>；3=审核中，2=已生效，4=已驳回）。</summary>
    [JsonPropertyName("available_state")]
    public int AvailableStatus { get; set; }

    /// <summary>获取或设置门店更新状态（官方 <c>update_status</c>；0=正常，1=更新中）。</summary>
    [JsonPropertyName("update_status")]
    public int UpdateStatus { get; set; }

    /// <summary>获取或设置门店 ID（官方 <c>poi_id</c>；仅列表端点返回）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }

    /// <summary>获取或设置微信门店库关联 ID（官方 <c>mapid</c>，可选；仅列表端点返回）。</summary>
    [JsonPropertyName("mapid")]
    public string? MapPoiId { get; set; }

    /// <summary>获取或设置升级状态（官方 <c>upgrade_status</c>，可选；仅列表端点返回）。</summary>
    [JsonPropertyName("upgrade_status")]
    public int? UpgradeStatus { get; set; }

    /// <summary>获取或设置升级失败原因（官方 <c>upgrade_comment</c>，可选；仅列表端点返回）。</summary>
    [JsonPropertyName("upgrade_comment")]
    public string? UpgradeRejectReason { get; set; }
}

/// <summary>
/// 门店图片项（官方 <c>photo_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Poi")]
public class MpPoiPhoto
{
    /// <summary>获取或设置图片 URL（官方 <c>photo_url</c>）。</summary>
    [JsonPropertyName("photo_url")]
    public string? PhotoUrl { get; set; }
}

/// <summary>
/// 「查询门店」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Poi")]
public class MpPoiGetResponse : MpResponse
{
    /// <summary>获取或设置门店信息（官方 <c>business</c>）。</summary>
    [JsonPropertyName("business")]
    public MpPoiBaseInfo? Business { get; set; }
}

/// <summary>
/// 「查询门店列表」请求（官方 <c>poi/getpoilist</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Poi")]
public class MpPoiListRequest
{
    /// <summary>获取或设置起始位置（官方 <c>begin</c>，从 0 开始）。</summary>
    [JsonPropertyName("begin")]
    public int Offset { get; set; }

    /// <summary>获取或设置拉取数量（官方 <c>limit</c>；官方建议 ≤ 50）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 20;
}

/// <summary>
/// 「查询门店列表」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Poi")]
public class MpPoiListResponse : MpResponse
{
    /// <summary>获取或设置门店列表（官方 <c>business_list</c>）。</summary>
    [JsonPropertyName("business_list")]
    public MpPoiBaseInfo[]? PoiList { get; set; }

    /// <summary>获取或设置门店总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}

/// <summary>
/// 「删除门店」请求（官方 <c>poi/delpoi</c>）。
/// </summary>
/// <remarks><b>覆盖删除语义</b>：删除后微信侧门店即时下线，不可恢复；新建同名门店会得到新的 <c>poi_id</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Poi")]
public class MpPoiDeleteRequest
{
    /// <summary>获取或设置门店 ID（官方 <c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;
}
