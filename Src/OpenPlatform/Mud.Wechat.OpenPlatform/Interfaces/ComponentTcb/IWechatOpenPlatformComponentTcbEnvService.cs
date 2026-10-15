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
/// 微信开放平台「云开发与云托管环境管理」域 SDK（<b>7 端点</b>：创建云开发环境 / 获取环境信息 / 查询共享信息 / 环境共享 / 解除共享 / 创建云托管环境 / 获取云托管环境信息）。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 <b>50</b> 条路由恒为 <c>/componenttcb/*</c> 前缀、恒为 <b>POST</b>、恒以 Query <c>access_token</c> 承载 <c>component_access_token</c>（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 <b>7</b> 条。</para>
/// <para>本组是<b>批量代云开发</b>面：官方「第三方调用」小节原文「本接口仅支持第三方平台使用 <c>component_access_token</c> <b>自己调用</b>」⇒ 与小程序线的 <c>/tcb/*</c>（<c>access_token</c> / <c>authorizer_access_token</c>，权限集 49）<b>是两套路由、两套令牌</b>，本域不声明 <c>/tcb/*</c>，小程序域也不声明 <c>/componenttcb/*</c>（守卫 CBZ2 双向锁定不交叉）。</para>
/// <para>「适用范围」小节原文「本接口支持「<b>第三方平台</b>」账号类型代调用，权限集请参考「调用方式」部分。其他账号类型如无特殊说明，均不可调用」⇒ 公众号/小程序自有 <c>access_token</c> <b>不可</b>调用本组路由。</para>
/// <para>本组唯一给出权限集要求的是 <c>batchshareenv</c>：官方注意事项「在调用该接口之前，小程序需将权限集 id 为 <b>18</b> 的「开发与数据分析」权限集授权给服务商」；其余端点<b>未给权限集 id</b>（平台以自身凭据调用）⇒ 空缺是官方原文状态，不得凭记忆补 49。</para>
/// <para><b>环境共享是跨 appid 的数据面开放</b>：<c>batchshareenv</c> 应答 <c>msg_info_list[].url</c> 为确认链接，官方原文「服务商需将 msg_info_list 中的确认链接发送给小程序管理员，经小程序管理员确认允许绑定后，绑定正式生效」⇒ 调用成功 ≠ 生效，宿主必须完成管理员确认链路。</para>
/// <para>官方全域<b>未给端点级频率数字</b>（无「次/小时」「限流」等原文）⇒ 注释一律留档该空缺，不得凭第三方 SDK 注释或记忆补数。</para>
/// <para><b>落位</b>：<c>Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbEnvService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.ComponentTcb</c>
/// （子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；<c>OpenPlatformModule.ComponentTcb</c>，注册组 <c>ComponentTcb</c>（六支接口共用一次注册）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ComponentTcb", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformComponentTcbEnvService
{
    // ==================== 云开发与云托管环境管理（7 端点）====================

    /// <summary>
    /// 「创建云开发环境」（<c>createTcbEnv</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_createtcbenv.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbCreateTcbEnvRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbCreateTcbEnvResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/createenv</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方原文：「说明：请求成功返回后，必须先手动调用“getTcbEnvList获取环境信息接口”进行初始化操作，并确认新环境的 status 为NORMAL后，才可以对环境进行创建集合、插入数据等操作」</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/createenv")]
    Task<OpenPlatformComponentTcbCreateTcbEnvResponse> CreateEnvAsync(
        [Body] OpenPlatformComponentTcbCreateTcbEnvRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取云开发环境列表」（<c>getTcbEnvList</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_gettcbenvlist.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetTcbEnvListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/describeenvs</c>；<b>无请求体</b>（POST 空体即可），Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方 <c>createTcbEnv</c> 页「其他说明」原文把本接口列为新建环境的<b>强制后续动作</b>：「请求成功返回后，必须先手动调用 getTcbEnvList 获取环境信息接口进行初始化操作，并确认新环境的 status 为 NORMAL 后，才可以对环境进行创建集合、插入数据等操作」⇒ 创建后须轮询本接口取状态，不得直接落到数据面接口。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/describeenvs")]
    Task<OpenPlatformComponentTcbGetTcbEnvListResponse> GetEnvListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询环境共享信息」（<c>getShareCloudbaseEnv</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_getsharecloudbaseenv.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetShareCloudbaseEnvRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetShareCloudbaseEnvResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchgetenvid</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>批量面部分失败</b>：应答带逐元素失败/成功列表 ⇒ 必须逐元素判定结果，不得只看顶层 <c>errcode</c>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchgetenvid")]
    Task<OpenPlatformComponentTcbGetShareCloudbaseEnvResponse> GetSharedEnvInfoAsync(
        [Body] OpenPlatformComponentTcbGetShareCloudbaseEnvRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「环境共享」（<c>shareCloudbaseEnv</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_sharecloudbaseenv.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbShareCloudbaseEnvRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbShareCloudbaseEnvResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchshareenv</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；权限集 id <b>18</b>；本接口不支持云调用。</para>
    /// <para><b>官方枚举值</b>：<c>Body.source_type</c> = <c>0</c>（云开发环境）、<c>1</c>（云托管环境）。</para>
    /// <para><b>批量面部分失败</b>：应答带逐元素失败/成功列表 ⇒ 必须逐元素判定结果，不得只看顶层 <c>errcode</c>。</para>
    /// <para>官方原文：「在调用该接口之前，小程序需将权限集id为 18 的「开发与数据分析」权限集授权给服务商」</para>
    /// <para>官方原文：「服务商需将 msg_info_list 中的确认链接发送给小程序管理员，经小程序管理员确认允许绑定后，绑定正式生效。」</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchshareenv")]
    Task<OpenPlatformComponentTcbShareCloudbaseEnvResponse> ShareEnvAsync(
        [Body] OpenPlatformComponentTcbShareCloudbaseEnvRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「解除环境共享关联关系」（<c>unshareCloudbaseEnv</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_unsharecloudbaseenv.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbUnshareCloudbaseEnvRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbUnshareCloudbaseEnvResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchunshareenv</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>批量面部分失败</b>：应答带逐元素失败/成功列表 ⇒ 必须逐元素判定结果，不得只看顶层 <c>errcode</c>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchunshareenv")]
    Task<OpenPlatformComponentTcbUnshareCloudbaseEnvResponse> UnshareEnvAsync(
        [Body] OpenPlatformComponentTcbUnshareCloudbaseEnvRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「创建云托管环境」（<c>createCloudbaseEnv</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/cloudenv-mgnt/api_createcloudbaseenv.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbCreateCloudbaseRunEnvRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbCreateCloudbaseRunEnvResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/createcloudbaserunenv</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/createcloudbaserunenv")]
    Task<OpenPlatformComponentTcbCreateCloudbaseRunEnvResponse> CreateCloudRunEnvAsync(
        [Body] OpenPlatformComponentTcbCreateCloudbaseRunEnvRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取云托管环境信息」（<c>getWxCloudBaseRunEnvs</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/cloudenv-mgnt/api_getwxcloudbaserunenvs.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/describecloudbaserunenvs</c>；<b>无请求体</b>（POST 空体即可），Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/describecloudbaserunenvs")]
    Task<OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponse> GetCloudRunEnvListAsync(
        CancellationToken cancellationToken = default);

}
