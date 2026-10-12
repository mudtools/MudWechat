// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.NearbyPoi;

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
