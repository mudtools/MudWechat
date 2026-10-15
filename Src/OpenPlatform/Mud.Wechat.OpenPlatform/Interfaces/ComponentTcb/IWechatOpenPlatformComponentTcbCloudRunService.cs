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
/// 微信开放平台「云托管服务与版本管理」域 SDK（<b>19 端点</b>：服务创建/删除/开通、版本创建/删除/发布/滚动更新/配置、流量分配、列表与详情查询（含 legacy 2.0 批量面））。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 <b>50</b> 条路由恒为 <c>/componenttcb/*</c> 前缀、恒为 <b>POST</b>、恒以 Query <c>access_token</c> 承载 <c>component_access_token</c>（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 <b>19</b> 条。</para>
/// <para>本组是全域<b>文档形态最混杂</b>的一组：19 条路由中 <b>13 条只有 legacy 2.0 页面</b>（<c>Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）、<b>1 条</b>为 <c>OpenApiDoc</c> 形态（<c>createCloudbaseMicrservice</c>）、<b>5 条</b>为现代 <c>cloudrun-batch</c> / <c>cloudbase-batch</c> 页面；全域其余 <b>31</b> 条均为现代页面形态。三种形态的<b>同名字段类型并不一致</b> ⇒ 本域按「一页一套 DTO」生成，跨页不共用。</para>
/// <para><b>官方类型分叉实例（照抄不修正）</b>：<c>cpu</c> 在现代创建页为 <c>number</c>、在 legacy <c>createcsversion</c> 与 <c>getcsversiondetail</c> 页为 <c>string</c>；<c>mem</c>/<c>policy_threshold</c> 同类分叉 ⇒ 宿主迁移文档面时不得复用同一 DTO。</para>
/// <para>令牌与开放面同 <see cref="IWechatOpenPlatformComponentTcbEnvService"/>；legacy 页无「第三方调用」小节，其令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」是同一结论。全域无权限集 id、无端点级频率数字。</para>
/// <para><b>凭据密集</b>：版本创建/滚动更新面含 <c>env_params</c>（环境变量）、<c>image_secret_info</c>（<c>username</c>/<c>password</c>/<c>email</c>）、<c>image_pull_secret</c>/<c>image_pulls_secret</c>、<c>es_info</c>（<c>secret_name</c>/<c>password</c>）、<c>nfs_volumes.secret_name</c>、<c>mount_wx_token</c>、<c>image_url</c> ⇒ <b>全部禁入日志、遥测与异常消息</b>。</para>
/// <para><b>流量与版本变更＝线上变更</b>：<c>releasecloudbaserunversion</c>/<c>rollupdatecloudbaserunserverversion</c>/<c>setcspropotion</c>/<c>batchsetcspropotion</c> 直接改变生产流量分配，批量面带 <c>fail_list</c>/<c>succ_list</c> ⇒ 须逐元素判定，不得只看 <c>errcode</c>。</para>
/// <para>「开通」类端点（<c>opencontainerservice</c>、<c>establishcloudbaserunserver</c>、<c>createcloudbaserunresauth</c>）为<b>资源与计费动作</b>，官方应答仅 <c>errcode</c>/<c>errmsg</c> ⇒ 调用前须确认宿主侧的授权与计费归属。</para>
/// <para><b>落位</b>：<c>Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbCloudRunService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.ComponentTcb</c>
/// （子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；<c>OpenPlatformModule.ComponentTcb</c>，注册组 <c>ComponentTcb</c>（六支接口共用一次注册）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ComponentTcb", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformComponentTcbCloudRunService
{
    // ==================== 云托管服务与版本管理（19 端点）====================

    /// <summary>
    /// 「批量新建云托管版本」（官方 legacy 页 URL 末段 <c>batchcreatecsversion</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/batchcreatecsversion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchcreatecsversion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para><b>敏感面</b>：<c>env_params</c>/<c>image_secret_info</c>/<c>image_pulls_secret</c>/<c>image_url</c>/<c>username</c>/<c>password</c>/<c>email</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchcreatecsversion")]
    Task<OpenPlatformResponse> BatchCreateVersionsAsync(
        [Body] OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「批量删除云托管版本」（官方 legacy 页 URL 末段 <c>batchDelCSVersion</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/batchDelCSVersion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbBatchDeleteContainerServiceVersionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbBatchDeleteContainerServiceVersionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchdelcsversion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para><b>批量面部分失败</b>：应答带逐元素失败/成功列表 ⇒ 必须逐元素判定结果，不得只看顶层 <c>errcode</c>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchdelcsversion")]
    Task<OpenPlatformComponentTcbBatchDeleteContainerServiceVersionResponse> BatchDeleteVersionsAsync(
        [Body] OpenPlatformComponentTcbBatchDeleteContainerServiceVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「批量分配流量比例」（官方 legacy 页 URL 末段 <c>batchSetCSPropotion</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/batchSetCSPropotion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbBatchSetContainerServicePropotionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbBatchSetContainerServicePropotionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/batchsetcspropotion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para><b>批量面部分失败</b>：应答带逐元素失败/成功列表 ⇒ 必须逐元素判定结果，不得只看顶层 <c>errcode</c>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/batchsetcspropotion")]
    Task<OpenPlatformComponentTcbBatchSetContainerServicePropotionResponse> BatchSetPropotionsAsync(
        [Body] OpenPlatformComponentTcbBatchSetContainerServicePropotionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「开通微服务」（<c>createCloudbaseMicrservice</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/cloudrun-batch/service-mgnt/createCloudbaseMicrservice.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbCreateCloudbaseMicroserviceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/createcloudbaserunresauth</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，该参数为 URL 参数，非 Body 参数。使用component_access_token」。</para>
    /// <para>本页属 <c>OpenApiDoc/cloudrun-batch/service-mgnt</c> 形态：官方<b>未给</b>「云调用 / 第三方调用 / 适用范围」小节，令牌行原文为「接口调用凭证，该参数为 URL 参数，非 Body 参数。使用 <c>component_access_token</c>」⇒ 开放面按令牌行取值。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/createcloudbaserunresauth")]
    Task<OpenPlatformResponse> OpenMicroserviceAsync(
        [Body] OpenPlatformComponentTcbCreateCloudbaseMicroserviceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「创建服务版本」（<c>createCloudbaseServiceVersion</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_createcloudbaseserviceversion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbCreateCloudbaseRunServerVersionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/createcloudbaserunserverversion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>敏感面</b>：<c>env_params</c>/<c>image_secret_info</c>/<c>image_pull_secret</c>/<c>secret_name</c>/<c>password</c>/<c>image_url</c>/<c>email</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/createcloudbaserunserverversion")]
    Task<OpenPlatformComponentTcbCreateCloudbaseRunServerVersionResponse> CreateServerVersionAsync(
        [Body] OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「新建云托管服务」（官方 legacy 页 URL 末段 <c>createcontainerservice</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/createcontainerservice.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbCreateContainerServiceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/createcontainerservice</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para>与 <see cref="IWechatOpenPlatformComponentTcbCloudRunService.CreateCloudRunServerAsync"/>（现代面 <c>createCloudbaseService</c>）为官方<b>并存的两条建服务路由</b>，本页属 legacy 2.0 批量面 ⇒ 参数表与应答字段与 Modern 面不同（本域 DTO 各自照抄官方，不复用）。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/createcontainerservice")]
    Task<OpenPlatformResponse> CreateServiceAsync(
        [Body] OpenPlatformComponentTcbCreateContainerServiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「新建云托管服务版本」（官方 legacy 页 URL 末段 <c>createcsversion</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/createcsversion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbCreateContainerServiceVersionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbCreateContainerServiceVersionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/createcsversion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para><b>敏感面</b>：<c>env_params</c>/<c>image_secret_info</c>/<c>image_pulls_secret</c>/<c>mount_wx_token</c>/<c>image_url</c>/<c>username</c>/<c>password</c>/<c>email</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>本页为 legacy 2.0 批量面，与 <see cref="IWechatOpenPlatformComponentTcbCloudRunService.CreateServerVersionAsync"/>（现代面 <c>createCloudbaseServiceVersion</c>）并存。官方<b>同名字段跨页类型不同</b>：本页 <c>cpu</c> 为 <c>string</c>，现代页为 <c>number</c> ⇒ 照官方原文各自成表，不做统一修正。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/createcsversion")]
    Task<OpenPlatformComponentTcbCreateContainerServiceVersionResponse> CreateVersionAsync(
        [Body] OpenPlatformComponentTcbCreateContainerServiceVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除云托管服务」（官方 legacy 页 URL 末段 <c>delcontainerservice</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/delcontainerservice.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbDeleteContainerServiceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/delcontainerservice</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/delcontainerservice")]
    Task<OpenPlatformResponse> DeleteServiceAsync(
        [Body] OpenPlatformComponentTcbDeleteContainerServiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除云托管版本」（官方 legacy 页 URL 末段 <c>delcsversion</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/delcsversion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbDeleteContainerServiceVersionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/delcsversion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/delcsversion")]
    Task<OpenPlatformResponse> DeleteVersionAsync(
        [Body] OpenPlatformComponentTcbDeleteContainerServiceVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除服务版本」（<c>deleteCloudbaseServiceVersion</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_deletecloudbaseserviceversion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/deletecloudbaserunserverversion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/deletecloudbaserunserverversion")]
    Task<OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionResponse> DeleteServerVersionAsync(
        [Body] OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「创建云托管服务」（<c>createCloudbaseService</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_createcloudbaseservice.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbCreateCloudbaseRunServerRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/establishcloudbaserunserver</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>敏感面</b>：<c>secret_name</c>/<c>password</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方同时保留<b>两条建服务路由</b>：现代 <c>cloudbase-batch/cloudrun</c> 面的 <c>createCloudbaseService</c>（本方法）与 legacy 2.0 批量面的 <c>createcontainerservice</c>（见 <see cref="IWechatOpenPlatformComponentTcbCloudRunService.CreateServiceAsync"/>）。两页均在官方目录内且请求面不同 ⇒ 本域照官方<b>双声明</b>，不做「顺手合并」；新建服务选哪条取决于宿主对接的文档面。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/establishcloudbaserunserver")]
    Task<OpenPlatformResponse> CreateCloudRunServerAsync(
        [Body] OpenPlatformComponentTcbCreateCloudbaseRunServerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查看云托管服务列表」（官方 legacy 页 URL 末段 <c>getcslist</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/getcslist.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetContainerServiceListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetContainerServiceListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/getcslist</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/getcslist")]
    Task<OpenPlatformComponentTcbGetContainerServiceListResponse> GetServiceListAsync(
        [Body] OpenPlatformComponentTcbGetContainerServiceListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查看云托管版本」（官方 legacy 页 URL 末段 <c>getcsversiondetail</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/getcsversiondetail.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetContainerServiceVersionDetailRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/getcsversiondetail</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para><b>敏感面</b>：<c>env_params</c>/<c>image_url</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>本页为 legacy 2.0 批量面，应答 <c>cpu</c> 为 <c>string</c>（现代面 <c>getCloudbaseServiceVersionDetail</c> 侧为 <c>number</c>）⇒ 类型照官方原文，宿主反序列化时不得共用 DTO。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/getcsversiondetail")]
    Task<OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse> GetVersionDetailAsync(
        [Body] OpenPlatformComponentTcbGetContainerServiceVersionDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查看云托管版本列表」（官方 legacy 页 URL 末段 <c>getcsversionlist</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/getcsversionlist.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetContainerServiceVersionListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetContainerServiceVersionListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/getcsversionlist</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/getcsversionlist")]
    Task<OpenPlatformComponentTcbGetContainerServiceVersionListResponse> GetVersionListAsync(
        [Body] OpenPlatformComponentTcbGetContainerServiceVersionListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「开通云托管」（官方 legacy 页 URL 末段 <c>opencontainerservice</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/opencontainerservice.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbOpenContainerServiceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/opencontainerservice</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/opencontainerservice")]
    Task<OpenPlatformResponse> OpenContainerServiceAsync(
        [Body] OpenPlatformComponentTcbOpenContainerServiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「发布服务版本」（<c>releaseCloudbaseServiceVersion</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_releasecloudbaseserviceversion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbReleaseCloudbaseRunVersionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbReleaseCloudbaseRunVersionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/releasecloudbaserunversion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/releasecloudbaserunversion")]
    Task<OpenPlatformComponentTcbReleaseCloudbaseRunVersionResponse> ReleaseServerVersionAsync(
        [Body] OpenPlatformComponentTcbReleaseCloudbaseRunVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「滚动更新服务版本」（<c>updateCloudbaseServiceVersion</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_updatecloudbaseserviceversion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/rollupdatecloudbaserunserverversion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「ACCESS_TOKEN」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>敏感面</b>：<c>env_params</c>/<c>image_url</c>/<c>secret_name</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para><b>滚动更新＝线上流量变更</b>：应答带回 <c>jobId</c> 类任务标识，官方 <c>cpu</c>/<c>mem</c>/<c>policy_threshold</c> 在本页为 <c>string</c>（创建页为 <c>number</c>）⇒ 类型照官方原文；<c>env_params</c> 与镜像仓库凭据禁入日志。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/rollupdatecloudbaserunserverversion")]
    Task<OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionResponse> RollUpdateServerVersionAsync(
        [Body] OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「分配流量比例」（官方 legacy 页 URL 末段 <c>setcspropotion</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/setcspropotion.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbSetContainerServicePropotionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/setcspropotion</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/setcspropotion")]
    Task<OpenPlatformResponse> SetPropotionAsync(
        [Body] OpenPlatformComponentTcbSetContainerServicePropotionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「配置云托管版本」（官方 legacy 页 URL 末段 <c>setcsversionconfig</c>，页内无「接口英文名」小节）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/setcsversionconfig.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbSetContainerServiceVersionConfigRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/setcsversionconfig</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「第三方接口调用令牌component_access_token」。</para>
    /// <para><b>文档形态</b>：本页为 <b>legacy 2.0 文档</b>形态（<c>doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*</c>）：官方页<b>无</b>「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 <c>component_access_token</c>」。</para>
    /// <para><b>敏感面</b>：<c>env_params</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/setcsversionconfig")]
    Task<OpenPlatformResponse> SetVersionConfigAsync(
        [Body] OpenPlatformComponentTcbSetContainerServiceVersionConfigRequest request,
        CancellationToken cancellationToken = default);

}
