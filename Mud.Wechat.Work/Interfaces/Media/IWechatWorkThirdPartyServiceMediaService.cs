// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Media;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「素材管理」域服务商通道「上传临时素材」第三方应用 SDK：
/// 官方仅向第三方应用开放本族端点（<c>/cgi-bin/service/media/upload</c>，服务商以自身
/// provider_access_token 代上传素材），全部声明于本接口。
/// <para>自建 / 代开发应用官方不开放本族端点，不设对应子接口；三类应用公共面见 <see cref="IWechatWorkMediaService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费服务商级 provider_access_token（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Media",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkServiceMediaService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyServiceMediaService : IWechatWorkServiceMediaService
{
    /// <summary>
    /// 服务商上传临时素材
    /// <para>服务商以自身 provider_access_token 代上传临时素材（multipart/form-data，文件标识名必须为 <c>media</c>，
    /// 须包含 filename、filelength、content-type 等信息；filename 不能以正斜线开始，不能包含反斜线与冒号）。</para>
    /// <para>官方限制：所有文件必须大于 5 个字节；图片（image）2MB、仅支持 JPG/PNG；
    /// 语音（voice）2MB、播放长度不超过 60s、仅支持 AMR 格式；视频（video）10MB、仅支持 MP4 格式；普通文件（file）20MB。</para>
    /// <para>返回的 media_id 仅三天内有效，同一企业内应用之间可以共享。</para>
    /// </summary>
    /// <param name="type">媒体文件类型（官方必填）：image - 图片，voice - 语音，video - 视频，file - 普通文件。</param>
    /// <param name="attachmentType">附件类型（官方选填）：用于特定场景（如 3 - 收银台），普通场景不指定。</param>
    /// <param name="formData">multipart 表单内容（文件字段名须为 <c>media</c>，由调用方构建
    /// <see cref="IFormContent"/> 实现，含 filename / filelength / content-type 信息）。
    /// 须标 <c>[MultipartForm]</c> —— 生成器 3.0.1 对 <c>[FormContent]</c> 发射的
    /// <c>GetFormDataContentAsync</c> 调用在运行时接口上不存在（CS1061），
    /// <c>[MultipartForm]</c> 才走 <see cref="IFormContent.ToHttpContentAsync"/> 通路。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒体文件类型（type）、素材 id（media_id，三天有效）与上传时间戳（created_at）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99310"/></para>
    /// </remarks>
    [Post("/cgi-bin/service/media/upload")]
    Task<UploadMediaResponse> UploadServiceMediaAsync(
        [Query("type")] string type,
        [MultipartForm] IFormContent formData,
        [Query("attachment_type")] int? attachmentType = null,
        CancellationToken cancellationToken = default);
}
