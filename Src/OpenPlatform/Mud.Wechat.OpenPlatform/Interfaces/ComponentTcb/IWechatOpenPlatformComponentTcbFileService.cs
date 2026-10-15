// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「云开发存储文件」域 SDK（<b>4 端点</b>：删除文件 / 获取下载链接 / 获取文件列表 / 获取上传链接）。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 <b>50</b> 条路由恒为 <c>/componenttcb/*</c> 前缀、恒为 <b>POST</b>、恒以 Query <c>access_token</c> 承载 <c>component_access_token</c>（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 <b>4</b> 条。</para>
/// <para>令牌与开放面同 <see cref="IWechatOpenPlatformComponentTcbEnvService"/>（仅 <c>component_access_token</c> 自己调用，无权限集 id、无端点级频率数字）。</para>
/// <para><b>本组只签发凭据、不搬字节</b>：<c>uploadfile</c> 返回 <c>url</c>/<c>token</c>/<c>authorization</c> 等 COS 直传参数，官方「其他说明」要求宿主自行拼装 <c>multipart/form-data</c> 的 POST（<c>key</c>/<c>Signature</c>/<c>x-cos-security-token</c>/<c>x-cos-meta-fileid</c>/<c>file</c>）；<c>batchdownloadfile</c> 返回带签名的 <c>download_url</c> ⇒ 直传通道<b>不由本 SDK 实现</b>（不引入新范式），凭据与签名链接一律<b>禁入日志、遥测与异常消息</b>。</para>
/// <para><c>batchdeletefile</c> 为<b>不可逆删除</b>，官方应答逐文件回 <c>file_list</c> 结果 ⇒ 须按元素判定成功与否，不得只看 <c>errcode</c>。</para>
/// <para><b>落位</b>：<c>Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbFileService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.ComponentTcb</c>
/// （子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；<c>OpenPlatformModule.ComponentTcb</c>，注册组 <c>ComponentTcb</c>（六支接口共用一次注册）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ComponentTcb", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformComponentTcbFileService
{
    // ==================== 云开发存储文件（4 端点）====================

    /// <summary>
    /// 「删除文件」（<c>deleteTcbFile</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/file-mgnt/api_deletetcbfile.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbDeleteTcbCloudFileRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbDeleteTcbCloudFileResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchdeletefile</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchdeletefile")]
    Task<OpenPlatformComponentTcbDeleteTcbCloudFileResponse> DeleteFilesAsync(
        [Body] OpenPlatformComponentTcbDeleteTcbCloudFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取下载文件链接」（<c>getDownloadFileLink</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/file-mgnt/api_getdownloadfilelink.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetDownloadTcbFileLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetDownloadTcbFileLinkResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchdownloadfile</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>敏感面</b>：<c>download_url</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>应答 <c>download_url</c> 为<b>带签名的临时链接</b> ⇒ 禁入日志、遥测与异常消息；批量面逐文件成功/失败可分离，须按元素判定而非仅看 <c>errcode</c>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchdownloadfile")]
    Task<OpenPlatformComponentTcbGetDownloadTcbFileLinkResponse> GetDownloadLinksAsync(
        [Body] OpenPlatformComponentTcbGetDownloadTcbFileLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取文件列表」（<c>getTcbFile</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/file-mgnt/api_gettcbfile.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetTcbCloudFileRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetTcbCloudFileResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/getbucket</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/getbucket")]
    Task<OpenPlatformComponentTcbGetTcbCloudFileResponse> GetFileListAsync(
        [Body] OpenPlatformComponentTcbGetTcbCloudFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取上传文件链接」（<c>getUploadFileLink</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/file-mgnt/api_getuploadfilelink.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetUploadTcbFileLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetUploadTcbFileLinkResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/uploadfile</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>敏感面</b>：<c>token</c>/<c>authorization</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方原文：「上传链接使用说明 用户获取到返回数据后，需拼装一个 HTTP POST 请求，其中 url 为返回包的 url 字段，Body 部分格式为 multipart/form-data，具体内容如下： key | value | 说明 | key | this/is/a/example/file.path | 请求包中的 path 字段 | Signature | q-sign-algorithm=sha1&amp;q-ak=AKID9... | 返回数据的 authorization 字段 | x-cos-security-token | Cukha70zkXIBqkh1Oh... | 返回数据的 token 字段 | x-cos-meta-fileid | HDze32/qZENCwWi5... | 返回数据的 cos_file_id 字段 | file | 文件内容 | 文件的二进制内容 |」</para>
    /// <para>本接口只<b>签发上传凭据</b>，不传输文件本体：官方「其他说明」要求宿主自行拼装 HTTP <c>POST</c>（<c>multipart/form-data</c>，Body 携带 <c>key</c>/<c>Signature</c>/<c>x-cos-security-token</c>/<c>x-cos-meta-fileid</c>/<c>file</c>）⇒ 该直传通道不在 SDK 内实现，<c>authorization</c>/<c>token</c> 属临时凭据。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/uploadfile")]
    Task<OpenPlatformComponentTcbGetUploadTcbFileLinkResponse> GetUploadLinkAsync(
        [Body] OpenPlatformComponentTcbGetUploadTcbFileLinkRequest request,
        CancellationToken cancellationToken = default);

}
