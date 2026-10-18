// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>V2 售后理由类型（官方 <c>AfterSalesReasonType</c>）。</summary>
/// <remarks>
/// <para><b>适用状态随类型而变</b>：官方逐值标注了「未发货仅退款 / 已发货仅退款 / 退货退款」的适用范围，</para>
/// <para>选错组合会被网关拒绝 ⇒ 常量注释照录该适用范围。</para>
/// <para><b>4 与 13 已废弃</b>：照录保留以解释历史数据，<b>不得</b>在新请求中使用。</para>
/// </remarks>
public static class WxaEcAftersaleReasonTypes
{
    /// <summary>1 商品无货（未发货仅退款）。</summary>
    public const int NoStock = 1;

    /// <summary>2 发货时间问题（未发货仅退款）。</summary>
    public const int DeliveryTimeIssue = 2;

    /// <summary>3 不想要了（未发货仅退款）。</summary>
    public const int NoLongerWanted = 3;

    /// <summary>4 废弃（禁止使用）。</summary>
    public const int DeprecatedFour = 4;

    /// <summary>5 地址信息填写错误（未发货 / 已发货仅退款）。</summary>
    public const int WrongAddress = 5;

    /// <summary>6 买多 / 买错 / 不想要了（已发货仅退款 / 退货退款）。</summary>
    public const int BoughtWrongOrTooMany = 6;

    /// <summary>7 商品损坏 / 包装脏污（已发货仅退款 / 退货退款）。</summary>
    public const int Damaged = 7;

    /// <summary>8 少 / 错商品 / 与页面描述不符（已发货仅退款 / 退货退款）。</summary>
    public const int WrongOrMissingItem = 8;

    /// <summary>9 无效的物流单号（已发货仅退款）。</summary>
    public const int InvalidWaybill = 9;

    /// <summary>10 物流超 72 小时停滞（已发货仅退款）。</summary>
    public const int LogisticsStalled72Hours = 10;

    /// <summary>11 快递无法送到指定地点（已发货仅退款）。</summary>
    public const int Undeliverable = 11;

    /// <summary>12 显示签收但未收到商品（已发货仅退款）。</summary>
    public const int SignedNotReceived = 12;

    /// <summary>13 废弃（禁止使用）。</summary>
    public const int DeprecatedThirteen = 13;

    /// <summary>14 质量问题（退货退款）。</summary>
    public const int QualityIssue = 14;

    /// <summary>15 其他（未发货 / 已发货仅退款 / 退货退款）。</summary>
    public const int Other = 15;
}
