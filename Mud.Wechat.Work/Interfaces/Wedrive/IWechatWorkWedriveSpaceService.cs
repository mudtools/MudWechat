// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedrive;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「微盘」模块管理空间域公共 SDK（新建空间 + 重命名空间 + 解散空间 + 获取空间信息）。
/// <para>
/// 官方对三类应用开放一致的 4 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedriveSpaceService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedriveSpaceService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedriveSpaceService"/>。
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
public interface IWechatWorkWedriveSpaceService
{
    /// <summary>
    /// 新建空间
    /// <para>在微盘内新建空间，创建者为应用本身。</para>
    /// <para>官方限制：应用空间管理员（auth = 7）最多可指定 3 个，且不支持设置部门；
    /// 「可预览」权限（auth = 4）仅专业版微盘企业可设置；space_sub_type 目前仅支持 0（普通空间）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateWedriveSpaceRequest"/>：space_name / auth_info / space_sub_type）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建空间的 spaceid。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93655"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95857"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96845"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/space_create")]
    Task<CreateWedriveSpaceResponse> CreateSpaceAsync(
        [Body] CreateWedriveSpaceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重命名空间
    /// <para>重命名已有空间。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="RenameWedriveSpaceRequest"/>：spaceid / space_name，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97856"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97872"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97862"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/space_rename")]
    Task<WechatWorkResponse> RenameSpaceAsync(
        [Body] RenameWedriveSpaceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解散空间
    /// <para>解散已有空间（解散后空间内文件不可访问，属不可逆操作，调用前须确认空间归属）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DismissWedriveSpaceRequest"/>：spaceid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97857"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97873"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97863"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/space_dismiss")]
    Task<WechatWorkResponse> DismissSpaceAsync(
        [Body] DismissWedriveSpaceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取空间信息
    /// <para>获取指定空间的基础信息（空间 ID / 空间名 / 成员及部门权限列表 / 空间类型）。</para>
    /// <para>官方契约陷阱：本端点为旧版「获取空间信息」（路由 <c>space_info</c>）；新版同标题端点挂官方
    /// 「管理空间权限」分组（路由 <c>new_space_info</c>），额外返回安全设置 secure_setting 与已退出空间成员
    /// quit_userid，见 <see cref="IWechatWorkWedriveSpaceAclService.GetNewSpaceInfoAsync"/>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedriveSpaceInfoRequest"/>：spaceid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>空间信息（space_info：spaceid / space_name / auth_list / space_sub_type）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97858"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97874"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97864"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/space_info")]
    Task<GetWedriveSpaceInfoResponse> GetSpaceInfoAsync(
        [Body] GetWedriveSpaceInfoRequest request,
        CancellationToken cancellationToken = default);
}
