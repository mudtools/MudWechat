// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「售后单主文档」DTO（aftersale/getaftersaleorder、aftersale/getaftersalelist、
// aftersale/reason/get、aftersale/rejectreason/get）+ 售后单状态 / 原因 / 类型常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>售后原因（官方 <c>reason</c> / <c>reason_text</c> 枚举，全部定义见 获取全量售后原因）。</summary>
public static class ChannelsAftersaleReasons
{
    /// <summary>拍错/多拍。</summary>
    public const string IncorrectSelection = "INCORRECT_SELECTION";

    /// <summary>不想要了。</summary>
    public const string NoLongerWant = "NO_LONGER_WANT";

    /// <summary>无快递信息。</summary>
    public const string NoExpressInfo = "NO_EXPRESS_INFO";

    /// <summary>包裹为空。</summary>
    public const string EmptyPackage = "EMPTY_PACKAGE";

    /// <summary>已拒签包裹。</summary>
    public const string RejectReceivePackage = "REJECT_RECEIVE_PACKAGE";

    /// <summary>快递长时间未送达。</summary>
    public const string NotDeliveredTooLong = "NOT_DELIVERED_TOO_LONG";

    /// <summary>与商品描述不符。</summary>
    public const string NotMatchProductDesc = "NOT_MATCH_PRODUCT_DESC";

    /// <summary>质量问题。</summary>
    public const string QualityIssue = "QUALITY_ISSUE";

    /// <summary>卖家发错货。</summary>
    public const string SendWrongGoods = "SEND_WRONG_GOODS";

    /// <summary>三无产品。</summary>
    public const string ThreeNoProduct = "THREE_NO_PRODUCT";

    /// <summary>假冒产品。</summary>
    public const string FakeProduct = "FAKE_PRODUCT";

    /// <summary>七天无理由。</summary>
    public const string NoReason7Days = "NO_REASON_7_DAYS";

    /// <summary>平台代发起。</summary>
    public const string InitiateByPlatform = "INITIATE_BY_PLATFORM";

    /// <summary>其它。</summary>
    public const string Others = "OTHERS";
}
