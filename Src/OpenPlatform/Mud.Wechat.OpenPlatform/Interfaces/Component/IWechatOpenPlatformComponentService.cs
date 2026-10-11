// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.Component;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台（第三方平台）「平台管理」域 SDK
/// （配额清零、快速注册小程序、服务器/跳转域名管理、用户隐私保护指引、授权方信息与选项——
/// 即官方「第三方平台管理」分组下消费 <c>component_access_token</c> 的管理面端点；
/// 令牌链 4 端点（<c>api_component_token</c> / <c>api_create_preauthcode</c> / <c>api_query_auth</c> /
/// <c>api_authorizer_token</c>）由 <see cref="IComponentTokenProvider"/> / <see cref="IComponentAuthorizationService"/>
/// 内部实现承载，<b>不重复声明</b>（方案 §3 T2 裁定）；推票引导 <c>api_start_push_ticket</c> 免令牌，
/// 见 <see cref="IWechatOpenPlatformComponentTicketFreeService"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌语义（官方契约，已逐端点核验）</b>：本组端点的令牌<b>值</b>是平台自身
/// <c>component_access_token</c>，但官方 query 参数名为 <c>access_token</c>（与小程序侧
/// <c>component_access_token</c> 命名不同，二者均已核验）⇒ 声明为 Query 注入 <c>access_token</c>。
/// </para>
/// <para>
/// MUD005 已知接受风险：开放平台官方契约强制令牌走 Query 参数，无法改用 Header；
/// URL 遥测已由组件 <c>SensitiveUrlRedactor</c> 与 <see cref="WechatOpenPlatformException"/> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Component", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatOpenPlatformComponentService
{
    /// <summary>
    /// 清除 API 每日调用配额（v2，第三方平台形态）。
    /// </summary>
    /// <param name="request">清零请求（<c>appid</c> = 待清零账号；<c>component_appid</c> / <c>appsecret</c> 均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/openapi/clearComponentQuotaByAppSecret.html"/></para>
    /// <para>官方约束：与公众号线「AppSecret 应急清零」（免令牌）不同，本端点是第三方平台代清，
    /// 令牌可用时走本端点；每月清零机会与公众号线口径一致（单账号 10 次/月，两版合计）。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40013</c>（invalid appid）/ <c>48006</c>（清零次数达到上限）。</para>
    /// </remarks>
    [Post("/cgi-bin/component/clear_quota/v2")]
    Task<OpenPlatformResponse> ClearQuotaV2Async(
        [Body] OpenPlatformClearQuotaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 快速创建小程序（企业法定名称主体）。
    /// </summary>
    /// <param name="request">注册请求（企业名 / 企业代码 / 法人信息，均官方必填）。</param>
    /// <param name="action">官方 <c>action</c> query 参数，固定 <c>create</c>（SDK 级默认值，无需显式传入）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（注册结果经 <c>notify_third_fasteregister</c> 事件推送）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-ent/registerMiniprogram.html"/></para>
    /// <para>
    /// 官方约束：同一企业主体连续失败多次将被限制提交；法人微信号须为绑卡实名账号；
    /// 注册为<b>异步任务</b>——同步应答仅代表受理，结果查 <see cref="SearchFastRegisterWeappAsync"/> 或事件推送。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40013</c> / <c>85056</c>（主体受限）等。</para>
    /// </remarks>
    [Post("/cgi-bin/component/fastregisterweapp")]
    Task<OpenPlatformResponse> CreateFastRegisterWeappAsync(
        [Body] OpenPlatformFastRegisterCreateRequest request,
        [Query("action")] string action = "create",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询快速创建小程序任务状态。
    /// </summary>
    /// <param name="request">查询请求（企业名 / 法人信息，与创建时一致）。</param>
    /// <param name="action">官方 <c>action</c> query 参数，固定 <c>search</c>（SDK 级默认值，无需显式传入）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（任务状态语义经 errcode 表达，如 <c>85033</c> 未受理 / <c>85034</c> 处理中）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-ent/registerMiniprogram.html"/></para>
    /// <para>
    /// <b>同路由双方法</b>：与 <see cref="CreateFastRegisterWeappAsync"/> 共享
    /// <c>/cgi-bin/component/fastregisterweapp</c>，以 <c>action</c> query 分派（守卫 OP2 登记）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/component/fastregisterweapp")]
    Task<OpenPlatformResponse> SearchFastRegisterWeappAsync(
        [Body] OpenPlatformFastRegisterSearchRequest request,
        [Query("action")] string action = "search",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改第三方平台服务器域名。
    /// </summary>
    /// <param name="request">域名操作请求（<c>action</c>：add / delete / set / get；域名列表分号拼接）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生效与校验失败的域名（分号拼接字符串）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/domain-mgnt/modifyThirdpartyServerDomain.html"/></para>
    /// <para>官方约束：域名须 ICP 备案且通过官方校验；每月可修改 50 次（全平台维度）。</para>
    /// <para>官方错误码：<c>-1</c> / <c>85057</c>（次数达到上限）等。</para>
    /// </remarks>
    [Post("/cgi-bin/component/modify_wxa_server_domain")]
    Task<OpenPlatformModifyWxaServerDomainResponse> ModifyWxaServerDomainAsync(
        [Body] OpenPlatformModifyWxaServerDomainRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改第三方平台跳转（H5）域名。
    /// </summary>
    /// <param name="request">域名操作请求（<c>action</c>：add / delete / set / get；域名列表分号拼接）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生效与校验失败的域名（分号拼接字符串）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/domain-mgnt/modifyThirdpartyJumpDomain.html"/></para>
    /// <para>官方约束：同服务器域名（备案 + 校验 + 每月 50 次限额）。</para>
    /// </remarks>
    [Post("/cgi-bin/component/modify_wxa_jump_domain")]
    Task<OpenPlatformModifyWxaJumpDomainResponse> ModifyWxaJumpDomainAsync(
        [Body] OpenPlatformModifyWxaJumpDomainRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取第三方平台业务域名校验文件。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>校验文件名与文件内容（文本）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/domain-mgnt/getThirdpartyJumpDomainConfirmFile.html"/></para>
    /// <para>官方契约：POST、请求体为空；把返回的文件内容放到待校验域名可访问的位置后，
    /// 再调用 <see cref="ModifyWxaJumpDomainAsync"/> 提交域名。</para>
    /// </remarks>
    [Post("/cgi-bin/component/get_domain_confirmfile")]
    Task<OpenPlatformGetDomainConfirmFileResponse> GetDomainConfirmFileAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户隐私保护指引配置。
    /// </summary>
    /// <param name="request">查询请求（<c>privacy_ver</c> 可选，1 表示新版指引）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>指引配置（负责人 / 条目列表 / 用途描述映射）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-management/getPrivacySetting.html"/></para>
    /// <para>官方约束：本端点查询的是<b>第三方平台自身</b>的小程序隐私指引（平台级配置）。</para>
    /// </remarks>
    [Post("/cgi-bin/component/getprivacysetting")]
    Task<OpenPlatformGetPrivacySettingResponse> GetPrivacySettingAsync(
        [Body] OpenPlatformGetPrivacySettingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置用户隐私保护指引配置。
    /// </summary>
    /// <param name="request">指引配置（<c>owner_setting</c> 官方必填；补充文件先经 <see cref="UploadPrivacyExtFileAsync"/> 上传取得 media_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-management/setPrivacySetting.html"/></para>
    /// <para>官方约束：隐私条目用途描述不超过 40 字；发布后修改需重新审核。</para>
    /// </remarks>
    [Post("/cgi-bin/component/setprivacysetting")]
    Task<OpenPlatformResponse> SetPrivacySettingAsync(
        [Body] OpenPlatformSetPrivacySettingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传用户隐私保护指引补充文件（multipart 表单，文件字段名固定 <c>file</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（文件字段名必须为 <c>file</c>；由调用方构建
    /// <see cref="IFormContent"/> 实现，须标 <c>[MultipartForm]</c>；格式限 txt / doc / docx / pdf，≤2MB）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>补充文件临时素材 <c>media_id</c>（填入 owner_setting 的 <c>ext_file_media_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-management/uploadPrivacySetting.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/component/uploadprivacyextfile")]
    Task<OpenPlatformUploadPrivacyExtFileResponse> UploadPrivacyExtFileAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取授权方的帐号基本信息。
    /// </summary>
    /// <param name="request">查询请求（<c>authorizer_appid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权方帐号基本信息与授权信息（含接口权限集）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/getAuthorizerInfo.html"/></para>
    /// <para>官方约束：仅支持第三方平台调用；授权关系已取消时 <c>authorization_info</c> 可能缺省。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40013</c> / <c>61010</c>（无授权关系）等。</para>
    /// </remarks>
    [Post("/cgi-bin/component/api_get_authorizer_info")]
    Task<OpenPlatformGetAuthorizerInfoResponse> GetAuthorizerInfoAsync(
        [Body] OpenPlatformGetAuthorizerInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取授权方列表（分页）。
    /// </summary>
    /// <param name="request">分页请求（<c>offset</c> / <c>count</c> 必填，count 最大 500）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权账号总数与当前页列表（含刷新令牌——<b>敏感字段勿落日志</b>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/getAuthorizerList.html"/></para>
    /// <para>官方错误码：<c>-1</c> / <c>40170</c>（个数超出限制）。</para>
    /// </remarks>
    [Post("/cgi-bin/component/api_get_authorizer_list")]
    Task<OpenPlatformGetAuthorizerListResponse> GetAuthorizerListAsync(
        [Body] OpenPlatformGetAuthorizerListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取授权方选项设置信息。
    /// </summary>
    /// <param name="request">查询请求（<c>authorizer_appid</c> + <c>option_name</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>选项当前值（官方可能以数字承载，原样字符串）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/getAuthorizerOptionInfo.html"/></para>
    /// <para>官方约束：选项为平台代授权方设置的开关（如 <c>voice_recognize</c> 语音识别、<c>customer_service</c> 客服）。</para>
    /// </remarks>
    [Post("/cgi-bin/component/api_get_authorizer_option")]
    Task<OpenPlatformGetAuthorizerOptionResponse> GetAuthorizerOptionAsync(
        [Body] OpenPlatformGetAuthorizerOptionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置授权方选项信息。
    /// </summary>
    /// <param name="request">设置请求（<c>authorizer_appid</c> + <c>option_name</c> + <c>option_value</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/setAuthorizerOptionInfo.html"/></para>
    /// </remarks>
    [Post("/cgi-bin/component/api_set_authorizer_option")]
    Task<OpenPlatformResponse> SetAuthorizerOptionAsync(
        [Body] OpenPlatformSetAuthorizerOptionRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 微信开放平台（第三方平台）「票据推送引导」端点（免令牌）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何独立接口（对齐公众号线 I4 裁决形态）</b>：官方 <c>api_start_push_ticket</c> 以
/// <c>component_appid</c> + <c>component_secret</c> 直呼（请求体），是票据链断裂时的
/// <b>引导/自救端点</b>——它发生在拿到第一张 <c>component_verify_ticket</c> 之前，
/// 平台令牌尚不存在，声明 [Token] 即编程错误。本接口<b>不带 [Token]</b>；凭证走请求体。
/// </para>
/// </remarks>
// HTTPCLIENT018：生成器要求显式声明 TokenManagerKey/TokenType。本接口是官方契约的**免令牌端点**
// ——api_start_push_ticket 的语义是「尚未建立票据链时的推送引导」，凭证（component_secret）走请求体，
// 刻意不声明 [Token]；声明之会在令牌尚不可能存在的阶段强制取令牌（必然失败）。
#pragma warning disable HTTPCLIENT018
[HttpClientApi(RegistryGroupName = "Component", TokenManage = nameof(IOpenPlatformAppManager))]
public interface IWechatOpenPlatformComponentTicketFreeService
{
    /// <summary>
    /// 开始推送票据（免令牌；凭证走请求体）。
    /// </summary>
    /// <param name="request">推票引导请求（<c>component_appid</c> / <c>component_secret</c>，SDK 不自动回填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（成功后微信服务器开始向票据接收 URL 推送 <c>component_verify_ticket</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ticket-token/startPushTicket.html"/></para>
    /// <para>
    /// <b>安全边界</b>：<c>component_secret</c> 经请求体提交、绝不进 URL/日志（AGENTS §8；
    /// 宿主侧亦不得将其写入自己的日志/遥测）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/component/api_start_push_ticket")]
    Task<OpenPlatformResponse> StartPushTicketAsync(
        [Body] OpenPlatformStartPushTicketRequest request,
        CancellationToken cancellationToken = default);
}
#pragma warning restore HTTPCLIENT018
