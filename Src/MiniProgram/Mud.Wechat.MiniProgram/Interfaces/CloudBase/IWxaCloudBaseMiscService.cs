// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.CloudBase;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「云开发（TCB）」域 SDK（<b>11 端点</b>：消息推送配置读写 2 + 短信任务与记录 2 + 扩展上传信息 1 + 数据统计与上报 2 + cloudID 开放数据与实时语音签名 2 + 微信支付商户号授权与列表 2）。
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
/// <para><b>落位</b>：<c>Interfaces/CloudBase/IWxaCloudBaseMiscService.cs</c>；DTO 命名空间
/// <c>Mud.Wechat.MiniProgram.DataModels.CloudBase</c>（子目录 Env/Database/Storage/Scf/Misc 仅作组织）；
/// <c>MiniProgramModule.CloudBase</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CloudBase", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaCloudBaseMiscService
{
    // ==================== 其它能力（11 端点）====================

    /// <summary>
    /// 「云开发上报接口」（<c>cloudbaseReportAPI</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_cloudbasereportapi.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseCloudbaseReportAPIRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/cloudbasereport</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。云调用方法 <c>cloudbase.report</c>。</para>
    /// <para>本接口为<b>上报</b>通道（写侧），应答仅 <c>errcode</c>/<c>errmsg</c>；重复上报由平台侧聚合，宿主不必自建去重。</para>
    /// </remarks>
    [Post("/tcb/cloudbasereport")]
    Task<WxaResponse> CloudbaseReportAPIAsync(
        [Body] WxaCloudBaseCloudbaseReportAPIRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「创建发短信任务」（<c>createSendSmsTask</c>）：创建发短信任务。发送的短信支持打开云开发静态网站 H5，进而在 H5 里可以打开小程序。详情可参考静态网站 H5 跳小程序。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_createsendsmstask.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseCreateSendSmsTaskRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseCreateSendSmsTaskResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/createsendsmstask</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。云调用方法 <c>cloudbase.createSendSmsTask</c>。</para>
    /// <para>短信格式官方固定为「【小程序名称】${content}，点击 云开发静态网站 URL 打开小程序名称小程序，退订回T」⇒ 正文最长 30 字，跳转地址须为静态托管路径。</para>
    /// <para>完整链路：先 <see cref="IWxaCloudBaseMiscService.DescribeExtensionUploadInfoAsync"/> 取 <c>upload_url</c>（<c>FileType</c> 填 <c>SMS</c>）→ <c>PUT</c> 上传 CSV → 本接口拿 <c>query_id</c> → <see cref="IWxaCloudBaseMiscService.DescribeSmsRecordsAsync"/> 查执行结果。</para>
    /// <para>官方英文接口名为 <c>createSendSmsTask</c>；<b>被官方下线的裸发送面（<c>/tcb/sendsms</c>、<c>/tcb/sendsmsv2</c>）不在本 SDK 面内</b>（守卫 CBZ 零声明锁定）。</para>
    /// </remarks>
    [Post("/tcb/createsendsmstask")]
    Task<WxaCloudBaseCreateSendSmsTaskResponse> CreateSendSmsTaskAsync(
        [Body] WxaCloudBaseCreateSendSmsTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「描述扩展上传文件信息」（<c>describeExtensionUploadInfo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_describeextensionuploadinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseDescribeExtensionUploadInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseDescribeExtensionUploadInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/describeextensionuploadinfo</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。云调用方法 <c>cloudbase.describeExtensionUploadInfo</c>。</para>
    /// <para>返回的是<b>直传链接</b>（<c>upload_url</c>）与素材归属参数，随后须用 <c>PUT</c> 上传文件本体；本接口只登记上传信息、不落文件。</para>
    /// </remarks>
    [Post("/tcb/describeextensionuploadinfo")]
    Task<WxaCloudBaseDescribeExtensionUploadInfoResponse> DescribeExtensionUploadInfoAsync(
        [Body] WxaCloudBaseDescribeExtensionUploadInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询短信记录」（<c>describeSmsRecords</c>）：查询 2 个月内的短信记录。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_describesmsrecords.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseDescribeSmsRecordsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseDescribeSmsRecordsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/describesmsrecords</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。云调用方法 <c>cloudbase.describeSmsRecords</c>。</para>
    /// <para>查询侧只读；<c>query_id</c> 来自 <see cref="IWxaCloudBaseMiscService.CreateSendSmsTaskAsync"/>。</para>
    /// </remarks>
    [Post("/tcb/describesmsrecords")]
    Task<WxaCloudBaseDescribeSmsRecordsResponse> DescribeSmsRecordsAsync(
        [Body] WxaCloudBaseDescribeSmsRecordsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取消息推送配置」（<c>getCallBackConfig</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/msg-push/api_getcallbackconfig.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetCallBackConfigResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/getcallbackconfig</c>；<b>无请求体</b>，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>无请求体</b>。应答 <c>config</c> 含云函数/云托管推送配置（可含环境变量），<b>环境变量值可能携带密钥 ⇒ 不得进日志</b>。</para>
    /// </remarks>
    [Post("/tcb/getcallbackconfig")]
    Task<WxaCloudBaseGetCallBackConfigResponse> GetCallBackConfigAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取云开发数据」（<c>getCloudBaseStatistics</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_getcloudbasestatistics.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetCloudBaseStatisticsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetCloudBaseStatisticsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/getstatistics</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。云调用方法 <c>cloudbase.getStatistics</c>。</para>
    /// <para><c>data_column</c>/<c>data_value</c> 为「列定义 + 值袋」形态，值列结构随 <c>metric</c> 变化 ⇒ 以 <c>List&lt;JsonElement&gt;?</c> 承载，不做类型推断。</para>
    /// </remarks>
    [Post("/tcb/getstatistics")]
    Task<WxaCloudBaseGetCloudBaseStatisticsResponse> GetCloudBaseStatisticsAsync(
        [Body] WxaCloudBaseGetCloudBaseStatisticsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「上传消息推送配置」（<c>setCallBackConfig</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/msg-push/api_setcallbackconfig.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseSetCallBackConfigRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/setcallbackconfig</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>上传即覆盖</b>：官方对未填子配置给出「不更新」语义，但同一子配置内部为整体替换 ⇒ 改前先 <see cref="IWxaCloudBaseMiscService.GetCallBackConfigAsync"/> 取回全量再改。</para>
    /// <para>请求体 <c>config</c> 的环境变量可能含密钥 ⇒ <b>不得进日志、遥测或异常消息</b>。应答仅 <c>errcode</c>/<c>errmsg</c>。</para>
    /// </remarks>
    [Post("/tcb/setcallbackconfig")]
    Task<WxaResponse> SetCallBackConfigAsync(
        [Body] WxaCloudBaseSetCallBackConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取授权绑定的商户号列表」（<c>getWechatPayList</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/wechatpay/api_getwechatpaylist.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetWechatPayListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/wxpaylist</c>；<b>无请求体</b>，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 access_token、authorizer_access_token」，权限集 id <b>49、102</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>权限集官方标注 <b>49、102</b>（102 为云开发微信支付）；返回该小程序已授权绑定的商户号列表，只读。</para>
    /// </remarks>
    [Post("/tcb/wxpaylist")]
    Task<WxaCloudBaseGetWechatPayListResponse> GetWechatPayListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「申请商户号授权」（<c>getWechatPayAuth</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/wechatpay/api_getwechatpayauth.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetWechatPayAuthRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/wxpayopenauth</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 access_token、authorizer_access_token」，权限集 id <b>49、102</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>权限集官方标注 <b>49、102</b>。<b>本接口只是发起授权申请</b>：官方原文要求商户号超管在【微信支付商家助手】小程序确认授权、并前往商户平台完成 jsapi 与 api 退款权限授权 ⇒ 调用成功≠授权生效，须以 <see cref="IWxaCloudBaseMiscService.GetWechatPayListAsync"/> 复核。</para>
    /// <para>应答仅 <c>errcode</c>/<c>errmsg</c>。</para>
    /// </remarks>
    [Post("/tcb/wxpayopenauth")]
    Task<WxaResponse> GetWechatPayAuthAsync(
        [Body] WxaCloudBaseGetWechatPayAuthRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取cloudID对应的数据」（<c>getOpenData</c>）：换取 cloudID 对应的开放数据。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_getopendata.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetOpenDataRequest"/>。</param>
    /// <param name="openid">官方 Query 参数 <c>openid</c>：换取敏感数据时必传，普通 cloudID 可省略。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetOpenDataResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/getopendata</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 access_token、authorizer_access_token」，权限集 id <b>18、49</b>，支持第三方平台代商家调用。云调用方法 <c>cloudbase.getOpenData</c>。</para>
    /// <para><b>敏感</b>：应答 <c>json</c> 为 cloudID 对应的<b>用户开放数据明文</b> ⇒ 不得进日志、遥测与异常消息。</para>
    /// <para>官方原文「<c>openid</c> 为敏感信息换取时必传」⇒ 以可选 Query 参数暴露（<c>敏感数据需传</c>）。</para>
    /// <para>权限集官方标注 <b>18、49</b>（本接口同时属小程序基础与云开发管理面）。</para>
    /// </remarks>
    [Post("/wxa/getopendata")]
    Task<WxaCloudBaseGetOpenDataResponse> GetOpenDataAsync(
        [Body] WxaCloudBaseGetOpenDataRequest request,
        [Query("openid")] string? openid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取实时语音签名」（<c>getCloudBaseVoIPSign</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_getcloudbasevoipsign.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetCloudBaseVoIPSignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetCloudBaseVoIPSignResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/getvoipsign</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 access_token、authorizer_access_token」，权限集 id <b>18</b>，支持第三方平台代商家调用。云调用方法 <c>cloudbase.getVoIPSign</c>。</para>
    /// <para>应答 <c>signature</c> 是对 <c>group_id</c>/<c>timestamp</c>/<c>nonce</c> 的签名，<b>客户端侧凭它接入实时语音</b>；<c>timestamp</c> 须为 UNIX 秒且与 <c>nonce</c>（&lt;128 字符）配套。</para>
    /// <para>权限集官方标注 <b>18</b>（非 49）⇒ 本端点属云开发之外的实时语音面，路由族 <c>/wxa/*</c> 由小程序令牌独占（守卫 CBZ/CBT 锁定跨线不交叉）。</para>
    /// </remarks>
    [Post("/wxa/getvoipsign")]
    Task<WxaCloudBaseGetCloudBaseVoIPSignResponse> GetCloudBaseVoIPSignAsync(
        [Body] WxaCloudBaseGetCloudBaseVoIPSignRequest request,
        CancellationToken cancellationToken = default);

}
