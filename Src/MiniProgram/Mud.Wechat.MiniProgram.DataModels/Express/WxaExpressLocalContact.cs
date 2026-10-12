// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送「发件人 / 收件人」信息（<c>sender</c> / <c>receiver</c> 内嵌对象，同构）。</summary>
/// <remarks>
/// <b>与 <see cref="WxaExpressContact"/> 的差异（勿混用）</b>：本结构带 <c>city</c> / <c>address</c> /
/// <c>address_detail</c> 三级地址与 <c>lng</c> / <c>lat</c> 坐标，<b>无</b> <c>tel</c> / <c>mobile</c> 二分。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalContact
{
    /// <summary>姓名（<c>name</c>，必填；最长不超过 256 个字符）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>城市名称（<c>city</c>，必填；如「广州市」）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>地址（<c>address</c>，必填；街道、小区、大厦等，用于定位）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>地址详情（<c>address_detail</c>，必填；楼号、单元号、层号）。</summary>
    [JsonPropertyName("address_detail")]
    public string? AddressDetail { get; set; }

    /// <summary>电话/手机号（<c>phone</c>，必填；最长不超过 64 个字符）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }

    /// <summary>经度（<c>lng</c>，必填；精确到小数点后 6 位；与 <c>coordinate_type</c> 配合使用）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>，必填；精确到小数点后 6 位；与 <c>coordinate_type</c> 配合使用）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>坐标类型（<c>coordinate_type</c>，可选；<c>0</c> 火星坐标——高德/腾讯地图，<c>1</c> 百度坐标）。</summary>
    [JsonPropertyName("coordinate_type")]
    public int? CoordinateType { get; set; }
}
