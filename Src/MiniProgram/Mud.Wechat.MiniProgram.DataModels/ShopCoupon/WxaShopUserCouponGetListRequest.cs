// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 获取用户券列表请求体（<c>POST /shop/coupon/get_usercoupon_list</c>）。
/// </summary>
/// <remarks>
/// <para>参数表只列 <c>page_size</c>/<c>offset</c>，请求示例另带 <c>openid</c> ⇒ 按示例建模为可空过滤键，不得依赖它做强约束。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopUserCouponGetListRequest
{
    /// <summary>每页条数（必填，官方最大 200）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>偏移量（从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>用户 openid（示例独有，可空过滤键；敏感标识禁落日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
