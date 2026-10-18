// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.product_album.get</c>：按商品 ID 读取商品图册详情。
/// </summary>
/// <remarks>
/// <para>
/// <b>与列表的分工</b>：列表已含全部业务字段，本工具用于「已知 product_id 单独取一件」；
/// 图册可读企业内所有企业级图册（不限应用自己创建的），故描述里点明这一可见性差异。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.product_album.get",
    Description = "按商品 ID 获取企业微信商品图册详情。参数 product_id 为商品 ID（先用 externalcontact.product_album.list 获取）。返回 product_id、product_sn 商品编码、description 商品描述、price 价格（单位为分）、create_time 创建时间戳与 image_count 图片张数。不含商品图片的 media_id（宿主素材资产）。该接口可读企业内所有企业级商品图册，不限于本应用创建的。",
    Source = "IWechatWorkExternalContactProductAlbumService.GetProductAlbumAsync",
    RequiredScopes = ["externalcontact.product_album.read"])]
public interface IWechatWorkExternalContactProductAlbumGetTool
{
    /// <summary>读取商品图册详情。</summary>
    /// <returns>白名单投影后的 JSON 文本（product_id、product_sn、description、price、create_time、image_count）。</returns>
    Task<string> GetProductAlbumAsync(
        [ToolParameter("product_id", "商品 ID（先用 externalcontact.product_album.list 获取）。", Required = true)] string product_id,
        CancellationToken cancellationToken = default);
}
