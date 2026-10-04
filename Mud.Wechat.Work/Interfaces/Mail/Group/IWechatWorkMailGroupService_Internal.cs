// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Mail;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「邮件」模块管理邮件群组族企业自建应用 SDK
/// （创建邮件群组 + 更新邮件群组 + 删除邮件群组 + 获取邮件群组详情 + 模糊搜索邮件群组）。
/// <para>
/// 官方仅向企业自建应用开放本域 5 个端点，全部收敛声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalSecurityVipService"/> 仅自建子接口承载）。
/// 官方未向第三方应用与服务商代开发应用开放本域，故本家族不声明对应应用类型子接口
/// （能力漂移守卫：继承链上恰好只有自建子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 应用调用接口只能访问自身创建的邮件群组。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Mail",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMailGroupService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMailGroupService : IWechatWorkMailGroupService
{
    /// <summary>
    /// 创建邮件群组
    /// <para>在企业管理端邮件群组中创建一个邮件群组，成员由成员邮箱、群组邮箱、部门与标签共同组成。</para>
    /// <para>官方业务限制：群组名称不能与其他群组重名，长度限定 200 字节；
    /// email_list、group_list、department_list、tag_list 至少填一个，不可同时为空；
    /// allow_type 值为 0/1/2 时不得传 allow_emaillist、allow_departmentlist、allow_taglist，值为 3 时必须至少传其中一项。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="CreateMailGroupRequest"/>：groupid / groupname 官方必填，成员与权限列表官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95510"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/group/create")]
    Task<WechatWorkResponse> CreateMailGroupAsync(
        [Body] CreateMailGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新邮件群组
    /// <para>更新指定的邮件群组；列表类型字段不传则保持不变，传空结构或空数组则清空。</para>
    /// <para>官方业务限制：群组名称不能与其他群组重名，长度限定 200 字节；
    /// 成员由 email_list、group_list、department_list、tag_list 共同组成，不允许全部清空；
    /// allow_type 值为 0/1/2 时不得传入 allow_emaillist、allow_departmentlist、allow_taglist，
    /// 值为 3 时必须传入至少一项。</para>
    /// </summary>
    /// <param name="request">更新请求体（<see cref="UpdateMailGroupRequest"/>：groupid 官方必填，其余字段官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97995"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/group/update")]
    Task<WechatWorkResponse> UpdateMailGroupAsync(
        [Body] UpdateMailGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除邮件群组
    /// <para>删除已有的邮件群组。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteMailGroupRequest"/>：groupid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97996"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/group/delete")]
    Task<WechatWorkResponse> DeleteMailGroupAsync(
        [Body] DeleteMailGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取邮件群组详情（官方即 GET，groupid 走 Query）
    /// <para>获取邮件群组详细信息，包含群组名称、群组成员、群组使用权限等。</para>
    /// </summary>
    /// <param name="groupid">邮件群组 ID，邮箱格式（官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>邮件群组详情（groupid / groupname / email_list / tag_list / department_list / group_list / allow_type /
    /// allow_emaillist / allow_departmentlist / allow_taglist）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97997"/></para>
    /// </remarks>
    [Get("/cgi-bin/exmail/group/get")]
    Task<GetMailGroupResponse> GetMailGroupAsync(
        [Query("groupid")] string groupid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 模糊搜索邮件群组（官方即 GET，fuzzy / groupid 走 Query）
    /// <para>通过群组 ID 模糊搜索邮件群组；fuzzy 传 0 时获取全部邮件群组。</para>
    /// </summary>
    /// <param name="fuzzy">是否模糊搜索（官方必填）：1 - 开启模糊搜索，0 - 获取全部邮件群组。</param>
    /// <param name="groupid">邮件群组 ID，邮箱格式（官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>返回条数（count）与邮件群组列表（groups：groupid / groupname）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97998"/></para>
    /// </remarks>
    [Get("/cgi-bin/exmail/group/search")]
    Task<SearchMailGroupResponse> SearchMailGroupAsync(
        [Query("fuzzy")] long fuzzy,
        [Query("groupid")] string? groupid = null,
        CancellationToken cancellationToken = default);
}
