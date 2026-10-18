// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 图片列表（<c>qualifications</c> 与 <c>business_addition_pics</c> 共用形态）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayPicFileList
{
    /// <summary>图片 <c>media_id</c> 数组（<b>必填</b>），每项格式同 <see cref="WxaShopWxPayPicFile"/>。</summary>
    [JsonPropertyName("pic_file_list")]
    public List<WxaShopWxPayPicFile>? PicFileList { get; set; }
}
