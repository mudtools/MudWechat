// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>物流轨迹推送的「寄件人 / 收件人」信息（<c>sender</c> / <c>receiver</c> 内嵌对象，<c>pathnotify</c> 专用）。</summary>
/// <remarks>
/// <para>
/// 官方字段以 <c>delivery/pathnotify</c> 的「Body.sender / Body.receiver Object Payload」为准（2026-10-12 核验）。
/// </para>
/// <para>
/// <b>与 <see cref="WxaExpressContact"/> 的差异（勿混用）</b>：本结构的收寄件人只有单一 <c>phone</c>
/// （无 <c>tel</c>/<c>mobile</c> 二选一），且带 <c>street</c> / <c>id</c> 两个地址扩展字段。
/// 官方原文：姓名<b>收件人必传、寄件人非必传</b>；电话<b>收件人必传、寄件人非必传</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathNotifyContact
{
    /// <summary>姓名（<c>name</c>；收件人必传，寄件人非必传）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>电话（<c>phone</c>；收件人必传，寄件人非必传）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }

    /// <summary>省（<c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>市（<c>city</c>，必填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区/县（<c>area</c>，必填）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>街道（<c>street</c>，可选）。</summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>地址（<c>address</c>，可选）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>地址 ID（<c>id</c>，可选）。</summary>
    [JsonPropertyName("id")]
    public string? AddressId { get; set; }
}
