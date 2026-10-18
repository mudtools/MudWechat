// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>V1 售后原因（官方 <c>AfterSaleReason</c>）。</summary>
/// <remarks>
/// <para><b>应答 <c>reason</c> 是符号名</b>（如 <c>NO_LONGER_WANT</c>），本类给数字值域 ⇒ 二者语义对应、形态不同。</para>
/// </remarks>
public static class WxaProductOrderAfterSaleReasonTypes
{
    /// <summary>1 拍错 / 多拍。</summary>
    public const int IncorrectSelection = 1;

    /// <summary>2 不想要了。</summary>
    public const int NoLongerWant = 2;

    /// <summary>3 无快递信息。</summary>
    public const int NoExpressInfo = 3;

    /// <summary>4 包裹为空。</summary>
    public const int EmptyPackage = 4;

    /// <summary>5 已拒签包裹。</summary>
    public const int RejectReceivePackage = 5;

    /// <summary>6 快递长时间未送达。</summary>
    public const int NotDeliveredTooLong = 6;

    /// <summary>7 与商品描述不符。</summary>
    public const int NotMatchProductDesc = 7;

    /// <summary>8 质量问题。</summary>
    public const int QualityIssue = 8;

    /// <summary>9 卖家发错货。</summary>
    public const int SendWrongGoods = 9;

    /// <summary>10 三无产品。</summary>
    public const int ThreeNoProduct = 10;

    /// <summary>11 假冒产品。</summary>
    public const int FakeProduct = 11;

    /// <summary>12 其它。</summary>
    public const int Others = 12;
}
