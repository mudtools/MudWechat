// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「保障单」DTO（aftersale/getguaranteeorder、aftersale/searchguaranteeorder、
// aftersale/merchantacceptguarantee、aftersale/merchantmodifyguarantee、aftersale/merchantproofguarantee、
// aftersale/merchantrefuseguarantee）+ 保障单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>凭证图片分组（官方 <c>bad_pay_info.title_pic_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeTitlePic
{
    /// <summary>获取或设置凭证图片类型（官方 <c>title</c>）：1 坏损商品图；2 全部商品图；3 包裹图。</summary>
    [JsonPropertyName("title")]
    public int? Title { get; set; }

    /// <summary>获取或设置该类型下的凭证图片 URL 列表（官方 <c>pic_list</c>，旧素材兼容字段，输出图片临时访问 URL）。</summary>
    [JsonPropertyName("pic_list")]
    public List<string>? PicList { get; set; }

    /// <summary>获取或设置新素材列表（官方 <c>new_media_list</c>，支持图片和视频）。</summary>
    [JsonPropertyName("new_media_list")]
    public List<ChannelsGuaranteeMedia>? NewMediaList { get; set; }
}
