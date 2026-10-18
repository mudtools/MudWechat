// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 指定地区具体计费方法（官方 <c>all_freight_calc_method</c>）。
/// </summary>
/// <remarks>
/// <para>外层对象只含 <c>freight_calc_method_list</c> 一支；默认运费项的 <c>address_info</c> 必须为空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductFreightCalcMethods
{
    /// <summary>计费项列表。</summary>
    [JsonPropertyName("freight_calc_method_list")]
    public List<WxaProductFreightCalcMethodItem>? FreightCalcMethodList { get; set; }
}
