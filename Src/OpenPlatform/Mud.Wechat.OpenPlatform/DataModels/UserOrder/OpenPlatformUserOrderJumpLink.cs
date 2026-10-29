// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.UserOrder;

/// <summary>
/// 订单/商品详情页跳转链接（官方 <c>order_detail_jump_link</c> 与 <c>item_detail_jump_link</c> 共用）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShoppingInfo.html"/></para>
/// <para><c>type</c> 取值 <c>MINI_PROGRAM</c>（小程序跳转，需 <c>appid</c> + <c>path</c>）或 <c>URL</c>（H5 链接，需 <c>url</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "UserOrder")]
public class OpenPlatformUserOrderJumpLink
{
    /// <summary>链接类型（官方必填 <c>type</c>；<c>MINI_PROGRAM</c> 或 <c>URL</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>小程序 AppId（官方 <c>appid</c>；<c>type = MINI_PROGRAM</c> 时必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>小程序页面路径（官方 <c>path</c>；<c>type = MINI_PROGRAM</c> 时必填）。</summary>
    [JsonPropertyName("path")]
    public string? PagePath { get; set; }

    /// <summary>H5 链接地址（官方 <c>url</c>；<c>type = URL</c> 时必填）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
