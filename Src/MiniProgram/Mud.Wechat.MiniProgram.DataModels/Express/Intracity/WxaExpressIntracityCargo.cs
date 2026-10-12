// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送货品信息（<c>cargo</c> 内嵌对象；询价 <c>preaddorder</c> 与下单 <c>addorder</c> 复用）。</summary>
/// <remarks>字段以官方「同城配送」文档 2.9/2.10 核验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityCargo
{
    /// <summary>货品名称（<c>cargo_name</c>，必填；长度 ≤ 50）。</summary>
    [JsonPropertyName("cargo_name")]
    public string? CargoName { get; set; }

    /// <summary>货品类型（<c>cargo_type</c>，必填；1 食品，2 文件，3 医药，4 电商）。</summary>
    [JsonPropertyName("cargo_type")]
    public int CargoType { get; set; }

    /// <summary>货品重量（<c>cargo_weight</c>，必填；单位：kg，精确到小数点后 2 位）。</summary>
    [JsonPropertyName("cargo_weight")]
    public double CargoWeight { get; set; }

    /// <summary>货品价值（<c>cargo_price</c>，必填；单位：元，精确到小数点后 2 位）。</summary>
    [JsonPropertyName("cargo_price")]
    public double CargoPrice { get; set; }

    /// <summary>货品数量（<c>cargo_num</c>，必填）。</summary>
    [JsonPropertyName("cargo_num")]
    public int CargoCount { get; set; }
}
