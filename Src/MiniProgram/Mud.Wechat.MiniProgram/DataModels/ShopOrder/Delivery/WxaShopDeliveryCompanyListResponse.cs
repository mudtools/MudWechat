// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 「获取快递公司列表」应答（<c>POST /shop/delivery/get_company_list</c>）。
/// </summary>
/// <remarks>
/// <para>官方请求示例是空对象 <c>{}</c> ⇒ 本端点<b>无请求体 DTO</b>（同 Vip 域 <c>shop/list/get</c> 口径，守卫 SO3 锁定零 [Body]）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/get_company_list.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopDeliveryCompanyListResponse : WxaResponse
{
    /// <summary>company_list（官方类型 <c>object[]</c>）：快递公司列表。</summary>
    [JsonPropertyName("company_list")]
    public List<WxaShopDeliveryCompany>? CompanyList { get; set; }
}
