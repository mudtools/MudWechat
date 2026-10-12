// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 门店基本信息（官方 <c>base_info</c>；<c>get_store_info</c> 的 <c>business.base_info</c> 与
/// <c>get_store_list</c> 的 <c>business_list[].base_info</c> 字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <b>官方文档缺陷（照录）</b>：① <c>get_store_list</c> 页把本对象的字段直接列为
/// <c>business_list[]</c> 的<b>平级字段</b>，而返回示例实际包裹在 <c>base_info</c> 内 ——
/// 按本 SDK 的冲突处置规则「<b>嵌套结构冲突取官方返回示例</b>」（表把示例嵌套扁平化属表的抽象失真），
/// 建模为 <c>base_info</c> 嵌套；② <c>longitude</c>/<c>latitude</c> 字段表标 <c>string</c>、示例为
/// <b>数字</b> ⇒ 按「标量类型冲突取官方返回示例」建模为 <c>double</c>；③
/// <c>qualification_name</c> 字段说明为「营业执照的名称」，但示例值恒与 <c>business_name</c> 相同，照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreBaseInfo
{
    /// <summary>获取或设置门店名称（官方 <c>business_name</c>）。</summary>
    [JsonPropertyName("business_name")]
    public string? BusinessName { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置电话（官方 <c>telephone</c>；可多个，使用英文分号间隔）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>获取或设置城市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置省份（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>获取或设置经度（官方 <c>longitude</c>；字段表标 string、示例为数字，取示例口径）。</summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>获取或设置纬度（官方 <c>latitude</c>；字段表标 string、示例为数字，取示例口径）。</summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>获取或设置图片列表（官方 <c>photo_list</c>）。</summary>
    [JsonPropertyName("photo_list")]
    public List<MpStorePhoto>? PhotoList { get; set; }

    /// <summary>获取或设置门店开放时间（官方 <c>open_time</c>）。</summary>
    [JsonPropertyName("open_time")]
    public string? OpenTime { get; set; }

    /// <summary>获取或设置门店 id（官方 <c>poi_id</c>）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }

    /// <summary>获取或设置审核结果（官方 <c>status</c>；取值见 <c>MpStoreStatuses</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置区（官方 <c>district</c>；官方示例为占位符 <c>value</c>，照录）。</summary>
    [JsonPropertyName("district")]
    public string? District { get; set; }

    /// <summary>获取或设置营业执照号（官方 <c>qualification_num</c>）。</summary>
    [JsonPropertyName("qualification_num")]
    public string? QualificationNumber { get; set; }

    /// <summary>获取或设置营业执照的名称（官方 <c>qualification_name</c>；官方示例值恒与 <c>business_name</c> 相同，照录）。</summary>
    [JsonPropertyName("qualification_name")]
    public string? QualificationName { get; set; }
}
