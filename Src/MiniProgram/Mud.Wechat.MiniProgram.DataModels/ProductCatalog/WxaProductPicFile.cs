// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 营业执照 / 身份证图片（官方 <c>pic_file</c>）。
/// </summary>
/// <remarks>
/// <para><c>media_id</c> 与 <c>pay_media_id</c> 均经上传图片接口换取，均必填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductPicFile
{
    /// <summary>图片 media_id（必填）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>支付侧图片 media_id（必填）。</summary>
    [JsonPropertyName("pay_media_id")]
    public string? PayMediaId { get; set; }
}
