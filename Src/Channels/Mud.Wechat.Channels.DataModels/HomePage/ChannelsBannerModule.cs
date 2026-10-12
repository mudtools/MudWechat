// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>
/// 精选展示位模块（官方 <c>banner</c> 对象；提交请求体与 <c>getbanner</c> 响应的生效模块/申请模块共用——
/// 查询响应不携带 <c>title</c>，提交时必填 <c>banner</c> 数组与 <c>scale</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerModule
{
    /// <summary>获取或设置展示位列表（官方 <c>banner</c>；提交时必填，最多 5 个，小图模式最少 3 个）。</summary>
    [JsonPropertyName("banner")]
    public List<ChannelsBannerItem>? Banner { get; set; }

    /// <summary>获取或设置展示位的展示样式（官方 <c>scale</c>，见 <see cref="ChannelsBannerScales"/>；提交时必填）。</summary>
    [JsonPropertyName("scale")]
    public int? Scale { get; set; }

    /// <summary>获取或设置模块标题（官方 <c>title</c>，选填，最多 32 字）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
