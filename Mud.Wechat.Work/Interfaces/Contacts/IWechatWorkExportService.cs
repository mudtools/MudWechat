// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Contacts.Export;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「异步导出接口」域公共 SDK（导出成员、导出成员详情、导出部门、导出标签成员、获取导出结果）。
/// <para>
/// 官方对企业自建应用、第三方应用、服务商代开发三类应用开放了完全一致的 5 个端点，
/// 因此全部端点声明于本公共父接口；三个应用类型子接口
/// （<see cref="IWechatWorkInternalExportService"/> / <see cref="IWechatWorkThirdPartyExportService"/> /
/// <see cref="IWechatWorkProviderExportService"/>）均为空标记，仅作为类型化契约入口。
/// </para>
/// <para>
/// 官方另有「导出任务完成通知」回调事件（Event = <c>batch_job_result</c>，JobType = <c>export_simple_user</c> /
/// <c>export_user</c> / <c>export_department</c> / <c>export_tag</c>）推送至提交任务的应用回调地址，
/// 属回调事件而非 HTTP 端点，见通讯录回调通知处理。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkTagsService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：导出成员 / 成员详情 / 部门仅会返回有权限的人员 / 部门列表；导出标签成员要求对标签有读取权限；
/// 获取导出结果的调用身份需要与提交任务的一致。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExportService
{
    /// <summary>
    /// 导出成员
    /// <para>异步导出企业成员（simple list）列表；仅会返回有权限的人员列表。</para>
    /// <para>数据文件以 encoding_aeskey 对应的 AES-256-CBC 加密，下载链接经获取导出结果端点取得，有效期 2 个小时。</para>
    /// </summary>
    /// <param name="request">导出请求体（<see cref="ExportRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务受理结果（jobid；经获取导出结果端点轮询或等待 batch_job_result 回调通知）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94849"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94950"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96293"/></para>
    /// </remarks>
    [Post("/cgi-bin/export/simple_user")]
    Task<ExportJobResponse> ExportUsersAsync(
        [Body] ExportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出成员详情
    /// <para>异步导出企业成员详情列表；仅会返回有权限的人员列表。</para>
    /// <para>密文解密后的 userlist 内容与获取部门成员详情接口一致。</para>
    /// </summary>
    /// <param name="request">导出请求体（<see cref="ExportRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务受理结果（jobid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94851"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94951"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96294"/></para>
    /// </remarks>
    [Post("/cgi-bin/export/user")]
    Task<ExportJobResponse> ExportUserDetailsAsync(
        [Body] ExportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出部门
    /// <para>异步导出企业部门列表；仅返回有权限的部门列表。</para>
    /// <para>密文解密后的 department 内容与获取部门列表接口一致。</para>
    /// </summary>
    /// <param name="request">导出请求体（<see cref="ExportRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务受理结果（jobid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94852"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94952"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96295"/></para>
    /// </remarks>
    [Post("/cgi-bin/export/department")]
    Task<ExportJobResponse> ExportDepartmentsAsync(
        [Body] ExportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出标签成员
    /// <para>异步导出指定标签下的成员与部门列表；要求对标签有读取权限。</para>
    /// <para>密文解密后的 userlist 与 partylist 内容与获取标签成员接口一致。</para>
    /// </summary>
    /// <param name="request">导出标签成员请求体（<see cref="ExportTagUsersRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务受理结果（jobid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94853"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94953"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96296"/></para>
    /// </remarks>
    [Post("/cgi-bin/export/taguser")]
    Task<ExportJobResponse> ExportTagUsersAsync(
        [Body] ExportTagUsersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取导出结果
    /// <para>查询异步导出任务的执行状态与数据文件列表；获取任务结果的调用身份需要与提交任务的一致。</para>
    /// <para>数据文件下载链接有效期 2 个小时、支持指定 Range 头分段下载；文件为提交任务时 encoding_aeskey
    /// 对应 AES-256-CBC 加密的密文（AESKey = Base64_Decode(encoding_aeskey + "=")），解密由调用方自行完成。</para>
    /// </summary>
    /// <param name="jobId">任务 ID（提交导出任务时返回的 jobid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务状态与数据文件列表（status：0 未处理 / 1 处理中 / 2 完成 / 3 异常失败）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94854"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94954"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96297"/></para>
    /// </remarks>
    [Get("/cgi-bin/export/get_result")]
    Task<GetExportResultResponse> GetExportResultAsync(
        [Query("jobid")] string jobId,
        CancellationToken cancellationToken = default);
}
