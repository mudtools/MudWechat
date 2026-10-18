// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 小程序方曾提交过的品牌资质（官方 <c>get_miniapp_certificate</c> 回包 <c>brand_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>本类型是提审侧 <see cref="WxaShopBrandAuditInfo"/> 的「历史投影」，二者不合并</b>：
/// 官方本页只回显三支（<c>brand_wording</c> / <c>sale_authorization</c> / <c>trademark_registration_certificate</c>），
/// 且图片均为微信侧转链后的永久链接（<c>store.mp.video.tencent-cloud.com</c> 等域名）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopMiniappBrandInfo
{
    /// <summary>品牌名（<c>brand_wording</c>，string；官方示例「商标名」）。</summary>
    [JsonPropertyName("brand_wording")]
    public string? BrandWording { get; set; }

    /// <summary>商标授权书（<c>sale_authorization</c>，string 数组，官方转链后的图片链接）。</summary>
    [JsonPropertyName("sale_authorization")]
    public List<string>? SaleAuthorization { get; set; }

    /// <summary>商标注册证（<c>trademark_registration_certificate</c>，string 数组，官方转链后的图片链接）。</summary>
    [JsonPropertyName("trademark_registration_certificate")]
    public List<string>? TrademarkRegistrationCertificate { get; set; }
}
