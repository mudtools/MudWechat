// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「部门管理」域企业自建应用 SDK：除继承自 <see cref="IWechatWorkDepartmentsService"/> 的公共读取端点外，
/// 本接口提供自建应用的通讯录写入端点（创建/更新/删除部门）。
/// <para>第三方应用见 <see cref="IWechatWorkThirdPartyDepartmentsService"/>；服务商代开发见 <see cref="IWechatWorkProviderDepartmentsService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方写入端点权限口径：
/// 「应用须拥有指定部门的管理权限」；第三方仅通讯录应用可调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Contact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkDepartmentsService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalDepartmentsService : IWechatWorkDepartmentsService
{
    /// <summary>
    /// 创建部门
    /// <para>向企业通讯录写入新部门。name 与 parentid 必填；未指定 id 时由官方自动生成。</para>
    /// <para>部门最大层级为 15 层；部门总数不能超过 3 万个；建议创建部门与创建成员串行处理。</para>
    /// </summary>
    /// <param name="request">部门请求体（<see cref="CreateDepartmentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（含新建部门的 ID）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90205"/></para>
    /// </remarks>
    [Post("/cgi-bin/department/create")]
    Task<CreateDepartmentResponse> CreateDepartmentAsync(
        [Body] CreateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新部门
    /// <para>更新通讯录中的既有部门。应用须拥有指定部门的管理权限；如若要移动部门，需要有新父部门的管理权限。</para>
    /// <para>非必须字段未指定则不更新该字段。</para>
    /// </summary>
    /// <param name="request">部门请求体（<see cref="UpdateDepartmentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90206"/></para>
    /// </remarks>
    [Post("/cgi-bin/department/update")]
    Task<WechatWorkResponse> UpdateDepartmentAsync(
        [Body] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除部门
    /// <para>从企业通讯录中删除部门。应用须拥有指定部门的管理权限。</para>
    /// <para>不能删除根部门；不能删除含有子部门、成员的部门。</para>
    /// </summary>
    /// <param name="id">部门 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90207"/></para>
    /// </remarks>
    [Get("/cgi-bin/department/delete")]
    Task<WechatWorkResponse> DeleteDepartmentAsync(
        [Query("id")] int id,
        CancellationToken cancellationToken = default);
}
