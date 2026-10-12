// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送收货人信息（<c>receiver_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityReceiver
{
    /// <summary>收货人姓名（<c>receiver_name</c>）。</summary>
    [JsonPropertyName("receiver_name")]
    public string? ReceiverName { get; set; }

    /// <summary>收货地址（<c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>经度（<c>lng</c>）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>收货人手机号（<c>phone_num</c>）。</summary>
    [JsonPropertyName("phone_num")]
    public string? PhoneNumber { get; set; }
}
