// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 品牌申请类型（<c>Application.apply_for</c> / 响应侧 <c>application.apply_for</c>）。
/// </summary>
/// <remarks>
/// <para><b>只有两值</b>：为「知名品牌」申请须交全套品牌材料；为「已接入小店商品组件」申请不需要
/// <c>audit_info</c>（官方标其为「当申请类型为知名品牌时必填」）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_famousbrandapply.html"/></para>
/// </remarks>
public static class WxaFamousBrandApplyForValues
{
    /// <summary>知名品牌（<c>1</c>）。</summary>
    public const int FamousBrand = 1;

    /// <summary>已接入小店商品组件（<c>2</c>）。</summary>
    public const int ShopGoodsComponent = 2;
}
