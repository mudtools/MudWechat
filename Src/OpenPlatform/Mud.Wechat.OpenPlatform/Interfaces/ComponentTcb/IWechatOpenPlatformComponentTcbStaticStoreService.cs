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
/// 微信开放平台「云开发静态网站托管」域 SDK（<b>4 端点</b>：开通静态网站 / 查看状态 / 文件列表 / 获取上传链接）。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 <b>50</b> 条路由恒为 <c>/componenttcb/*</c> 前缀、恒为 <b>POST</b>、恒以 Query <c>access_token</c> 承载 <c>component_access_token</c>（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 <b>4</b> 条。</para>
/// <para>令牌与开放面同 <see cref="IWechatOpenPlatformComponentTcbEnvService"/>（仅 <c>component_access_token</c> 自己调用，无权限集 id、无端点级频率数字）。</para>
/// <para><b>开通即计费</b>：<c>createstaticstore</c> 官方应答仅 <c>errcode</c>/<c>errmsg</c>，无 dry-run ⇒ 调用前须确认环境归属。</para>
/// <para><c>staticuploadfile</c> 只签发直传凭据（官方注意事项原文：PUT 到 <c>signed_url</c>，请求头 <c>x-cos-security-token</c> 取 <c>token</c>），两个字段<b>禁入日志、遥测与异常消息</b>；直传不由本 SDK 实现。</para>
/// <para><b>落位</b>：<c>Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbStaticStoreService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.ComponentTcb</c>
/// （子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；<c>OpenPlatformModule.ComponentTcb</c>，注册组 <c>ComponentTcb</c>（六支接口共用一次注册）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ComponentTcb", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformComponentTcbStaticStoreService
{
    // ==================== 云开发静态网站托管（4 端点）====================

    /// <summary>
    /// 「开通静态网站」（<c>createStaticStore</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_createstaticstore.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbCreateStaticStoreRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/createstaticstore</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/createstaticstore")]
    Task<OpenPlatformResponse> CreateStaticStoreAsync(
        [Body] OpenPlatformComponentTcbCreateStaticStoreRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查看静态网站状态」（<c>getStaticStore</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_getstaticstore.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbDescribeStaticStoreRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbDescribeStaticStoreResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/describestaticstore</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/describestaticstore")]
    Task<OpenPlatformComponentTcbDescribeStaticStoreResponse> DescribeStaticStoreAsync(
        [Body] OpenPlatformComponentTcbDescribeStaticStoreRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取静态网站文件列表」（<c>getStaticStoreFile</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_getstaticstorefile.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetStaticStoreFileListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetStaticStoreFileListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/staticfilelist</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/staticfilelist")]
    Task<OpenPlatformComponentTcbGetStaticStoreFileListResponse> GetStaticFileListAsync(
        [Body] OpenPlatformComponentTcbGetStaticStoreFileListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取上传静态网站文件链接」（<c>getUploadStaticStoreFile</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_getuploadstaticstorefile.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetUploadStaticStoreFileLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetUploadStaticStoreFileLinkResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/staticuploadfile</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>敏感面</b>：<c>signed_url</c>/<c>token</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方原文：「上传链接使用说明：用户获取到返回数据后，需拼装一个请求来上传文件，注意以下几点： 请求方法为 PUT url 为返回包的 signed_url 字段 需增加 key 为 x-cos-security-token，value 为返回包的中token字段的Header 请求体为需上传的文件内容」</para>
    /// <para>应答 <c>signed_url</c> 与 <c>token</c> 为 COS 直传临时凭据：官方「注意事项」原文要求「请求方法为 PUT，url 为返回包的 <c>signed_url</c> 字段，需增加 <c>x-cos-security-token</c> 请求头（value 为返回包的 <c>token</c> 字段），请求体为文件内容」⇒ 直传本身由宿主完成，两个字段<b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/staticuploadfile")]
    Task<OpenPlatformComponentTcbGetUploadStaticStoreFileLinkResponse> GetStaticUploadLinkAsync(
        [Body] OpenPlatformComponentTcbGetUploadStaticStoreFileLinkRequest request,
        CancellationToken cancellationToken = default);

}
