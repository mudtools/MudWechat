// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 身份证信息（官方 <c>id_card_info</c>）。
/// </summary>
/// <remarks>
/// <para>人像面与国徽面均须 media_id；身份证号、姓名属敏感个人信息，不得写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductIdCardInfo
{
    /// <summary>人像面照片。</summary>
    [JsonPropertyName("portrait_pic_file")]
    public WxaProductPicFile? PortraitPicFile { get; set; }

    /// <summary>国徽面照片。</summary>
    [JsonPropertyName("nation_pic_file")]
    public WxaProductPicFile? NationPicFile { get; set; }

    /// <summary>身份证姓名（敏感）。</summary>
    [JsonPropertyName("id_card_name")]
    public string? IdCardName { get; set; }

    /// <summary>身份证号码（敏感）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>注册日期（yyyy-MM-dd）。</summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>结束有效期。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>身份证居住地（敏感）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}
