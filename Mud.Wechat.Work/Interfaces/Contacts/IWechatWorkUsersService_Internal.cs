// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Contracts.Users;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「成员管理」域企业自建应用 SDK：除继承自 <see cref="IWechatWorkUsersService"/> 的公共读取端点外，
/// 本接口提供自建应用的通讯录写入端点（创建/更新/删除/批量删除成员）、登录二次验证、获取加入企业二维码与邀请成员。
/// <para>第三方应用见 <see cref="IWechatWorkThirdPartyUsersService"/>；服务商代开发见 <see cref="IWechatWorkProviderUsersService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（<c>corpid</c> + <c>corpsecret</c> 换取，路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// 通讯录写入端点官方权限口径：「仅通讯录同步助手或第三方通讯录应用可调用」——自建应用须以通讯录同步
/// （或具备通讯录写权限）的 secret 签发的令牌调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Contact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkUsersService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalUsersService : IWechatWorkUsersService
{
    /// <summary>
    /// 创建成员
    /// <para>向企业通讯录写入新成员。userid 与 name 必填，mobile/email 不能同时为空。</para>
    /// <para>每个部门下的部门、成员总数不能超过 3 万个，建议创建部门与创建成员串行处理；填写的部门不存在时会自动新建（见返回值 created_department_list）。</para>
    /// </summary>
    /// <param name="request">成员请求体（<see cref="CreateUserRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（含因填写不存在部门而自动新建的部门列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90195"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/create")]
    Task<CreateUserResponse> CreateUserAsync(
        [Body] CreateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新成员
    /// <para>更新通讯录中的既有成员。除 userid 外全部字段可选，未传入的字段不更新。</para>
    /// <para>系统自动生成的 userid 仅允许修改一次（经 <see cref="UpdateUserRequest.NewUserId"/> 指定）；BizMail/BizMailAlias 与其他字段的更新不具备原子性。</para>
    /// </summary>
    /// <param name="request">成员请求体（<see cref="UpdateUserRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90197"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/update")]
    Task<WechatWorkResponse> UpdateUserAsync(
        [Body] UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除成员
    /// <para>从企业通讯录中删除成员。若是绑定了腾讯企业邮，则会同时删除邮箱账号。</para>
    /// </summary>
    /// <param name="userid">成员 UserID，对应管理端的账号。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90198"/></para>
    /// </remarks>
    [Get("/cgi-bin/user/delete")]
    Task<WechatWorkResponse> DeleteUserAsync(
        [Query("userid")] string userid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除成员
    /// <para>批量从企业通讯录中删除成员。若存在无效 UserID，直接返回错误（整批失败）。</para>
    /// </summary>
    /// <param name="request">请求体（useridlist：最多 200 个成员 UserID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90199"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/batchdelete")]
    Task<WechatWorkResponse> BatchDeleteUsersAsync(
        [Body] BatchDeleteUsersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 登录二次验证
    /// <para>成员登录二次验证通过后调用本接口使其成功加入企业。开启二次验证后，成员登录会跳转企业验证页并携带 code 参数，企业验证成员信息后须调用本接口放行。</para>
    /// </summary>
    /// <param name="userid">成员 UserID，对应管理端的账号。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>验证结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90203"/></para>
    /// </remarks>
    [Get("/cgi-bin/user/authsucc")]
    Task<WechatWorkResponse> CompleteSecondaryAuthAsync(
        [Query("userid")] string userid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取加入企业二维码
    /// <para>获取企业成员实时加入二维码。须拥有通讯录的管理权限，使用通讯录同步的 secret。</para>
    /// </summary>
    /// <param name="sizeType">二维码尺寸类型（1: 171x171；2: 399x399；3: 741x741；4: 2052x2052），不传时由官方默认。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码链接（join_qrcode，有效期 7 天）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91714"/></para>
    /// </remarks>
    [Get("/cgi-bin/corp/get_join_qrcode")]
    Task<GetJoinQrcodeResponse> GetJoinQrcodeAsync(
        [Query("size_type")] int? sizeType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 邀请成员
    /// <para>邀请成员（企业成员或未安装企业微信的成员）使用企业微信，user/party/tag 三者不能同时为空。须拥有指定成员、部门或标签的查看权限。</para>
    /// <para>同一用户只须邀请一次；邀请频率是异步检查的，调用返回成功并不代表接收者一定能收到邀请消息。</para>
    /// </summary>
    /// <param name="request">请求体（user 成员 ID 列表 ≤ 1000；party 部门 ID 列表 ≤ 100；tag 标签 ID 列表 ≤ 100）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>邀请结果（invaliduser/invalidparty/invalidtag 非法列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90975"/></para>
    /// </remarks>
    [Post("/cgi-bin/batch/invite")]
    Task<InviteMembersResponse> InviteMembersAsync(
        [Body] InviteMembersRequest request,
        CancellationToken cancellationToken = default);
}
