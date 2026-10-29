// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送门店地址（<c>address_info</c> 内嵌对象；创建 / 查询 / 更新门店复用）。</summary>
/// <remarks>字段以官方「同城配送」文档 2.1 AddressInfo 为准（2026-10-11 核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityAddress
{
    /// <summary>省/自治区/直辖市（<c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>地级市（<c>city</c>，必填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区县（<c>area</c>，必填）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>街道（<c>street</c>，必填）。</summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>门牌号（<c>house</c>，必填）。</summary>
    [JsonPropertyName("house")]
    public string? Address { get; set; }

    /// <summary>经度（<c>lng</c>，必填）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>，必填）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>联系电话（<c>phone</c>，必填）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }
}
