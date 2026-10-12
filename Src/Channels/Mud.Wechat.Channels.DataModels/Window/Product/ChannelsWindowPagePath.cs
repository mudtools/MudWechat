// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>橱窗商品详情页路径（官方 <c>page_path</c> 对象，<c>getwindowproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowPagePath
{
    /// <summary>获取或设置商品详情半屏页、全屏页所属 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置商品详情半屏页 path（官方 <c>half_page_path</c>）。</summary>
    [JsonPropertyName("half_page_path")]
    public string? HalfPagePath { get; set; }

    /// <summary>获取或设置商品详情全屏页 path（官方 <c>full_page_path</c>）。</summary>
    [JsonPropertyName("full_page_path")]
    public string? FullPagePath { get; set; }
}
