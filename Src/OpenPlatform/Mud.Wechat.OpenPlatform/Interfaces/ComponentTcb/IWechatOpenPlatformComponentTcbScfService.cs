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
/// 微信开放平台「云开发云函数管理」域 SDK（<b>7 端点</b>：创建云函数 / 删除云函数 / 获取列表 / 获取触发器 / 更新触发器 / 更新代码 / 更新配置）。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 <b>50</b> 条路由恒为 <c>/componenttcb/*</c> 前缀、恒为 <b>POST</b>、恒以 Query <c>access_token</c> 承载 <c>component_access_token</c>（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 <b>7</b> 条。</para>
/// <para>令牌与开放面同 <see cref="IWechatOpenPlatformComponentTcbEnvService"/>（仅 <c>component_access_token</c> 自己调用，无权限集 id、无端点级频率数字）。</para>
/// <para><b>代码包不经本 SDK 上传</b>：<c>batchuploadscf</c> 的 <c>code_secret</c> 由 <c>batchuploadscfcode</c> 侧取得，官方注意事项原文「zipfile 打包时，请直接选中所要打包的函数代码文件直接打包，请不要放入文件夹中对文件夹进行打包」⇒ 打包形态错误是官方点名的常见故障。</para>
/// <para><c>updatescfconfig</c> 的 <c>environment_variables</c> 为云函数环境变量（惯例存放密钥）⇒ <b>禁入日志、遥测与异常消息</b>；配置更新为<b>整面覆盖</b>语义，未提交的字段会被清掉。</para>
/// <para><c>batchupdatetriggers</c> 请求体带 <c>err_list</c> 应答（按环境回失败原因）⇒ 批量面「部分失败」必须逐元素判定。</para>
/// <para><b>落位</b>：<c>Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbScfService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.ComponentTcb</c>
/// （子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；<c>OpenPlatformModule.ComponentTcb</c>，注册组 <c>ComponentTcb</c>（六支接口共用一次注册）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ComponentTcb", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformComponentTcbScfService
{
    // ==================== 云开发云函数管理（7 端点）====================

    /// <summary>
    /// 「创建云函数」（<c>batchuUloadCloudFunction</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_batchuuloadcloudfunction.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbBatchUploadCloudFunctionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbBatchUploadCloudFunctionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchuploadscf</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方原文：「zipfile打包时，请直接选中所要打包的函数代码文件直接打包，请不要放入文件夹中对文件夹进行打包。」</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchuploadscf")]
    Task<OpenPlatformComponentTcbBatchUploadCloudFunctionResponse> CreateFunctionsAsync(
        [Body] OpenPlatformComponentTcbBatchUploadCloudFunctionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除云函数」（<c>deleteCloudFunction</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_deletecloudfunction.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbDeleteCloudFunctionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/deletescf</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/deletescf")]
    Task<OpenPlatformResponse> DeleteFunctionAsync(
        [Body] OpenPlatformComponentTcbDeleteCloudFunctionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取云函数列表」（<c>getCloudFunctionList</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_getcloudfunctionlist.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetCloudFunctionListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetCloudFunctionListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/getscflist</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/getscflist")]
    Task<OpenPlatformComponentTcbGetCloudFunctionListResponse> GetFunctionListAsync(
        [Body] OpenPlatformComponentTcbGetCloudFunctionListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取触发器」（<c>getTriggers</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_gettriggers.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetTriggersRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetTriggersResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/gettriggers</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/gettriggers")]
    Task<OpenPlatformComponentTcbGetTriggersResponse> GetTriggersAsync(
        [Body] OpenPlatformComponentTcbGetTriggersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「更新触发器」（<c>updateTriggers</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_updatetriggers.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbUpdateTriggersRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbUpdateTriggersResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchupdatetriggers</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>批量面部分失败</b>：应答带逐元素失败/成功列表 ⇒ 必须逐元素判定结果，不得只看顶层 <c>errcode</c>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchupdatetriggers")]
    Task<OpenPlatformComponentTcbUpdateTriggersResponse> UpdateTriggersAsync(
        [Body] OpenPlatformComponentTcbUpdateTriggersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「更新云函数代码」（<c>uploadCloudFunctionCode</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_uploadcloudfunctioncode.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbUploadCloudFunctionCodeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbUploadCloudFunctionCodeResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchuploadscfcode</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>批量面部分失败</b>：应答带逐元素失败/成功列表 ⇒ 必须逐元素判定结果，不得只看顶层 <c>errcode</c>。</para>
    /// <para>官方原文：「zipfile打包时，请直接选中所要打包的函数代码文件直接打包，请不要放入文件夹中对文件夹进行打包。」</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchuploadscfcode")]
    Task<OpenPlatformComponentTcbUploadCloudFunctionCodeResponse> UpdateFunctionCodeAsync(
        [Body] OpenPlatformComponentTcbUploadCloudFunctionCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「更新云函数配置」（<c>uploadCloudFunctionConfig</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_uploadcloudfunctionconfig.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbUploadCloudFunctionConfigRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/updatescfconfig</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>敏感面</b>：<c>environment_variables</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/updatescfconfig")]
    Task<OpenPlatformResponse> UpdateFunctionConfigAsync(
        [Body] OpenPlatformComponentTcbUploadCloudFunctionConfigRequest request,
        CancellationToken cancellationToken = default);

}
