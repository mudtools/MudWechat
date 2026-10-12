// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>精选展示位展示样式常量（官方 <c>scale</c> 枚举值）。</summary>
public static class ChannelsBannerScales
{
    /// <summary>小图模式（展示位 3–5 个，标题 ≤12 字，描述不可设置）。</summary>
    public const int Small = 1;

    /// <summary>大图模式（标题 ≤40 字，描述 ≤100 字）。</summary>
    public const int Large = 2;
}
