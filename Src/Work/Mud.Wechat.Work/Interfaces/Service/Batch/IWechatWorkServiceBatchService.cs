// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Service;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「服务商服务端」域「异步批量任务」接口族公共父接口（官方 91882）。
/// <para>
/// 与自建应用侧的 <c>/cgi-bin/batch/getresult</c>（<c>access_token</c> 面，见
/// <c>IWechatWorkBatchService.GetBatchJobResultAsync</c>）<b>是同能力不同令牌链路的两条路由</b>：
/// 本族路由带 <c>service/</c> 段、以 <c>provider_access_token</c> 鉴权，官方响应字段集也更窄。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 应用类型开放面：官方正文请求地址为 <c>?provider_access_token=ACCESS_TOKEN&amp;jobid=JOBID</c>，
/// 仅服务商代开发 / 第三方服务端可见 ⇒ 只落 <see cref="IWechatWorkProviderServiceBatchService"/> 一个子接口。
/// </para>
/// <para>
/// <b>官方开放面收窄（零端点风险面）</b>：SKIT 转述的「服务商服务端」15 行里，除本端点与
/// <c>service/contact/*</c> 四支外，其余 10 支（<c>service/add_device</c>、<c>service/del_device</c>、
/// <c>service/list_device</c>、<c>service/fetch_device_log</c>、<c>service/get_device_auth_info</c>、
/// <c>service/reset_secret_no</c>、<c>service/set_printer_support_state</c>、<c>service/get_admin_list</c>、
/// <c>service/externalcontact/unionid_to_external_userid_3rd</c>、<c>service/getuserdetail3rd</c> 旧形态）
/// 均已逐页复核判定为非契约或已按现行形态落位，由 <c>WechatNonContractRouteZeroDeclarationContractGuards</c>（RD1）锁定零声明。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkServiceBatchService
{
    /// <summary>
    /// 获取异步批量任务结果（服务商服务端）
    /// <para>轮询由 <see cref="IWechatWorkServiceContactService.TranslateContactIdsAsync"/>（官方 91846）
    /// 等异步端点提交的 <c>jobid</c>。</para>
    /// <para>
    /// <b>官方状态机</b>：<c>status</c> 1 任务开始 / 2 任务进行中 / 3 任务已完成；
    /// 「详细的处理结果」<c>result</c> 与产物地址 <c>url</c> <b>仅任务完成后有效</b>，
    /// 轮询方须以 <c>status = 3</c> 作为终止条件，不得以字段缺失判失败。
    /// </para>
    /// <para>
    /// <b>本端点官方即 GET</b>，<c>jobid</c> 走 Query（最大 64 字节）。
    /// </para>
    /// <para>
    /// <b>产物链接安全约束</b>：<c>url</c>（含 <c>result.contact_id_translate.url</c>）为带身份语义的
    /// 临时资源地址，官方要求须经用户 oauth2 授权登录或单点登录在<b>用户侧</b>打开、<b>24 小时有效</b>
    /// —— 不得写入日志、遥测或对外持久化。
    /// </para>
    /// </summary>
    /// <param name="jobId">异步任务 id（<c>jobid</c>，Query，必填，最大 64 字节）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务状态与结果（status / type / result / url；<c>type</c> 目前有 <c>contact_id_translate</c>）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91882"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制；轮询请自行控制间隔。</para>
    /// </remarks>
    [Get("/cgi-bin/service/batch/getresult")]
    Task<GetServiceBatchJobResultResponse> GetServiceBatchJobResultAsync(
        [Query("jobid")] string jobId,
        CancellationToken cancellationToken = default);
}
