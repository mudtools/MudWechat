// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.product_album.list</c>：分页获取企业商品图册（商品编码/描述/价格）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只给商品事实</b>：<c>media_id</c>（商品图片，宿主素材资产）不投影——模型读不到二进制，
/// 与朋友圈、群发附件同一条边界；图片只给张数。
/// </para>
/// <para>
/// <b>价格单位是分</b>：官方 <c>price</c> 为整数分，描述里写明，免得模型按「元」解读后算出 100 倍的错。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.product_album.list",
    Description = "分页获取企业微信商品图册。参数 limit 为单页条数（1~100，默认 50）；cursor 为分页游标（首页省略，续页传上一页的 page_token）。返回 items（每项 product_id、product_sn 商品编码、description 商品描述、price 价格（单位为分）、create_time 创建时间戳、image_count 图片张数）、count 与 page_token。不含商品图片的 media_id（宿主素材资产）。自建应用只返回应用可见范围内用户的情况；拿到 product_id 后用 externalcontact.product_album.get 取详情。",
    Source = "IWechatWorkExternalContactProductAlbumService.GetProductAlbumListAsync",
    RequiredScopes = ["externalcontact.product_album.read"])]
public interface IWechatWorkExternalContactProductAlbumListTool
{
    /// <summary>分页获取商品图册。</summary>
    /// <returns>白名单投影后的 JSON 文本（items、count、page_token）。</returns>
    Task<string> GetProductAlbumListAsync(
        [ToolParameter("limit", "单页条数（1~100，可空 ⇒ 50）。")] int? limit,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        CancellationToken cancellationToken = default);
}
