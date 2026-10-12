// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>退货地址（<c>no_worry_return</c> 与 <c>return</c> 两族的 <c>biz_addr</c> / <c>user_addr</c> 内嵌对象）。</summary>
/// <remarks>字段以官方退货相关文档的 Body 对象 Payload 为准（2026-10-11 逐页核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressReturnAddress
{
    /// <summary>国家（<c>country</c>）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>省份（<c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>城市（<c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区县（<c>area</c>）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>详细地址（<c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>联系人（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>联系电话（<c>mobile</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? MobileNumber { get; set; }
}
