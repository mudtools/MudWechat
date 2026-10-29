// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>商品受限场景常量（官方 <c>scene</c> 枚举值，<c>getproductrestrictedinfo</c>）。</summary>
public static class ChannelsProductRestrictedScenes
{
    /// <summary>直播间。</summary>
    public const int LiveRoom = 1;

    /// <summary>短视频。</summary>
    public const int ShortVideo = 2;

    /// <summary>店铺主页及达人橱窗。</summary>
    public const int StoreWindow = 3;

    /// <summary>搜索。</summary>
    public const int Search = 4;

    /// <summary>广告。</summary>
    public const int Advertisement = 5;

    /// <summary>订单及卡片。</summary>
    public const int Order = 6;

    /// <summary>优选联盟。</summary>
    public const int League = 7;

    /// <summary>商品推荐。</summary>
    public const int Recommend = 8;

    /// <summary>商品售卖。</summary>
    public const int Sale = 9;

    /// <summary>隐藏不合规信息。</summary>
    public const int HideInvalidInfo = 10;
}
