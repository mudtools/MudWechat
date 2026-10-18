// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>获取提现记录详情应答（<c>POST /shop/funds/getwithdrawdetail</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/funds/getwithdrawdetail.html"/>。
/// <para><b>单对象 <c>withdraw_info</c></b>（官方建为 Object WithDrawInfo，非列表）；官方参数表把它的说明误写成「退款详情信息」，字段名与页面标题均为提现 ⇒ 照录留档、不另建退款 DTO。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsWithdrawDetailResponse : WxaResponse
{
    /// <summary>提现详情（<c>withdraw_info</c>，官方类型 Object WithDrawInfo），见 <see cref="WxaShopFundsWithdrawInfo"/>。</summary>
    [JsonPropertyName("withdraw_info")]
    public WxaShopFundsWithdrawInfo? WithdrawInfo { get; set; }
}
