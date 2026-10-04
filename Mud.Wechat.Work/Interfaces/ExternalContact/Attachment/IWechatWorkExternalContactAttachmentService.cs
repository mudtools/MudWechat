// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.Attachment;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块上传附件资源域公共 SDK（朋友圈 / 商品图册场景的附件上传）。
/// <para>
/// 官方对三类应用开放完全一致的 1 个端点，收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactAttachmentService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactAttachmentService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactAttachmentService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「客户联系 可调用接口的应用」中，第三方 / 代开发应用须具有「企业客户」权限。
/// </para>
/// <para>
/// 官方约束：请求体为 multipart/form-data（文件标识名 <c>media</c>）；所有文件必须大于 5 字节、不超过 10MB；
/// 图片仅支持 JPG/PNG（朋友圈图片长边 ≤10800 像素、短边 ≤1080 像素），视频仅支持 MP4（时长 ≤30 秒）；
/// 商品图册只支持图片，朋友圈只支持图片、视频；media_id 三天有效。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactAttachmentService
{
    /// <summary>
    /// 上传附件资源
    /// <para>上传朋友圈 / 商品图册场景的附件资源，返回三天有效的 media_id。</para>
    /// <para>请求体为 multipart/form-data，文件标识名必须为 <c>media</c>；
    /// <paramref name="mediaType"/> 取 image / video / file（商品图册仅支持 image，朋友圈仅支持图片、视频），
    /// <paramref name="attachmentType"/> 取 1（朋友圈）/ 2（商品图册）；
    /// 所有文件必须大于 5 字节且不超过 10MB。</para>
    /// </summary>
    /// <param name="mediaType">媒体文件类型（官方必填）：image - 图片，video - 视频，file - 普通文件。</param>
    /// <param name="attachmentType">附件类型（官方必填）：1 - 朋友圈，2 - 商品图册。</param>
    /// <param name="formData">multipart 表单内容（文件字段名须为 <c>media</c>，由调用方构建
    /// <see cref="IFormContent"/> 实现，含 filename / filelength / content-type 信息）。
    /// 须标 <c>[MultipartForm]</c> —— 生成器 3.0.1 对 <c>[FormContent]</c> 发射的
    /// <c>GetFormDataContentAsync</c> 调用在运行时接口上不存在（CS1061），
    /// <c>[MultipartForm]</c> 才走 <see cref="IFormContent.ToHttpContentAsync"/> 通路。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒体文件类型（type）、素材 id（media_id，三天有效）与上传时间戳（created_at）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95098"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95178"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96347"/></para>
    /// </remarks>
    [Post("/cgi-bin/media/upload_attachment")]
    Task<UploadAttachmentResponse> UploadAttachmentAsync(
        [Query("media_type")] string mediaType,
        [Query("attachment_type")] int attachmentType,
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);
}
