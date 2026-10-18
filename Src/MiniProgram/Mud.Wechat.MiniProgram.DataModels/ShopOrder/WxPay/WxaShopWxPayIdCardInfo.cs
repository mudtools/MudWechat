// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 经营者 / 法人身份证信息（<c>/shop/wxpay/apply</c>，<c>IDENTIFICATION_TYPE_MAINLAND_IDCARD</c> 时必填）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayIdCardInfo
{
    /// <summary>身份证人像面照片（<b>必填</b>）。</summary>
    [JsonPropertyName("wxa_id_card_copy")]
    public WxaShopWxPayPicFile? WxaIdCardCopy { get; set; }

    /// <summary>身份证国徽面照片（<b>必填</b>）。</summary>
    [JsonPropertyName("wxa_id_card_national")]
    public WxaShopWxPayPicFile? WxaIdCardNational { get; set; }

    /// <summary>身份证姓名（<b>必填</b>）：2～30 个中文字符 / 英文字符 / 符号。</summary>
    [JsonPropertyName("id_card_name")]
    public string? IdCardName { get; set; }

    /// <summary>身份证号码（<b>必填</b>；个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>身份证有效期结束时间（<b>必填</b>）：<c>yyyy-MM-dd</c>，长期填「长期」，有效期须大于 60 天。</summary>
    [JsonPropertyName("id_card_valid_time")]
    public string? IdCardValidTime { get; set; }

    /// <summary>身份证有效期开始时间（<b>必填</b>）：<c>yyyy-MM-dd</c>，长期填「长期」，结束须大于开始。</summary>
    [JsonPropertyName("id_card_valid_time_begin")]
    public string? IdCardValidTimeBegin { get; set; }

    /// <summary>身份证居住地址：主体类型为企业时需填写（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("id_card_address")]
    public string? IdCardAddress { get; set; }
}
