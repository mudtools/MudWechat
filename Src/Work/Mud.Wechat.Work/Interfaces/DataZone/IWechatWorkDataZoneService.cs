// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.DataZone;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「数据与智能专区」模块基础接口域公共 SDK（设置公钥 / 获取会话存档授权成员列表 /
/// 设置专区接收回调事件 / 会话组件敏感信息隐藏设置 / 设置与获取日志打印级别 / 上传临时文件到专区 /
/// 获取文件内容存档授权成员列表 / 开启·关闭·查询专区调试模式，十二端点收敛面）。
/// <para>
/// 官方对三类应用开放一致的 12 个端点，全部收敛声明于本接口；应用类型子接口承载官方开放面差异端点：
/// 企业自建应用见 <see cref="IWechatWorkInternalDataZoneService"/>（零差异端点空标记），
/// 服务商代开发见 <see cref="IWechatWorkProviderDataZoneService"/>（额外开放「获取数据与智能专区授权信息」），
/// 第三方应用见 <see cref="IWechatWorkThirdPartyDataZoneService"/>（额外开放「获取数据与智能专区授权信息」与
/// 「获取数据与智能专区文档存档授权信息」）。
/// 应用调用专区程序（同步/异步调用）见 <see cref="IWechatWorkDataZoneProgramService"/> 接口族。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkMediaService"/>：三类应用消费的令牌路由键均为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。各端点均要求应用具备
/// 「数据与智能专区」权限（文件内容存档端点要求「数据与智能专区-分析企业文件数据」权限，且需企业管理员二次授权）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkDataZoneService
{
    /// <summary>
    /// 设置公钥
    /// <para>授权完成后需调用本接口上传公钥（RSA-2048），公钥经 openssl 生成；
    /// <b>设置公钥之后，消息才开始存档</b>。公钥用于加密每条消息的密钥——通过「获取会话记录」等接口得到
    /// encrypt_secret_key 后，以私钥执行 RSA 解密得到 secret_key，再传入会话展示组件展示。</para>
    /// <para>官方限制：更换公钥时 public_key_ver 必须大于旧公钥版本号。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetDataZonePublicKeyRequest"/>：public_key / public_key_ver）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99961"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99845"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100016"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/set_public_key")]
    Task<WechatWorkResponse> SetPublicKeyAsync(
        [Body] SetDataZonePublicKeyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会话存档授权成员列表
    /// <para>通过该接口可获取授权企业中「会话内容存档」所有生效中的成员列表（cursor + limit 翻页拉取）。</para>
    /// <para>官方限制：limit 不超过 1000，默认 200 条。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetChatArchiveAuthUserListRequest"/>：cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生效成员列表（auth_user_list：userid / edition_list）、翻页游标（next_cursor）与是否还有更多数据（has_more）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99962"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99846"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100017"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/get_auth_user_list")]
    Task<GetChatArchiveAuthUserListResponse> GetChatArchiveAuthUserListAsync(
        [Body] GetChatArchiveAuthUserListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置专区接收回调事件
    /// <para>设置应用关联的程序接收专区回调事件。</para>
    /// <para>官方限制：同一个应用只能设置一个程序接收；若先设置了程序 A 接收，
    /// 再调用本接口设置程序 B 时，会更改为程序 B 接收。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetDataZoneReceiveCallbackRequest"/>：program_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99963"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99850"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100018"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/set_receive_callback")]
    Task<WechatWorkResponse> SetReceiveCallbackAsync(
        [Body] SetDataZoneReceiveCallbackRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 会话组件敏感信息隐藏设置
    /// <para>设置成员使用会话组件时，敏感信息（手机号、身份证号、银行卡号）是否打星；
    /// 未调用接口开启展示敏感信息时，会话组件默认为打星。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetDataZoneHideSensitiveInfoConfigRequest"/>：userid / config（hide_mobile / hide_idcard / hide_bankno，未设置时官方默认均为 false））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100139"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100055"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100054"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/set_hide_sensitiveinfo_config")]
    Task<WechatWorkResponse> SetHideSensitiveInfoConfigAsync(
        [Body] SetDataZoneHideSensitiveInfoConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会话组件敏感信息隐藏配置
    /// <para>获取成员使用会话组件时敏感信息（手机号、身份证号、银行卡号）的打星配置。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetDataZoneHideSensitiveInfoConfigRequest"/>：userid（官方参数表原文为「成员的密文 userid」））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>敏感信息隐藏配置（config：hide_mobile / hide_idcard / hide_bankno）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100139"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100055"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100054"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/get_hide_sensitiveinfo_config")]
    Task<GetDataZoneHideSensitiveInfoConfigResponse> GetHideSensitiveInfoConfigAsync(
        [Body] GetDataZoneHideSensitiveInfoConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置日志打印级别
    /// <para>专区里的程序可以调用 SDK 打印三种级别的日志：ERR、INFO、DBG（级别顺序为 ERR &lt; INFO &lt; DBG）；
    /// 指定级别后，仅会存储不高于该级别的日志（如指定 2，则只存储级别为 1 或 2 的日志）。
    /// 程序稳定后可降低级别以节省日志存储空间。</para>
    /// <para>官方限制：应用需具有数据专区权限；指定的程序 program_id 需与应用有授权关系；官方模型不支持。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetDataZoneLogLevelRequest"/>：program_id / log_level（1 - ERR；2 - INFO；3 - DBG；默认 2））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100108"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100106"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100109"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/set_log_level")]
    Task<WechatWorkResponse> SetLogLevelAsync(
        [Body] SetDataZoneLogLevelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前日志打印级别
    /// <para>获取专区程序当前的日志打印级别。</para>
    /// <para>官方限制：应用需具有数据专区权限；指定的程序 program_id 需与应用有授权关系；官方模型不支持。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetDataZoneLogLevelRequest"/>：program_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>当前日志打印级别（log_level：1 - ERR；2 - INFO；3 - DBG）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100108"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100106"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100109"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/get_log_level")]
    Task<GetDataZoneLogLevelResponse> GetLogLevelAsync(
        [Body] GetDataZoneLogLevelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传临时文件到专区
    /// <para>上传临时文件供数据专区接口或 SDK 使用（multipart/form-data，文件标识名必须为 <c>media</c>，
    /// 须包含 filename、filelength、content-type 等信息；filename 决定下载文件时展示的文件名）。</para>
    /// <para>官方限制：文件类型 type 目前仅支持普通文件 file；所有文件 size 必须介于 6B ~ 60MB；
    /// 返回的 media_id 仅三天内有效，不能跨企业使用，也不能跨应用使用，仅可以用于数据专区接口或 SDK。</para>
    /// </summary>
    /// <param name="type">文件类型（官方必填）：目前仅支持普通文件 file。</param>
    /// <param name="formData">multipart 表单内容（文件字段名须为 <c>media</c>，由调用方构建
    /// <see cref="IFormContent"/> 实现，含 filename / filelength / content-type 信息）。
    /// 须标 <c>[MultipartForm]</c> —— 生成器 3.0.1 对 <c>[FormContent]</c> 发射的
    /// <c>GetFormDataContentAsync</c> 调用在运行时接口上不存在（CS1061），
    /// <c>[MultipartForm]</c> 才走 <see cref="IFormContent.ToHttpContentAsync"/> 通路。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件类型（type）、文件唯一标识（media_id，三天有效）与上传时间戳（created_at）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100174"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100140"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100175"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/upload_media")]
    Task<UploadDataZoneMediaResponse> UploadMediaAsync(
        [Query("type")] string type,
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文件内容存档授权成员列表
    /// <para>通过该接口可获取授权企业中「文件内容存档」所有生效中的成员列表（cursor + limit 翻页拉取）。</para>
    /// <para>官方限制：limit 不超过 1000，默认 200 条；应用需具备「数据与智能专区-分析企业文件数据」权限，
    /// 「分析企业文档数据」权限需要企业管理员二次授权；企业或服务商需已灰度文件内容存档功能。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetDocArchiveAuthUserListRequest"/>：cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生效成员列表（auth_user_list：userid）、翻页游标（next_cursor）与是否还有更多数据（has_more）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101873"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101681"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101882"/></para>
    /// </remarks>
    [Post("/cgi-bin/docdata/get_auth_user_list")]
    Task<GetDocArchiveAuthUserListResponse> GetDocArchiveAuthUserListAsync(
        [Body] GetDocArchiveAuthUserListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开启专区调试模式
    /// <para>开启应用关联的专区程序的调试模式；调试凭证 debug_token 由专区程序侧生成，
    /// 仅用于调试链路，请勿与 <see cref="WechatTokenTypes"/> 体系下的业务令牌混淆。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="OpenDataZoneDebugModeRequest"/>：program_id / debug_token）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100087"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100083"/></para>
    /// <para>官方权限口径：自建 / 代开发 / 第三方应用均需具备「数据与智能专区权限」；
    /// 服务商代开发侧官方文档树虽列出「开启专区调试模式」导航项，但对应文档页未发布正文，本注释以自建 / 第三方文档页为准。</para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/open_debug_mode")]
    Task<WechatWorkResponse> OpenDebugModeAsync(
        [Body] OpenDataZoneDebugModeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭专区调试模式
    /// <para>关闭应用关联的专区程序的调试模式。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CloseDataZoneDebugModeRequest"/>：program_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100088"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100084"/></para>
    /// <para>官方权限口径：自建 / 代开发 / 第三方应用均需具备「数据与智能专区权限」；
    /// 服务商代开发侧官方文档树虽列出「关闭专区调试模式」导航项，但对应文档页未发布正文，本注释以自建 / 第三方文档页为准。</para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/close_debug_mode")]
    Task<WechatWorkResponse> CloseDebugModeAsync(
        [Body] CloseDataZoneDebugModeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取专区调试模式状态
    /// <para>查询应用关联的专区程序当前的调试模式状态。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CheckDataZoneDebugModeRequest"/>：program_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>程序当前的调试模式状态（debug_mode_status：1 - 关闭；2 - 开启）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100113"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100112"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100114"/></para>
    /// <para>官方权限口径：自建 / 代开发 / 第三方应用均需具备「数据与智能专区权限」。</para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/check_debug_mode")]
    Task<CheckDataZoneDebugModeResponse> CheckDebugModeAsync(
        [Body] CheckDataZoneDebugModeRequest request,
        CancellationToken cancellationToken = default);
}
