// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Contacts.Batch;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信通讯录「异步导入接口」域公共 SDK（增量更新成员、全量覆盖成员、全量覆盖部门、获取异步任务结果）。
/// <para>
/// 官方对企业自建应用与第三方应用开放了完全一致的 4 个端点（服务商代开发无此功能，不设对应子接口），
/// 因此全部端点声明于本公共父接口；自建与第三方应用类型子接口
/// （<see cref="IWechatWorkInternalBatchService"/> / <see cref="IWechatWorkThirdPartyBatchService"/>）
/// 均为空标记，仅作为类型化契约入口。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkTagsService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：写入端点要求「须拥有通讯录的写权限」；获取异步任务结果只能查询已经提交过的历史任务。
/// csv 文件须先经素材上传接口取得 media_id。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkBatchService
{
    /// <summary>
    /// 增量更新成员
    /// <para>以 userid（账号）为主键增量更新企业微信通讯录成员：文件与通讯录均存在的成员更新指定字段，
    /// 仅文件存在的执行添加，仅通讯录存在的保持不变；须先上传 csv 模板取得 media_id。</para>
    /// </summary>
    /// <param name="request">导入请求体（<see cref="BatchImportUsersRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务受理结果（jobid；经获取异步任务结果端点轮询进度）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90980"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91130"/></para>
    /// </remarks>
    [Post("/cgi-bin/batch/syncuser")]
    Task<BatchJobResponse> SyncUsersAsync(
        [Body] BatchImportUsersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 全量覆盖成员
    /// <para>以文件为准全量覆盖通讯录成员：文件中不存在、通讯录中存在的成员将被删除（危险操作）。
    /// 出于安全考虑，需删除成员多于 50 人且多于现有人数 20% 以上，或少于 50 人且多于现有人数 80% 以上时，
    /// 官方将中止导入并返回相应错误码。</para>
    /// </summary>
    /// <param name="request">导入请求体（<see cref="BatchImportUsersRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务受理结果（jobid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90981"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91131"/></para>
    /// </remarks>
    [Post("/cgi-bin/batch/replaceuser")]
    Task<BatchJobResponse> ReplaceUsersAsync(
        [Body] BatchImportUsersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 全量覆盖部门
    /// <para>以文件为准全量覆盖通讯录部门：文件中不存在的部门当其下无任何成员或子部门时删除，
    /// 仍有成员或子部门的暂缓删除（待下次导入成员把人移出后自动删除）。</para>
    /// </summary>
    /// <param name="request">导入请求体（<see cref="BatchImportPartiesRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务受理结果（jobid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90982"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91132"/></para>
    /// </remarks>
    [Post("/cgi-bin/batch/replaceparty")]
    Task<BatchJobResponse> ReplaceDepartmentsAsync(
        [Body] BatchImportPartiesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取异步任务结果
    /// <para>查询异步导入任务的执行状态与逐条处理结果；只能查询已经提交过的历史任务。</para>
    /// </summary>
    /// <param name="jobId">异步任务 id（提交任务时返回的 jobid，最大长度 64 字节）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务状态与结果明细（status / type / total / percentage / result；result 形状按任务类型区分）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90983"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91133"/></para>
    /// </remarks>
    [Get("/cgi-bin/batch/getresult")]
    Task<GetBatchJobResultResponse> GetBatchJobResultAsync(
        [Query("jobid")] string jobId,
        CancellationToken cancellationToken = default);
}
