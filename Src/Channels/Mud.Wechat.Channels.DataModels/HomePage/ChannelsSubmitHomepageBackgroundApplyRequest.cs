// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>
/// 提交主页背景图申请（<c>submitbackgroundapply</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：图片支持 bmp、jpg/jpeg、png、svg、webp 格式，上传大小上限 10MB；
/// 须用「上传图片」接口（resp_type=1）返回的 img_url，不接受其他格式的图片 url（前缀 <c>mmecimage.cn/p/</c> 可直接提交）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsSubmitHomepageBackgroundApplyRequest
{
    /// <summary>获取或设置图片链接（官方 <c>img_url</c>，必填；经「上传图片」接口获取，仅支持一张背景图，按 16:9 比例展示在主页顶部）。</summary>
    [JsonPropertyName("img_url")]
    public string ImgUrl { get; set; } = string.Empty;
}
