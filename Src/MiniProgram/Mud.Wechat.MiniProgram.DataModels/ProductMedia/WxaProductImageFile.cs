// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>
/// 图片上传回包 <c>pic_file</c> 层。
/// </summary>
/// <remarks>
/// <para><b>官方表与示例冲突（照录）</b>：参数表把 <c>media_id</c> 写作 number，示例为字符串
/// （如 <c>"xlvjalsdhgas"</c>）⇒ 本仓按示例取 <c>string</c>。</para>
/// <para><b>三支按模式出现</b>：<c>resp_type=0</c> 回 <c>media_id</c> + <c>pay_media_id</c>；
/// <c>resp_type=1</c> 回 <c>temp_img_url</c> ⇒ 同一回包三字段均可空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductMedia")]
public class WxaProductImageFile
{
    /// <summary>媒体 id（resp_type=0）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>支付媒体 id（resp_type=0）。</summary>
    [JsonPropertyName("pay_media_id")]
    public string? PayMediaId { get; set; }

    /// <summary>图片链接（resp_type=1）；有访问频率限制，不可当图床。</summary>
    [JsonPropertyName("temp_img_url")]
    public string? TempImgUrl { get; set; }
}
