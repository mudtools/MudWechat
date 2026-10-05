// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedrive;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微盘」模块管理文件权限域公共 SDK
/// （新增成员 + 删除成员 + 分享设置 + 获取分享链接 + 获取文件权限信息 + 修改文件安全设置）。
/// <para>
/// 官方对三类应用开放一致的 6 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedriveFileAclService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedriveFileAclService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedriveFileAclService"/>。
/// </para>
/// <para>
/// 官方契约陷阱：获取文件权限信息（<c>get_file_permission</c>）路由不走 <c>wedrive/file_</c> 前缀；
/// 其响应 <c>file_member_list</c> 官方参数表列 obj、返回示例实为数组；文件权限成员的
/// type/departmentid 官方标注「后续将废弃」、auth 取值仅 1（仅浏览/仅下载）。
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
public interface IWechatWorkWedriveFileAclService
{
    /// <summary>
    /// 新增成员（文件权限）
    /// <para>为指定文件添加成员/部门，可一次性添加多个。</para>
    /// <para>官方限制：auth 取值仅 1（仅下载/仅浏览）；成员类型 type 与部门 departmentid 官方标注「后续将废弃」。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddWedriveFileAclRequest"/>：fileid / auth_info，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93658"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95860"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96848"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_acl_add")]
    Task<WechatWorkResponse> AddFileAclAsync(
        [Body] AddWedriveFileAclRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除成员（文件权限）
    /// <para>删除指定文件的成员/部门，可一次性删除多个。</para>
    /// <para>官方限制：成员类型 type 与部门 departmentid 官方标注「后续将废弃」。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DelWedriveFileAclRequest"/>：fileid / auth_info，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97888"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97959"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97922"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_acl_del")]
    Task<WechatWorkResponse> DelFileAclAsync(
        [Body] DelWedriveFileAclRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分享设置（文件权限）
    /// <para>设置文件的分享权限范围与权限信息。</para>
    /// <para>官方限制：auth_scope 取值 1:指定人 2:企业内 3:企业外 4:企业内需管理员审批（仅有管理员时可设置）
    /// 5:企业外需管理员审批（仅有管理员时可设置）；auth 不填充则保持原有状态。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetWedriveFileSettingRequest"/>：fileid / auth_scope / auth）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97889"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97960"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97923"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_setting")]
    Task<WechatWorkResponse> SetFileSettingAsync(
        [Body] SetWedriveFileSettingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分享链接（文件权限）
    /// <para>获取文件的分享链接。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedriveFileShareUrlRequest"/>：fileid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分享文件的链接（share_url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97890"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97961"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97924"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_share")]
    Task<GetWedriveFileShareUrlResponse> GetFileShareUrlAsync(
        [Body] GetWedriveFileShareUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文件权限信息
    /// <para>获取文件的分享范围、安全设置、父路径继承权限、成员授权列表与水印设置。</para>
    /// <para>官方契约陷阱：响应 <c>file_member_list</c> 官方参数表列 obj（fileid 为文档时返回，
    /// 为文档所在目录成员及其他授权列表）、返回示例实为数组；<c>inherit_father_auth</c> 参数表列
    /// member_list、示例实为 auth_list，本 SDK 两个成员列表均承载；成员元素复用空间成员权限结构。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedriveFilePermissionRequest"/>：fileid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件权限信息（share_range / secure_setting / inherit_father_auth / file_member_list / co_auth_list / watermark）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97891"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97962"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97925"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/get_file_permission")]
    Task<GetWedriveFilePermissionResponse> GetFilePermissionAsync(
        [Body] GetWedriveFilePermissionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改文件安全设置（文件权限）
    /// <para>修改文件安全设置及水印相关设置。</para>
    /// <para>官方限制：仅支持在线文档类型；水印各字段不填保持原样；
    /// show_visitor_name 是否显示访问人名称仅专业版支持。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetWedriveFileSecureSettingRequest"/>：fileid / watermark）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97892"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97965"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97926"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/file_secure_setting")]
    Task<WechatWorkResponse> SetFileSecureSettingAsync(
        [Body] SetWedriveFileSecureSettingRequest request,
        CancellationToken cancellationToken = default);
}
