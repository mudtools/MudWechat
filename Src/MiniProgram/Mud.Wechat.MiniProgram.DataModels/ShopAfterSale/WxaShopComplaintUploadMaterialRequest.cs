// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 上传纠纷材料请求体（<c>POST /shop/complaint/upload_material</c>）。
/// </summary>
/// <remarks>
/// <para><b>只补材料、不改状态</b>：本端点仅向指定纠纷单追加说明文本与图片 URL，</para>
/// <para>不推进纠纷状态；重复上传属追加语义、<b>非幂等安全</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopComplaintUploadMaterialRequest
{
    /// <summary>纠纷单号（必填）。</summary>
    [JsonPropertyName("complaint_order_id")]
    public long? ComplaintOrderId { get; set; }

    /// <summary>说明文本与图片，见 <see cref="WxaShopComplaintMaterialInfo"/>（必填）。</summary>
    [JsonPropertyName("material_info")]
    public WxaShopComplaintMaterialInfo? MaterialInfo { get; set; }
}
