// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.CloudBase;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「云开发（TCB）」域 SDK（<b>9 端点</b>：函数创建/列表/触发/延时触发 4 + 配置读取与覆盖上传 2 + 代码保护密钥/下载/上传凭证 3）。
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
/// <para><b>落位</b>：<c>Interfaces/CloudBase/IWxaCloudBaseScfService.cs</c>；DTO 命名空间
/// <c>Mud.Wechat.MiniProgram.DataModels.CloudBase</c>（子目录 Env/Database/Storage/Scf/Misc 仅作组织）；
/// <c>MiniProgramModule.CloudBase</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CloudBase", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaCloudBaseScfService
{
    // ==================== 云函数（9 端点）====================

    /// <summary>
    /// 「延时调用云函数」（<c>addDelayedFunctionTask</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/functions/api_adddelayedfunctiontask.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseAddDelayedFunctionTaskRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/adddelayedfunctiontask</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「可使用 access_token、authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。云调用方法 <c>cloudbase.addDelayedFunctionTask</c>。</para>
    /// <para><c>delay_time</c> 为延时秒数，到期后由平台触发目标云函数；触发时刻云函数的执行结果不经本接口回传。</para>
    /// </remarks>
    [Post("/tcb/adddelayedfunctiontask")]
    Task<WxaResponse> AddDelayedFunctionTaskAsync(
        [Body] WxaCloudBaseAddDelayedFunctionTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「创建云函数」（<c>createFunction</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/scf-management/api_createfunction.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseCreateFunctionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/createfunction</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/createfunction")]
    Task<WxaResponse> CreateFunctionAsync(
        [Body] WxaCloudBaseCreateFunctionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取云函数下载地址」（<c>getFuntionLink</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/scf-management/api_getfuntionlink.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetFuntionLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetFuntionLinkResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/downloadfunction</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>敏感</b>：应答 <c>url</c> 为带签名的代码下载地址（含代码保护场景），不得进日志、遥测或异常消息。</para>
    /// <para>官方原文「只允许下载本第三方平台上传的代码；若该小程序曾授权给其他第三方平台且其提交过有代码保护的代码，本接口会失败，需由本第三方平台先覆盖更新一次代码」。</para>
    /// </remarks>
    [Post("/tcb/downloadfunction")]
    Task<WxaCloudBaseGetFuntionLinkResponse> GetFuntionLinkAsync(
        [Body] WxaCloudBaseGetFuntionLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取代码保护密钥」（<c>getCodeSecret</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/scf-management/api_getcodesecret.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetCodeSecretResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/getcodesecret</c>；<b>无请求体</b>，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>敏感</b>：应答 <c>codesecret</c> 为代码保护密钥，<b>禁入日志、遥测、异常消息与明文配置</b>。</para>
    /// <para>官方原文「密钥是<b>小程序维度</b>的，不同环境、不同云函数密钥相同；同一小程序只需获取一次，取消授权后重新授权也不会变更；下载云函数代码时平台静默校验密钥，第三方平台不得自行提供」。</para>
    /// </remarks>
    [Post("/tcb/getcodesecret")]
    Task<WxaCloudBaseGetCodeSecretResponse> GetCodeSecretAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取云函数配置」（<c>getFuntionConfig</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/scf-management/api_getfuntionconfig.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetFuntionConfigRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetFuntionConfigResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/getfuncconfig</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方 <c>type</c> 合法值：<c>1</c> 云调用配置 / <c>2</c> 定时触发器配置 / <c>3</c> 环境配置（<c>timeout</c> 1–300 秒、默认 3 秒 + <c>environment</c> 环境变量数组）。</para>
    /// <para>环境变量值可能含密钥 ⇒ <b>不得进日志</b>。</para>
    /// </remarks>
    [Post("/tcb/getfuncconfig")]
    Task<WxaCloudBaseGetFuntionConfigResponse> GetFuntionConfigAsync(
        [Body] WxaCloudBaseGetFuntionConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取上传凭证」（<c>getYploadSignature</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/scf-management/api_getyploadsignature.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetYploadSignatureRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetYploadSignatureResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/getuploadsignature</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>敏感</b>：应答 <c>headers</c> 为上传用的<b>已签名头集合</b>（内含 <c>authorization</c> 与 <c>token</c>），<b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方英文接口名拼写为 <c>getYploadSignature</c>（路由 <c>/tcb/getuploadsignature</c>）⇒ 本 SDK 照抄官方拼写，不做「顺手修正」。</para>
    /// </remarks>
    [Post("/tcb/getuploadsignature")]
    Task<WxaCloudBaseGetYploadSignatureResponse> GetYploadSignatureAsync(
        [Body] WxaCloudBaseGetYploadSignatureRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「触发云函数」（<c>invokeCloudFunction</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_invokecloudfunction.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseInvokeCloudFunctionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseInvokeCloudFunctionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/invokecloudfunction</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>令牌三面</b>：官方本页「注意事项」原文「服务商模式-批量代云开发 ⇒ <c>component_access_token</c>；服务商模式-普通代云开发 ⇒ <c>authorizer_access_token</c>；小程序普通调用 ⇒ <c>access_token</c>」⇒ <b>同一条 <c>/tcb/invokecloudfunction</c> 路由由三面令牌复用</b>。官方全文<b>未给出</b> <c>/componenttcb/invokecloudfunction</c> 这条路由（批量面换的是<b>令牌值</b>而非路由前缀）⇒ 开放平台线不重复声明该端点，<c>/componenttcb/*</c> 路由全集由守卫 CBT 钉死。</para>
    /// <para>官方原文：「使用本 API 触发云函数，在云函数中<b>无法获取 OpenID</b> 等用户相关信息，无法使用涉及用户登录态的其他 API」「POST BODY 会传递给云函数作为输入参数」「由 HTTP API 触发云函数的<b>超时时间为 5s</b>」；同批原文亦明写「由 HTTP API <b>触发的云函数可以使用云调用</b>」（指被触发的函数侧，与本接口自身不支持云调用是两面）。</para>
    /// <para>应答 <c>resp_data</c> 为云函数返回的原始 JSON 字符串（不解析），<b>可能含用户数据 ⇒ 由宿主决定落盘与脱敏</b>。</para>
    /// </remarks>
    [Post("/tcb/invokecloudfunction")]
    Task<WxaCloudBaseInvokeCloudFunctionResponse> InvokeCloudFunctionAsync(
        [Body] WxaCloudBaseInvokeCloudFunctionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取云函数列表」（<c>getFuntionList</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/scf-management/api_getfuntionlist.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetFuntionListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetFuntionListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/listfunctions</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/listfunctions")]
    Task<WxaCloudBaseGetFuntionListResponse> GetFuntionListAsync(
        [Body] WxaCloudBaseGetFuntionListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「上传云函数配置」（<c>getUploadFuntionConfig</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/scf-management/api_getuploadfuntionconfig.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetUploadFuntionConfigRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/uploadfuncconfig</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>覆盖式上传</b>：<c>type</c> 决定被覆盖的配置面（1 云调用 / 2 定时触发器 / 3 环境配置），一次只覆盖一面；<c>config</c> 为对应面的 JSON 字符串。</para>
    /// <para>环境配置面 <c>timeout</c> 合法区间 1–300 秒、默认 3 秒；<c>environment</c> 为环境变量键值数组，值可能含密钥 ⇒ <b>不得进日志</b>。应答仅 <c>errcode</c>/<c>errmsg</c>。</para>
    /// </remarks>
    [Post("/tcb/uploadfuncconfig")]
    Task<WxaResponse> GetUploadFuntionConfigAsync(
        [Body] WxaCloudBaseGetUploadFuntionConfigRequest request,
        CancellationToken cancellationToken = default);

}
