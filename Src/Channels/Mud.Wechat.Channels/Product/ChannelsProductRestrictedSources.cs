// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>受限策略来源常量（官方 <c>source</c> 枚举值，<c>getproductrestrictedinfo</c>）。</summary>
public static class ChannelsProductRestrictedSources
{
    /// <summary>上架策略-低风险。</summary>
    public const int LowRiskStrategy = 1;

    /// <summary>上架策略-相似品。</summary>
    public const int DuplicatedStrategy = 2;

    /// <summary>上架策略-隐藏图片。</summary>
    public const int HideImageStrategy = 3;

    /// <summary>试运营类目。</summary>
    public const int TrialCategory = 4;

    /// <summary>橱窗隐藏。</summary>
    public const int WindowHide = 5;

    /// <summary>极简模式。</summary>
    public const int SimpleMode = 6;

    /// <summary>售卖限制。</summary>
    public const int SaleLimit = 7;

    /// <summary>库存限制（优选联盟观察期）。</summary>
    public const int StockLimit = 8;
}
