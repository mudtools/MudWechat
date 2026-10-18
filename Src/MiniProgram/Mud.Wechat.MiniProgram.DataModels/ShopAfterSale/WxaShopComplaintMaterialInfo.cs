// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 纠纷材料（文本 + 图片 URL 列表）。
/// </summary>
/// <remarks>
/// <para><b>媒体是 URL 直链而非 media_id</b>：传输组件与交易投诉两套体系不同，本域 <c>media_url_list</c></para>
/// <para>直接给可访问 URL；官方对交易投诉图片另有「URL 有过期时间、需每次重新调用接口」的告警（见 guarantee/complaint 页）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopComplaintMaterialInfo
{
    /// <summary>说明文本。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>图片 URL 列表。</summary>
    [JsonPropertyName("media_url_list")]
    public List<string>? MediaUrlList { get; set; }
}
