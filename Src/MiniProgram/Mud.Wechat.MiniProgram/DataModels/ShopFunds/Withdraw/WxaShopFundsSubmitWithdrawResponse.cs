// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>发起提现应答（<c>POST /shop/funds/submitwithdraw</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/funds/submitwithdraw.html"/>。
/// <para><b>受理成功 ≠ 提现成功</b>：应答只回单号与二维码 ticket，终态须经 <see cref="WxaShopFundsWithdrawDetailResponse"/> 的 <c>status</c> 判定。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsSubmitWithdrawResponse : WxaResponse
{
    /// <summary>提现单号（<c>out_request_no</c>，后续查详情的定位键）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>二维码 ticket（<c>qrcode_ticket</c>，官方说明「可用于获取二维码和查询扫码状态，扫码验证身份提现」）。凭据类字段，禁入日志与遥测。</summary>
    [JsonPropertyName("qrcode_ticket")]
    public string? QrcodeTicket { get; set; }
}
