// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>获取生效中的品牌资质列表（<c>getvalidbrandlistlogic</c>）响应。</summary>
/// <remarks>仅返回状态为生效中（3）和即将过期（5）的品牌资质；即将过期的资质不影响商品正常售卖，但建议尽快续期。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetValidBrandListResponse : ChannelsResponse
{
    /// <summary>获取或设置生效中的品牌资质列表（官方 <c>brands</c>）。</summary>
    [JsonPropertyName("brands")]
    public List<ChannelsBrandApplicationInfo>? Brands { get; set; }

    /// <summary>获取或设置符合条件的品牌资质总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置翻页游标（官方 <c>next_key</c>，返回空表示已到最后一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
