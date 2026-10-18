// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 支付方式取值（官方 <c>pay_method_type</c>，写侧三值、读侧一值）。
/// </summary>
/// <remarks>
/// <para><b>值域宽窄随页不同</b>：<c>/shop/order/add</c> 请求表给 0 / 1 / 2 三值，
/// 详情表原文「0: 微信支付(目前只有微信支付)」⇒ 读侧只可能回 0；把 1 / 2 当「读得到」会做出死分支。</para>
/// <para><b>0 有连带必填</b>：官方明写 <c>pay_method_type = 0</c> 时 <c>prepay_id</c> 与 <c>prepay_time</c> 必填，
/// 且 <c>/shop/order/pay</c> 在「action_type=1 且 add 时传 0」时必填 <c>transaction_id</c>。</para>
/// </remarks>
public static class WxaShopPayMethodTypes
{
    /// <summary>0 微信支付（默认值；读侧唯一取值）。</summary>
    public const int WechatPay = 0;

    /// <summary>1 货到付款（仅写侧可传）。</summary>
    public const int CashOnDelivery = 1;

    /// <summary>2 商家会员储蓄卡（仅写侧可传）。</summary>
    public const int MemberStoredValueCard = 2;
}
