// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.DataZone;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「数据与智能专区」模块应用调用专区程序域公共 SDK（应用同步调用专区程序 /
/// 创建专区程序调用任务 / 获取专区程序任务结果，三端点收敛面）。
/// <para>
/// 官方对三类应用开放一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalDataZoneProgramService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderDataZoneProgramService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyDataZoneProgramService"/>。
/// 基础接口（设置公钥 / 授权成员列表等）见 <see cref="IWechatWorkDataZoneService"/> 接口族。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkMediaService"/>：三类应用消费的令牌路由键均为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。各端点均要求应用具备
/// 「数据与智能专区」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkDataZoneProgramService
{
    /// <summary>
    /// 应用同步调用专区程序
    /// <para>以同步方式调用应用关联的专区程序能力，直接返回专区程序的输出结果。</para>
    /// <para>官方限制：request_data 要求与配置的输入协议格式匹配（如使用专区程序示例，
    /// 留意 Java 版本 demo 的输入无需在 request_data 内包裹一层 input 对象）；
    /// response_data 为自定义的 JSON 字符串，要求与管理端配置的输出协议格式匹配。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SyncCallDataZoneProgramRequest"/>：program_id / ability_id / notify_id（选填，由「专区通知应用」返回）/ request_data）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>专区程序的输出结果（response_data，自定义 JSON 字符串）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99965"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99811"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100020"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/sync_call_program")]
    Task<SyncCallDataZoneProgramResponse> SyncCallProgramAsync(
        [Body] SyncCallDataZoneProgramRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建专区程序调用任务
    /// <para>以异步方式创建专区程序调用任务，返回任务 id；任务结果须凭 jobid 调用
    /// <see cref="GetAsyncProgramResultAsync"/> 查询（专区程序完成执行后经「上报异步任务结果」上报）。</para>
    /// <para>官方限制：request_data 要求与配置的输入协议格式匹配。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateDataZoneAsyncProgramTaskRequest"/>：program_id / ability_id / request_data）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务 id（jobid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99966"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99812"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100021"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/async_program_task")]
    Task<CreateDataZoneAsyncProgramTaskResponse> CreateAsyncProgramTaskAsync(
        [Body] CreateDataZoneAsyncProgramTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取专区程序任务结果
    /// <para>凭 <see cref="CreateAsyncProgramTaskAsync"/> 返回的 jobid 查询专区程序异步调用任务的执行结果。</para>
    /// <para>官方限制：response_errcode 为「上报异步任务结果」中上报的 errcode（代表专区程序返回的错误码）；
    /// response_data 为自定义的 JSON 字符串，要求与管理端配置的输出协议格式匹配。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetDataZoneAsyncProgramResultRequest"/>：jobid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>专区程序返回的错误码（response_errcode）与输出结果（response_data）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99966"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99812"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100021"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/async_program_result")]
    Task<GetDataZoneAsyncProgramResultResponse> GetAsyncProgramResultAsync(
        [Body] GetDataZoneAsyncProgramResultRequest request,
        CancellationToken cancellationToken = default);
}
