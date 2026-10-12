// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取带货者商品数据列表（<c>talent/getproductdatalist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetTalentProductDataListRequest
{
    /// <summary>获取或设置获取的商品数据类型（官方 <c>get_data_type</c>，必填，见 <see cref="ChannelsLeagueGetDataTypes"/>）。</summary>
    [JsonPropertyName("get_data_type")]
    public int GetDataType { get; set; }

    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，必填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int PageIndex { get; set; }

    /// <summary>获取或设置单页数量（官方 <c>page_size</c>，必填，不超过 20）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>，必填）。</summary>
    [JsonPropertyName("talent_appid")]
    public string TalentAppId { get; set; } = string.Empty;
}
