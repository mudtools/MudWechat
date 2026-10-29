// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Midas;

/// <summary>
/// 取消扣除游戏币订单回包（<c>POST /cgi-bin/midas/cancelpay</c>）。
/// </summary>
/// <remarks>
/// <para>错误码：90000 订单不存在、90010 未登录/登录态过期、90011 sig 签名错误、
/// 90014 订单已支付确认完成、90015 订单已回退、90016 订单处理中、90017 无调用权限、90018 参数错误。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Midas")]
public class WxaMidasCancelPayResponse : WxaResponse
{
    /// <summary>扣除游戏币的订单号。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }
}
