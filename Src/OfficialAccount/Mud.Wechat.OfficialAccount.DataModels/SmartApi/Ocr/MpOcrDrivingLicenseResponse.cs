// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 驾驶证识别（<c>POST /cv/ocr/drivinglicense</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>id_num</c> / <c>name</c> / <c>sex</c> / <c>address</c> /
/// <c>birth_date</c> / <c>issue_date</c> / <c>car_class</c> / <c>valid_from</c> / <c>valid_to</c> /
/// <c>official_seal</c>（官方说明原文误作「印章文构」，照录）。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：<c>nationality</c> 出现在官方返回示例中但字段表未收录；
/// 云函数返回示例使用驼峰命名（<c>birthDate</c>/<c>carClass</c>…），与 HTTPS 示例的下划线命名不一致
/// ——SDK 以 HTTPS 示例的下划线字段名为准。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101003</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrDrivingLicenseResponse : MpResponse
{
    /// <summary>获取或设置证号（官方 <c>id_num</c>）。</summary>
    [JsonPropertyName("id_num")]
    public string? IdNumber { get; set; }

    /// <summary>获取或设置姓名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置性别（官方 <c>sex</c>）。</summary>
    [JsonPropertyName("sex")]
    public string? Sex { get; set; }

    /// <summary>获取或设置国籍（官方返回示例字段 <c>nationality</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }

    /// <summary>获取或设置地址（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置出生日期（官方 <c>birth_date</c>）。</summary>
    [JsonPropertyName("birth_date")]
    public string? BirthDate { get; set; }

    /// <summary>获取或设置初次领证日期（官方 <c>issue_date</c>）。</summary>
    [JsonPropertyName("issue_date")]
    public string? IssueDate { get; set; }

    /// <summary>获取或设置准驾车型（官方 <c>car_class</c>）。</summary>
    [JsonPropertyName("car_class")]
    public string? CarClass { get; set; }

    /// <summary>获取或设置有效期限起始日（官方 <c>valid_from</c>）。</summary>
    [JsonPropertyName("valid_from")]
    public string? ValidFrom { get; set; }

    /// <summary>获取或设置有效期限终止日（官方 <c>valid_to</c>）。</summary>
    [JsonPropertyName("valid_to")]
    public string? ValidTo { get; set; }

    /// <summary>获取或设置印章文字（官方 <c>official_seal</c>；官方说明原文误作「印章文构」，照录）。</summary>
    [JsonPropertyName("official_seal")]
    public string? OfficialSeal { get; set; }
}
