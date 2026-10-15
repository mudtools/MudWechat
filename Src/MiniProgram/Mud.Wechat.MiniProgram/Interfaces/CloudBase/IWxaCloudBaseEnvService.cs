// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.CloudBase;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「云开发（TCB）」域 SDK（<b>8 端点</b>：开通云开发 1 + 云环境创建/查询/转换/共享 4 + 腾讯云凭证与微信令牌权限 2 + 手机号绑定查询 1）。
/// </summary>
/// <remarks>
/// <para><b>官方文档面</b>：本域路由的官方页面分布在两处文档树 —— 小程序自研面
/// <c>miniprogram/dev/server/API/cloudbase/{functions,database,storage,others}/*</c>，
/// 公众平台通用面（第三方/自研同页）<c>doc/oplatform/openApi/cloudbase-common/*</c> 与
/// <c>cloudbase-batch/*</c>；两处<b>路由同为 <c>/tcb/*</c></b>（2026-10-15 逐页核验）。
/// 逐端点官方链接见各方法注释。</para>
/// <para><b>令牌形态（与 MP-X2 / Vip 域同口径）</b>：全域使用<b>小程序 access_token</b>（Query 注入，
/// <c>MpTokenTypes.AccessToken</c>）。官方各页令牌说明给出「<c>access_token</c> / <c>authorizer_access_token</c>」
/// 或仅 <c>authorizer_access_token</c>（第三方代调用面）⇒ 本 SDK <b>不新增令牌类型</b>、也不声明
/// <c>component_access_token</c>；第三方代调用形态由宿主以小程序应用配置供给令牌。
/// <c>/componenttcb/*</c>（仅 <c>component_access_token</c> 的批量代云开发面）<b>属开放平台线</b>，
/// 本域不得声明（守卫 CBZ 锁定两线路由不交叉）。</para>
/// <para><b>权限集</b>：44 端点中 40 支官方标注权限集 id <b>49</b>（小程序云开发管理），
/// <c>/tcb/wxpaylist</c> 与 <c>/tcb/wxpayopenauth</c> 为 <b>49、102</b>，<c>/wxa/getopendata</c> 为 <b>18、49</b>，
/// <c>/wxa/getvoipsign</c> 为 <b>18</b>。全部端点<b>支持第三方平台代商家调用</b>。</para>
/// <para><b>端点级频率数字</b>：官方仅对 <c>/tcb/getqcloudtoken</c> 给出「<b>10 次每小时</b>」
///（超限提示可经 <c>clear_quota</c> 恢复额度）；其余 43 端点<b>未给任何数字</b> ⇒ 注释一律留档该空缺，
/// 不得凭第三方 SDK 注释或记忆补数。</para>
/// <para><b>敏感面</b>：<c>/tcb/getqcloudtoken</c>（<c>secretid</c>/<c>secretkey</c>/<c>token</c>）、
/// <c>/tcb/getcodesecret</c>（<c>codesecret</c>）、<c>/tcb/getuploadsignature</c>（签名 <c>headers</c>）、
/// <c>/tcb/uploadfile</c>（COS 直传 <c>authorization</c>/<c>token</c>）、
/// <c>/tcb/{set,get}callbackconfig</c> 与 <c>/tcb/{get,upload}funcconfig</c>（配置含环境变量）、
/// <c>/wxa/getopendata</c>（用户开放数据明文）⇒ 这些字段<b>禁入日志、遥测与异常消息</b>。</para>
/// <para><b>官方拼写照抄</b>：<c>getFuntionLink</c> / <c>getFuntionList</c> / <c>getFuntionConfig</c> /
/// <c>getUploadFuntionConfig</c> / <c>getYploadSignature</c> 为官方原文拼写（非笔误修正对象），
/// 方法名与英文接口名一律照抄。</para>
/// <para><b>落位</b>：<c>Interfaces/CloudBase/IWxaCloudBaseEnvService.cs</c>；DTO 命名空间
/// <c>Mud.Wechat.MiniProgram.DataModels.CloudBase</c>（子目录 Env/Database/Storage/Scf/Misc 仅作组织）；
/// <c>MiniProgramModule.CloudBase</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CloudBase", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaCloudBaseEnvService
{
    // ==================== 云环境与账号（8 端点）====================

    /// <summary>
    /// 「查询是否绑定手机号」（<c>checkMobileConfig</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/admin-management/api_checkmobileconfig.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseCheckMobileConfigRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseCheckMobileConfigResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/checkmobile</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>用于开通/续费链路上的前置校验；官方本页令牌面只给 <c>authorizer_access_token</c>（第三方侧文档），小程序自有 <c>access_token</c> 同一路由可用。</para>
    /// </remarks>
    [Post("/tcb/checkmobile")]
    Task<WxaCloudBaseCheckMobileConfigResponse> CheckMobileConfigAsync(
        [Body] WxaCloudBaseCheckMobileConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「开通云开发」（<c>createCloudUser</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/admin-management/api_createclouduser.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/createclouduser</c>；<b>无请求体</b>，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>无请求体</b>（POST 空体即可）。开通为<b>异步</b>流程：管理员需确认授权，官方原文「模板消息发送频率限制，24 小时内一个小程序的管理员只会收到一次授权消息」⇒ 轮询本接口催办无意义。</para>
    /// </remarks>
    [Post("/tcb/createclouduser")]
    Task<WxaResponse> CreateCloudUserAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「创建云环境」（<c>createEnv</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/env-management/api_createenv.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseCreateEnvRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseCreateEnvResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/createenvandresource</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。云调用方法 <c>tcb.createenvandresource</c>。</para>
    /// <para>官方原文「旧版创建云环境调整至当前接口，请开发者逐步进行接口切换」⇒ 旧版创建入口不再登记。</para>
    /// </remarks>
    [Post("/tcb/createenvandresource")]
    Task<WxaCloudBaseCreateEnvResponse> CreateEnvAsync(
        [Body] WxaCloudBaseCreateEnvRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取云环境」（<c>getEnvInfo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/env-management/api_getenvinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetEnvInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetEnvInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/getenvinfo</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/getenvinfo")]
    Task<WxaCloudBaseGetEnvInfoResponse> GetEnvInfoAsync(
        [Body] WxaCloudBaseGetEnvInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取腾讯云API调用凭证」（<c>getCloudToken</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/admin-management/api_getcloudtoken.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetCloudTokenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetCloudTokenResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/getqcloudtoken</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>敏感</b>：应答 <c>secretid</c>/<c>secretkey</c>/<c>token</c> 为腾讯云临时凭证，<b>禁入日志、遥测、异常消息</b>；<c>expired_time</c> 为过期时间戳，凭证最大有效期 7200 秒。</para>
    /// <para><b>官方端点级频率数字：10 次每小时</b>（全域唯一给出数字的端点），超限时返回「调用超过天级别频率限制，可调用 clear_quota 接口恢复调用额度」⇒ 宿主必须自建缓存复用，不得每次云调用都换凭证。</para>
    /// </remarks>
    [Post("/tcb/getqcloudtoken")]
    Task<WxaCloudBaseGetCloudTokenResponse> GetCloudTokenAsync(
        [Body] WxaCloudBaseGetCloudTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「转换云环境」（<c>changeTcbEnv</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_changetcbenv.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseChangeTcbEnvRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/modifyenv</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>环境级不可逆变更</b>（套餐/资源转换）。官方本页令牌面为 <c>component_access_token</c>（批量代云开发）或 <c>authorizer_access_token</c>；本 SDK 以小程序 <c>access_token</c> 声明，代调用面归开放平台线 <c>/componenttcb/modifyenv</c>。应答仅 <c>errcode</c>/<c>errmsg</c>。</para>
    /// </remarks>
    [Post("/tcb/modifyenv")]
    Task<WxaResponse> ChangeTcbEnvAsync(
        [Body] WxaCloudBaseChangeTcbEnvRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「环境共享」（<c>shareEnv</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/env-management/api_shareenv.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseShareEnvRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/shareenv</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>跨账号数据面变更</b>：把本环境共享给目标 appid 后，对方即可读写该环境 ⇒ 调用前须确认 <c>env_id</c>/<c>appid</c> 归属。官方对应取消共享接口在批量面（<c>/componenttcb/unshareenv</c>）。应答仅 <c>errcode</c>/<c>errmsg</c>。</para>
    /// </remarks>
    [Post("/tcb/shareenv")]
    Task<WxaResponse> ShareEnvAsync(
        [Body] WxaCloudBaseShareEnvRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「管理微信令牌权限」（<c>setCloudAccessToken</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_setcloudaccesstoken.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseSetCloudAccessTokenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseSetCloudAccessTokenResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/usecloudaccesstoken</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>开关型危险操作</b>：官方原文「此权限默认为开启，调用本接口关闭后该账号便无法使用 <c>cloudbase_access_token</c>」；且「按需配置允许使用微信令牌的开放接口，未配置的接口无法使用云调用」⇒ <c>action</c> 与接口白名单必须整面提交。</para>
    /// <para>官方本页令牌面为 <c>component_access_token</c>（批量）或 <c>authorizer_access_token</c>（普通代开发）。</para>
    /// </remarks>
    [Post("/tcb/usecloudaccesstoken")]
    Task<WxaCloudBaseSetCloudAccessTokenResponse> SetCloudAccessTokenAsync(
        [Body] WxaCloudBaseSetCloudAccessTokenRequest request,
        CancellationToken cancellationToken = default);

}
