// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

// ---------------------------------------------------------------- 在地图中创建门店（POST /wxa/create_map_poi）

/// <summary>
/// 在地图中创建门店（<c>POST /wxa/create_map_poi</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表 14 项全部标必填；官方文档缺陷（照录）：<c>poi_id</c> 虽标必填，但说明为
/// 「如果是迁移门店，必须填 poi_id 字段」⇒ 实为条件必填。<c>introduct</c> 为官方字段原文
/// （疑为 <c>introduction</c> 的官方拼写，<b>不得「规范化」</b>）。
/// </para>
/// <para>
/// <c>longitude</c>/<c>latitude</c> 本页字段表标 <c>string</c> 且无返回示例可对照 ⇒ 无类型冲突，
/// 按字段表取 <c>string</c>（与 <c>get_district</c> / <c>get_store_list</c> 的示例数字口径不同——
/// 那两处属「标量类型冲突取示例」，此处无冲突故取表，矛盾根源在官方跨页不一致，照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpCreateMapPoiRequest
{
    /// <summary>获取或设置名字（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置经度（官方 <c>longitude</c>，必填；字段表标 string）。</summary>
    [JsonPropertyName("longitude")]
    public string Longitude { get; set; } = string.Empty;

    /// <summary>获取或设置纬度（官方 <c>latitude</c>，必填；字段表标 string）。</summary>
    [JsonPropertyName("latitude")]
    public string Latitude { get; set; } = string.Empty;

    /// <summary>获取或设置省份（官方 <c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string Province { get; set; } = string.Empty;

    /// <summary>获取或设置城市（官方 <c>city</c>，必填）。</summary>
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>获取或设置区（官方 <c>district</c>，必填）。</summary>
    [JsonPropertyName("district")]
    public string District { get; set; } = string.Empty;

    /// <summary>获取或设置详细地址（官方 <c>address</c>，必填）。</summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>获取或设置类目（官方 <c>category</c>，必填；如「美食:中餐厅」）。</summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>获取或设置电话（官方 <c>telephone</c>，必填；可多个，使用英文分号间隔）。</summary>
    [JsonPropertyName("telephone")]
    public string Telephone { get; set; } = string.Empty;

    /// <summary>获取或设置门店图片 url（官方 <c>photo</c>，必填）。</summary>
    [JsonPropertyName("photo")]
    public string Photo { get; set; } = string.Empty;

    /// <summary>获取或设置营业执照 url（官方 <c>license</c>，必填）。</summary>
    [JsonPropertyName("license")]
    public string License { get; set; } = string.Empty;

    /// <summary>获取或设置介绍（官方 <c>introduct</c>，必填；官方字段原文拼写，不得规范化）。</summary>
    [JsonPropertyName("introduct")]
    public string Introduct { get; set; } = string.Empty;

    /// <summary>获取或设置省市区 id（官方 <c>districtid</c>，必填；腾讯地图拉取省市区信息接口返回的 id）。</summary>
    [JsonPropertyName("districtid")]
    public string DistrictId { get; set; } = string.Empty;

    /// <summary>获取或设置门店迁移用 poi_id（官方 <c>poi_id</c>；官方标必填但说明为迁移场景才必填，矛盾照录）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;
}
