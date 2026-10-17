// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Resource;

/// <summary>
/// 图片信息（官方 <c>img_info</c> 对象，<c>img_uploadv2</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceImgUploadV2ImgInfo
{
    /// <summary>
    /// 获取或设置图片 URL（官方 <c>img_url</c>）。
    /// </summary>
    /// <remarks>
    /// 官方原文：接口返回的图片链接支持图片处理（缩放 / 裁剪 / 压缩 / 水印等），
    /// 「图片处理」文档内的 <c>download_url</c> 即为当前接口返回的图片链接。
    /// </remarks>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
}
