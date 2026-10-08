// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Media;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「素材管理」域公共 SDK（上传临时素材 / 获取临时素材 / 上传图片 / 获取高清语音素材 / 异步上传临时素材，
/// 三类应用均可调用的端点收敛面）。
/// <para>
/// 服务商通道「上传临时素材」（<c>/cgi-bin/service/media/upload</c>，走 provider_access_token）仅第三方应用开放，
/// 声明于 <see cref="IWechatWorkThirdPartyServiceMediaService"/>（零端点父接口
/// <see cref="IWechatWorkServiceMediaService"/>，形态对齐 <see cref="IWechatWorkIdentitySuiteService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkDepartmentsService"/>：三类应用消费的令牌路由键均为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMediaService
{
    /// <summary>
    /// 上传临时素材
    /// <para>用于发送消息的图片、语音、视频、普通文件上传（multipart/form-data，文件标识名必须为 <c>media</c>，
    /// 须包含 filename、filelength、content-type 等信息；filename 决定发消息时展示的文件名）。</para>
    /// <para>官方限制：所有文件必须大于 5 个字节；图片（image）10MB、仅支持 JPG/PNG；
    /// 语音（voice）2MB、播放长度不超过 60s、仅支持 AMR 格式；视频（video）10MB、仅支持 MP4 格式；普通文件（file）20MB。</para>
    /// <para>返回的 media_id 仅三天内有效，同一企业内应用之间可以共享。</para>
    /// </summary>
    /// <param name="type">媒体文件类型（官方必填）：image - 图片，voice - 语音，video - 视频，file - 普通文件。</param>
    /// <param name="formData">multipart 表单内容（文件字段名须为 <c>media</c>，由调用方构建
    /// <see cref="IFormContent"/> 实现，含 filename / filelength / content-type 信息）。
    /// 须标 <c>[MultipartForm]</c> —— 生成器 3.0.1 对 <c>[FormContent]</c> 发射的
    /// <c>GetFormDataContentAsync</c> 调用在运行时接口上不存在（CS1061），
    /// <c>[MultipartForm]</c> 才走 <see cref="IFormContent.ToHttpContentAsync"/> 通路。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒体文件类型（type）、素材 id（media_id，三天有效）与上传时间戳（created_at）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90253"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90389"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96484"/></para>
    /// </remarks>
    [Post("/cgi-bin/media/upload")]
    Task<UploadMediaResponse> UploadTemporaryMediaAsync(
        [Query("type")] string type,
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取临时素材
    /// <para>凭 media_id 获取临时素材文件内容。成功时与普通 HTTP 下载相同（Content-Type / Content-disposition
    /// 等头部 + 二进制文件内容），失败时返回 JSON（如 errcode 40007 invalid media_id）。</para>
    /// <para>官方限制：异步上传临时素材获得的 media_id 超过 20M 时必须使用 Range 分块下载且分块不超过 20M，
    /// 否则返回错误码 830002（其余 media_id 文件过大同样返回 830002，建议分块不超过 20M）；
    /// 加密存储场景下指定下载分片长度只能为 16 字节的倍数，否则无法进行分片解密
    /// （以 Content-Range 结束位置 + 1 是否等于 size 判断是否为最后一个分片）。</para>
    /// </summary>
    /// <param name="mediaId">媒体文件 id（官方必填），来源为「上传临时素材」及「异步上传临时素材」接口。</param>
    /// <param name="range">HTTP <c>Range</c> 请求头（可选，如 <c>bytes=0-1023</c>）；
    /// 分块下载成功返回状态码 206 Partial Content，响应含 Accept-Ranges 与 Content-Range 头。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>素材文件二进制内容（media_id 三天有效；错误时由 errcode 判定器抛 <see cref="WechatWorkException"/>）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90256"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90390"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96486"/></para>
    /// </remarks>
    [Get("/cgi-bin/media/get")]
    Task<byte[]?> GetTemporaryMediaAsync(
        [Query("media_id")] string mediaId,
        [Header("Range")] string? range = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传图片
    /// <para>上传图片得到图片 URL（multipart/form-data，文件标识名须包含 filename、content-type 等信息），该 URL 永久有效。</para>
    /// <para>官方限制：图片文件大小应在 5B ~ 2MB 之间；每个企业每月最多可上传 3000 张图片，每天最多可上传 1000 张图片；
    /// 返回的图片 URL 仅能用于图文消息正文中的图片展示，或者给客户发送欢迎语等——
    /// 若用于非企业微信环境下的页面，图片将被屏蔽。</para>
    /// </summary>
    /// <param name="formData">multipart 表单内容（由调用方构建 <see cref="IFormContent"/> 实现，
    /// 含 filename / content-type 信息）。
    /// 须标 <c>[MultipartForm]</c> —— 生成器 3.0.1 对 <c>[FormContent]</c> 发射的
    /// <c>GetFormDataContentAsync</c> 调用在运行时接口上不存在（CS1061），
    /// <c>[MultipartForm]</c> 才走 <see cref="IFormContent.ToHttpContentAsync"/> 通路。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>上传后得到的图片 URL（永久有效，仅限企业微信环境使用）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90254"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90392"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96485"/></para>
    /// </remarks>
    [Post("/cgi-bin/media/uploadimg")]
    Task<UploadImageResponse> UploadImageAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高清语音素材
    /// <para>获取 JSSDK uploadVoice 接口上传的语音文件内容。成功时与普通 HTTP 下载相同
    /// （Content-Type: voice/speex 等头部 + 二进制音频内容），失败时返回 JSON（如 errcode 40007 invalid media_id）。</para>
    /// <para>官方限制：该素材格式为 speex、16K 采样率，比普通临时素材接口（amr、8K）更清晰，
    /// 适合用作语音识别等对音质要求较高的业务，转码需使用 Speex 官方解码库并结合微信提供的解码库；
    /// 仅企业微信 2.4 及以上版本支持，暂时不支持鸿蒙系统上传的语音。</para>
    /// </summary>
    /// <param name="mediaId">通过 JSSDK 的 uploadVoice 接口上传的语音文件 id（官方必填）。</param>
    /// <param name="range">HTTP <c>Range</c> 请求头（可选，如 <c>bytes=0-1023</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>语音文件二进制内容（错误时由 errcode 判定器抛 <see cref="WechatWorkException"/>）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90255"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90391"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96487"/></para>
    /// </remarks>
    [Get("/cgi-bin/media/get/jssdk")]
    Task<byte[]?> GetHdVoiceMediaAsync(
        [Query("media_id")] string mediaId,
        [Header("Range")] string? range = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成异步上传任务
    /// <para>提交一个从 CDN URL 拉取文件并转存为临时素材的异步任务（适合超过 20M 的大文件场景）。
    /// 任务完成时官方会向应用回调地址推送 <c>upload_media_job_finish</c> 事件（仅携带 JobId，不含具体结果），
    /// 结果须凭 <see cref="UploadMediaByUrlAsync"/> 返回的 jobid 调用 <see cref="GetUploadMediaByUrlResultAsync"/> 查询。</para>
    /// <para>官方限制：须配置「客户联系」可调用接口的应用并具有客户联系权限；
    /// 所有文件必须大于 5 个字节，视频与普通文件上限 200MB，视频仅支持 MP4 格式（图片与语音暂不支持）；
    /// url 要求支持 Range 分块下载（腾讯云 COS 链接需设为「公有读」权限）。</para>
    /// </summary>
    /// <param name="request">异步上传任务请求体（<see cref="UploadMediaByUrlRequest"/>：scene / type / filename / url / md5）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步上传任务 id（jobid，60 分钟内有效）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96219"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97126"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96488"/></para>
    /// </remarks>
    [Post("/cgi-bin/media/upload_by_url")]
    Task<UploadMediaByUrlResponse> UploadMediaByUrlAsync(
        [Body] UploadMediaByUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询异步上传任务结果
    /// <para>凭 <see cref="UploadMediaByUrlAsync"/> 返回的 jobid 查询异步上传任务的执行结果。</para>
    /// <para>官方限制：jobid 最长 128 字节、60 分钟内有效；任务失败（status = 3）时 detail.errcode 返回非 0，
    /// 常见错误码：830001 - url 非法、830003 - url 下载数据失败、45001 - 文件大小超过限制（5 字节 ~ 200M）、
    /// 301019 - 文件 MD5 不匹配。</para>
    /// <para>注意：本接口返回的 media_id 与普通「上传临时素材」的使用场景不通用，
    /// 目前适用于「获取临时素材」（超 20M 须 Range 分块下载）与入群欢迎语素材管理（scene = 1）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetUploadMediaByUrlResultRequest"/>：jobid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务状态（status：1 - 处理中，2 - 完成，3 - 异常失败）与执行明细（<see cref="MediaUploadJobDetail"/>）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96219"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97126"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96488"/></para>
    /// </remarks>
    [Post("/cgi-bin/media/get_upload_by_url_result")]
    Task<GetUploadMediaByUrlResultResponse> GetUploadMediaByUrlResultAsync(
        [Body] GetUploadMediaByUrlResultRequest request,
        CancellationToken cancellationToken = default);
}
