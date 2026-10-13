// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「创建提现单」请求体（<c>POST /xpay/create_withdraw_order</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>withdraw_amount</c> 官方说明「允许不传，不传的情况下表示全额提现」</b>，而参数表把它标注为必填 ⇒ 本 SDK 取<b>参数表</b>形态（属性可空即允许省略），并在注释留档该自相矛盾。</para>
/// <para><b>单号格式是硬约束</b>：长度 [8,32]、仅字母/数字/下划线/短横线（与订阅扣款单号同一规则）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_create_withdraw_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayCreateWithdrawOrderRequest
{
    /// <summary>withdraw_no（官方类型 <c>string</c>，<b>必填</b>）：提现单单号，长度为[8,32]，字符只允许使用字母、数字、'\_'、'-'</summary>
    [JsonPropertyName("withdraw_no")]
    public string? WithdrawNo { get; set; }

    /// <summary>withdraw_amount（官方类型 <c>string</c>，<b>必填</b>）：提现的金额，单位元，例如提现1分钱请使用0.01，允许不传，不传的情况下表示全额提现</summary>
    [JsonPropertyName("withdraw_amount")]
    public string? WithdrawAmount { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }
}
