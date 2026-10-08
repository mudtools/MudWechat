// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「家校沟通」模块家校沟通基础域公共 SDK。
/// <para>
/// 官方对三类应用开放完全一致的 7 个端点（「学校通知」二维码 + 关注模式 + 班级群创建方式 +
/// 外部联系人 openid 转换 + 可使用的家长范围），全部收敛声明于本接口；
/// 应用类型子接口均为零差异端点空标记：自建见 <see cref="IWechatWorkInternalSchoolService"/>，
/// 第三方见 <see cref="IWechatWorkThirdPartySchoolService"/>，服务商代开发见
/// <see cref="IWechatWorkProviderSchoolService"/>。
/// </para>
/// <para>
/// 官方仅向自建与第三方开放的差异端点（老师可查看班级模式 + 手机号转外部联系人 ID）
/// 落位于 <see cref="IWechatWorkSchoolSettingService"/> 家族，不进入本家族。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「家校沟通-读取和编辑家校通讯录的应用」等清单；
/// 第三方应用须具有「家校沟通」使用权限；代开发应用须具有「家校沟通」权限。
/// 企业必须完成验证才可调用，否则返回错误码 43009（企业需要验证）；
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSchoolService
{
    /// <summary>
    /// 获取「学校通知」二维码
    /// <para>获取家长扫码关注「学校通知」的二维码（大 / 中 / 小三种尺寸），
    /// 家长扫码关注后可接收学校推送消息。</para>
    /// <para>官方业务限制：企业必须完成验证才可调用，否则返回错误码 43009（企业需要验证）。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>三种尺寸的二维码（qrcode_big 1200px / qrcode_middle 430px / qrcode_thumb 258px）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92320"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92197"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96719"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/get_subscribe_qr_code")]
    Task<GetSchoolSubscribeQrCodeResponse> GetSubscribeQrCodeAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置关注「学校通知」的模式
    /// <para>设置家长关注「学校通知」的方式：可扫码填写资料加入或禁止扫码填写资料加入。</para>
    /// </summary>
    /// <param name="request">设置请求体（<see cref="SetSchoolSubscribeModeRequest"/>：
    /// subscribe_mode（1 可扫码填写资料加入 / 2 禁止扫码填写资料加入））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92318"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92290"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/set_subscribe_mode")]
    Task<WechatWorkResponse> SetSubscribeModeAsync(
        [Body] SetSchoolSubscribeModeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取关注「学校通知」的模式
    /// <para>获取家长当前的「学校通知」关注模式。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>关注模式（subscribe_mode：1 可扫码填写资料加入 / 2 禁止扫码填写资料加入）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92318"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92290"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/get_subscribe_mode")]
    Task<GetSchoolSubscribeModeResponse> GetSubscribeModeAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取「班级群创建方式」
    /// <para>获取班级群的创建方式（自动创建或手动创建）。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建方式（create_mode：0 自动创建 / 1 手动创建）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92430"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/get_chat_create_mode")]
    Task<GetSchoolChatCreateModeResponse> GetChatCreateModeAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置「班级群创建方式」
    /// <para>设置班级群的创建方式（自动创建或手动创建）。</para>
    /// </summary>
    /// <param name="request">设置请求体（<see cref="SetSchoolChatCreateModeRequest"/>：
    /// create_mode（0 自动创建 / 1 手动创建））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92430"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/set_chat_create_mode")]
    Task<WechatWorkResponse> SetChatCreateModeAsync(
        [Body] SetSchoolChatCreateModeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 外部联系人 openid 转换
    /// <para>将微信外部联系人的 userid 转为微信 openid，用于调用支付相关接口。</para>
    /// <para>官方业务限制：暂不支持企业微信外部联系人（ExternalUserid 为 wo 开头）的 userid 转 openid。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="SchoolConvertToOpenIdRequest"/>：
    /// external_userid（注意不是企业成员的账号））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果（openid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92323"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92292"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96721"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/convert_to_openid")]
    Task<SchoolConvertToOpenIdResponse> ConvertToOpenIdAsync(
        [Body] SchoolConvertToOpenIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可使用的家长范围
    /// <para>获取可在微信「学校通知-学校应用」使用该应用的家长范围（学生或部门列表形式），
    /// 应用只能给该列表下的家长发送「学校通知」。</para>
    /// <para>官方业务限制：该范围只能由学校的系统管理员在「管理端-家校沟通-配置」配置。</para>
    /// </summary>
    /// <param name="agentId">应用 id（官方必填，以 <c>agentid</c> Query 参数传递）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>家长范围（allow_scope：students.userid 学生列表 / departments.partyid 部门列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94895"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94960"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96725"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/agent/get_allow_scope")]
    Task<GetSchoolAllowScopeResponse> GetAllowScopeAsync(
        [Query("agentid")] int agentId,
        CancellationToken cancellationToken = default);
}
