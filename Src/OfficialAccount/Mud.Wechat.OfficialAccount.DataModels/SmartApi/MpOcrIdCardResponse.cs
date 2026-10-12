// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 身份证识别（<c>POST /cv/ocr/idcard</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>type</c>（正面 / 背面，取值为 <c>Front</c>/<c>Back</c>）/
/// <c>name</c> / <c>id</c> / <c>valid_date</c> / <c>addr</c> / <c>gender</c> / <c>nationality</c>。
/// 正反面返回字段不同（正面返回 name/id/addr/gender/nationality，背面返回 valid_date）。
/// </para>
/// <para>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c>（invalid image url）/
/// <c>101001</c>（certificate not found）/<c>101002</c>（decode image failed，官方解决方案列写
/// 「图片大小超过限制，resp_type = 0: 2MB，resp_type = 1: 10MB」但全文未定义 <c>resp_type</c>）/
/// <c>101003</c>（not enough market quota，官方解决方案列留空，照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrIdCardResponse : MpResponse
{
    /// <summary>获取或设置正反面标志（官方 <c>type</c>，<c>Front</c> 正面 / <c>Back</c> 背面）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置姓名（官方 <c>name</c>，正面返回）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置身份证号（官方 <c>id</c>，正面返回）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置有效期（官方 <c>valid_date</c>，背面返回）。</summary>
    [JsonPropertyName("valid_date")]
    public string? ValidDate { get; set; }

    /// <summary>获取或设置地址（官方 <c>addr</c>，正面返回）。</summary>
    [JsonPropertyName("addr")]
    public string? Address { get; set; }

    /// <summary>获取或设置性别（官方 <c>gender</c>，正面返回）。</summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>获取或设置民族（官方 <c>nationality</c>，正面返回）。</summary>
    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }
}
