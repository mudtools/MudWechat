// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 快递公司条目（官方 <c>company_list[]</c> 元素；与订单读侧物流单里的 <c>delivery_id</c> 同源取值）。
/// </summary>
/// <remarks>
/// <para><b>官方示例含明显笔误</b>：<c>YTO</c> / <c>ZTO</c> 的名称印成「圆通快速」「中通快速」（官方通行名为圆通速递、中通快递）
/// ⇒ 名称照 <c>delivery_name</c> 取值显示，不得硬编码校对。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/get_company_list.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopDeliveryCompany
{
    /// <summary>delivery_id（官方类型 <c>string</c>）：快递公司 id（如 SF / YTO），发货时填入 <c>delivery_list[].delivery_id</c>。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>delivery_name（官方类型 <c>string</c>）：快递公司名称。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}
