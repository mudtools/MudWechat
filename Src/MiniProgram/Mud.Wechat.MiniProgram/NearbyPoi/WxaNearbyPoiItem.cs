// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.NearbyPoi;

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
