// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>无忧退货「发货地址 / 收货地址」（<c>delivery_place</c> / <c>receipt_place</c> 内嵌对象，同构）。</summary>
/// <remarks>官方字段以 <c>insurance_freight/createorder</c> 的「Body.delivery_place Object Payload」为准（2026-10-12 核验）；<c>county</c> 为<b>区</b>（非 <c>area</c>，勿按其它域习惯改写）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressInsuranceFreightPlace
{
    /// <summary>省（<c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>市（<c>city</c>，必填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区（<c>county</c>，必填）。</summary>
    [JsonPropertyName("county")]
    public string? County { get; set; }

    /// <summary>详细地址（<c>address</c>，必填）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}
