// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.NearbyPoi;

/// <summary>附近小程序「添加地点」请求体（<c>POST /wxa/addnearbypoi</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>nearby-poi/api_addnearbypoi.html</c>。</para>
/// <para>
/// 门店信息（<c>store_info</c>）与客服信息（<c>kf_info</c>）为<b>选填</b>；<c>media_id</c> 为小程序
/// <c>material/addMaterial</c> 返回的<b>永久素材</b>媒体 ID（用于门店头图）；<c>poi_id</c> 为商户平台
/// 创建的门店 ID。添加成功后进入<b>审核</b>流程（结果以官方页面为准）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaAddNearbyPoiRequest
{
    /// <summary>是否展示（<c>is_show</c>，必填）：<c>1</c> 展示 / <c>0</c> 不展示。</summary>
    [JsonPropertyName("is_show")]
    public long? IsShow { get; set; }

    /// <summary>门店类目（<c>categories</c>，必填；列表项数量与取值以官方页面为准）。</summary>
    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }

    /// <summary>门店详细地址（<c>address</c>，必填；不得为空字符串）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>门店所在区县（<c>district</c>，必填）。</summary>
    [JsonPropertyName("district")]
    public string? District { get; set; }

    /// <summary>门店所在城市（<c>city</c>，必填；都市圈等特殊行政区划以官方说明为准）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>门店所在省份（<c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>门店纬度（<c>lat</c>，必填；十进制经纬度）。</summary>
    [JsonPropertyName("lat")]
    public string? Lat { get; set; }

    /// <summary>门店经度（<c>lng</c>，必填）。</summary>
    [JsonPropertyName("lng")]
    public string? Lng { get; set; }

    /// <summary>门店头图素材媒体 ID（<c>media_id</c>，必填；小程序永久素材）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>门店 ID（<c>poi_id</c>，必填；商户平台创建）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }

    /// <summary>客服信息（<c>kf_info</c>，选填），见 <see cref="WxaNearbyPoiKfInfo"/>。</summary>
    [JsonPropertyName("kf_info")]
    public WxaNearbyPoiKfInfo? KfInfo { get; set; }

    /// <summary>门店信息（<c>store_info</c>，选填），见 <see cref="WxaNearbyPoiStoreInfo"/>。</summary>
    [JsonPropertyName("store_info")]
    public WxaNearbyPoiStoreInfo? StoreInfo { get; set; }
}

/// <summary>附近小程序客服信息（<c>kf_info</c>）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaNearbyPoiKfInfo
{
    /// <summary>微信客服账号 ID（<c>open_kf_id</c>）。</summary>
    [JsonPropertyName("open_kf_id")]
    public string? OpenKfId { get; set; }

    /// <summary>客服头像（<c>kf_headimg</c>）。</summary>
    [JsonPropertyName("kf_headimg")]
    public string? KfHeadimg { get; set; }

    /// <summary>客服昵称（<c>kf_name</c>）。</summary>
    [JsonPropertyName("kf_name")]
    public string? KfName { get; set; }
}

/// <summary>附近小程序门店信息（<c>store_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaNearbyPoiStoreInfo
{
    /// <summary>品牌名（<c>brand_name</c>）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>分店名（<c>branch_name</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

    /// <summary>营业时间（<c>open_time</c>，格式以官方页面为准）。</summary>
    [JsonPropertyName("open_time")]
    public string? OpenTime { get; set; }

    /// <summary>营业时间扩展（<c>open_time_ext</c>）。</summary>
    [JsonPropertyName("open_time_ext")]
    public string? OpenTimeExt { get; set; }

    /// <summary>联系电话（<c>telephone</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }
}

/// <summary>删除附近小程序地点请求体（<c>POST /wxa/delnearbypoi</c>）。</summary>
/// <remarks>官方文档：<c>nearby-poi/api_deletenearbypoi.html</c>。<c>poi_id</c> 取自 <c>addnearbypoi</c> 结果。</remarks>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaDeleteNearbyPoiRequest
{
    /// <summary>门店 ID（<c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }
}

/// <summary>查看附近小程序地点列表请求体（<c>POST /wxa/getnearbypoilist</c>）。</summary>
/// <remarks>官方文档：<c>nearby-poi/api_getnearbypoilist.html</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaNearbyPoiListRequest
{
    /// <summary>页码（<c>page</c>，必填；从 <c>0</c> 开始）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>每页条数（<c>page_rows</c>，必填；上限以官方页面为准）。</summary>
    [JsonPropertyName("page_rows")]
    public long? PageRows { get; set; }
}

/// <summary>附近小程序地点条目（<c>list[]</c>）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaNearbyPoiItem
{
    /// <summary>门店 ID（<c>poi_id</c>）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }

    /// <summary>门店详细地址（<c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>门店头图（<c>pic_list</c> 等头图字段以官方页面为准；此处透传媒体 ID 列表）。</summary>
    [JsonPropertyName("pic_list")]
    public List<string>? PicList { get; set; }
}

/// <summary>查看附近小程序地点列表应答（<c>list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaNearbyPoiListResponse : WxaResponse
{
    /// <summary>地点列表（<c>list</c>），见 <see cref="WxaNearbyPoiItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaNearbyPoiItem>? List { get; set; }
}

/// <summary>设置附近小程序展示状态请求体（<c>POST /wxa/setnearbypoishowstatus</c>）。</summary>
/// <remarks>官方文档：<c>nearby-poi/api_setshowstatus.html</c>。<c>status</c>：<c>0</c> 取消展示 / <c>1</c> 展示。</remarks>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaSetNearbyPoiShowStatusRequest
{
    /// <summary>门店 ID（<c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }

    /// <summary>展示状态（<c>status</c>，必填）：<c>0</c> 取消展示 / <c>1</c> 展示。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }
}