// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 物流助手「寄件人 / 收件人」地址信息（<c>sender</c> / <c>receiver</c> 内嵌对象，
/// 商家寄件域 <c>order/add</c> 与面单预览 <c>template/preview</c> 复用）。
/// </summary>
/// <remarks>
/// <para>官方字段以 <c>express/business</c>（商家寄件）各页「Body 对象 Payload」为准（2026-10-11 逐页核验）。</para>
/// <para><c>tel</c> 与 <c>mobile</c> 二选一必填（官方原文：若不填写则必须填写另一个，不超过 32 字节）；姓名不超过 64 字节、地址不超过 512 字节。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressContact
{
    /// <summary>发/收件人姓名（<c>name</c>，不超过 64 字节）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>发/收件人座机号码（<c>tel</c>，与 <c>mobile</c> 二选一，不超过 32 字节）。</summary>
    [JsonPropertyName("tel")]
    public string? TelephoneNumber { get; set; }

    /// <summary>发/收件人手机号码（<c>mobile</c>，与 <c>tel</c> 二选一，不超过 32 字节）。</summary>
    [JsonPropertyName("mobile")]
    public string? MobileNumber { get; set; }

    /// <summary>发/收件人公司名称（<c>company</c>，不超过 64 字节）。</summary>
    [JsonPropertyName("company")]
    public string? Company { get; set; }

    /// <summary>发/收件人邮编（<c>post_code</c>，不超过 10 字节）。</summary>
    [JsonPropertyName("post_code")]
    public string? PostCode { get; set; }

    /// <summary>发/收件人国家（<c>country</c>，不超过 64 字节）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>发/收件人省份（<c>province</c>，例如“广东省”，不超过 64 字节）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>发/收件人市/地区（<c>city</c>，例如“广州市”，不超过 64 字节）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>发/收件人区/县（<c>area</c>，例如“海珠区”，不超过 64 字节）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>发/收件人详细地址（<c>address</c>，不超过 512 字节）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}
