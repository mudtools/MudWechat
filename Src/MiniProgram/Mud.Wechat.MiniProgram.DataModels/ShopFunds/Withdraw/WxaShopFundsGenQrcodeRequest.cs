// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>生成提款二维码请求（<c>POST /shop/funds/qrcode/gen</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/qrcode/gen.html"/>。
/// <para><b>官方请求体只有 <c>identity_type</c> 一支</b>（必填）⇒ 提现单号、金额等一律不在本请求体内，ticket 与提现单的绑定发生在发起提现返回的 ticket 或宿主侧映射。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsGenQrcodeRequest
{
    /// <summary>需要验证的身份（<c>identity_type</c>，必填），取值见 <see cref="WxaShopFundsIdentityTypes"/>。</summary>
    [JsonPropertyName("identity_type")]
    public int? IdentityType { get; set; }
}
