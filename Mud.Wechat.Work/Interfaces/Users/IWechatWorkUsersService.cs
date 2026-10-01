// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Users;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「成员管理」域公共 SDK（自建应用 / 第三方应用 / 服务商代开发三类应用均可调用的读取面端点）。
/// <para>
/// 通讯录写入端点（创建/更新/删除成员等）与能力差异端点（成员授权模式查询等）声明于派生接口：
/// 自建应用见 <see cref="IWechatWorkInternalUsersService"/>，第三方应用见
/// <see cref="IWechatWorkThirdPartyUsersService"/>，服务商代开发见 <see cref="IWechatWorkProviderUsersService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkProviderAuthenticationService"/>：BaseAddress 交由运行时解析
/// （per-app <c>WechatAppConfig.BaseUrl</c>，默认 <c>https://qyapi.weixin.qq.com</c>）。
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（企业自建为应用自身 access_token；
/// 第三方/代开发为授权企业级 access_token，scope = authCorpId），由多应用基座按当前应用上下文路由——
/// 企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header，
/// 该诊断属预期且不可规避（详见详细设计 §1）。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkUsersService
{
    /// <summary>
    /// 读取成员
    /// <para>按 userid 读取成员信息。应用只能获取可见范围内的成员信息，且每种应用获取的字段有所不同（详见 <see cref="UserInfo"/> 字段说明）。</para>
    /// <para>自 2022-06-20 起新创建的自建与代开发应用不再返回头像、性别、手机、邮箱、企业邮箱、员工个人二维码、地址，需经 oauth2 手工授权获取；自 2022-08-15 起「通讯录同步」新增 IP 不能再调用此接口（官方建议改用获取成员 ID 列表）。</para>
    /// </summary>
    /// <param name="userid">成员 UserID，对应管理端账号，企业内唯一，不区分大小写，1~64 字节。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员对象（<see cref="UserInfo"/>；第三方应用调用时 userid 字段返回 open_userid）。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90196"/></remarks>
    [Get("/cgi-bin/user/get")]
    Task<UserInfo> GetUserAsync(
        [Query("userid")] string userid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门成员
    /// <para>获取部门成员摘要（userid/name/department/open_userid）。应用须拥有指定部门的查看权限。</para>
    /// <para>接口不递归子部门：如需部门及其子部门全部成员，须先获取子部门再逐层递归调用。第三方应用自 2019-12-30 起不再返回真实 name（以 userid 代替）。</para>
    /// </summary>
    /// <param name="departmentId">获取的部门 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>部门成员摘要列表（userlist）。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90200"/></remarks>
    [Get("/cgi-bin/user/simplelist")]
    Task<GetUserSimpleListResponse> GetUserSimpleListAsync(
        [Query("department_id")] int departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门成员详情
    /// <para>获取部门成员完整信息（同读取成员的字段集）。应用须拥有指定部门的查看权限。</para>
    /// <para>接口不递归子部门：如需部门及其子部门全部成员，须先获取子部门再逐层递归调用。敏感字段的返回范围同读取成员。</para>
    /// </summary>
    /// <param name="departmentId">获取的部门 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>部门成员详情列表（userlist）。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90201"/></remarks>
    [Get("/cgi-bin/user/list")]
    Task<GetUserDetailListResponse> GetUserDetailListAsync(
        [Query("department_id")] int departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员 ID 列表
    /// <para>以游标分页方式获取企业成员 ID 与所属部门的关系列表（2022-08 通讯录安全加固后的官方推荐替代接口）。</para>
    /// <para>自建应用须以「通讯录同步 secret」调用（返回 userid）；第三方应用须通讯录编辑授权（返回 open_userid）。</para>
    /// </summary>
    /// <param name="request">分页请求体（cursor 首次不填；limit 取值 1~10000）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户-部门关系列表（dept_user）与下一页游标（next_cursor）。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96067"/>（自建）、<see href="https://developer.work.weixin.qq.com/document/path/96021"/>（第三方）、<see href="https://developer.work.weixin.qq.com/document/path/96269"/>（代开发）。</remarks>
    [Post("/cgi-bin/user/list_id")]
    Task<ListUserIdsResponse> ListUserIdsAsync(
        [Body] ListUserIdsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 手机号获取 userid
    /// <para>通过手机号换取其所对应的成员 userid。应用须拥有指定成员的查看权限。</para>
    /// <para>请确保手机号的正确性：若出错的次数超出企业人数上限的 20%，会导致 1 天不可调用。第三方应用获取的是密文 userid。</para>
    /// </summary>
    /// <param name="request">请求体（mobile：5~32 字节的手机号）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员 userid。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95402"/></remarks>
    [Post("/cgi-bin/user/getuserid")]
    Task<GetUserIdByMobileResponse> GetUserIdByMobileAsync(
        [Body] GetUserIdByMobileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 邮箱获取 userid
    /// <para>通过邮箱换取其所对应的成员 userid。应用须拥有指定成员的查看权限。</para>
    /// <para>请确保邮箱的正确性：若出错的次数超过企业人数上限的 20%，会导致 1 天不可调用。已升级 openid 的代开发或第三方，获取的是密文 userid。</para>
    /// </summary>
    /// <param name="request">请求体（email 必填；email_type：1 企业邮箱（默认），2 个人邮箱）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员 userid。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95895"/></remarks>
    [Post("/cgi-bin/user/get_userid_by_email")]
    Task<GetUserIdByEmailResponse> GetUserIdByEmailAsync(
        [Body] GetUserIdByEmailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// userid 转 openid
    /// <para>将成员 userid 转换为 openid（使用场景为企业支付：企业红包、向员工付款）。成员必须处于应用的可见范围内。</para>
    /// <para>需要成员使用微信登录企业微信或关注微信插件（原企业号）才能转成 openid；外部联系人请使用外部联系人 openid 转换接口。</para>
    /// </summary>
    /// <param name="request">请求体（userid：企业内的成员 ID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员 userid 对应的 openid。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90202"/></remarks>
    [Post("/cgi-bin/user/convert_to_openid")]
    Task<ConvertUserIdToOpenIdResponse> ConvertUserIdToOpenIdAsync(
        [Body] ConvertUserIdToOpenIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// openid 转 userid
    /// <para>将 openid 转换为成员 userid（主要应用于企业支付之后的结果查询）。管理组需对 openid 对应的企业微信成员有查看权限。</para>
    /// </summary>
    /// <param name="request">请求体（openid：企业支付之后返回结果的 openid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>openid 对应的成员 userid。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90202"/></remarks>
    [Post("/cgi-bin/user/convert_to_userid")]
    Task<ConvertOpenIdToUserIdResponse> ConvertOpenIdToUserIdAsync(
        [Body] ConvertOpenIdToUserIdRequest request,
        CancellationToken cancellationToken = default);
}
