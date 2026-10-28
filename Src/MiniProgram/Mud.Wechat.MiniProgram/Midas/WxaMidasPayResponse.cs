// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Midas;

/// <summary>
/// 扣除游戏币回包（<c>POST /cgi-bin/midas/pay</c>）。
/// </summary>
/// <remarks>
/// <para><b>系统失败 ≠ 未扣币</b>：返回 <c>-1</c> 等系统失败时须同 <c>bill_no</c> 重试，不可直接换单号。</para>
/// <para>错误码：90010 未登录/登录态过期、90011 sig 签名错误、90012 订单已存在、90013 余额不足、
/// 90017 无调用权限、90018 参数错误。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Midas")]
public class WxaMidasPayResponse : WxaResponse
{
    /// <summary>订单号（<b>有效期 48 小时</b>）。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>预扣后的余额。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>本次扣掉的赠送币金额。</summary>
    [JsonPropertyName("used_gen_amt")]
    public long? UsedGenAmount { get; set; }
}
