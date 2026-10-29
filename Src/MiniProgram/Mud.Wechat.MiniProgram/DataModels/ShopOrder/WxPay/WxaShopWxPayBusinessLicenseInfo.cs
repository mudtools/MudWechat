// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 营业执照 / 登记证书信息（<c>/shop/wxpay/apply</c> 写侧）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayBusinessLicenseInfo
{
    /// <summary>营业执照扫描件（<b>必填</b>）：须彩色扫描或彩色数码拍摄；黑白复印件须加盖完整公章；不得添加无关水印；正面、完整、信息清晰，不接受二次剪裁、翻拍、PS。</summary>
    [JsonPropertyName("wxa_business_license_copy")]
    public WxaShopWxPayPicFile? WxaBusinessLicenseCopy { get; set; }

    /// <summary>经营者 / 法定代表人姓名（<b>必填</b>），须与证件一致。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }
}
