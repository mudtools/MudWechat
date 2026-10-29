// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Resource;

/// <summary>
/// 上传图片 V2 场景类型常量（官方 <c>Query.scene_type</c> 枚举值，<c>img_uploadv2</c>，URL 参数、必填）。
/// </summary>
/// <remarks>
/// <b>与视频上传场景类型不共用</b>：<see cref="ChannelsResourceVideoSceneTypes"/> 的取值来自
/// <c>video_initupload</c> 页（162 商品 / 173 售后），本枚举来自 <c>img_uploadv2</c> 页（当前官方仅列 170），
/// 两者数值不同、语义亦不同 ⇒ 分建（合并即静默错读）。
/// <c>upload_type</c>（0 二进制流 / 1 图片 URL）与 <c>img_upload</c> 同义同值，复用
/// <see cref="ChannelsResourceImgUploadTypes"/>。
/// </remarks>
public static class ChannelsResourceImgUploadV2SceneTypes
{
    /// <summary>售后相关图片（大小 ≤ 10MB；官方 2026-10-12 变更日志：由 5MB 调整为 10MB。支持 JPEG/PNG/BMP/GIF，不支持 WEBP/SVG）。</summary>
    public const int Aftersale = 170;
}
