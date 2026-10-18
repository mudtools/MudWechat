// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 新增 / 更新地址请求体（<c>POST /product/address/add</c> 与 <c>/product/address/update</c> 共用）。
/// </summary>
/// <remarks>
/// <para><b>官方回包示例键名重复（照录）</b>：两页回包示例都写成两个 <c>"errcode"</c>（第二个应为 <c>errmsg</c>）</para>
/// <para>⇒ 回包只用 <see cref="WxaResponse"/>，不按错误示例建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductAddressUpsertRequest
{
    /// <summary>地址详情（必填，单对象包装）。</summary>
    [JsonPropertyName("address_detail")]
    public WxaProductAddressDetail? AddressDetail { get; set; }
}
