// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>精选展示位视频号参数（官方 <c>banner.banner[].finder</c> 对象，type=3 时设置）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerFinder
{
    /// <summary>获取或设置视频号 ID（官方 <c>finder_user_name</c>；登录视频号助手在首页查看）。</summary>
    [JsonPropertyName("finder_user_name")]
    public string? FinderUserName { get; set; }

    /// <summary>获取或设置视频 ID（官方 <c>feed_id</c>，视频号视频的唯一标识；登录视频号助手在「动态管理」模块复制）。</summary>
    [JsonPropertyName("feed_id")]
    public string? FeedId { get; set; }
}
