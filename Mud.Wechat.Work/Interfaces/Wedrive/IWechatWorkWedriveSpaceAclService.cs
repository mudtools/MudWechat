// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedrive;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微盘」模块管理空间权限域公共 SDK
/// （添加成员/部门 + 移除成员/部门 + 安全设置 + 获取邀请链接 + 获取空间信息（新版））。
/// <para>
/// 官方对三类应用开放一致的 5 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedriveSpaceAclService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedriveSpaceAclService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedriveSpaceAclService"/>。
/// </para>
/// <para>
/// 官方契约陷阱：新版「获取空间信息」（路由 <c>new_space_info</c>）官方将其归入本「管理空间权限」分组
/// （区别于旧版 <c>space_info</c> 挂「管理空间」分组），返回信息含安全设置 secure_setting 与
/// 已退出空间成员 quit_userid，见 <see cref="GetNewSpaceInfoAsync"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)</c> 建立「应用 + 企业」作用域后再调用。
/// 官方权限口径：自建应用需配置到「可调用应用」列表中，使用对应应用 secret 获取的 accesstoken 调用；
/// 第三方应用与服务商代开发应用需具有「微盘」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedriveSpaceAclService
{
    /// <summary>
    /// 添加成员/部门
    /// <para>对指定空间添加成员/部门，可一次性添加多个，操作者为应用本身。</para>
    /// <para>官方限制：应用空间管理员（auth = 7）连同已设置的管理员最多可指定 3 个，且不支持将部门设为管理员。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddWedriveSpaceAclRequest"/>：spaceid / auth_info，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93656"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95858"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96846"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/space_acl_add")]
    Task<WechatWorkResponse> AddSpaceAclAsync(
        [Body] AddWedriveSpaceAclRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除成员/部门
    /// <para>对指定空间移除成员/部门，操作者为应用本身。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DelWedriveSpaceAclRequest"/>：spaceid / auth_info，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97875"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97947"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97910"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/space_acl_del")]
    Task<WechatWorkResponse> DelSpaceAclAsync(
        [Body] DelWedriveSpaceAclRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 安全设置
    /// <para>修改空间权限的安全设置（水印 / 保密模式 / 邀请链接加入与默认权限 / 文件默认可查看范围 / 禁止分享到企业外）。</para>
    /// <para>官方限制：应用通过 api 调用仅支持设置由本应用创建的空间；启用水印（enable_watermark）仅专业版企业可设置；
    /// 未填充的可选字段保持原有状态。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetWedriveSpaceSettingRequest"/>：spaceid / enable_watermark / enable_confidential_mode / share_url_no_approve / share_url_no_approve_default_auth / default_file_scope / ban_share_external）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97876"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97948"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97911"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/space_setting")]
    Task<WechatWorkResponse> SetSpaceSettingAsync(
        [Body] SetWedriveSpaceSettingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取邀请链接
    /// <para>获取空间邀请分享链接（成员可通过链接申请加入空间，是否需审批由安全设置 share_url_no_approve 决定）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedriveSpaceShareUrlRequest"/>：spaceid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>邀请链接（space_share_url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97877"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97949"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97912"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/space_share")]
    Task<GetWedriveSpaceShareUrlResponse> GetSpaceShareUrlAsync(
        [Body] GetWedriveSpaceShareUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取空间信息（新版）
    /// <para>获取空间成员、权限及安全设置（区别于「管理空间」分组的旧版 <c>space_info</c>：本端点额外返回
    /// 安全设置 secure_setting 与已退出空间成员列表 quit_userid；官方将本端点归入「管理空间权限」分组）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedriveNewSpaceInfoRequest"/>：spaceid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>空间信息（space_info：spaceid / space_name / auth_list / space_sub_type / secure_setting）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97878"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97950"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97913"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/new_space_info")]
    Task<GetWedriveNewSpaceInfoResponse> GetNewSpaceInfoAsync(
        [Body] GetWedriveNewSpaceInfoRequest request,
        CancellationToken cancellationToken = default);
}
