// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>
/// 精选展示位项（官方 <c>banner.banner[]</c> 数组元素；提交请求体、<c>getbanner</c> 生效列表与申请明细共用——三处结构一致）。
/// </summary>
/// <remarks>
/// 官方契约：<b>type=商品/视频号/公众号三选一，只能设置对应类型的参数</b>（官方错误码 268552621/268552622/268553233）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerItem
{
    /// <summary>获取或设置展示位内容（官方 <c>banner</c>：图片 / 标题 / 描述；提交时必填）。</summary>
    [JsonPropertyName("banner")]
    public ChannelsBannerContent? Banner { get; set; }

    /// <summary>获取或设置展示位类型（官方 <c>type</c>，见 <see cref="ChannelsBannerTypes"/>；提交时必填）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置商品参数（官方 <c>product</c>；type=1 商品时设置）。</summary>
    [JsonPropertyName("product")]
    public ChannelsBannerProduct? Product { get; set; }

    /// <summary>获取或设置视频号参数（官方 <c>finder</c>；type=3 视频号时设置）。</summary>
    [JsonPropertyName("finder")]
    public ChannelsBannerFinder? Finder { get; set; }

    /// <summary>获取或设置公众号参数（官方 <c>official_account</c>；type=4 公众号时设置）。</summary>
    [JsonPropertyName("official_account")]
    public ChannelsBannerOfficialAccount? OfficialAccount { get; set; }
}
