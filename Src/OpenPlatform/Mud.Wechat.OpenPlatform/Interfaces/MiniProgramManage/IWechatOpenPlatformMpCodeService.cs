// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 代码管理」子域 SDK（<b>16 端点</b>：提交代码 / 提审 / 查状态 / 加急 / 撤回 / 配额 / 发布 / 灰度 / 回退 / 页面 / 版本信息 / 可被搜索开关 / 上传提审素材）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载（第三方平台代授权方小程序调用），Query 参数名恒为 <c>access_token</c>（2026-10-17 逐页核验）。</para>
/// <para><b>动词陷阱</b>：本族查询类端点动词不统一（<c>get_auditstatus</c> 为 POST、<c>get_latest_auditstatus</c>/<c>get_page</c>/<c>queryquota</c>/<c>undocodeaudit</c>/<c>revertgrayrelease</c>/<c>revertcoderelease</c> 为 GET），逐页照抄官方。</para>
/// <para><b>落位</b>：注册组 <c>MiniProgramManage</c>；<c>uploadmedia</c>（multipart）落在本接口末端；本族 <c>get_qrcode</c>（成功应答为图片二进制流）不走声明式管线，由手写通道 <c>IOpenPlatformMpTrialQrCodeService</c> 承载（守卫 QRC 系列）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpCodeService
{
    /// <summary>
    /// 提交代码（<c>commit</c>，把模板代码上传到体验版）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/commit.html"/>。
    /// </summary>
    [Post("/wxa/commit")]
    Task<OpenPlatformResponse> CommitCodeAsync(
        [Body] OpenPlatformMpCommitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交审核（<c>submit_audit</c>，串行、<c>item_list</c> 最多 5 项）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/submitAudit.html"/>。
    /// </summary>
    [Post("/wxa/submit_audit")]
    Task<OpenPlatformMpSubmitAuditResponse> SubmitAuditAsync(
        [Body] OpenPlatformMpSubmitAuditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询审核状态（<c>get_auditstatus</c>，按 <c>auditid</c>；官方为 <b>POST</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getAuditStatus.html"/>。
    /// </summary>
    [Post("/wxa/get_auditstatus")]
    Task<OpenPlatformMpAuditStatusResponse> GetAuditStatusAsync(
        [Body] OpenPlatformMpAuditIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取最新一次审核提交状态（<c>get_latest_auditstatus</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getLatestAuditStatus.html"/>。
    /// </summary>
    [Get("/wxa/get_latest_auditstatus")]
    Task<OpenPlatformMpLatestAuditStatusResponse> GetLatestAuditStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 加急审核（<c>speedupaudit</c>，消耗加急额度）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/speedupCodeAudit.html"/>。
    /// </summary>
    [Post("/wxa/speedupaudit")]
    Task<OpenPlatformResponse> SpeedUpAuditAsync(
        [Body] OpenPlatformMpAuditIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤回审核（<c>undocodeaudit</c>，日 5 / 月 10 次上限，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/undoAudit.html"/>。
    /// </summary>
    [Get("/wxa/undocodeaudit")]
    Task<OpenPlatformResponse> UndoCodeAuditAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询剩余审核 / 加急次数（<c>queryquota</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/setCodeAuditQuota.html"/>。
    /// </summary>
    [Get("/wxa/queryquota")]
    Task<OpenPlatformMpQueryQuotaResponse> QueryAuditQuotaAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布代码（<c>release</c>，审核通过后全量发布，请求体为空 <c>{}</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/release.html"/>。
    /// </summary>
    [Post("/wxa/release")]
    Task<OpenPlatformResponse> ReleaseCodeAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 灰度发布（<c>grayrelease</c>，需已通过审核的线上版本）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/grayRelease.html"/>。
    /// </summary>
    [Post("/wxa/grayrelease")]
    Task<OpenPlatformResponse> GrayReleaseAsync(
        [Body] OpenPlatformMpGrayReleaseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消灰度发布（<c>revertgrayrelease</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/revertGrayRelease.html"/>。
    /// </summary>
    [Get("/wxa/revertgrayrelease")]
    Task<OpenPlatformResponse> RevertGrayReleaseAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 回退发布版本 / 获取历史版本（<c>revertcoderelease</c>，以 Query <c>action</c> 分派：<c>get_history_version</c> 返回 <c>version_list</c>，<c>set_specific_version</c> 需 <c>app_version</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/revertCodeRelease.html"/>。
    /// </summary>
    [Get("/wxa/revertcoderelease")]
    Task<OpenPlatformMpRevertCodeReleaseResponse> RevertCodeReleaseAsync(
        [Query("action")] string action,
        [Query("app_version")] long? appVersion = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已提交代码的页面列表（<c>get_page</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getCodePage.html"/>。
    /// </summary>
    [Get("/wxa/get_page")]
    Task<OpenPlatformMpGetPageResponse> GetPageAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取体验版 / 线上版信息（<c>getversioninfo</c>，请求体为空 <c>{}</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getVersionInfo.html"/>。
    /// </summary>
    [Post("/wxa/getversioninfo")]
    Task<OpenPlatformMpVersionInfoResponse> GetVersionInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可被搜索 / 客服入口状态（<c>getvisitstatus</c>，请求体须为空对象 <c>{}</c>，否则官方报 <c>44002</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getVisitStatus.html"/>。
    /// </summary>
    [Post("/wxa/getvisitstatus")]
    Task<OpenPlatformMpVisitStatusResponse> GetVisitStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改可被搜索 / 客服入口状态（<c>change_visitstatus</c>，<c>action</c> 取 <c>open</c> / <c>close</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/setVisitStatus.html"/>。
    /// </summary>
    [Post("/wxa/change_visitstatus")]
    Task<OpenPlatformResponse> ChangeVisitStatusAsync(
        [Body] OpenPlatformMpVisitStatusActionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传提审素材（<c>uploadmedia</c>，<c>multipart/form-data</c>，文件字段名固定 <c>media</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/uploadMediaToCodeAudit.html"/>。
    /// </summary>
    /// <param name="formData">
    /// multipart 表单内容（文件字段名必须为 <c>media</c>；由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>；
    /// 官方约束：图片 ≤2M 支持 PNG/JPEG/JPG/GIF，视频 ≤10MB 支持 MP4）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformMpUploadMediaResponse"/>（回填 <c>mediaid</c> 进提审接口的 <c>preview_info</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/uploadmedia</c> + <c>multipart/form-data</c>，表单仅 <c>media</c> 一支字段 ⇒ JSON DTO 通路无法承载二进制字段，守卫 MPG11 断言本端点无 JSON 请求 DTO（对齐授权域 <c>uploadauthmaterial</c> 的 AUTH5 处置）。</para>
    /// <para>Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>，权限集 18）。</para>
    /// <para><b>返回的 <c>mediaid</c> 有效期三天</b>（官方原文），过期须重新上传。</para>
    /// <para>官方错误码：<c>40005</c> 格式不对 / <c>40006</c> 大小超限 / <c>41005</c> 缺少多媒体数据 / <c>43002</c> 要求 POST。</para>
    /// </remarks>
    [Post("/wxa/uploadmedia")]
    Task<OpenPlatformMpUploadMediaResponse> UploadMediaAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);
}
