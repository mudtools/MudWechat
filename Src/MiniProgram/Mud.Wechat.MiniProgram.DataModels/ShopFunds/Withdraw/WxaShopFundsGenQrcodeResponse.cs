// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>生成提款二维码应答（<c>POST /shop/funds/qrcode/gen</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/qrcode/gen.html"/>。
/// <para><b>本域只产出 ticket、不产出图片</b>：官方取图（<c>qrcode_buf</c>，base64 二进制）与查扫码状态两支路由不在小程序线（归小店线，见守卫 SHOPF8 的零声明断言）⇒ 宿主须自行完成扫码闭环。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsGenQrcodeResponse : WxaResponse
{
    /// <summary>二维码 ticket（<c>qrcode_ticket</c>，官方说明「可用于获取二维码和检查扫码状态」）。凭据类字段，禁入日志与遥测。</summary>
    [JsonPropertyName("qrcode_ticket")]
    public string? QrcodeTicket { get; set; }
}
