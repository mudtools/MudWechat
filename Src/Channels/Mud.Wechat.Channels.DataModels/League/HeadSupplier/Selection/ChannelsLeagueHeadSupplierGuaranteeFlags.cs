// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 选品广场保障标常量（官方 <c>spu_item_condition.flags</c> 数组元素取值，照抄官方原文顺序）。
/// </summary>
/// <remarks>
/// 与商品详情 <c>extra_service</c> 的各布尔开关（见 <see cref="ChannelsLeagueHeadSupplierProductDetailExtraService"/>）
/// 是「筛选条件 vs 回包字段」两种形态，取值语义相近但不共用类型。
/// </remarks>
public static class ChannelsLeagueHeadSupplierGuaranteeFlags
{
    /// <summary>7 天无理由。</summary>
    public const int SevenDayReturn = 0;

    /// <summary>运费险。</summary>
    public const int FreightInsurance = 1;

    /// <summary>品牌（官方标注已废弃）。</summary>
    public const int Brand = 2;

    /// <summary>放心买。</summary>
    public const int BuyWithConfidence = 3;

    /// <summary>损坏包退。</summary>
    public const int DamageGuarantee = 4;

    /// <summary>假一赔三。</summary>
    public const int FakeOnePayThree = 5;

    /// <summary>先用后付。</summary>
    public const int PayAfterUse = 6;

    /// <summary>包邮。</summary>
    public const int FreeShipping = 7;
}
