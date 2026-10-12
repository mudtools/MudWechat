// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 当日售卖上限提醒（官方 <c>sale_limit_info</c> 对象；店铺受到售卖管控时返回，无本字段表示无额外限制）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSaleLimitInfo
{
    /// <summary>获取或设置是否受到管控（官方 <c>is_limited</c>，存在售卖限制时固定返回 1）。</summary>
    [JsonPropertyName("is_limited")]
    public int? IsLimited { get; set; }

    /// <summary>获取或设置售卖限制标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置售卖限制描述（官方 <c>sub_title</c>）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }
}
